using UnityEngine;
using UnityEngine.InputSystem;

public class JoystickRateControlInteraction : RateControlInteraction
{

    //[SerializeField]
    //InputActionReference leftJoyStickPos;

    [SerializeField]
    InputActionReference rightJoyStickPos;

    [SerializeField]
    float rightRateCoef = 10f;

    public float leftJoyStickX{
        get;
        protected set;
    }

    public float rightJoyStickX{
        get;
        protected set;
    }

    public override void OnRateControlInteractionDisabled()
    {
        StopInteraction();
    }

    public override void OnRateControlInteractionEnabled()
    {
        leftJoyStickX = 0f;
        rightJoyStickX = 0f;
        StartInteraction();
    }

    public override float CalculateRate()
    {
        //leftJoyStickX = leftJoyStickPos.action.ReadValue<Vector2>().x;
        rightJoyStickX = rightJoyStickPos.action.ReadValue<Vector2>().x;
        return rightJoyStickX * rightRateCoef;
    }
}
