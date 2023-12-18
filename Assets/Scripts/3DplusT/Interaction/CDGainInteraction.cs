using UnityEngine;
using System;

[Serializable]
public struct CDGainParameter{
    
    [SerializeField]
    public int numberOfObjectsRequired;

    [SerializeField]
    public float minGain;

    [SerializeField]
    public float maxGain;

    [SerializeField]
    public float lambda;

    [Range(0.0f, 1.0f), SerializeField]
    public float ratio;

    public CDGainParameter(int numberOfObjectsRequired, float minGain, float maxGain, float lambda, float ratio){
        this.numberOfObjectsRequired = numberOfObjectsRequired;
        this.minGain = minGain;
        this.maxGain = maxGain;
        this.lambda = lambda;
        this.ratio = Mathf.Clamp(ratio, 0f, 1f);
    }

}


public class CDGainInteraction : Interaction
{

    [SerializeField]
    CDGainParameter[] parameters;

    [HideInInspector]
    public float numberOfObjects;

    [SerializeField]
    protected float minDistanceToMove;

    [HideInInspector]
    public float maxGain;

    [HideInInspector]
    public float minGain;

    [SerializeField]
    float distanceForMaxGain;

    
    public float cDGain{
        get;
        protected set;
    }

    public virtual int CalculateTimeIncrease(){
        return 0;
    }


    protected float CalculateCDGain(float distance){

        var parameter = new CDGainParameter(int.MaxValue, 1f, 1f, 1f, 1f);

        foreach(CDGainParameter parameter_ in parameters){
            if(parameter_.numberOfObjectsRequired >= numberOfObjects){
                parameter = parameter_;
                break;
            }
        }

        minGain = parameter.minGain;
        maxGain = parameter.maxGain;
        var CD_min = parameter.minGain;
        var CD_max = maxGain;
        var Vmin = 0f;
        var Vmax = distanceForMaxGain;

        var Vinf = (Vmax - Vmin) * parameter.ratio + Vmin;
        var gain = (CD_max - CD_min) / (1 + Mathf.Exp(-parameter.lambda * (distance - Vinf))) + CD_min;

        return gain;
    }

}
