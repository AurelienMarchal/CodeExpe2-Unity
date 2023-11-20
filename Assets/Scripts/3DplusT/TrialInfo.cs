using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class TrialInfo : MonoBehaviour
{

    [SerializeField]
    TextMeshProUGUI trialText;

    [SerializeField]
    TextMeshProUGUI participantText;

    [SerializeField]
    TextMeshProUGUI tiText;

    [SerializeField]
    TextMeshProUGUI visualizationText;

    [SerializeField]
    TextMeshProUGUI numberOfObjectsText;

    [SerializeField]
    TextMeshProUGUI taskText;

    [SerializeField]
    public Button startTrialButton;

    private ExpeBlock expeBlock_;

    public ExpeBlock expeBlock{
        set{
            expeBlock_ = value;
            if(value == null){
                trialText.text = $"Trial :";
                tiText.text = $"TI : ";
                visualizationText.text = $"Visualization : ";
                numberOfObjectsText.text = $"Number Of Objects : ";
                taskText.text = $"Task : ";
            }
            else{
                trialText.text = $"Trial {(value.currentTrial <= value.trainingTrialNum ? "(Training)" : "")} : {value.currentTrial}/{value.trainingTrialNum+value.monitoredTrialNum}";
                tiText.text = $"TI : {value.ti.ToString()}";
                visualizationText.text = $"Visualization : {value.visualization.ToString()}";
                numberOfObjectsText.text = $"Number Of Objects : {value.numberOfObjects}";
                taskText.text = $"Task : {value.task.ToString()}";
            }
        }
        get{
            return expeBlock_;
        }
    }

    private ExpeUser expeUser_;

    public ExpeUser expeUser{
        set{
            expeUser_ = value;
            if(value == null){
                participantText.text = "Participant ID :";
            }
            else{
                participantText.text = $"Participant ID : {value.userId}";
            }
        }
        get{
            return expeUser_;
        }
    }

    void Start()
    {
        expeBlock = null;
        expeUser = null;
        startTrialButton.interactable = false;
    }

    void Update(){
        if(expeBlock != null && expeUser != null){
            startTrialButton.interactable = true;
        }
        else{
            startTrialButton.interactable = false;
        }
    }
}
