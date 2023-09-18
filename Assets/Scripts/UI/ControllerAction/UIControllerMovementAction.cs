using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MovementAction", menuName = "UI Controller Action/Single Action/Movement Action", order = 2)]
public class UIControllerMovementAction : UISingleControllerAction{

    public MovementAction movementAction;


}

public enum MovementAction{
    HandsCloser,
    HandsFurther
}

