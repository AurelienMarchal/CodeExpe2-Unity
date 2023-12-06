using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ObjectManager : MonoBehaviour
{

    [SerializeField]
    protected ObjectType objectType;
    
    //[SerializeField]
    private int t_ = 0;
    public int t{
        get{return t_;}
        set{
            var oldT = t_;
            t_ = Math.Clamp(value, 0, maxTimeStamp - 1);
            if(objectList != null){
                HandleActivation((t_ - oldT) * 2);
                HandleCanBeSelected();
                HandleSelectionRectangle();
                HandleFlagActivation();
                HandleActivatedObjectsPosition();
                HandleActivatedObjectsRotation();
                HandleActivatedObjectsSize();
                //Debug.Log($"Set t = {t}");
            }
        }
    }

    [SerializeField]
    [Range(0f, 1f)]
    float ajustableZoom;

    [SerializeField]
    bool allowAjustableZoom = false;

    private float zoom_ = 0f;

    public float zoom{
        get{return zoom_;}
        set{
            zoom_ = Mathf.Clamp(Mathf.Abs(value), 0f, maxTimeStamp - 1f);
            if(objectList != null){
                HandleCanBeSelected();
                HandleSelectionRectangle();
                HandleFlagActivation();
                HandleActivatedObjectsPosition();
                HandleActivatedObjectsRotation();
                HandleActivatedObjectsSize();
                //Debug.Log($"Zoom {zoom}");
            }
            
        }
    }

    [SerializeField]
    public int maxTimeStamp;

    public int numberOfObjectDisplayedOnTheFront{protected set; get;}

    [SerializeField]
    protected bool allowZoom;

    public int rangeToSelect{get; set;}

    public bool allowSelection{get; set;}

    [SerializeField]
    protected Material transparentMat;

    [SerializeField]
    protected GameObject flagPrefab;

    [SerializeField]
    protected bool generateFlags = true;

    protected List<GameObject> flags_;

    [Range(0.0f, 1.0f), SerializeField]
    protected float flagRatio = 0.1f;

    [SerializeField]
    protected int[] flagSpacings;

    [SerializeField]
    protected int[] zoomThresholdsForFlagSpacings;

    protected int lastT = 0;

    protected bool _3DManipulationEnabled = false;

    protected bool last_3DManipulationEnabled = false;

    [SerializeField]
    GameObject selectionRectangleCanvasPrefab;

    protected GameObject selectionRectangleCanvasPrefabInstance;

    [SerializeField]
    protected InputActionReference enable3DManipulationAction;

    [SerializeField]
    protected InputActionReference rightControllerRotationAction;

    [SerializeField]
    protected float gapBetweenTwoCubes;

    [SerializeField]
    protected float gapBetweenTwoSphereCells;

    [SerializeField]
    protected float gapBetweenTwoCells;

    [SerializeField]
    protected GameObject cubePrefab;

    [SerializeField]
    protected GameObject sphereCellPrefab;

    [SerializeField]
    List<Gradient> gradients;

    UnityEngine.Object[] cellPrefabs;

    protected float cubeScale = 0.5f;

    protected float sphereCellScale = 0.4f;

    protected float cellScale = 0.005f;

    [SerializeField]
    Vector3 cellOffset;

    [SerializeField]
    GameObject cellNumberTextCanvas;

    [SerializeField]
    Vector3 cellNumberTextCanvasOffset;

    protected List<GameObject> objectList;

    protected List<GameObject> completeObjectList;

    public IntBoolBoolIntEvent objectSelectedEvent = new IntBoolBoolIntEvent();

    protected Quaternion lastRightControllerRotation = Quaternion.identity;

    protected Quaternion lastCubeRotation = Quaternion.identity;

    public virtual void JumpInTime(int timeJump){
        t += timeJump;
    }

    void Awake(){
        rangeToSelect = 0;
        numberOfObjectDisplayedOnTheFront = 5;
        allowSelection = false;
        selectionRectangleCanvasPrefabInstance = Instantiate(selectionRectangleCanvasPrefab, transform);
        selectionRectangleCanvasPrefabInstance.SetActive(false);
        CreateObjects();
        t = 0;
    }

    void Update(){
        if(allowAjustableZoom){
            zoom = ajustableZoom * maxTimeStamp;
        }
    }

    void DestroyAllObjects(){

        objectList?.Clear();
        completeObjectList?.Clear();
        flags_?.Clear();

        for (int i = 0; i < transform.childCount; i++){
            if(transform.GetChild(i).gameObject != selectionRectangleCanvasPrefabInstance){
                Destroy(transform.GetChild(i).gameObject);
            }
        }
    }

    public void CreateObjects(){

        SetupListeners();
        objectList = new List<GameObject>();
        completeObjectList = new List<GameObject>();

        if(generateFlags){
            flags_ = new List<GameObject>();
        }

        DestroyAllObjects();

        switch(objectType){
            case ObjectType.Cube: CreateCubes(); break;
            case ObjectType.SphereCell: CreateSphereCells(); break;
            case ObjectType.Cell: CreateCells(); break;
        }
    }

    protected void CreateCubes(){        

        //lastCubeRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));

        lastCubeRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        
        for(int i = 0; i <= maxTimeStamp; i++){

            var cube = Instantiate(cubePrefab, Vector3.zero, Quaternion.identity);
            //cube.transform.Rotate(new Vector3(0f, 180f, 0f));

            var holder = new GameObject("holder");

            holder.transform.SetParent(transform, false);
            cube.transform.SetParent(holder.transform, false);

            completeObjectList.Add(cube);
            ObjectDataCube objectData = cube.GetComponent<ObjectDataCube>();
            
            if(objectData != null){
                //A changer
                objectData.number = i;

                objectData.objectSelectedEvent.AddListener(OnObjectSelected);

                for(var powOfTen = 0; powOfTen<Mathf.Min(Mathf.Log10(maxTimeStamp), gradients.Count); powOfTen++){
                    var tenPow = (int)Mathf.Pow(10, powOfTen);

                    var tenth = (objectData.number/tenPow) - objectData.number/(tenPow*10)*10;

                    var gradEval = tenth / 10f;
                    objectData.col += gradients[powOfTen].Evaluate(gradEval);

                    /*
                    if(objectData.number % tenPow == 0){
                        
                        var gradEval = (float)(objectData.number%(tenPow*10)) / (float)(tenPow * 10);
                        objectData.col = gradients[powOfTen].Evaluate(gradEval);
                    }*/
                }

                if(generateFlags){
                    if(/*objectData.number%(maxTimeStamp/10)==0*/ true){
                        var flagInstance = Instantiate(flagPrefab, holder.transform);
                        flagInstance.transform.SetParent(holder.transform, false);
                        flagInstance.GetComponentInChildren<TextMeshProUGUI>().text = objectData.number.ToString();
                    }
                }
            }

            holder.SetActive(false);
        }
    }

    protected void CreateSphereCells(){
        //lastCubeRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));

        lastCubeRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        
        for(int i = 0; i <= maxTimeStamp; i++){

            var sphereCell = Instantiate(sphereCellPrefab, Vector3.zero, Quaternion.identity);
            //sphereCell.transform.Rotate(new Vector3(0f, 180f, 0f));

            var holder = new GameObject("holder");

            holder.transform.SetParent(transform, false);
            sphereCell.transform.SetParent(holder.transform, false);

            completeObjectList.Add(sphereCell);
            ObjectDataSphereCell objectData = sphereCell.GetComponent<ObjectDataSphereCell>();
            
            if(objectData != null){
                //A changer
                objectData.number = i;

                objectData.objectSelectedEvent.AddListener(OnObjectSelected);

                if(generateFlags){
                    if(/*objectData.number%(maxTimeStamp/10)==0*/ true){
                        var flagInstance = Instantiate(flagPrefab, holder.transform);
                        flagInstance.transform.SetParent(holder.transform, false);
                        flagInstance.GetComponentInChildren<TextMeshProUGUI>().text = objectData.number.ToString();
                    }
                }
            }

            holder.SetActive(false);
        }

        RandomizeSphereCellsColor();
    }

    protected void CreateCells(){
        cellPrefabs = Resources.LoadAll("Dataset/Astec-pm1", typeof(GameObject));
        maxTimeStamp = cellPrefabs.Length - 1;

        foreach(UnityEngine.Object cellPrefab in cellPrefabs){
            var cell = Instantiate((GameObject)cellPrefab, Vector3.zero, Quaternion.identity);

            GameObject holder = new GameObject("Holder");
            GameObject sphereHolder = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Destroy(sphereHolder.GetComponent<SphereCollider>());
            Destroy(sphereHolder.GetComponent<MeshRenderer>());

            holder.transform.SetParent(transform, false);
            sphereHolder.transform.SetParent(holder.transform, false);
            cell.transform.SetParent(sphereHolder.transform, false);
            cell.transform.localPosition = cellOffset;

            var canvas = Instantiate(cellNumberTextCanvas, cell.transform.position, Quaternion.identity);
        
            canvas.transform.SetParent(holder.transform, false);
            canvas.transform.localPosition = cellNumberTextCanvasOffset;

            ObjectData objectData = cell.AddComponent(typeof(ObjectData)) as ObjectData;
            objectData.number = objectList.Count;

            canvas.GetComponentInChildren<TextMeshProUGUI>().text = objectData.number.ToString();

            cell.transform.localScale = new Vector3(cellScale, cellScale, cellScale);
            //canvas.transform.localScale = new Vector3(0.01f/cellScale, 0.01f/cellScale, 1f/cellScale);
            completeObjectList.Add(cell);
        }
    }

    protected virtual void HandleActivatedObjectsPosition(){

    }

    protected virtual void HandleActivatedObjectsRotation(){
        Quaternion rightControllerRotation = Quaternion.identity;

        if(_3DManipulationEnabled){
            rightControllerRotation = rightControllerRotationAction.action.ReadValue<Quaternion>();
        }

        if(!last_3DManipulationEnabled && _3DManipulationEnabled){
            lastRightControllerRotation = rightControllerRotation;
            lastCubeRotation = objectList[0].transform.rotation;
        }

        foreach(GameObject obj in objectList){
                
            if(_3DManipulationEnabled){
                obj.transform.rotation = lastCubeRotation;

                var rotationAngle = lastRightControllerRotation.eulerAngles - rightControllerRotation.eulerAngles;
                rotationAngle.x = -rotationAngle.x;
                obj.transform.Rotate(rotationAngle, Space.World);
                //cube.transform.Rotate(new Vector3(0f, 180f, 0f));
            }
        }

        last_3DManipulationEnabled = _3DManipulationEnabled;
    }

    protected virtual void HandleActivation(int increase){

    }

    protected virtual void HandleActivatedObjectsSize(){

    }

    protected virtual void HandleCanBeSelected(){
        foreach(GameObject obj in objectList){
            ObjectData objectData = obj.GetComponent<ObjectData>();
            objectData.canBeSelected = (Math.Abs(objectData.number - t) <= rangeToSelect) && allowSelection;
        }
    }

    protected virtual void HandleSelectionRectangle()
    {
        selectionRectangleCanvasPrefabInstance.SetActive(allowSelection);
        if(!allowSelection){
            return;
        }

        var gap = 0.1f;
        switch(objectType){
            case ObjectType.Cube: gap = gapBetweenTwoCubes; break;
            case ObjectType.SphereCell: gap = gapBetweenTwoSphereCells; break;
            case ObjectType.Cell: gap = gapBetweenTwoCells; break;
        }

        selectionRectangleCanvasPrefabInstance.GetComponent<RectTransform>().sizeDelta = new Vector2(
            rangeToSelect * 3 * gap * 100,
            gap * 100
        );
    }

    protected virtual void HandleFlagActivation(){
        if(generateFlags && flagSpacings.Length > 0 && zoomThresholdsForFlagSpacings.Length == flagSpacings.Length){
            var flagSpacingsIndex = FindClosestGreaterIndex(zoomThresholdsForFlagSpacings, Mathf.FloorToInt(zoom));
            flagSpacingsIndex = Math.Clamp(flagSpacingsIndex, 0, flagSpacings.Length - 1);
            foreach(GameObject obj in objectList){
                ObjectData objectData = obj.GetComponent<ObjectData>();
                foreach(Transform tr in obj.transform.parent){
                    if(tr.tag == "Flag"){
                        tr.gameObject.SetActive(objectData.number % Mathf.RoundToInt(flagSpacings[flagSpacingsIndex])==0);
                        //tr.gameObject.SetActive(objectData.number % Mathf.RoundToInt(maxTimeStamp*flagRatio)==0);
                        //Debug.Log($"Flag Ratio {flagRatios[flagSpacingsIndex]}");
                    }
                }
            }
        }
    }

    protected static int FindClosestGreaterIndex(int[] numbers, int target)
    {
        int closestIndex = -1;
        int minDifference = int.MaxValue;

        for (int i = 0; i < numbers.Length; i++)
        {
            int difference = target - numbers[i];

            if (difference > 0 && difference < minDifference)
            {
                minDifference = difference;
                closestIndex = i;
            }
        }

        return closestIndex;
    }

    public int PickRandomPos(int leftMargin = 0, int rightMargin = 0){
        return UnityEngine.Random.Range(leftMargin, maxTimeStamp - rightMargin);
    }

    public int PickRandomLocateStartingPos(int distance, bool direction){
        if(distance > maxTimeStamp/2){
            Debug.Log("Impossible to pick pos to locate");
            return maxTimeStamp/2;
        }

        var start = direction ? 0 : maxTimeStamp/2;
        var end = direction ? maxTimeStamp/2 : maxTimeStamp;

        var numberPicked = UnityEngine.Random.Range(start, end);

        return numberPicked;

    }

    public void RandomizeSphereCellsColor(){
        if(objectType != ObjectType.SphereCell){
            return;
        }
        if(completeObjectList == null){
            return;
        }
        foreach(GameObject obj in completeObjectList){
            ObjectDataSphereCell objectData = obj.GetComponent<ObjectDataSphereCell>();
            objectData.colSphere1 = SphereCellColorsMethods.RandomSphereCellColor();
            objectData.colSphere2 = SphereCellColorsMethods.RandomSphereCellColor();
            objectData.colSphere3 = SphereCellColorsMethods.RandomSphereCellColor();
            objectData.colSphere4 = SphereCellColorsMethods.RandomSphereCellColor();
            objectData.colSphere5 = SphereCellColorsMethods.RandomSphereCellColor();
            objectData.colSphere6 = SphereCellColorsMethods.RandomSphereCellColor();
        }
    }

    public int[] GeneratePattern(int patternSize, int numberOfPattern, PatternType patternType, int minSpacingBetweenPatterns){

        int[] posList = {0, 0, 0, 0, 0, 0};
        

        if(objectType != ObjectType.SphereCell){
            Debug.Log("Impossible to generate pattern");
            return posList;
        }

        if(numberOfPattern * (patternSize + minSpacingBetweenPatterns) > maxTimeStamp){
            Debug.Log("Impossible to generate pattern");
            return posList;
        }

        if(objectList == null){
            Debug.Log("Impossible to generate pattern");
            return posList;
        }

        RandomizeSphereCellsColor();

        var maxIteration = maxTimeStamp;

        var iter = 0;
        var nbPatternGenrerated = 0;

        for(var i = 0; i < numberOfPattern; i++){
            posList[i] = 0;
        }

        while(iter < maxIteration && nbPatternGenrerated < numberOfPattern){

            var randomNb = PickRandomPos(patternSize/2 + minSpacingBetweenPatterns, patternSize/2);

            //Check if a pattern can be generated here
            var canBeThere = true;

            var start1 = Math.Max(randomNb - patternSize/2 - minSpacingBetweenPatterns, 0);
            var stop1 = Math.Min(randomNb + patternSize/2 + minSpacingBetweenPatterns, maxTimeStamp);

            for(var i = start1; i < stop1; i++){
                var objectData = (ObjectDataSphereCell)GetObjectDataAtIndex(i);

                if(objectData != null){
                    if(objectData.isGoal){
                        canBeThere = false;
                    }
                }
            }

            if(canBeThere){
                var start2 = Math.Max(randomNb - patternSize/2, 0);
                var stop2 = Math.Min(randomNb + patternSize/2, maxTimeStamp - 1);
                for(var i = start2; i <= stop2; i++){
                    var objectData = (ObjectDataSphereCell)GetObjectDataAtIndex(i);

                    if(objectData != null){
                        objectData.isGoal = true;
                        objectData.patternPos = randomNb;

                        switch(patternType){
                            case PatternType.FullColor1: 
                                objectData.colSphere1 = SphereCellColors.Color1;
                                objectData.colSphere2 = SphereCellColors.Color1;
                                objectData.colSphere3 = SphereCellColors.Color1;
                                objectData.colSphere4 = SphereCellColors.Color1;
                                objectData.colSphere5 = SphereCellColors.Color1;
                                objectData.colSphere6 = SphereCellColors.Color1;
                                break;

                            case PatternType.FullColor2: 
                                objectData.colSphere1 = SphereCellColors.Color2;
                                objectData.colSphere2 = SphereCellColors.Color2;
                                objectData.colSphere3 = SphereCellColors.Color2;
                                objectData.colSphere4 = SphereCellColors.Color2;
                                objectData.colSphere5 = SphereCellColors.Color2;
                                objectData.colSphere6 = SphereCellColors.Color2;
                                break;

                            case PatternType.FullColor3: 
                                objectData.colSphere1 = SphereCellColors.Color3;
                                objectData.colSphere2 = SphereCellColors.Color3;
                                objectData.colSphere3 = SphereCellColors.Color3;
                                objectData.colSphere4 = SphereCellColors.Color3;
                                objectData.colSphere5 = SphereCellColors.Color3;
                                objectData.colSphere6 = SphereCellColors.Color3;
                                break;

                            case PatternType.FullColor4: 
                                objectData.colSphere1 = SphereCellColors.Color4;
                                objectData.colSphere2 = SphereCellColors.Color4;
                                objectData.colSphere3 = SphereCellColors.Color4;
                                objectData.colSphere4 = SphereCellColors.Color4;
                                objectData.colSphere5 = SphereCellColors.Color4;
                                objectData.colSphere6 = SphereCellColors.Color4;
                                break;

                            case PatternType.HorizontalColor1: 
                                objectData.colSphere1 = SphereCellColors.Color1;
                                objectData.colSphere2 = SphereCellColors.Color1;
                                objectData.colSphere3 = SphereCellColors.Color2;
                                objectData.colSphere4 = SphereCellColors.Color2;
                                objectData.colSphere5 = SphereCellColors.Color1;
                                objectData.colSphere6 = SphereCellColors.Color1;
                                break;

                            case PatternType.HorizontalColor2: 
                                objectData.colSphere1 = SphereCellColors.Color2;
                                objectData.colSphere2 = SphereCellColors.Color2;
                                objectData.colSphere3 = SphereCellColors.Color3;
                                objectData.colSphere4 = SphereCellColors.Color3;
                                objectData.colSphere5 = SphereCellColors.Color2;
                                objectData.colSphere6 = SphereCellColors.Color2;
                                break;

                            case PatternType.HorizontalColor3: 
                                objectData.colSphere1 = SphereCellColors.Color3;
                                objectData.colSphere2 = SphereCellColors.Color3;
                                objectData.colSphere3 = SphereCellColors.Color4;
                                objectData.colSphere4 = SphereCellColors.Color4;
                                objectData.colSphere5 = SphereCellColors.Color3;
                                objectData.colSphere6 = SphereCellColors.Color3;
                                break;

                            case PatternType.HorizontalColor4: 
                                objectData.colSphere1 = SphereCellColors.Color4;
                                objectData.colSphere2 = SphereCellColors.Color4;
                                objectData.colSphere3 = SphereCellColors.Color1;
                                objectData.colSphere4 = SphereCellColors.Color1;
                                objectData.colSphere5 = SphereCellColors.Color4;
                                objectData.colSphere6 = SphereCellColors.Color4;
                                break;
                            
                            case PatternType.VerticalColor1:
                                objectData.colSphere1 = SphereCellColors.Color2;
                                objectData.colSphere2 = SphereCellColors.Color2;
                                objectData.colSphere3 = SphereCellColors.Color1;
                                objectData.colSphere4 = SphereCellColors.Color1;
                                objectData.colSphere5 = SphereCellColors.Color1;
                                objectData.colSphere6 = SphereCellColors.Color1;
                                break;
                            case PatternType.VerticalColor2:
                                objectData.colSphere1 = SphereCellColors.Color2;
                                objectData.colSphere2 = SphereCellColors.Color2;
                                objectData.colSphere3 = SphereCellColors.Color1;
                                objectData.colSphere4 = SphereCellColors.Color1;
                                objectData.colSphere5 = SphereCellColors.Color1;
                                objectData.colSphere6 = SphereCellColors.Color1;
                                break;
                        }
                    }
                }
                posList[nbPatternGenrerated] = randomNb;
                nbPatternGenrerated ++;
            }

            iter ++;
        }

        return posList;
    }


    private void OnEnable3DManipulationActionPerformed(InputAction.CallbackContext context){
        _3DManipulationEnabled = true;
    }

    private void OnEnable3DManipulationActionCanceled(InputAction.CallbackContext context){
        _3DManipulationEnabled = false;
    }

    private void OnDestroy() {
        enable3DManipulationAction.action.performed -= OnEnable3DManipulationActionPerformed;
        enable3DManipulationAction.action.canceled -= OnEnable3DManipulationActionCanceled;
        objectSelectedEvent.RemoveAllListeners();
        Destroy(selectionRectangleCanvasPrefabInstance);
    }

    protected void SetupListeners(){
        enable3DManipulationAction.action.performed += OnEnable3DManipulationActionPerformed;
        enable3DManipulationAction.action.canceled += OnEnable3DManipulationActionCanceled;
    }

    protected void OnObjectSelected(int num, bool isGoal, bool isSelect, int patternPos){
        objectSelectedEvent.Invoke(num, isGoal, isSelect, patternPos);
    }

    //Returns whether or not the object was already validated 
    public bool ValidateObject(int num, bool propagate = false){
        if(objectList != null){

            var objectData = GetObjectDataAtIndex(num);

            if(objectData == null){
                return false;
            }

            if(objectData.validated){
                return true;
            }

            if(objectData.isGoal){
                objectData.validated = true;

                if(propagate){
                    var currentIndex = num - 1;

                    while(currentIndex > 0){
                        var objectDataCurrentIndex = GetObjectDataAtIndex(currentIndex);
                        if(objectDataCurrentIndex == null){
                            break;
                        }
                        if(!objectDataCurrentIndex.validated && objectDataCurrentIndex.isGoal){
                            objectDataCurrentIndex.validated = true;
                        }
                        else{
                            break;
                        }

                        currentIndex --;

                    }

                    currentIndex = num + 1;

                    while(currentIndex < maxTimeStamp){
                        var objectDataCurrentIndex = GetObjectDataAtIndex(currentIndex);
                        if(objectDataCurrentIndex == null){
                            break;
                        }
                        if(!objectDataCurrentIndex.validated && objectDataCurrentIndex.isGoal){
                            objectDataCurrentIndex.validated = true;
                        }
                        else{
                            break;
                        }
                        
                        currentIndex ++;
                    }
                }
            }
        }

        return false;
    }

    public ObjectData GetObjectDataAtIndex(int i){
        if(objectList != null){
            return completeObjectList[i].GetComponent<ObjectData>();
        }
        return null;
    }

    public void ResetAllIsGoal(){
        if(completeObjectList != null){
            foreach(GameObject obj in completeObjectList){
                var objectData = obj.GetComponent<ObjectData>();
                if(objectData != null){
                    objectData.isGoal = false;
                }
            }
        }
    }

    public void ResetAllValidated(){
        if(completeObjectList != null){
            foreach(GameObject obj in completeObjectList){
                var objectData = obj.GetComponent<ObjectData>();
                if(objectData != null){
                    objectData.validated = false;
                }
            }
        }
    }

    public void ResetAllPatternPos(){
        if(completeObjectList != null){
            foreach(GameObject obj in completeObjectList){
                var objectData = obj.GetComponent<ObjectData>();
                if(objectData != null){
                    objectData.patternPos = objectData.number;
                }
            }
        }
    }
}


public enum ObjectType{
    Cube, SphereCell, Cell
}

public enum PatternType{
    FullColor1, FullColor2, FullColor3, FullColor4,
    HorizontalColor1, HorizontalColor2, HorizontalColor3, HorizontalColor4,
    VerticalColor1, VerticalColor2, VerticalColor3, VerticalColor4
}
