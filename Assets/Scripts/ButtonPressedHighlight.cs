using UnityEngine.InputSystem;
using UnityEngine;

public class ButtonPressedHighlight : MonoBehaviour
{


    [SerializeField]
    Material controllerMaterial;

    [SerializeField]
    Material highlightedMaterial;

    [SerializeField]
    InputActionReference joyStickPos;

    [SerializeField]
    InputActionReference primaryButtonAction;

    [SerializeField]
    InputActionReference secondaryButtonAction;

    [SerializeField]
    InputActionReference triggerAxis;

    [SerializeField]
    InputActionReference gripAxis;

    [SerializeField]
    GameObject joyStick;

    [SerializeField]
    GameObject primaryButton;

    [SerializeField]
    GameObject secondaryButton;

    [SerializeField]
    GameObject trigger;

    [SerializeField]
    GameObject grip;

    [SerializeField]
    float triggerThreshold = 0.01f;

    [SerializeField]
    float gripThreshold = 0.01f;

    private const float joyStickRotationScale = 10f;

    // Start is called before the first frame update
    void Start()
    {
        primaryButtonAction.action.performed += OnPrimaryButtonPerformed;
        secondaryButtonAction.action.performed += OnSecondaryButtonPerformed;

        primaryButtonAction.action.canceled += OnPrimaryButtonCanceled;
        secondaryButtonAction.action.canceled += OnSecondaryButtonCanceled;
    }

    // Update is called once per frame
    void Update()
    {   
        Vector2 joyStickValue = joyStickPos.action.ReadValue<Vector2>();
        float triggerValue = triggerAxis.action.ReadValue<float>();
        float gripValue = gripAxis.action.ReadValue<float>();

        //Debug.Log($"TriggerValue : {triggerValue}, GripValue : {gripValue}");

        if(joyStickValue != Vector2.zero){
            joyStick.GetComponent<MeshRenderer>().material = highlightedMaterial;
        }
        else{
            joyStick.GetComponent<MeshRenderer>().material = controllerMaterial;
        }

        if(Mathf.Abs(triggerValue) > triggerThreshold){
            trigger.GetComponent<MeshRenderer>().material = highlightedMaterial;
        }
        else{
            trigger.GetComponent<MeshRenderer>().material = controllerMaterial;
        }

        if(Mathf.Abs(gripValue) > gripThreshold){
            grip.GetComponent<MeshRenderer>().material = highlightedMaterial;
        }
        else{
            grip.GetComponent<MeshRenderer>().material = controllerMaterial;
        }

        joyStick.transform.eulerAngles = new Vector3(transform.parent.eulerAngles.x + joyStickValue.y * joyStickRotationScale, 
                                                        transform.parent.eulerAngles.y, 
                                                        transform.parent.eulerAngles.z - joyStickValue.x * joyStickRotationScale);
    }

    void OnDestroy(){
        primaryButtonAction.action.performed -= OnPrimaryButtonPerformed;
        secondaryButtonAction.action.performed -= OnSecondaryButtonPerformed;
        primaryButtonAction.action.canceled -= OnPrimaryButtonCanceled;
        secondaryButtonAction.action.canceled -= OnSecondaryButtonCanceled;
    }

    void OnPrimaryButtonPerformed(InputAction.CallbackContext context){
        primaryButton.GetComponent<MeshRenderer>().material = highlightedMaterial;
    }

    void OnPrimaryButtonCanceled(InputAction.CallbackContext context){
        primaryButton.GetComponent<MeshRenderer>().material = controllerMaterial;
    }

    void OnSecondaryButtonPerformed(InputAction.CallbackContext context){
        secondaryButton.GetComponent<MeshRenderer>().material = highlightedMaterial;
    }

    void OnSecondaryButtonCanceled(InputAction.CallbackContext context){
        secondaryButton.GetComponent<MeshRenderer>().material = controllerMaterial;
    }
}
