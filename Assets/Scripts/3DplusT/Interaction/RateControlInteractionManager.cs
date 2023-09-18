using UnityEngine;

public class RateControlInteractionManager : InteractionManager
{

    float timePassedSinceLastActivation = 0f;

    public float rate;

    // Start is called before the first frame update

    void Update(){
        if(objectManager != null){
            rate = 0f;
            foreach(Interaction interaction in interactions){
                RateControlInteraction rateControlInteraction = interaction as RateControlInteraction;
                if (rateControlInteraction != null)
                {
                    if(rateControlInteraction.interationEnabled){
                        rate = rateControlInteraction.CalculateRate();
                        objectManager.zoom = rate * zoomRatio * 10f;
                    }
                }
            }
            CalculateTimeStamps();
        }
    }

    private void CalculateTimeStamps()
    {
        if(rate != 0f){

            var rateAbs = Mathf.Abs(rate);

            timePassedSinceLastActivation += Time.deltaTime;
            int timeStamps = Mathf.FloorToInt(timePassedSinceLastActivation*rateAbs);
            
            if(timeStamps != 0){
                if(rate < 0){
                    objectManager.JumpInTime(-timeStamps);
                }
                else{
                    objectManager.JumpInTime(timeStamps);
                }
                timePassedSinceLastActivation = Mathf.Max(timePassedSinceLastActivation - ((1/rateAbs) * (float)timeStamps), 0f);
            }
        }
        else if(timePassedSinceLastActivation!= 0f){
            timePassedSinceLastActivation = 0f;
        }
    }

}
