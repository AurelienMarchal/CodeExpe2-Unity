using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SimultaneousAction", menuName = "UI Controller Action/Simultaneous Action", order = 0)]
public class UISimultaneousControllerAction : UIControllerAction
{
    public List<UISingleControllerAction> singleActions;
}
