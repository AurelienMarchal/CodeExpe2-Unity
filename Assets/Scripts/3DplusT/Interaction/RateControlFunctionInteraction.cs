using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RateControlFuctionInteraction : RateControlInteraction
{

    [SerializeField]
    ControlFunction controlFunction = ControlFunction.Linear;

    [SerializeField]
    float linearCoef = 10f;
    
    [SerializeField]
    List<float> stairsRates;

    [SerializeField]
    float distanceBetweenTwoSteps;

    [SerializeField]
    AnimationCurve animCurve1;

    [SerializeField]
    AnimationCurve animCurve2;

    AnimationCurve animCurveClutch;

    [SerializeField]
    float maxRate;

    [SerializeField]
    float distanceForMaxRate;

    [SerializeField]
    float lambda = 30f;


    protected float ProcessControlFunction(float distance){
        switch(controlFunction){
            case ControlFunction.Linear: return distance * linearCoef;
            case ControlFunction.Stairs: 
            int ind = Mathf.FloorToInt(distance/distanceBetweenTwoSteps);
            ind = Math.Min(ind, stairsRates.Count - 1);
            if(ind >= 0){
                return stairsRates[ind];
            }
            else{
                return 0f;
            }
            case ControlFunction.AnimCurve1: 
            return animCurve1.Evaluate(distance/distanceForMaxRate)*maxRate;
            case ControlFunction.AnimCurve2: 
            return animCurve2.Evaluate(distance/distanceForMaxRate)*maxRate;
            case ControlFunction.ClassicClutch:
            return ClassicClutch(Mathf.Abs(distance));
            default: return distance;
        }
    }

    float ClassicClutch(float distance){
        var CD_min = 0f;
        var CD_max = maxRate;
        var Vmin = 0f;
        var Vmax = distanceForMaxRate;

        var Vinf = (Vmax - Vmin) * 0.5f + Vmin;
        var gain = (CD_max - CD_min) / (1 + Mathf.Exp(-lambda * (distance - Vinf))) + CD_min;

        return gain;
    }

    enum ControlFunction{
        Linear, Stairs, AnimCurve1, AnimCurve2, ClassicClutch
    }
}
