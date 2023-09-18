using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine;
using TMPro;

[System.Serializable]
public class IntBoolBoolIntEvent : UnityEvent<int, bool, bool, int>
{
}

public class ObjectData : MonoBehaviour
{

    protected TextMeshProUGUI numberText;

    [SerializeField]
    protected XRSimpleInteractable simpleInteractable;

    [SerializeField]
    Canvas dwellTimeCanvas;

    [SerializeField]
    Image dwellImage;

    [SerializeField]
    Image goodSelectImage;

    [SerializeField]
    Image badSelectImage;

    private int number_; // field
    public int number{
        get { return number_; }   // get method
        set { number_ = value;
            patternPos = value;
            if(numberText != null)
            numberText.text = number_.ToString(); }  // set method
    }

    public bool isEyeHovered{get; set; }

    public int patternPos{get; set;}

    public IntBoolBoolIntEvent objectSelectedEvent = new IntBoolBoolIntEvent();

    [SerializeField]
    InputActionReference grabObjectInputAction;

    public bool isGoal{
        get; set;
    }

    protected bool validated_;

    public virtual bool validated{
        get{
            return validated_;
        } 
        set{
            validated_ = value;
        }
    }

    public bool canBeSelected{get; set;}

    [SerializeField]
    protected float timeBeforeDwell = 0.5f;

    protected float gazeHoverTimer = 0f;

    private void Awake(){
        numberText = GetComponentInChildren<TextMeshProUGUI>();
        //simpleInteractable = GetComponent<XRSimpleInteractable>();
        grabObjectInputAction.action.performed += OnGrabObjectInputActionTriggered;
        dwellTimeCanvas.gameObject.SetActive(false);
        canBeSelected = false;
        gazeHoverTimer = -timeBeforeDwell;
    }

    private void OnEnable(){
        dwellTimeCanvas.gameObject.SetActive(false);
        gazeHoverTimer = -timeBeforeDwell;
    }

    private void OnGrabObjectInputActionTriggered(InputAction.CallbackContext context)
    {

        if(CheckIfRayHovered()){
            objectSelectedEvent.Invoke(number, isGoal, false, patternPos);
        }

    }

    public void OnSelect(BaseInteractionEventArgs args){
        Debug.Log($"Object {number} selected");
    }
    
    private bool CheckIfGazeHovered(){
        var gazeHovered = false;
        foreach(IXRHoverInteractor hoverInteractor in simpleInteractable.interactorsHovering){
            if(hoverInteractor.GetType() == typeof(XRGazeInteractor)){
                gazeHovered = true;
            }
        }
        return gazeHovered;
    }
    
    

    private bool CheckIfRayHovered(){
        var rayHovered = false;
        foreach(IXRHoverInteractor hoverInteractor in simpleInteractable.interactorsHovering){
            if(hoverInteractor.GetType() == typeof(XRRayInteractor)){
                rayHovered = true;
            }
        }
        return rayHovered;
    }


    void Update(){
        numberText.color = Color.white;

        if(CheckIfRayHovered()){
            numberText.color = Color.blue;
        }
        if(CheckIfGazeHovered() || isEyeHovered){
            //numberText.color = Color.red;
            if(!validated && canBeSelected){
                if(gazeHoverTimer <= 0f){
                    dwellTimeCanvas.gameObject.SetActive(true);
                    dwellImage.gameObject.SetActive(true);
                    goodSelectImage.gameObject.SetActive(false);
                    badSelectImage.gameObject.SetActive(false);
                }

                gazeHoverTimer += Time.deltaTime;

                if(gazeHoverTimer > simpleInteractable.gazeTimeToSelect){
                    objectSelectedEvent.Invoke(number, isGoal, true, patternPos);
                    gazeHoverTimer = -(timeBeforeDwell*3);
                }
                
                else if(gazeHoverTimer > simpleInteractable.gazeTimeToSelect - 0.1){
                    dwellImage.gameObject.SetActive(false);
                    if(isGoal){
                        goodSelectImage.gameObject.SetActive(true);
                    }
                    else{
                        badSelectImage.gameObject.SetActive(true);
                    }
                    
                }
                else{
                    dwellImage.fillAmount =  gazeHoverTimer / simpleInteractable.gazeTimeToSelect;
                }
            }

            else{
                gazeHoverTimer = -timeBeforeDwell;
                dwellTimeCanvas.gameObject.SetActive(false);
            }
        }
        else{
            gazeHoverTimer = -timeBeforeDwell;
            dwellTimeCanvas.gameObject.SetActive(false);
        }
    }

    void OnDestroy(){
        objectSelectedEvent.RemoveAllListeners();
    }
}
