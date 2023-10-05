using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using TMPro;
using UnityEngine.UI;

public class ExpeManager : MonoBehaviour
{

    ExpeState expeState = ExpeState.Wait;

    private ExpeUser currentUser_ = null;
    public ExpeUser currentUser{
        get{
            return currentUser_;
        }
        set{
            currentUser_ = value;
            trialInfo.expeUser = value;
            if(expeState != ExpeState.Wait){
                StopCurrentTrial();
            }
        }
    }

    private ExpeBlock currentBlock_ = null;
    public ExpeBlock currentBlock{
        get{
            return currentBlock_;
        }
        set{
            currentBlock_ = value;
            trialInfo.expeBlock = value;
            FillPatternSequence();
            FillLocateSequence();
            if(expeState != ExpeState.Wait){
                StopCurrentTrial();
            }
        }
    }
    ExpeTrial currentTrial = null;

    //Timers
    float currentTime = 0f;
    float timeBeforeFirstMove = 0f;
    float timeGrab = 0f;
    float timeForFinalSelection = 0f;
    float timeUsingInteraction = 0f;

    float timeToValidatePattern1 = 0f;
    float timeToValidatePattern2 = 0f;
    float timeToValidatePattern3 = 0f;
    float timeToValidatePattern4 = 0f;
    float timeToValidatePattern5 = 0f;
    float timeAfterPattern5Validated = 0f;

    float timeBetweenLastPatternValidationAndButtonFinishClicked = 0f;

    int numberOfInteractionStarted = 0;

    bool currentlyUsingInteraction = false;

    //Counters
    int numberOfRetrieve = 0;
    int numberOfBadSelect = 0;

    int numberOfClickOutsideScrollbarHandler = 0;
    int numberOfClickInsideScrollbarHandler = 0;

    int numberOfClickOnPlus = 0;
    int numberOfClickOnMinus = 0;

    int numberOfClickOnA = 0;

    int numberOfClickOnB = 0;

    int numberOfClickOnBothButton = 0;

    float currentHeadAmplitude = 0f;

    float maxHeadAmplitude = 0f;

    float maxZoom = 0f;
    float? maxControllerDistance = 0f;
    float? maxOrthoDistance = 0f;
    float? maxCDGain = 0f;
    float currentZoom = 0f;
    float? currentOrthoDistance = 0f;
    float? currentCdGain = 0f;

    float? currentControllerDistance = 0f;
    float? currentJoystickX = 0f;
    
    float zoomGrab = 0f;
    float zoomForFinalSelection = 0f;
    
    float zoomToValidatePattern1 = 0f;
    float zoomToValidatePattern2 = 0f;
    float zoomToValidatePattern3 = 0f;
    float zoomToValidatePattern4 = 0f;
    float zoomToValidatePattern5 = 0f;

    float? controllerDistanceGrab = 0f;
    float? controllerDistanceForFinalSelection = 0f;

    float? controllerDistanceToValidatePattern1 = 0f;
    float? controllerDistanceToValidatePattern2 = 0f;
    float? controllerDistanceToValidatePattern3 = 0f;
    float? controllerDistanceToValidatePattern4 = 0f;
    float? controllerDistanceToValidatePattern5 = 0f;

    [SerializeField]
    int[] distances = {44, 88};

    List<int> distanceSequence = new List<int>();

    List<bool> directionSequence = new List<bool>();

    int currentDistance = 0;
    bool currentDirection = false;

    [SerializeField]
    int rangeToSelectObject = 2;

    [SerializeField]
    int patternSize = 3;

    [SerializeField]
    int minSpacingBetweenPatterns = 15;

    int[] patternPoses = {0, 0, 0, 0, 0};

    int[] patternFoundPoses = {0, 0, 0, 0, 0};

    int numPatternValidated = 0;

    bool didFindAllPatterns = false;

    [SerializeField]
    int[] numbersOfPattern = {1, 5};

    List<int> numberOfPatternSequence = new List<int>();

    [SerializeField]
    PatternType[] patternTypes = {PatternType.VerticalColor1, PatternType.HorizontalColor1};

    [SerializeField]
    float patternTaskMaxDuration;

    bool wasTimedOut = false;
    List<PatternType> patternTypeSequence = new List<PatternType>();

    PatternType currentPatternType = PatternType.HorizontalColor1;

    int currentNumberOfPattern = 0;

    int numberToLocate = -1;

    [SerializeField]
    TrialInfo trialInfo;

    ResumeLogger resumeLogger;

    FullLogger fullLogger;

    [SerializeField]
    float timeBetweenFullLogs = 0.2f;

    float fullLogTimer = 0f;

    [SerializeField]
    Transform headTransform;

    Vector3 startHeadRotation = Vector3.zero;

    Vector3 startHeadForewardVector = Vector3.forward;

    ObjectManager objectManager;

    [SerializeField]    
    ObjectManager objectManagerCircle;

    [SerializeField]
    ObjectManager objectManagerPerspectiveWall;

    [SerializeField]
    ObjectManager objectManagerSlider;

    [SerializeField]
    ObjectManager objectManagerHelice;

    [SerializeField]
    InteractionManager rateControlInteractionManager;

    [SerializeField]
    InteractionManager cDGainInteractionManager;

    [SerializeField]
    Interaction orthozoomABInteraction;

    [SerializeField]
    Interaction orthozoomOneHandDepthInteraction;

    [SerializeField]
    Interaction orthozoomOneHandHeightInteraction;

    [SerializeField]
    Interaction orthozoomJoystickControllerDistInteraction;

    [SerializeField]
    Interaction orthozoomDepthJoystickInteraction;

    [SerializeField]
    ScrollBarController scrollBarController;

    GoalNumber goalNumber;

    [SerializeField]
    GameObject goalNumberCanvas;

    [SerializeField]
    ObjectManager patternDisplayObjectManager;

    [SerializeField]
    Button finishPatternTaskButton;

    [SerializeField]
    GameObject finishPatternTaskButtonCanvas;


    [SerializeField]
    ActionBasedControllerManager leftActionBasedControllerManager;

    [SerializeField]
    ActionBasedControllerManager rightActionBasedControllerManager;

    [SerializeField]
    bool showDebugInfo = true;

    [SerializeField]
    GameObject debugCanvas;

