using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class OrthozoomHeightCDGainInteraction : CDGainInteraction{

    [SerializeField]
    bool rightHand = true;

    [SerializeField]
    InputActionReference resetStartingPosInteraction;

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

    Vector3 rightLastPos = Vector3.zero;

    Vector3 leftLastPos = Vector3.zero;

    Vector3 rightEnabledPos = Vector3.zero;

    Vector3 leftEnabledPos = Vector3.zero;

    float distanceSinceLastTimeIncrease = 0f;

    public float orthoDistance{
        get;
        protected set;
    }


    public override void StartInteraction(){
        base.StartInteraction();
        rightLastPos = rightControllerTransform.position;
        leftLastPos = leftControllerTransform.position;
        distanceSinceLastTimeIncrease = 0f;
        
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

        var distance = 0f;
        var height = 0f;

        if(rightHand){
            if(interactionStarted){
                distance = rightCurrentPos.x - rightLastPos.x;
            }
            height = rightCurrentPos.y - rightEnabledPos.y;
            controllerDistance = rightEnabledPos.x - rightCurrentPos.x;
        }
        else{
            if(interactionStarted){
                distance = leftCurrentPos.x - leftLastPos.x;
            }
            height = leftCurrentPos.y - leftEnabledPos.y;
            controllerDistance = leftEnabledPos.x - leftCurrentPos.x;
        }

        
        orthoDistance = height;

        cDGain = Mathf.Round(CalculateCDGain(height)); 

        int timeIncrease = 0;
        if(Mathf.Abs(distance) > minDistanceToMove){
            distanceSinceLastTimeIncrease += distance;
            timeIncrease = Mathf.RoundToInt(distanceSinceLastTimeIncrease * cDGain);
        }

        if(timeIncrease != 0){
            distanceSinceLastTimeIncrease = 0f;
        }
            
        rightLastPos = rightCurrentPos;
        leftLastPos = leftCurrentPos;
        
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







