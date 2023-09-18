using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using TMPro;

public class OrthozoomButtonCDGainInteraction : CDGainInteraction
{

    [SerializeField]
    bool rightHand = true;

    [SerializeField]
    GameObject controllerDistanceLinePrefab;

    GameObject controllerDistanceLineInstanceRight;
    GameObject controllerDistanceLineInstanceLeft;

    LineRenderer lineRendererRight;

    TextMeshProUGUI rateTextMeshRight;

    LineRenderer lineRendererLeft;

    TextMeshProUGUI rateTextMeshLeft;

    [SerializeField]
    Transform leftControllerTransform;

    [SerializeField]
    Transform rightControllerTransform;

    [SerializeField]
    InputActionReference primaryButton;

    [SerializeField]
    InputActionReference secondaryButton;

    bool primaryButtonPressed = false;

    bool secondaryButtonPressed = false;

    public UnityEvent primaryButtonPressedEvent = new UnityEvent();

    public UnityEvent secondaryButtonPressedEvent = new UnityEvent();

    public UnityEvent bothButtonPressedEvent = new UnityEvent();

    [SerializeField]
    public float primaryButtonGainCoef;

    [SerializeField]
    public float secondaryButtonGainCoef;

    Vector3 rightLastPos = Vector3.zero;

    Vector3 leftLastPos = Vector3.zero;

    Vector3 rightEnabledPos = Vector3.zero;

    Vector3 leftEnabledPos = Vector3.zero;

    float distanceSinceLastTimeIncrease = 0f;


    protected override void OnInteractionEnabled(){

        DestroyAllControllerDistanceLine();
        controllerDistance = 0f;
        cDGain = 0f;

        primaryButton.action.performed += OnPrimaryButtonPerformed;
        secondaryButton.action.performed += OnSecondaryButtonPerformed;
        primaryButton.action.canceled += OnPrimaryButtonCanceled;
        secondaryButton.action.canceled += OnSecondaryButtonCanceled;


        if(rightHand){
            rightEnabledPos = rightControllerTransform.position;
            controllerDistanceLineInstanceRight = Instantiate(controllerDistanceLinePrefab, Vector3.zero, Quaternion.identity);

            lineRendererRight = controllerDistanceLineInstanceRight.GetComponent<LineRenderer>();
            rateTextMeshRight = controllerDistanceLineInstanceRight.GetComponentInChildren<TextMeshProUGUI>();
            lineRendererRight.enabled = true;
        }
        else{
            controllerDistanceLineInstanceLeft = Instantiate(controllerDistanceLinePrefab, Vector3.zero, Quaternion.identity);

            lineRendererLeft = controllerDistanceLineInstanceLeft.GetComponent<LineRenderer>();
            rateTextMeshLeft = controllerDistanceLineInstanceLeft.GetComponentInChildren<TextMeshProUGUI>();
            lineRendererLeft.enabled = true;
        }
    }

    protected override void OnInteractionDisabled(){

        DestroyAllControllerDistanceLine();

        primaryButton.action.performed -= OnPrimaryButtonPerformed;
        secondaryButton.action.performed -= OnSecondaryButtonPerformed;
        primaryButton.action.canceled -= OnPrimaryButtonCanceled;
        secondaryButton.action.canceled -= OnSecondaryButtonCanceled;

        if(rightHand){
            lineRendererRight.enabled = false;
            lineRendererRight = null;
            rateTextMeshRight = null;
        }
        else{
            lineRendererLeft.enabled = false;
            lineRendererLeft = null;
            rateTextMeshLeft = null;
        }
    }


    private void OnSecondaryButtonCanceled(InputAction.CallbackContext obj)
    {
        secondaryButtonPressed = false;
    }

    private void OnPrimaryButtonCanceled(InputAction.CallbackContext obj)
    {
        primaryButtonPressed = false;
    }

    private void OnSecondaryButtonPerformed(InputAction.CallbackContext obj)
    {
        secondaryButtonPressed = true;
        rightLastPos = rightControllerTransform.position;
        leftLastPos = leftControllerTransform.position;
        distanceSinceLastTimeIncrease = 0f;
        secondaryButtonPressedEvent.Invoke();
        if(primaryButtonPressed){
            bothButtonPressedEvent.Invoke();
        }
    }

    private void OnPrimaryButtonPerformed(InputAction.CallbackContext obj)
    {
        primaryButtonPressed = true;
        rightLastPos = rightControllerTransform.position;
        leftLastPos = leftControllerTransform.position;
        distanceSinceLastTimeIncrease = 0f;
        primaryButtonPressedEvent.Invoke();
        if(secondaryButtonPressed){
            bothButtonPressedEvent.Invoke();
        }
    }
    

    public override int CalculateTimeIncrease(){

        var rightCurrentPos = rightControllerTransform.position;
        var leftCurrentPos = leftControllerTransform.position;

        var distance = 0f;

        if(rightHand){
            
            distance = rightCurrentPos.x - rightLastPos.x;
            controllerDistance = rightEnabledPos.x - rightCurrentPos.x;
            
        }
        else{
            
            distance = leftCurrentPos.x - leftLastPos.x;
            controllerDistance = leftEnabledPos.x - leftCurrentPos.x;
        }

        int timeIncrease = 0;

        if(primaryButtonPressed || secondaryButtonPressed){
            if(secondaryButtonPressed){
                cDGain = secondaryButtonGainCoef;
            }
            else if(primaryButtonPressed){
                cDGain = primaryButtonGainCoef;
            }
            //cDGain = primaryButtonPressed.ToInt() * primaryButtonGainCoef + secondaryButtonPressed.ToInt() * secondaryButtonGainCoef;
            
            if(Mathf.Abs(distance) > minDistanceToMove){
                distanceSinceLastTimeIncrease += distance;
                timeIncrease = Mathf.RoundToInt(distanceSinceLastTimeIncrease * cDGain);
            }

            if(timeIncrease != 0){
                distanceSinceLastTimeIncrease = 0f;
            }
        }

            
        rightLastPos = rightCurrentPos;
        leftLastPos = leftCurrentPos;

        if(controllerDistanceLineInstanceRight != null){
            controllerDistanceLineInstanceRight.transform.position = rightCurrentPos;
        }

        if(controllerDistanceLineInstanceLeft != null){
            controllerDistanceLineInstanceLeft.transform.position = leftCurrentPos;
        }

        if(lineRendererRight != null){
            lineRendererRight.SetPosition(0, rightCurrentPos);
            lineRendererRight.SetPosition(1, rightCurrentPos);
        }

        if(lineRendererLeft != null){
            lineRendererLeft.SetPosition(0, rightCurrentPos);
            lineRendererLeft.SetPosition(1, leftCurrentPos);
        }

        if(rateTextMeshRight != null){
            if(rightHand){
                rateTextMeshRight.text = "CDGain :\n" + cDGain.ToString("0.00");
            }
            else{
                rateTextMeshRight.text = "";
            }
            
        }

        if(rateTextMeshLeft != null){
            if(!rightHand){
                rateTextMeshLeft.text = "CDGain :\n" + cDGain.ToString("0.00");
            }
            else{
                rateTextMeshLeft.text = "";
            }
        }

        return timeIncrease;
    }
}