    [SerializeField]
    TextMeshProUGUI debugTextMesh;

    int startingPos;

    bool objectWasSelectedThisFrame = false;

    void Start()
    {
        resumeLogger = new ResumeLogger();
        fullLogger = new FullLogger();

        trialInfo.startTrialButton.onClick.AddListener(OnStartTrialButtonPressed);
        finishPatternTaskButton.onClick.AddListener(OnFinishPatternTaskButtonClicked);

        patternDisplayObjectManager.gameObject.SetActive(false);
        goalNumber = goalNumberCanvas.GetComponent<GoalNumber>();
        
        DisableEverything();
    }

    // Update is called once per frame
    void Update()
    {
        debugCanvas.SetActive(showDebugInfo);

        if(currentUser != null){

            switch(expeState){
                case ExpeState.Wait:
                    //Un peu bizarre
                    if(currentBlock != null){
                        if (currentTrial == null){
                            // Go to next trial
                            currentTrial = currentBlock.generateNextTrial();
                            if(currentTrial == null){
                                //Block fini
                                trialInfo.expeBlock = null;
                            }
                            else{
                                SetupTrial();
                            }
                        }
                    }

                break;

                case ExpeState.ReadytoStart: break;

                case ExpeState.ExpeStarted:
                    timeBeforeFirstMove += Time.deltaTime;
                    if(objectManager.t != startingPos){
                        objectManager.allowSelection = true;
                        expeState = ExpeState.AfterFirstMove;
                    }
                    
                    break;

                case ExpeState.AfterFirstMove:
                    timeGrab += Time.deltaTime;
                    timeToValidatePattern1 += Time.deltaTime;
    
                    if(Math.Abs(numberToLocate - objectManager.t) <= rangeToSelectObject && currentBlock.task == Task.Locate){
                        expeState = ExpeState.ObjectInSelectionArea;
                        zoomGrab = GetCurrentZoom();
                        controllerDistanceGrab = GetCurrentControllerDistance();
                    }

                    break;
                    

                case ExpeState.ObjectInSelectionArea:
                    timeForFinalSelection += Time.deltaTime;
                    break;

                case ExpeState.Pattern1Validated:
                    timeToValidatePattern2 += Time.deltaTime;
                    break;
                
                case ExpeState.Pattern2Validated:
                    timeToValidatePattern3 += Time.deltaTime;
                    break;

                case ExpeState.Pattern3Validated:
                    timeToValidatePattern4 += Time.deltaTime;
                    
                    break;

                case ExpeState.Pattern4Validated:
                    timeToValidatePattern5 += Time.deltaTime;
                    break;

                case ExpeState.Pattern5Validated:
                    timeAfterPattern5Validated += Time.deltaTime;
                    break;

                case ExpeState.TaskCompleted: break;
                default: break;
            }

            if(currentBlock != null){
                switch(currentBlock.task){
                    case Task.Locate:
                        currentTime = timeBeforeFirstMove + timeForFinalSelection + timeGrab;
                        break;
                    case Task.Pattern:
                        currentTime = timeBeforeFirstMove + timeToValidatePattern1 + timeToValidatePattern2 + timeToValidatePattern3 + timeToValidatePattern4 + timeToValidatePattern5 + timeAfterPattern5Validated;
                        break;
                    default: 
                        currentTime = 0f;
                        break;
                }
            }


            if(expeState != ExpeState.Wait && expeState != ExpeState.ReadytoStart && expeState != ExpeState.TaskCompleted){
                if(currentlyUsingInteraction){
                    timeUsingInteraction += Time.deltaTime;
                }
                CalculateHeadAmplitude();
                HandleInteractionLogVariables();
                HandleFullLogTimer();

                if(currentTime > patternTaskMaxDuration && currentBlock.task == Task.Pattern){
                    Debug.Log("Max pattern Time reached");
                    wasTimedOut = true;
                    
                    OnFinishPatternTaskButtonClicked();
                }
            }
            
        }
        objectWasSelectedThisFrame = false;
        HandleDebugInfo();
    }

    void CalculateHeadAmplitude(){
        currentHeadAmplitude = Vector3.Angle(startHeadForewardVector, headTransform.forward);
        if(currentHeadAmplitude > maxHeadAmplitude){
            maxHeadAmplitude = currentHeadAmplitude;
        }
    }

    
    Vector3 CalculateCurrentHeadRotation(){
        var result = startHeadRotation - headTransform.rotation.eulerAngles;

        if(result.x > 180){
            result.x-=360;
        }
        if(result.y > 180){
            result.y-=360;
        }
        if(result.z > 180){
            result.z-=360;
        }

        
        if(result.x < -180){
            result.x+=360;
        }
        if(result.y < -180){
            result.y+=360;
        }
        if(result.z < -180){
            result.z+=360;
        }

        return result;

    }


    public void OnStartTrialButtonPressed(){
        StartCurrentTrial();
    }

    void StartCurrentTrial(){
        if(currentTrial != null){
            Debug.Log("Starting Trial");
            StartTask();
            SetupInteraction();

            startHeadRotation = headTransform.localRotation.eulerAngles;
            startHeadForewardVector = headTransform.forward;

            trialInfo.gameObject.SetActive(false);
            
            controllerDistanceGrab = GetCurrentControllerDistance();
            controllerDistanceForFinalSelection = GetCurrentControllerDistance();

            controllerDistanceToValidatePattern1 = GetCurrentControllerDistance();
            controllerDistanceToValidatePattern2 = GetCurrentControllerDistance();
            controllerDistanceToValidatePattern3 = GetCurrentControllerDistance();
            controllerDistanceToValidatePattern4 = GetCurrentControllerDistance();
            controllerDistanceToValidatePattern5 = GetCurrentControllerDistance();

            expeState = ExpeState.ExpeStarted;
        }
    }

    void StopCurrentTrial(){
        if(currentTrial != null){
            Debug.Log("Stopping Trial");
            LogCurrentTrial();
            DisableEverything();
            currentTrial = null;
            goalNumber.numberToLocate = -1;
            numberToLocate = -1;
            trialInfo.gameObject.SetActive(true);
        }
    }

    void OnGoalReached(){
        expeState = ExpeState.TaskCompleted;
        StopCurrentTrial();
    }

