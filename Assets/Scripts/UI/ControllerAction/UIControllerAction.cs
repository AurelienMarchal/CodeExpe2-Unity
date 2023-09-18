using UnityEngine;


public abstract class UIControllerAction : ScriptableObject {

}

public abstract class UISingleControllerAction : UIControllerAction{
    public bool leftController;
    public bool rightController;
}