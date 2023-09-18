using System;
using System.Collections.Generic;
using UnityEngine;

public class RateControlInteraction : Interaction
{
    public virtual float CalculateRate(){
        return 0f;
    }

    public virtual void OnRateControlInteractionEnabled(){

    }

    public virtual void OnRateControlInteractionDisabled(){
        
    }

    public override void EnableInteraction(){
        interationEnabled = true;
        OnRateControlInteractionEnabled();
    }

    public override void DisableInteraction(){
        interationEnabled = false;
        OnRateControlInteractionDisabled();
    }


    
}