    void OnObjectSelected(int num, bool isGoal, bool isSelect, int patternPos){

        switch(currentBlock.task){
            case Task.Locate: 
                if(!objectWasSelectedThisFrame){
                    objectWasSelectedThisFrame = true;
                    if(isSelect){
                        if(isGoal){
                            zoomForFinalSelection = GetCurrentZoom();
                            controllerDistanceForFinalSelection = GetCurrentControllerDistance();
                            OnGoalReached();
                        }
                        else{
                            numberOfBadSelect ++;
                        }
                    }

                    else if(!isSelect && currentBlock.ti == TI.Slider){
                        //Grab Object/Put in front of the user
                        objectManager.t = num;
                        scrollBarController.UpdateValueFromObjectManager();
                        numberOfRetrieve ++;
                    }
                }
                break;

            case Task.Pattern:
                if(isSelect){
                    if(isGoal){
                        var wasValidatedBefore = objectManager.ValidateObject(num, true);
                        if(!wasValidatedBefore){
                            numPatternValidated ++;
                            if(numPatternValidated >= currentNumberOfPattern){
                                didFindAllPatterns = true;
                            }
                            switch(numPatternValidated){
                                case 1: 
                                    expeState = ExpeState.Pattern1Validated; 
                                    patternFoundPoses[0] = patternPos; 
                                    zoomToValidatePattern1 = GetCurrentZoom();
                                    controllerDistanceToValidatePattern1 = GetCurrentControllerDistance();
                                    break;
                                case 2: 
                                    expeState = ExpeState.Pattern2Validated; 
                                    patternFoundPoses[1] = patternPos; 
                                    zoomToValidatePattern2 = GetCurrentZoom();
                                    controllerDistanceToValidatePattern2 = GetCurrentControllerDistance();
                                    break;
                                case 3: 
                                    expeState = ExpeState.Pattern3Validated; 
                                    patternFoundPoses[2] = patternPos; 
                                    zoomToValidatePattern3 = GetCurrentZoom();
                                    controllerDistanceToValidatePattern3 = GetCurrentControllerDistance();
                                    break;
                                case 4: 
                                    expeState = ExpeState.Pattern4Validated; 
                                    patternFoundPoses[3] = patternPos;
                                    zoomToValidatePattern4 = GetCurrentZoom();
                                    controllerDistanceToValidatePattern4 = GetCurrentControllerDistance();
                                    break;
                                case 5: 
                                    expeState = ExpeState.Pattern5Validated; 
                                    patternFoundPoses[4] = patternPos;
                                    zoomToValidatePattern5 = GetCurrentZoom();
                                    controllerDistanceToValidatePattern5 = GetCurrentControllerDistance();
                                    break;
                                default:
                                    Debug.LogError($"Impossible number of pattern validated {numPatternValidated}");
                                    numPatternValidated = 5;
                                    break;
                            }
                        }
                        else{
                            numberOfBadSelect ++;
                        }
                    }
                    else{
                        numberOfBadSelect ++;
                    }
                }
                else if(!(Math.Abs(num - objectManager.t) <= rangeToSelectObject) && currentBlock.ti == TI.Slider){
                    objectManager.t = num;
                    scrollBarController.UpdateValueFromObjectManager();
                    numberOfRetrieve ++;
                }

                break;


        }
    }

    void OnInteractionStarted(){
        //Debug.Log("Interaction started");
        numberOfInteractionStarted ++;
        currentlyUsingInteraction  = true;
    }

    void OnInteractionStopped(){
        //Debug.Log("Interaction stopped");
        currentlyUsingInteraction = false;
    }

    void OnFinishPatternTaskButtonClicked(){
        if(!wasTimedOut){
            if(currentBlock.ti == TI.OrthozoomDepth || currentBlock.ti == TI.OrthozoomHeight ||currentBlock.ti == TI.OrthozoomJoystickControllerDist){
                numberOfInteractionStarted --;
            }
        }
        

        switch(expeState){
            case ExpeState.ExpeStarted:
                timeBetweenLastPatternValidationAndButtonFinishClicked = timeBeforeFirstMove;
                break;

            case ExpeState.AfterFirstMove:
                timeBetweenLastPatternValidationAndButtonFinishClicked = timeToValidatePattern1;
                timeToValidatePattern1 = 0f;
                break;

            case ExpeState.Pattern1Validated:
                timeBetweenLastPatternValidationAndButtonFinishClicked = timeToValidatePattern2;
                timeToValidatePattern2 = 0f;
                break;
                
            case ExpeState.Pattern2Validated:
                timeBetweenLastPatternValidationAndButtonFinishClicked = timeToValidatePattern3;
                timeToValidatePattern3 = 0f;
                break;

            case ExpeState.Pattern3Validated:
                timeBetweenLastPatternValidationAndButtonFinishClicked = timeToValidatePattern4;
                timeToValidatePattern4 = 0f;
                break;

            case ExpeState.Pattern4Validated:
                timeBetweenLastPatternValidationAndButtonFinishClicked = timeToValidatePattern5;
                timeToValidatePattern5 = 0f;
                break;

            case ExpeState.Pattern5Validated:
                timeBetweenLastPatternValidationAndButtonFinishClicked = timeAfterPattern5Validated;
                break;

            default:
                Debug.LogError("Impossible state when clicked on finish pattern task button was clicked");
                break;
        }

        OnGoalReached();
    }

    void DisableObjectManager(ObjectManager objectManager_){
        objectManager_.zoom = 0f;
        objectManager_.objectSelectedEvent.RemoveAllListeners();
        objectManager_.ResetAllIsGoal();
        objectManager_.ResetAllValidated();
        objectManager_.ResetAllPatternPos();
        objectManager_.allowSelection = false;
        objectManager_.gameObject.SetActive(false);
    }

    void DisableAllVisualization(){

        DisableObjectManager(objectManagerCircle);
        DisableObjectManager(objectManagerPerspectiveWall);
        DisableObjectManager(objectManagerSlider);
        DisableObjectManager(objectManagerHelice);

        objectManager = null;
        rateControlInteractionManager.objectManager = null;
        cDGainInteractionManager.objectManager = null;
        scrollBarController.objectManager = null;
    }

