using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class ScrollBarController : MonoBehaviour
{   

    public UnityEvent clickedOutsideScrollbarHandle = new UnityEvent();
    public UnityEvent clickedInsideScrollbarHandle = new UnityEvent();

    public UnityEvent clickedOnPlus = new UnityEvent();

    public UnityEvent clickedOnMinus = new UnityEvent();

    public UnityEvent startedScrollBarInteraction = new UnityEvent();

    public UnityEvent stoppedScrollBarInteraction = new UnityEvent();

    ObjectManager objectManager_;
    public ObjectManager objectManager{
        get{
            return objectManager_;
        } 
        set{
            objectManager_ = value;
            if(objectManager !=  null){
                textMax.text = objectManager.maxTimeStamp.ToString();
            }
        }}

    [SerializeField]
    Scrollbar scrollbar_;

    [SerializeField]
    public Scrollbar scrollbar{get{
        return scrollbar_;
    }}

    [SerializeField]
    int jumpValue = 5;

    [SerializeField]
    float timeToActivateAutoSliding = 0.5f;

    [SerializeField]
    float timeBetweenAutoSlidings = 0.1f;

    float autoSlidingTimer = 0f;

    bool autoSlidingActivated = false;

    bool autoSlidingOnRight = false;

    float lastValue = 0;

    bool wasBlocked = false;

    Vector2 currentPointerPos = Vector2.zero;

    [SerializeField]
    Button buttonMinus;

    [SerializeField]
    Button buttonPlus;
    
    [SerializeField]
    TextMeshProUGUI textMax;


    // Start is called before the first frame update
    void Start(){
        scrollbar.onValueChanged.AddListener(OnValueChanged);
        buttonPlus.onClick.AddListener(PlusOne);
        buttonMinus.onClick.AddListener(MinusOne);
    }

    // Update is called once per frame
    void Update(){
        if(objectManager != null){

            scrollbar.size = CalculateSize(objectManager.numberOfObjectDisplayedOnTheFront, objectManager.maxTimeStamp);

            //Debug.Log($"Size : {scrollbar.size}, Zoom : {objectManager.zoom}");
            
            buttonMinus.gameObject.SetActive(scrollbar.IsInteractable() || wasBlocked);
            buttonPlus.gameObject.SetActive(scrollbar.IsInteractable() || wasBlocked);

            if(!scrollbar.IsInteractable()){
                UpdateValueFromObjectManager();
            }

            lastValue = scrollbar.value;

            if(wasBlocked){
                autoSlidingTimer += Time.deltaTime;
                if(!autoSlidingActivated){
                    if(autoSlidingTimer > timeToActivateAutoSliding){
                        autoSlidingActivated = true;
                        autoSlidingTimer = 0f;
                    }
                }
                else if(autoSlidingTimer > timeBetweenAutoSlidings){
                    if(!IsPointInRectTransform(currentPointerPos,
                        scrollbar.handleRect, out autoSlidingOnRight)){
                            scrollbar.value += (float)(jumpValue)/objectManager.maxTimeStamp * (autoSlidingOnRight?1f:-1f);
                        }
                    autoSlidingTimer = 0f;
                }
            }
            else if(autoSlidingTimer > 0f){
                autoSlidingTimer = 0f;
            }
            
        }
    }

    public void OnPointerDown(BaseEventData baseEventData){

        if(!scrollbar.IsInteractable()){
            return;
        }
        startedScrollBarInteraction.Invoke();
        
        PointerEventData pointerEventData = (PointerEventData)baseEventData;

        currentPointerPos = new Vector2(
            pointerEventData.pointerPressRaycast.worldPosition.x, 
            pointerEventData.pointerPressRaycast.worldPosition.y);

        if(IsPointInRectTransform(currentPointerPos,
            scrollbar.handleRect, out autoSlidingOnRight)){
            
            clickedInsideScrollbarHandle.Invoke();
        }
        else{
            
            clickedOutsideScrollbarHandle.Invoke();
            scrollbar.interactable = false;
            wasBlocked = true;
            scrollbar.value = lastValue + (float)(jumpValue)/objectManager.maxTimeStamp * (autoSlidingOnRight?1f:-1f);
        }
    }

    public void OnPointerUp(BaseEventData baseEventData){
        if(wasBlocked){
            scrollbar.interactable = true;
            wasBlocked = false;
            autoSlidingActivated = false;
        }

        if(scrollbar.IsInteractable()){
            stoppedScrollBarInteraction.Invoke();
        }
    }

    public void OnDrag(BaseEventData baseEventData){

        PointerEventData pointerEventData = (PointerEventData)baseEventData;

        currentPointerPos = new Vector2(
            pointerEventData.pointerPressRaycast.worldPosition.x, 
            pointerEventData.pointerPressRaycast.worldPosition.y);
        
    }


    bool IsPointInRectTransform(Vector2 point, RectTransform rt, out bool isOnRight){
        // Get the rectangular bounding box of your UI element
        Rect rect = rt.rect;

        // Get the left, right, top, and bottom boundaries of the rect
        float leftSide = rt.position.x - rect.width*rt.lossyScale.x / 2;
        float rightSide = rt.position.x + rect.width*rt.lossyScale.x  / 2;
        float topSide = rt.position.y + rect.height*rt.lossyScale.y  / 2;
        float bottomSide = rt.position.y - rect.height*rt.lossyScale.y / 2;

        //Debug.Log(leftSide + ", " + rightSide + ", " + topSide + ", " + bottomSide);

        isOnRight = point.x >= leftSide;

        // Check to see if the point is in the calculated bounds
        if (point.x >= leftSide &&
            point.x <= rightSide &&
            point.y >= bottomSide &&
            point.y <= topSide)
        {
            return true;
        }
        return false;
    }

    public void PlusOne(){
        objectManager.t ++;
        clickedOnPlus.Invoke();
        UpdateValueFromObjectManager();
    }

    public void MinusOne(){
        objectManager.t --;
        clickedOnMinus.Invoke();
        UpdateValueFromObjectManager();
    }


    public void UpdateValueFromObjectManager(){
        var value = (float)objectManager.t/(float)objectManager.maxTimeStamp;
        scrollbar.SetValueWithoutNotify(value);
    }

    private void OnValueChanged(float value){
        if(objectManager != null){
            objectManager.t = Mathf.CeilToInt(value * objectManager.maxTimeStamp);
        }
    }

    private static float CalculateSize(int numberOfObjectDisplayedOnTheFront, int maxTimeStamp){
        return (float)numberOfObjectDisplayedOnTheFront/maxTimeStamp;
    }

}
