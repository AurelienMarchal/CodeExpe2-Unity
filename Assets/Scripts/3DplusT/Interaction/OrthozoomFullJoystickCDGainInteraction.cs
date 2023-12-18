using UnityEngine.InputSystem;
using UnityEngine;

public class OrthozoomFullJoystickCDGainInteraction : CDGainInteraction
{
    [SerializeField]
    float timeIncreaseCoef;

    [SerializeField]
    float joystickOrthoDistCoef;

    [SerializeField]
    bool rightHand = true;

    [SerializeField]
    InputActionReference rightJoyStickPos;

    [SerializeField]
    InputActionReference leftJoyStickPos;

    [SerializeField]
    float joystickXDeadZone;

    [SerializeField]
    float joystickYDeadZoneActivation;


    float distanceSinceLastTimeIncrease = 0f;

    public float joystickX{
        get;
        protected set;
    }

    public float joystickY{
        get;
        protected set;
    }

    public float orthoDistance{
        get;
        protected set;
    }

    public override void StartInteraction(){
        distanceSinceLastTimeIncrease = 0f;
        controllerDistance = 0f;
        base.StartInteraction();
    }

    public override void StopInteraction(){
        base.StopInteraction();
        
    }

    protected override void OnInteractionEnabled()
    {
        orthoDistance = 0f;
        controllerDistance = 0f;
    }

    public override int CalculateTimeIncrease(){

        joystickX = 0f;

        if(rightHand){
            joystickX = rightJoyStickPos.action.ReadValue<Vector2>().x;
            joystickY = rightJoyStickPos.action.ReadValue<Vector2>().y;
        }

        else{
            joystickX = leftJoyStickPos.action.ReadValue<Vector2>().x;
            joystickY = leftJoyStickPos.action.ReadValue<Vector2>().y;
        }

        if(Mathf.Abs(joystickY) > joystickYDeadZoneActivation && orthoDistance < 0.95f){
            if(Mathf.Abs(joystickX) < joystickXDeadZone){
                joystickX = 0f;
            }
        }

        orthoDistance = Mathf.Clamp01(orthoDistance + joystickY*joystickOrthoDistCoef);

        cDGain = Mathf.Round(CalculateCDGain(orthoDistance));

        int timeIncrease = 0;
        if(Mathf.Abs(joystickX) > minDistanceToMove){
            distanceSinceLastTimeIncrease += -joystickX;
            timeIncrease = Mathf.RoundToInt(cDGain * distanceSinceLastTimeIncrease * timeIncreaseCoef);
        }
        
        if(timeIncrease != 0){
            distanceSinceLastTimeIncrease = 0f;
        }

        return timeIncrease;
    }
}