    void DisableAllInteractions(){
        rateControlInteractionManager.DisableAllInteractions();
        cDGainInteractionManager.DisableAllInteractions();

        cDGainInteractionManager.interactionStarted.RemoveAllListeners();
        rateControlInteractionManager.interactionStarted.RemoveAllListeners();
        cDGainInteractionManager.interactionStopped.RemoveAllListeners();
        rateControlInteractionManager.interactionStopped.RemoveAllListeners();

        rateControlInteractionManager.allowInteration = false;
        cDGainInteractionManager.allowInteration = false;

        scrollBarController.scrollbar.interactable = false;

        currentlyUsingInteraction = false;
        
        scrollBarController.clickedInsideScrollbarHandle.RemoveAllListeners();
        scrollBarController.clickedOutsideScrollbarHandle.RemoveAllListeners();
        scrollBarController.startedScrollBarInteraction.RemoveAllListeners();
        scrollBarController.stoppedScrollBarInteraction.RemoveAllListeners();
        scrollBarController.clickedOnPlus.RemoveAllListeners();
        scrollBarController.clickedOnMinus.RemoveAllListeners();

        (orthozoomABInteraction as OrthozoomButtonCDGainInteraction).primaryButtonPressedEvent.RemoveAllListeners();
        (orthozoomABInteraction as OrthozoomButtonCDGainInteraction).secondaryButtonPressedEvent.RemoveAllListeners();
        (orthozoomABInteraction as OrthozoomButtonCDGainInteraction).bothButtonPressedEvent.RemoveAllListeners();
    }

    void DisableAllTaskComponent(){
        goalNumberCanvas.SetActive(false);
        patternDisplayObjectManager.gameObject.SetActive(false);
        finishPatternTaskButtonCanvas.SetActive(false);
    }

    void DisableEverything(){
        Debug.Log("Disabling Everything");
        DisableAllTaskComponent();
        DisableAllVisualization();
        DisableAllInteractions();
        expeState = ExpeState.Wait;
    }

    void SetupTrial(){
        Debug.Log("Setting up Trial");
        SetupVisualization();
        //SetupInteraction();
        SetupTask();

        trialInfo.expeBlock = currentBlock;

        //Reset Timers
        currentTime = 0f;
        timeBeforeFirstMove = 0f;
        timeGrab = 0f;
        timeForFinalSelection = 0f;
        fullLogTimer = 0f;
        timeUsingInteraction = 0f;

        timeToValidatePattern1 = 0f;
        timeToValidatePattern2 = 0f;
        timeToValidatePattern3 = 0f;
        timeToValidatePattern4 = 0f;
        timeToValidatePattern5 = 0f;
        timeAfterPattern5Validated = 0f;
        timeBetweenLastPatternValidationAndButtonFinishClicked = 0f;

        maxHeadAmplitude = 0f;
        currentHeadAmplitude = 0f;

        didFindAllPatterns = false;
        wasTimedOut = false;

        patternPoses[0] = 0; 
        patternPoses[1] = 0;
        patternPoses[2] = 0; 
        patternPoses[3] = 0; 
        patternPoses[4] = 0;

        patternFoundPoses[0] = 0;
        patternFoundPoses[1] = 0;
        patternFoundPoses[2] = 0;
        patternFoundPoses[3] = 0;
        patternFoundPoses[4] = 0;

        currentZoom = 0f;
        currentOrthoDistance = 0f;
        currentCdGain = 0f;

        currentControllerDistance = 0f;
        currentJoystickX = 0f;
        
        maxZoom = 0f;
        maxControllerDistance = 0f;
        maxOrthoDistance = 0f;
        maxCDGain = 0f;
    
        zoomGrab = 0f;
        zoomForFinalSelection = 0f;
    
        zoomToValidatePattern1 = 0f;
        zoomToValidatePattern2 = 0f;
        zoomToValidatePattern3 = 0f;
        zoomToValidatePattern4 = 0f;
        zoomToValidatePattern5 = 0f;

        numberOfBadSelect = 0;
        numberOfRetrieve = 0;
        numberOfClickInsideScrollbarHandler = 0;
        numberOfClickOutsideScrollbarHandler = 0;
        numberOfInteractionStarted = 0;
        numberOfClickOnPlus = 0;
        numberOfClickOnMinus = 0;

        numberOfClickOnA = 0;
        numberOfClickOnB = 0;
        numberOfClickOnBothButton = 0;

        expeState = ExpeState.ReadytoStart;
        
    }

    void SetupObjectManager(ObjectManager objectManager_){
        objectManager_.gameObject.SetActive(true);
        objectManager = objectManager_;
        rateControlInteractionManager.objectManager = objectManager_;
        cDGainInteractionManager.objectManager = objectManager_;
        goalNumber.objectManager = objectManager_;
        scrollBarController.objectManager = objectManager_;
        objectManager.rangeToSelect = rangeToSelectObject;
        objectManager_.objectSelectedEvent.AddListener(OnObjectSelected);
    }

    void SetupVisualization(){
        DisableAllVisualization();
        if(currentBlock != null){
            switch(currentBlock.visualization){
                case Visualization.Circle:
                    SetupObjectManager(objectManagerCircle);
                    break;
                case Visualization.PerspectiveWall:
                    SetupObjectManager(objectManagerPerspectiveWall);
                    break;
                case Visualization.Helice:
                    SetupObjectManager(objectManagerHelice);
                    break;
                case Visualization.NoTimeline:
                    SetupObjectManager(objectManagerSlider);
                    break;
                default:
                    SetupObjectManager(objectManagerSlider);
                    break;
            }
        }
    }

    void OnClickedInsideScrollbarHandle(){
        numberOfClickInsideScrollbarHandler ++;
    }

    void OnClickedOutsideScrollbarHandle(){
        numberOfClickOutsideScrollbarHandler ++;
    }


