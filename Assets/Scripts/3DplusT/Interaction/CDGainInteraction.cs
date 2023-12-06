using UnityEngine;
using System;

[Serializable]
public struct Parameter{
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

    public Parameter(int numberOfObjectsRequired, float minGain, float maxGain, float lambda, float ratio){
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
    Parameter[] parameters;

    [HideInInspector]
    public float numberOfObjects;

    [SerializeField]
    protected float minDistanceToMove;

    [SerializeField]
    public float minGain = 0f;

    [SerializeField]
    public float maxGain;

    [SerializeField]
    float distanceForMaxGain;

    [SerializeField]
    float lambda;

    [Range(0.0f, 1.0f), SerializeField]
    float ratio = 0.5f;

    
    public float cDGain{
        get;
        protected set;
    }

    public virtual int CalculateTimeIncrease(){
        return 0;
    }


    protected float CalculateCDGain(float distance){

        var parameter = new Parameter(int.MaxValue, 0f, 0f, 0f, 0f);

        foreach(Parameter parameter_ in parameters){
            if(parameter_.numberOfObjectsRequired >= numberOfObjects){
                parameter = parameter_;
                break;
            }
        }


        var CD_min = parameter.minGain;
        var CD_max = parameter.maxGain;
        var Vmin = 0f;
        var Vmax = distanceForMaxGain;

        var Vinf = (Vmax - Vmin) * parameter.ratio + Vmin;
        var gain = (CD_max - CD_min) / (1 + Mathf.Exp(-parameter.lambda * (distance - Vinf))) + CD_min;

        return gain;
    }

}
