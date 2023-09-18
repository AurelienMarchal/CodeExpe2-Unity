using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class OrthozoomDepthJoystickCDGainInteraction : CDGainInteraction
{
    [SerializeField]
    float timeIncreaseCoef;

    [SerializeField]
    bool rightHand = true;

    [SerializeField]
    InputActionReference resetStartingPosInteraction;

    [SerializeField]
    InputActionReference rightJoyStickPos;

    [SerializeField]
    InputActionReference leftJoyStickPos;

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

    Vector3 rightEnabledPos = Vector3.zero;

    Vector3 leftEnabledPos = Vector3.zero;

    float distanceSinceLastTimeIncrease = 0f;

    public float joystickX{
        get;
        protected set;
    }

    public float orthoDistance{
        get;
        protected set;
    }

    public override void StartInteraction(){
        base.StartInteraction();
    }

    public override void StopInteraction(){
        base.StopInteraction();
        
    }

    protected override void OnInteractionEnabled()
    {
        DestroyAllControllerDistanceLine();
        orthoDistance = 0f;
        controllerDistance = 0f;
        if(rightHand){
            rightEnabledPos = rightControllerTransform.position;

            controllerDistanceLineInstanceRight = Instantiate(controllerDistanceLinePrefab, rightEnabledPos, Quaternion.identity);

            lineRendererRight = controllerDistanceLineInstanceRight.GetComponent<LineRenderer>();
            rateTextMeshRight = controllerDistanceLineInstanceRight.GetComponentInChildren<TextMeshProUGUI>();
            lineRendererRight.enabled = true;
        }
        else{
            leftEnabledPos = leftControllerTransform.position;
            controllerDistanceLineInstanceLeft = Instantiate(controllerDistanceLinePrefab, leftEnabledPos, Quaternion.identity);

            lineRendererLeft = controllerDistanceLineInstanceLeft.GetComponent<LineRenderer>();
            rateTextMeshLeft = controllerDistanceLineInstanceLeft.GetComponentInChildren<TextMeshProUGUI>();
            lineRendererLeft.enabled = true;
        }

        resetStartingPosInteraction.action.performed += ResetEnabledPos;
    }

    protected override void OnInteractionDisabled(){
        DestroyAllControllerDistanceLine();
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

        resetStartingPosInteraction.action.performed -= ResetEnabledPos;
    }

    private void ResetEnabledPos(InputAction.CallbackContext context){
        Debug.Log("Reset Enabled Pos");
        if(rightHand){
            rightEnabledPos = rightControllerTransform.position;
        }
        else{
            leftEnabledPos = leftControllerTransform.position;
        }
    }

    public override int CalculateTimeIncrease(){

        var rightCurrentPos = rightControllerTransform.position;
        var leftCurrentPos = leftControllerTransform.position;

        var depth = 0f;
        joystickX = 0f;

        if(rightHand){
            joystickX = rightJoyStickPos.action.ReadValue<Vector2>().x;
        }

        else{
            joystickX = leftJoyStickPos.action.ReadValue<Vector2>().x;
        }

        if(rightHand){
            depth = rightCurrentPos.z - rightEnabledPos.z;
        }
        else{
            depth = leftCurrentPos.z - leftEnabledPos.z;
        }

        orthoDistance = depth;

        cDGain = Mathf.Round(CalculateCDGain(depth));

        int timeIncrease = 0;
        if(Mathf.Abs(joystickX) > minDistanceToMove){
            distanceSinceLastTimeIncrease += -joystickX;
            timeIncrease = Mathf.RoundToInt(cDGain * distanceSinceLastTimeIncrease * timeIncreaseCoef);
        }
        
        if(timeIncrease != 0){
            distanceSinceLastTimeIncrease = 0f;
        }
        
        if(controllerDistanceLineInstanceRight != null){
            controllerDistanceLineInstanceRight.transform.position = (rightCurrentPos + rightEnabledPos)/2;
        }

        if(controllerDistanceLineInstanceLeft != null){
            controllerDistanceLineInstanceLeft.transform.position = (leftCurrentPos + leftEnabledPos)/2;
        }

        if(lineRendererRight != null){
            lineRendererRight.SetPosition(0, rightEnabledPos);
            lineRendererRight.SetPosition(1, rightCurrentPos);
        }

        if(lineRendererLeft != null){
            lineRendererLeft.SetPosition(0, leftEnabledPos);
            lineRendererLeft.SetPosition(1, leftCurrentPos);
        }

        if(rateTextMeshRight != null){
            if(rightHand){
                rateTextMeshRight.text = "CDGain :\n" + ((cDGain - minGain)/(maxGain - minGain) * 100).ToString("0.00") + "%";
            }
            else{
                rateTextMeshRight.text = "";
            }
            
        }

        if(rateTextMeshLeft != null){
            if(!rightHand){
                rateTextMeshLeft.text = "CDGain :\n" + ((cDGain - minGain)/(maxGain - minGain) * 100).ToString("0.00") + "%";
            }
            else{
                rateTextMeshLeft.text = "";
            }
        }

        return timeIncrease;
    }
}