    void SetupInteraction(){
        rateControlInteractionManager.DisableAllInteractions();
        cDGainInteractionManager.DisableAllInteractions();
        rateControlInteractionManager.allowInteration = true;
        cDGainInteractionManager.allowInteration = true;
        if(currentBlock != null){
            switch(currentBlock.ti){
                case TI.AB:             
                    orthozoomABInteraction.EnableInteraction();       
                    (orthozoomABInteraction as OrthozoomButtonCDGainInteraction).primaryButtonPressedEvent.AddListener(() => {numberOfClickOnA++;});
                    (orthozoomABInteraction as OrthozoomButtonCDGainInteraction).secondaryButtonPressedEvent.AddListener(() => {numberOfClickOnB++;});
                    (orthozoomABInteraction as OrthozoomButtonCDGainInteraction).bothButtonPressedEvent.AddListener(() => {numberOfClickOnBothButton++;});
                    break;
                case TI.OrthozoomDepthJoystick:       orthozoomDepthJoystickInteraction.EnableInteraction();             break;
                case TI.OrthozoomDepth:
                    orthozoomOneHandDepthInteraction.EnableInteraction(); 
                    cDGainInteractionManager.interactionStarted.AddListener(OnInteractionStarted);
                    rateControlInteractionManager.interactionStarted.AddListener(OnInteractionStarted);
                    cDGainInteractionManager.interactionStopped.AddListener(OnInteractionStopped);
                    rateControlInteractionManager.interactionStopped.AddListener(OnInteractionStopped);
                    break;
                    
                case TI.OrthozoomHeight:
                    orthozoomOneHandHeightInteraction.EnableInteraction(); 
                    cDGainInteractionManager.interactionStarted.AddListener(OnInteractionStarted);
                    rateControlInteractionManager.interactionStarted.AddListener(OnInteractionStarted);
                    cDGainInteractionManager.interactionStopped.AddListener(OnInteractionStopped);
                    rateControlInteractionManager.interactionStopped.AddListener(OnInteractionStopped);
                    break;

                case TI.OrthozoomJoystickControllerDist:
                    orthozoomJoystickControllerDistInteraction.EnableInteraction(); 
                    cDGainInteractionManager.interactionStarted.AddListener(OnInteractionStarted);
                    rateControlInteractionManager.interactionStarted.AddListener(OnInteractionStarted);
                    cDGainInteractionManager.interactionStopped.AddListener(OnInteractionStopped);
                    rateControlInteractionManager.interactionStopped.AddListener(OnInteractionStopped);
                    break;
                case TI.Slider:
                    scrollBarController.scrollbar.interactable = true;
                    scrollBarController.clickedInsideScrollbarHandle.AddListener(OnClickedInsideScrollbarHandle);
                    scrollBarController.clickedOutsideScrollbarHandle.AddListener(OnClickedOutsideScrollbarHandle);
                    scrollBarController.startedScrollBarInteraction.AddListener(OnInteractionStarted);
                    scrollBarController.stoppedScrollBarInteraction.AddListener(OnInteractionStopped);
                    scrollBarController.clickedOnPlus.AddListener(() => {numberOfClickOnPlus++;});
                    scrollBarController.clickedOnMinus.AddListener(() => {numberOfClickOnMinus++;});
                    break;
                default: break;
            }
        }
    }


    void FillLocateSequence(){
        
        distanceSequence.Clear();
        directionSequence.Clear();

        foreach(var distance in distances){
            distanceSequence.Add(distance);
            directionSequence.Add(true);
            distanceSequence.Add(distance);
            directionSequence.Add(false);
        }
    }

    void PickDistanceAndDirection(){
        if(distanceSequence.Count == 0 || directionSequence.Count == 0){
            FillLocateSequence();
        }

        var indPicked = UnityEngine.Random.Range(0, distanceSequence.Count);

        currentDistance = distanceSequence[indPicked];
        currentDirection = directionSequence[indPicked];

        distanceSequence.RemoveAt(indPicked);
        directionSequence.RemoveAt(indPicked);
    }
    
    void FillPatternSequence(){
        patternTypeSequence.Clear();
        numberOfPatternSequence.Clear();

        foreach(var numberOfPattern in numbersOfPattern){
            foreach(var patternType in patternTypes){
                patternTypeSequence.Add(patternType);
                numberOfPatternSequence.Add(numberOfPattern);
            }
        }
    }

    void PickPatternNumberAndType(){
        if(numberOfPatternSequence.Count == 0 || patternTypeSequence.Count == 0){
            FillPatternSequence();
        }

        var indPicked = UnityEngine.Random.Range(0, numberOfPatternSequence.Count);

        currentPatternType = patternTypeSequence[indPicked];
        currentNumberOfPattern = numberOfPatternSequence[indPicked];


        numberOfPatternSequence.RemoveAt(indPicked);
        patternTypeSequence.RemoveAt(indPicked);
    }

    void SetupTask(){
        DisableAllTaskComponent();

        if(currentBlock != null){
            switch(currentBlock.task){
                case Task.Locate:
                    Debug.Log("Setting up Locate Task");
                    goalNumberCanvas.SetActive(true);
                    PickDistanceAndDirection();
                    startingPos = objectManager.PickRandomLocateStartingPos(currentDistance, currentDirection);
                    numberToLocate = startingPos + (currentDirection ? 1 : -1) * currentDistance;
                    objectManager.GetObjectDataAtIndex(numberToLocate).isGoal = true;
                    objectManager.RandomizeSphereCellsColor();
                    objectManager.t = objectManager.maxTimeStamp /2;
                    break;

                case Task.Pattern:
                    Debug.Log("Setting up Pattern Task");
                    patternDisplayObjectManager.gameObject.SetActive(true);
                    patternDisplayObjectManager.ResetAllIsGoal();
                    patternDisplayObjectManager.ResetAllValidated();
                    patternDisplayObjectManager.rangeToSelect = -1;
                    patternDisplayObjectManager.t = 1;
                    PickPatternNumberAndType();
                    patternDisplayObjectManager.GeneratePattern(patternSize, 1, currentPatternType, 0);
                    objectManager.t = 0;
                    startingPos = objectManager.t;
                    numPatternValidated = 0;
                    finishPatternTaskButtonCanvas.SetActive(true);
                    finishPatternTaskButton.interactable = false;
                    break;
                default: break;
            }
        }
    }

    void StartTask(){
        if(currentBlock != null){
            switch(currentBlock.task){
                case Task.Locate:
                    goalNumber.numberToLocate = numberToLocate;
                    objectManager.t = startingPos;
                    scrollBarController.UpdateValueFromObjectManager();
                    break;

                case Task.Pattern:
                    patternPoses = objectManager.GeneratePattern(patternSize, currentNumberOfPattern, currentPatternType, minSpacingBetweenPatterns);
                    Debug.Log($"Pattern generated at : {patternPoses[0]}, {patternPoses[1]}, {patternPoses[2]}, {patternPoses[3]}, {patternPoses[4]}");
                    finishPatternTaskButton.interactable = true;
                    break;
                default: break;
            }
        }
    }

