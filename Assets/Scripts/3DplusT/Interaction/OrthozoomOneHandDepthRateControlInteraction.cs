using UnityEngine;
using TMPro;

public class OrthozoomOneHandDepthRateControlInteraction : RateControlFuctionInteraction
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

    Vector3 rightStartPos = Vector3.zero;

    Vector3 leftStartPos = Vector3.zero;


    public override void StartInteraction(){
        base.StartInteraction();
        DestroyAllControllerDistanceLine();
        if(rightHand){
            rightStartPos = rightControllerTransform.position;

            controllerDistanceLineInstanceRight = Instantiate(controllerDistanceLinePrefab, rightStartPos, Quaternion.identity);

            lineRendererRight = controllerDistanceLineInstanceRight.GetComponent<LineRenderer>();
            rateTextMeshRight = controllerDistanceLineInstanceRight.GetComponentInChildren<TextMeshProUGUI>();
            lineRendererRight.enabled = true;
        }
        else{
            leftStartPos = leftControllerTransform.position;

            controllerDistanceLineInstanceLeft = Instantiate(controllerDistanceLinePrefab, leftStartPos, Quaternion.identity);

            lineRendererLeft = controllerDistanceLineInstanceLeft.GetComponent<LineRenderer>();
            rateTextMeshLeft = controllerDistanceLineInstanceLeft.GetComponentInChildren<TextMeshProUGUI>();
            lineRendererLeft.enabled = true;
        }
    }

    public override void StopInteraction(){
        base.StopInteraction();
        if(rightHand){
            lineRendererRight.enabled = false;
            Destroy(controllerDistanceLineInstanceRight);
            lineRendererRight = null;
            rateTextMeshRight = null;
        }
        else{
            lineRendererLeft.enabled = false;
            Destroy(controllerDistanceLineInstanceLeft);
            lineRendererLeft = null;
            rateTextMeshLeft = null;
        }
    }

    public override float CalculateRate(){

        if(!interactionStarted){
            return 0f;
        }

        var rightCurrentPos = rightControllerTransform.position;
        var leftCurrentPos = leftControllerTransform.position;

        var distance = 0f;
        var depth = 0f;

        if(rightHand){
            distance = rightCurrentPos.x - rightStartPos.x;
            depth = rightCurrentPos.z - rightStartPos.z;
        }
        else{
            distance = leftCurrentPos.x - leftStartPos.x;
            depth = leftCurrentPos.z - leftStartPos.z;
        }


        var rate = ProcessControlFunction(depth);

        rate = Mathf.Sign(distance) * rate;

        if(controllerDistanceLineInstanceRight != null){
            controllerDistanceLineInstanceRight.transform.position = (rightCurrentPos + rightStartPos)/2;
        }

        if(controllerDistanceLineInstanceLeft != null){
            controllerDistanceLineInstanceLeft.transform.position = (leftCurrentPos + leftStartPos)/2;
        }

        if(lineRendererRight != null){
            lineRendererRight.SetPosition(0, rightStartPos);
            lineRendererRight.SetPosition(1, rightCurrentPos);
        }

        if(lineRendererLeft != null){
            lineRendererLeft.SetPosition(0, leftStartPos);
            lineRendererLeft.SetPosition(1, leftCurrentPos);
        }

        if(rateTextMeshRight != null){
            if(rightHand){
                rateTextMeshRight.text = "Rate :\n" + rate.ToString("0.00");
            }
            else{
                rateTextMeshRight.text = "";
            }
            
        }

        if(rateTextMeshLeft != null){
            if(!rightHand){
                rateTextMeshLeft.text = "Rate :\n" + rate.ToString("0.00");
            }
            else{
                rateTextMeshLeft.text = "";
            }
        }

        return rate;
    }
}
