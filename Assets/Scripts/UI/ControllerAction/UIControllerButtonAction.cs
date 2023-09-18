using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ButtonAction", menuName = "UI Controller Action/Single Action/Button Action", order = 0)]
public class UIControllerButtonAction : UISingleControllerAction{
    public ButtonAction buttonAction;
    public ButtonType buttonType;
}

public enum ButtonAction{
    Press, Hold, Release
}

public enum ButtonType{
    Button1, Button2, Menu, ThumbTrigger, IndexTrigger, Stick
}
