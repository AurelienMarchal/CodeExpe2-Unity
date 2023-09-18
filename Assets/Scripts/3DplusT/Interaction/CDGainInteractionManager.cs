

public class CDGainInteractionManager : InteractionManager
{

    void Update(){
        if(objectManager != null){
            foreach(Interaction interaction in interactions){
                CDGainInteraction cDGainInteraction = interaction as CDGainInteraction;
                if (cDGainInteraction != null)
                {
                    if(cDGainInteraction.interationEnabled){
                        
                        var timeIncrease = cDGainInteraction.CalculateTimeIncrease();
                        
                        objectManager.t -= timeIncrease;
                        var percentageOfMaxGain = 0f;
                        if(cDGainInteraction is OrthozoomButtonCDGainInteraction){
                            var ortozoomButtonCDGainInteraction = cDGainInteraction as OrthozoomButtonCDGainInteraction;
                            if(ortozoomButtonCDGainInteraction.cDGain == 0f){
                                percentageOfMaxGain = 0f;
                            }
                            else if(ortozoomButtonCDGainInteraction.cDGain == ortozoomButtonCDGainInteraction.primaryButtonGainCoef){
                                percentageOfMaxGain = 0f;
                            }
                            else if(ortozoomButtonCDGainInteraction.cDGain == ortozoomButtonCDGainInteraction.secondaryButtonGainCoef){
                                percentageOfMaxGain = 1f;
                            }
                            else if(ortozoomButtonCDGainInteraction.cDGain == ortozoomButtonCDGainInteraction.primaryButtonGainCoef + ortozoomButtonCDGainInteraction.secondaryButtonGainCoef){
                                percentageOfMaxGain = 1f;
                            }
                        }
                        else{
                            percentageOfMaxGain = (cDGainInteraction.cDGain - cDGainInteraction.minGain)/(cDGainInteraction.maxGain - cDGainInteraction.minGain);
                        }

                        
                        objectManager.zoom = percentageOfMaxGain * objectManager.maxTimeStamp * zoomRatio;
                    }
                }
            }
        }
    }
}
