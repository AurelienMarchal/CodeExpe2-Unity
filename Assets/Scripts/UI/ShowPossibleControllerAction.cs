using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class ShowPossibleControllerAction : MonoBehaviour
{
    public Material controllerMaterial;
    public Material highlightedMaterial;

    private bool hasEntered = false;

    [SerializeField]
    private bool isOnLeftController;

    [SerializeField]
    private GameObject Button1;
    [SerializeField]
    private GameObject Button2;

    [SerializeField]
    private GameObject ThumbStick;
    private MeshRenderer rendererButton1;
    private MeshRenderer rendererButton2;
    private MeshRenderer rendererThumbStick;

    [SerializeField]
    private Image stickActionArrowLeftImage;

    [SerializeField]
    private Image stickActionArrowRightImage;

    [SerializeField]
    private Image stickActionArrowUpImage;

    [SerializeField]
    private Image stickActionArrowDownImage;

    [SerializeField]
    private Image stickActionRotateImage;

    [SerializeField]
    private Color visibleColor;

    [SerializeField]
    private Color transparentColor;
    
    [SerializeField]
    UIInputModule inputModule;

    private void Start() {
        rendererButton1 = Button1.GetComponent<MeshRenderer>();
        rendererButton2 = Button2.GetComponent<MeshRenderer>();
        rendererThumbStick = ThumbStick.GetComponent<MeshRenderer>();
        ResetAllControllerAction();
    }

    void Update(){
        if(hasEntered){
            Debug.Log($"Entered {EventSystem.current.currentSelectedGameObject}", this);
            hasEntered = false;
            if (EventSystem.current.currentSelectedGameObject != null){
                PossibleControllerAction possibleControllerAction = EventSystem.current.currentSelectedGameObject.GetComponent<PossibleControllerAction>();
                if(possibleControllerAction != null){
                    foreach(UIControllerAction uIControllerAction in possibleControllerAction.listControllerAction){
                        HandleControllerAction(uIControllerAction);
                    }
                }
            }
        }
    }


    private void OnEnable()
    {
        if (inputModule != null)
        {
            inputModule.pointerEnter += OnDevicePointerEnter;
            inputModule.pointerExit += OnDevicePointerExit;

        }
    }

    private void OnDisable()
    {
        if (inputModule != null)
        {
            inputModule.pointerEnter -= OnDevicePointerEnter;
            inputModule.pointerExit -= OnDevicePointerExit;
        }
    }

    // This method will fire after registering with the UIInputModule callbacks. The UIInputModule will
    // pass the PointerEventData for the device responsible for triggering the callback and can be used to
    // find the pointerId registered with the EventSystem for that device-specific event.
    private void OnDevicePointerEnter(GameObject selected, PointerEventData pointerData)
    {
        
        if (EventSystem.current.IsPointerOverGameObject(pointerData.pointerId))
        {
            Debug.Log("Entering with pointerID " + pointerData.pointerId);
            hasEntered = true;
        }
    }

    private void OnDevicePointerExit(GameObject selected, PointerEventData pointerData){
        if (EventSystem.current.IsPointerOverGameObject(pointerData.pointerId))
        {
            Debug.Log("Exiting with pointerID " + pointerData.pointerId);
            ResetAllControllerAction();
        }
    }

    private void HandleControllerAction(UIControllerAction action){
        if (action.GetType() == typeof(UIControllerButtonAction)){
            
            UIControllerButtonAction buttonAction = (UIControllerButtonAction)action;
            if((buttonAction.leftController && isOnLeftController) || (buttonAction.rightController && !isOnLeftController)){
                switch(buttonAction.buttonType){
                case ButtonType.Button1: 
                    rendererButton1.material = highlightedMaterial;
                    break;
                case ButtonType.Button2: 
                    rendererButton2.material = highlightedMaterial;
                    break;
                case ButtonType.Menu: break;
                case ButtonType.ThumbTrigger: break;
                case ButtonType.IndexTrigger: break;
                case ButtonType.Stick: break;
                }
            }
            else{
                return;
            }
            
        }
        else if(action.GetType() == typeof(UIControllerStickAction)){
            UIControllerStickAction stickAction = (UIControllerStickAction)action;
            if((stickAction.leftController && isOnLeftController) || (stickAction.rightController && !isOnLeftController)){
                rendererThumbStick.material = highlightedMaterial;
                switch(stickAction.stickAction){
                    case StickAction.Left:
                    stickActionArrowLeftImage.color = visibleColor;
                    break;
                    case StickAction.Right: 
                    stickActionArrowRightImage.color = visibleColor;
                    break;
                    case StickAction.Up:
                    stickActionArrowUpImage.color = visibleColor;
                    break;
                    case StickAction.Down :
                    stickActionArrowDownImage.color = visibleColor;
                    break;
                    case StickAction.Rotate:
                    stickActionRotateImage.color = visibleColor;
                    break;
                }
            }
            else{
                return;
            }
        }
    }


    private void ResetAllControllerAction(){
        rendererButton1.material = controllerMaterial;
        rendererButton2.material = controllerMaterial;
        rendererThumbStick.material = controllerMaterial;

        stickActionArrowLeftImage.color = transparentColor;
        stickActionArrowRightImage.color = transparentColor;
        stickActionArrowUpImage.color = transparentColor;
        stickActionArrowDownImage.color = transparentColor;
        stickActionRotateImage.color = transparentColor;
    }
}
