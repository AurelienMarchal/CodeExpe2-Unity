using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StickAction", menuName = "UI Controller Action/Single Action/Stick Action", order = 1)]
public class UIControllerStickAction : UISingleControllerAction{
    public StickAction stickAction;
}

public enum StickAction{
    Left,
    Right,
    Up,
    Down,
    Rotate
}