    float GetCurrentZoom(){
        if(objectManager != null){
            return objectManager.zoom/objectManager.maxTimeStamp;
        }
        return 0f;
    }

    float? GetCurrentControllerDistance(){
        if(currentBlock != null){
            switch(currentBlock.ti){
                case TI.AB:             return (orthozoomABInteraction as OrthozoomButtonCDGainInteraction).controllerDistance;
                case TI.OrthozoomDepth: return (orthozoomOneHandDepthInteraction as OrthozoomDepthCDGainInteraction).controllerDistance;
                case TI.OrthozoomHeight: return (orthozoomOneHandHeightInteraction as OrthozoomHeightCDGainInteraction).controllerDistance;
                case TI.OrthozoomJoystickControllerDist: return (orthozoomJoystickControllerDistInteraction as OrthozoomFullJoystickCDGainInteraction).controllerDistance;
                case TI.OrthozoomDepthJoystick: return (orthozoomDepthJoystickInteraction as OrthozoomDepthJoystickCDGainInteraction).controllerDistance;
                default: return null;
            }
        }
        return null;
    }

    float? GetCurrentOrthoDistance(){
        if(currentBlock != null){
            switch(currentBlock.ti){
                case TI.OrthozoomDepth: return (orthozoomOneHandDepthInteraction as OrthozoomDepthCDGainInteraction).orthoDistance;
                case TI.OrthozoomHeight: return (orthozoomOneHandHeightInteraction as OrthozoomHeightCDGainInteraction).orthoDistance;
                case TI.OrthozoomJoystickControllerDist: return (orthozoomJoystickControllerDistInteraction as OrthozoomFullJoystickCDGainInteraction).orthoDistance;
                case TI.OrthozoomDepthJoystick: return (orthozoomDepthJoystickInteraction as OrthozoomDepthJoystickCDGainInteraction).orthoDistance;
                default: return null;
            }
        }
        return null;
    }

    float? GetCurrentCDGain(){
        if(currentBlock != null){
            switch(currentBlock.ti){
                case TI.AB:             return (orthozoomABInteraction as OrthozoomButtonCDGainInteraction).cDGain;
                case TI.OrthozoomDepth: return (orthozoomOneHandDepthInteraction as OrthozoomDepthCDGainInteraction).cDGain;
                case TI.OrthozoomHeight: return (orthozoomOneHandHeightInteraction as OrthozoomHeightCDGainInteraction).cDGain;
                case TI.OrthozoomJoystickControllerDist: return (orthozoomJoystickControllerDistInteraction as OrthozoomFullJoystickCDGainInteraction).cDGain;
                case TI.OrthozoomDepthJoystick: return (orthozoomDepthJoystickInteraction as OrthozoomDepthJoystickCDGainInteraction).cDGain;
                default: return null;
            }
        }
        return null;
    }

    void HandleInteractionLogVariables(){
        currentZoom = GetCurrentZoom();
        if(currentZoom > maxZoom){
            maxZoom = currentZoom;
        }

        currentOrthoDistance = GetCurrentOrthoDistance();
        if(currentOrthoDistance != null){
            if(currentOrthoDistance > maxOrthoDistance){
                maxOrthoDistance = currentOrthoDistance;
            }
        }
        else{
            maxOrthoDistance = null;
        }


        currentCdGain = GetCurrentCDGain();
        if(currentCdGain != null){
            if(currentCdGain > maxCDGain){
                maxCDGain = currentCdGain;
            }
        }
        else{
            maxCDGain = null;
        }

        currentControllerDistance = GetCurrentControllerDistance();
        if(currentControllerDistance != null){
            if(Mathf.Abs((float)currentControllerDistance) > Mathf.Abs((float)maxControllerDistance)){
                maxControllerDistance = currentControllerDistance;
            }
        }
        else{
            maxControllerDistance = null;
        }

        if(currentBlock.ti == TI.OrthozoomDepthJoystick){
            currentJoystickX = (orthozoomDepthJoystickInteraction as OrthozoomDepthJoystickCDGainInteraction).joystickX;

            if(Mathf.Abs((float)currentJoystickX) > 0.01 && !currentlyUsingInteraction){
                OnInteractionStarted();
            }
            else if(!(Mathf.Abs((float)currentJoystickX) > 0.01) && currentlyUsingInteraction){
                OnInteractionStopped();
            }
        }

        else{
            currentJoystickX = null;
        }

        if(currentBlock.ti == TI.AB){
            if((orthozoomABInteraction as OrthozoomButtonCDGainInteraction).cDGain == 0f && currentlyUsingInteraction){
                OnInteractionStopped();
            }
            else if((orthozoomABInteraction as OrthozoomButtonCDGainInteraction).cDGain != 0f && !currentlyUsingInteraction){
                OnInteractionStarted();
            }
        }
    }

