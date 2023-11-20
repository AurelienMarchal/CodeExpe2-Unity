using UnityEngine;


public class CDGainInteraction : Interaction
{

    [HideInInspector]
    public float numberOfObjects;

    [SerializeField]
    protected float minDistanceToMove;

    [SerializeField]
    public float minGain = 0f;

    [SerializeField]
    public float maxGain;

    public float maxGain2;

    [SerializeField]
    float distanceForMaxGain;

    [SerializeField]
    float lambda;

    [SerializeField]
    float lambda2;

    [Range(0.0f, 1.0f), SerializeField]
    float ratio = 0.5f;

    [Range(0.0f, 1.0f), SerializeField]
    float ratio2 = 0.5f;

    
    public float cDGain{
        get;
        protected set;
    }

    public virtual int CalculateTimeIncrease(){
        return 0;
    }


    protected float CalculateCDGain(float distance){
        var CD_min = minGain;
        var CD_max = maxGain;
        var Vmin = 0f;
        var Vmax = distanceForMaxGain;

        var Vinf = (Vmax - Vmin) * ratio + Vmin;
        var gain = (CD_max - CD_min) / (1 + Mathf.Exp(-lambda * (distance - Vinf))) + CD_min;

        var CD_min2 = 0f;
        maxGain2 = (numberOfObjects-200) * CD_max/800;
        var CD_max2 = maxGain2;

        var Vinf2 = (Vmax - Vmin) * ratio2 + Vmin;
        var gain2 = (CD_max2 - CD_min2) / (1 + Mathf.Exp(-lambda2 * (distance - Vinf2))) + CD_min2;

        //Debug.Log($"Gain : {gain + gain2}");

        return gain + gain2;
    }


}
