
using UnityEngine;
using TMPro;

public class ControllerDistanceRateControlInteraction : RateControlFuctionInteraction
{

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
        rightStartPos = rightControllerTransform.position;

        controllerDistanceLineInstanceRight = Instantiate(controllerDistanceLinePrefab, rightStartPos, Quaternion.identity);

        lineRendererRight = controllerDistanceLineInstanceRight.GetComponent<LineRenderer>();
        rateTextMeshRight = controllerDistanceLineInstanceRight.GetComponentInChildren<TextMeshProUGUI>();
        lineRendererRight.enabled = true;

        leftStartPos = leftControllerTransform.position;

        controllerDistanceLineInstanceLeft = Instantiate(controllerDistanceLinePrefab, leftStartPos, Quaternion.identity);

        lineRendererLeft = controllerDistanceLineInstanceLeft.GetComponent<LineRenderer>();
        rateTextMeshLeft = controllerDistanceLineInstanceLeft.GetComponentInChildren<TextMeshProUGUI>();
        lineRendererLeft.enabled = true;
    }

    public override void StopInteraction(){
        lineRendererRight.enabled = false;
        Destroy(controllerDistanceLineInstanceRight);
        lineRendererRight = null;
        rateTextMeshRight = null;

        lineRendererLeft.enabled = false;
        Destroy(controllerDistanceLineInstanceLeft);
        lineRendererLeft = null;
        rateTextMeshLeft = null;
    }

    public override float CalculateRate(){

        var rightCurrentPos = rightControllerTransform.position;
        var leftCurrentPos = leftControllerTransform.position;

        var rateRight = ProcessControlFunction(rightCurrentPos.x - rightStartPos.x);
        var rateLeft = ProcessControlFunction(leftStartPos.x - leftCurrentPos.x);

        var rate = rateRight - rateLeft;

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
            rateTextMeshRight.text = "Rate Right :\n" + rateRight.ToString("0.00");
        }

        if(rateTextMeshLeft != null){
            rateTextMeshLeft.text = "Rate Left :\n" + (-rateLeft).ToString("0.00");
        }

        return rate;
    }

}