    private void LogCurrentTrial(){
        resumeLogger.LogTrial(
            currentUser.userId,
            currentUser.group,
            currentBlock.blockID,
            currentBlock.visualization.ToString(),
            currentBlock.ti.ToString(),
            currentBlock.task.ToString(),
            currentTrial.num,
            currentTrial.training,
            startingPos,
            currentBlock.task == Task.Locate ? numberToLocate : null,
            currentBlock.task == Task.Locate ? currentDistance : null,
            currentBlock.task == Task.Locate ? currentDirection ? "RIGHT": "LEFT" : null,
            currentBlock.task == Task.Pattern ? currentNumberOfPattern : null,
            currentBlock.task == Task.Pattern ? currentPatternType.ToString(): null,
            currentBlock.task == Task.Pattern ? numPatternValidated : null,
            currentBlock.task == Task.Pattern ? currentNumberOfPattern - numPatternValidated : null,
            currentBlock.task == Task.Pattern ? didFindAllPatterns : null,
            currentBlock.task == Task.Pattern ? wasTimedOut : null,
            currentBlock.task == Task.Pattern ? patternPoses[0] : null,
            currentBlock.task == Task.Pattern ? patternPoses[1] : null,
            currentBlock.task == Task.Pattern ? patternPoses[2] : null,
            currentBlock.task == Task.Pattern ? patternPoses[3] : null,
            currentBlock.task == Task.Pattern ? patternPoses[4] : null,
            currentBlock.task == Task.Pattern ? patternFoundPoses[0] : null,
            currentBlock.task == Task.Pattern ? patternFoundPoses[1] : null,
            currentBlock.task == Task.Pattern ? patternFoundPoses[2] : null,
            currentBlock.task == Task.Pattern ? patternFoundPoses[3] : null,
            currentBlock.task == Task.Pattern ? patternFoundPoses[4] : null,
            currentTime,
            timeBeforeFirstMove,
            currentBlock.task == Task.Locate ? timeGrab : null,
            currentBlock.task == Task.Locate ? timeForFinalSelection : null,
            currentBlock.task == Task.Pattern ? timeToValidatePattern1 : null,
            currentBlock.task == Task.Pattern ? timeToValidatePattern2 : null,
            currentBlock.task == Task.Pattern ? timeToValidatePattern3 : null,
            currentBlock.task == Task.Pattern ? timeToValidatePattern4 : null,
            currentBlock.task == Task.Pattern ? timeToValidatePattern5 : null,
            currentBlock.task == Task.Pattern ? timeBetweenLastPatternValidationAndButtonFinishClicked : null,
            timeUsingInteraction,
            currentBlock.ti == TI.Slider ? numberOfRetrieve : null,
            numberOfBadSelect,
            currentBlock.ti == TI.Slider ? numberOfClickInsideScrollbarHandler : null,
            currentBlock.ti == TI.Slider ? numberOfClickOutsideScrollbarHandler : null,
            currentBlock.ti == TI.Slider ? numberOfClickOnPlus : null, // click plus
            currentBlock.ti == TI.Slider ? numberOfClickOnMinus : null, // click minus
            currentBlock.ti == TI.AB ? numberOfClickOnA : null, // click plus
            currentBlock.ti == TI.AB ? numberOfClickOnB : null, // click minus
            currentBlock.ti == TI.AB ? numberOfClickOnBothButton : null, // click minus
            numberOfInteractionStarted,
            maxHeadAmplitude,
            maxZoom, //max_zoom
            maxControllerDistance, //max_controller_distance,
            maxOrthoDistance, //max_ortho_distance
            maxCDGain, //max_cd_gain
            currentBlock.task == Task.Locate ? zoomGrab : null, //zoom_grab
            currentBlock.task == Task.Locate ? zoomForFinalSelection : null, //zoom_for_final_selection
            currentBlock.task == Task.Pattern ? zoomToValidatePattern1 : null, //zoom_to_validate_pattern1
            currentBlock.task == Task.Pattern ? zoomToValidatePattern2 : null, //zoom_to_validate_pattern2
            currentBlock.task == Task.Pattern ? zoomToValidatePattern3 : null, //zoom_to_validate_pattern3
            currentBlock.task == Task.Pattern ? zoomToValidatePattern4 : null, //zoom_to_validate_pattern4
            currentBlock.task == Task.Pattern ? zoomToValidatePattern5 : null, //zoom_to_validate_pattern5
            currentBlock.task == Task.Locate ? controllerDistanceGrab : null, //controller_distance_grab
            currentBlock.task == Task.Locate ? controllerDistanceForFinalSelection : null, //controller_distance_for_final_selection
            currentBlock.task == Task.Pattern ? controllerDistanceToValidatePattern1 : null, //controller_distance_to_validate_pattern1
            currentBlock.task == Task.Pattern ? controllerDistanceToValidatePattern2 : null, //controller_distance_to_validate_pattern2
            currentBlock.task == Task.Pattern ? controllerDistanceToValidatePattern3 : null, //controller_distance_to_validate_pattern3
            currentBlock.task == Task.Pattern ? controllerDistanceToValidatePattern4 : null, //controller_distance_to_validate_pattern4
            currentBlock.task == Task.Pattern ? controllerDistanceToValidatePattern5 : null //controller_distance_to_validate_pattern5
        );

        fullLogger.WriteLogLines(currentUser.userId, currentTime, timeBetweenLastPatternValidationAndButtonFinishClicked, maxHeadAmplitude);
    }

    void HandleFullLogTimer(){
        fullLogTimer += Time.deltaTime;
        if(fullLogTimer > timeBetweenFullLogs){
            LogTimeStamp();
            fullLogTimer = fullLogTimer - timeBetweenFullLogs;
        }
    }

