using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OrthozoomTwoHandsHeightRateControlInteraction : RateControlFuctionInteraction
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
        base.StartInteraction();
        DestroyAllControllerDistanceLine();
        
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
        base.StopInteraction();
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
        
        if(!interactionStarted){
            return 0f;
        }

        var rightCurrentPos = rightControllerTransform.position;
        var leftCurrentPos = leftControllerTransform.position;

        var distanceRight = rightCurrentPos.x - rightStartPos.x;
        var height = leftCurrentPos.y - leftStartPos.y;


        var rate = ProcessControlFunction(height);
        rate = Mathf.Sign(distanceRight) * rate;

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
            rateTextMeshRight.text = "";
        }

        if(rateTextMeshLeft != null){
            rateTextMeshLeft.text = "Rate :\n" + rate.ToString("0.00");
        }

        return rate;
    }
}
