
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{

    [Range(0.0f, 1.0f)]
    public float zoomRatio = 0.5f;

    public UnityEvent interactionStarted = new UnityEvent();

    public UnityEvent interactionStopped = new UnityEvent();

    [SerializeField]
    protected InputActionReference startInteractions;

    [SerializeField]
    protected List<Interaction> interactions;

    [HideInInspector]
    public ObjectManager objectManager;

    private bool allowInteration_;

    void Start(){
        allowInteration = false;
    }

    public bool allowInteration{
        get{
            return allowInteration_;
        }
        set{
            if(value){
                startInteractions.action.performed += OnStartInteractionsActionPerformed;
                startInteractions.action.canceled += OnStartInteractionsActionCanceled;
                DisableAllInteractions();
            }
            else{
                startInteractions.action.performed -= OnStartInteractionsActionPerformed;
                startInteractions.action.canceled -= OnStartInteractionsActionCanceled;
                DisableAllInteractions();
            }
        }
    }

    private void OnDestroy(){
        startInteractions.action.performed -= OnStartInteractionsActionPerformed;
        startInteractions.action.canceled -= OnStartInteractionsActionCanceled;
        DisableAllInteractions();    
    }

    void OnStartInteractionsActionPerformed(InputAction.CallbackContext context){
        foreach(Interaction interaction in interactions){
            if(interaction.interationEnabled){
                interaction.StartInteraction();
                interactionStarted.Invoke();
            }
        }
    }

    void OnStartInteractionsActionCanceled(InputAction.CallbackContext context){
        if(objectManager != null){
            objectManager.zoom = 0f;
        }
        foreach(Interaction interaction in interactions){
            if(interaction.interationEnabled){
                interaction.StopInteraction();
                interactionStopped.Invoke();
            }
        }
    }

    public void DisableAllInteractions(){
        foreach(Interaction interaction in interactions){
            interaction.DisableInteraction();
            interaction.DestroyAllControllerDistanceLine();
        }
        if(objectManager != null){
            objectManager.zoom = 0f;
        }
    }
}