    private void LogTimeStamp(){
        fullLogger.LogTimeStamp(
            currentUser.userId,
            currentUser.group,
            currentBlock.blockID,
            currentBlock.visualization.ToString(),
            currentBlock.ti.ToString(),
            currentBlock.task.ToString(),
            currentTrial.num,
            currentTrial.training,
            expeState.ToString(),
            objectManager.t,
            startingPos,
            currentBlock.task == Task.Locate ? numberToLocate : null,
            currentBlock.task == Task.Locate ? currentDistance : null,
            currentBlock.task == Task.Locate ? currentDirection ? "RIGHT": "LEFT" : null,
            currentBlock.task == Task.Pattern ? currentNumberOfPattern : null,
            currentBlock.task == Task.Pattern ? currentPatternType.ToString(): null,
            currentBlock.task == Task.Pattern ? numPatternValidated : null,
            currentBlock.task == Task.Pattern ? currentNumberOfPattern - numPatternValidated : null,
            currentBlock.task == Task.Pattern ? didFindAllPatterns : null,
            currentBlock.task == Task.Pattern ? patternPoses[0] : null,
            currentBlock.task == Task.Pattern ? patternPoses[1] : null,
            currentBlock.task == Task.Pattern ? patternPoses[2] : null,
            currentBlock.task == Task.Pattern ? patternPoses[3] : null,
            currentBlock.task == Task.Pattern ? patternPoses[4] : null,
            currentBlock.task == Task.Pattern ? patternFoundPoses[0] : null,
            currentBlock.task == Task.Pattern ? patternFoundPoses[1] : null,
            currentBlock.task == Task.Pattern ? patternFoundPoses[2] : null,
            currentBlock.task == Task.Pattern ? patternFoundPoses[3] : null,
            currentBlock.task == Task.Pattern ? patternFoundPoses[4] : null,
            currentTime,
            timeBeforeFirstMove,
            currentBlock.task == Task.Locate ? timeGrab : null,
            currentBlock.task == Task.Locate ? timeForFinalSelection : null,
            currentBlock.task == Task.Pattern ? timeToValidatePattern1 : null,
            currentBlock.task == Task.Pattern ? timeToValidatePattern2 : null,
            currentBlock.task == Task.Pattern ? timeToValidatePattern3 : null,
            currentBlock.task == Task.Pattern ? timeToValidatePattern4 : null,
            currentBlock.task == Task.Pattern ? timeToValidatePattern5 : null,
            timeUsingInteraction,
            currentBlock.ti == TI.Slider ? numberOfRetrieve : null,
            numberOfBadSelect,
            currentBlock.ti == TI.Slider ? numberOfClickInsideScrollbarHandler : null,
            currentBlock.ti == TI.Slider ? numberOfClickOutsideScrollbarHandler : null,
            currentBlock.ti == TI.Slider ? numberOfClickOnPlus : null, // click plus
            currentBlock.ti == TI.Slider ? numberOfClickOnMinus : null, // click minus
            currentBlock.ti == TI.AB ? numberOfClickOnA : null, // click plus
            currentBlock.ti == TI.AB ? numberOfClickOnB : null, // click minus
            currentBlock.ti == TI.AB ? numberOfClickOnBothButton : null, // click minus
            currentHeadAmplitude,
            CalculateCurrentHeadRotation(),
            currentZoom, //currentZoom
            currentOrthoDistance, //currentOrthoDistance
            currentCdGain, //currentCdGain
            currentControllerDistance, //currentControllerDistance
            currentJoystickX  //currentJoystickX
        );
    }

    private void HandleDebugInfo(){
        if(currentBlock == null){
            debugTextMesh.text = $"Expe phase: \n{expeState.ToString()}";
        }
        else{
            if(showDebugInfo){
                switch(currentBlock.task){
                    case Task.Pattern:
debugTextMesh.text = $@"
Expe phase:
{expeState.ToString()}
Number Of Pattern:
{currentNumberOfPattern}
CurrentTime: 
{currentTime}
CurrentZoom:
{currentZoom}
CurrentControllerDistance
{currentControllerDistance}
CurrentCDGain
{currentCdGain}
CurrentOrthoDistance
{currentOrthoDistance}
Zoom Validate Pattern 1: 
{zoomToValidatePattern1}
Zoom Validate Pattern 2: 
{zoomToValidatePattern2}
Zoom Validate Pattern 3: 
{zoomToValidatePattern3}
Zoom Validate Pattern 4: 
{zoomToValidatePattern4}
Zoom Validate Pattern 5: 
{zoomToValidatePattern5}
Time Using Interaction: 
{timeUsingInteraction}
Number Of Interaction Started:
{numberOfInteractionStarted}
Bad Select:
{numberOfBadSelect}
Click A:
{numberOfClickOnA}
Click B:
{numberOfClickOnB}
Click Both
{numberOfClickOnBothButton}
";
                        break;
                    case Task.Locate:
debugTextMesh.text = $@"
Expe phase: 
{expeState.ToString()}
Current Distance:
{currentDistance}
Current Direction:
{currentDirection}
CurrentTime:
{currentTime}
CurrentZoom:
{currentZoom}
CurrentControllerDistance
{currentControllerDistance}
CurrentCDGain
{currentCdGain}
CurrentOrthoDistance
{currentOrthoDistance}
Ctrl dist Grab: 
{controllerDistanceGrab}
Zoom for final Selection: 
{controllerDistanceForFinalSelection}
Zoom Grab: 
{zoomGrab}
Ctrl dist for final Selection:
{zoomForFinalSelection}
Time Using Interaction: 
{timeUsingInteraction}
Number Of Interaction Started:
{numberOfInteractionStarted}
Bad Select:
{numberOfBadSelect}
Click A:
{numberOfClickOnA}
Click B:
{numberOfClickOnB}
Click Both
{numberOfClickOnBothButton}
";
                        break;
                }
                
            }
        }
        
    }
    
    void OnDestroy(){
        if(expeState != ExpeState.Wait && expeState != ExpeState.ReadytoStart){
            LogCurrentTrial();
        }
    }
    
    /*
    private void EnableLocomotionActions(){
        // Call a private function lol
        typeof(ActionBasedControllerManager).GetMethod("UpdateLocomotionActions", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(leftActionBasedControllerManager, null);
        typeof(ActionBasedControllerManager).GetMethod("UpdateLocomotionActions", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(rightActionBasedControllerManager, null);
    }

    private void DisableLocomotionActions(){
        // Call a private function lol
        typeof(ActionBasedControllerManager).GetMethod("DisableLocomotionActions", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(leftActionBasedControllerManager, null);
        typeof(ActionBasedControllerManager).GetMethod("DisableLocomotionActions", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(rightActionBasedControllerManager, null);
    }
    */
}


enum ExpeState{
    Wait, 
    ReadytoStart, 
    ExpeStarted, 
    AfterFirstMove, 
    ObjectInSelectionArea, 
    Pattern1Validated,
    Pattern2Validated,
    Pattern3Validated,
    Pattern4Validated,
    Pattern5Validated,
    TaskCompleted
}

static class ExpeStateMethods{

    public static string ToString(this ExpeState expeState){
        switch(expeState){
            case ExpeState.Wait: return "Wait";
            case ExpeState.ReadytoStart: return "ReadyToStart";
            case ExpeState.ExpeStarted: return "ExpeStarted";
            case ExpeState.AfterFirstMove: return "AfterFirstMove";
            case ExpeState.ObjectInSelectionArea: return "ObjectInSelectionArea";
            case ExpeState.Pattern1Validated: return "Pattern1Validated";
            case ExpeState.Pattern2Validated: return "Pattern2Validated";
            case ExpeState.Pattern3Validated: return "Pattern3Validated";
            case ExpeState.Pattern4Validated: return "Pattern4Validated";
            case ExpeState.Pattern5Validated: return "Pattern5Validated";
            case ExpeState.TaskCompleted: return "TaskCompleted";
            default: return "Unknown state";
        }
    }
}
