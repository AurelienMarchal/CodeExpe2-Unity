using UnityEngine;


public class CDGainInteraction : Interaction
{

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
        var CD_min = minGain;
        var CD_max = maxGain;
        var Vmin = 0f;
        var Vmax = distanceForMaxGain;

        var Vinf = (Vmax - Vmin) * ratio + Vmin;
        var gain = (CD_max - CD_min) / (1 + Mathf.Exp(-lambda * (distance - Vinf))) + CD_min;

        return gain;
    }


}
