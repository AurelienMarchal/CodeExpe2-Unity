using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class OrthozoomButtonRateControlInteraction : RateControlInteraction
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

    [SerializeField]
    InputActionReference primaryButton;

    [SerializeField]
    InputActionReference secondaryButton;

    bool primaryButtonPressed = false;

    bool secondaryButtonPressed = false;

    [SerializeField]
    float primaryButtonRateCoef;

    [SerializeField]
    float secondaryButtonRateCoef;

    [SerializeField]
    float deadZoneDistance;

    public override void StartInteraction(){
        base.StartInteraction();

        DestroyAllControllerDistanceLine();
        
        primaryButton.action.performed += OnPrimaryButtonPerformed;
        secondaryButton.action.performed += OnSecondaryButtonPerformed;
        primaryButton.action.canceled += OnPrimaryButtonCanceled;
        secondaryButton.action.canceled += OnSecondaryButtonCanceled;

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
    }

    private void OnPrimaryButtonPerformed(InputAction.CallbackContext obj)
    {
        primaryButtonPressed = true;
    }

    public override void StopInteraction(){
        base.StopInteraction();

        primaryButton.action.performed -= OnPrimaryButtonPerformed;
        secondaryButton.action.performed -= OnSecondaryButtonPerformed;
        primaryButton.action.canceled -= OnPrimaryButtonCanceled;
        secondaryButton.action.canceled -= OnSecondaryButtonCanceled;

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

        if(rightHand){
            distance = rightCurrentPos.x - rightStartPos.x;
        }
        else{
            distance = leftCurrentPos.x - leftStartPos.x;
        }

        var rate = 0f;

        if(Mathf.Abs(distance) > Mathf.Abs(deadZoneDistance)){
            rate = primaryButtonPressed.ToInt() * primaryButtonRateCoef + secondaryButtonPressed.ToInt() * secondaryButtonRateCoef;
            rate = Mathf.Sign(distance) * rate;
        }

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

public static class BooleanExtensions
{
    public static int ToInt(this bool value)
    {
        return value ? 1 : 0;
    }
}

