using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class CellManager : ObjectManager
{
    /*
    Object[] cellPrefabs;

    List<GameObject> cells;

    float cellScale = 0.005f;

    [SerializeField]
    int numberOfCubeDisplayed;

    [SerializeField]
    float gapBetweenTwoCubes;

    [SerializeField]
    Vector3 cellOffset;

    [SerializeField]
    GameObject cellNumberTextCanvas;

    [SerializeField]
    Vector3 cellNumberTextCanvasOffset;

    

    void Start(){
        cellPrefabs = Resources.LoadAll("Dataset/Astec-pm1", typeof(GameObject));
        maxTimeStamp = cellPrefabs.Length - 1;

        SetupListeners();
        cells = new List<GameObject>();
        foreach(Object cellPrefab in cellPrefabs){
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
            objectData.number = cells.Count;

            canvas.GetComponentInChildren<TextMeshProUGUI>().text = objectData.number.ToString();

            cell.transform.localScale = new Vector3(cellScale, cellScale, cellScale);
            //canvas.transform.localScale = new Vector3(0.01f/cellScale, 0.01f/cellScale, 1f/cellScale);
            cells.Add(cell);
        }
    }

    protected override void HandleActivation(){
        foreach(GameObject cell in cells){
            ObjectData objectData = cell.GetComponent<ObjectData>();
            if (objectData.number >= t - numberOfCubeDisplayed/2 && objectData.number <= t + numberOfCubeDisplayed/2){
                //cell.SetActive(true);
                cell.transform.parent.parent.gameObject.SetActive(true);
            }
            else if (cell.activeSelf){
                //cell.SetActive(false);
                cell.transform.parent.parent.gameObject.SetActive(false);
                
            }
        }
    }

    protected override void HandleActivatedObjectsPosition(){
        foreach(GameObject cell in cells){
            if(cell.activeSelf){
                ObjectData objectData = cell.GetComponent<ObjectData>();
                var relativeInd = objectData.number - t;
                float x = gapBetweenTwoCubes * relativeInd;
                Vector3 newPos = new Vector3(transform.position.x + x, transform.position.y, transform.position.z);
                cell.transform.parent.parent.position = newPos;
                
            }
        }
    }

    protected override void HandleActivatedObjectsRotation(){

        Quaternion rightControllerRotation = Quaternion.identity;


        if(_3DManipulationEnabled){
            rightControllerRotation = rightControllerRotationAction.action.ReadValue<Quaternion>();

        }

        if(!last_3DManipulationEnabled && _3DManipulationEnabled){
            lastRightControllerRotation = rightControllerRotation;
            lastCubeRotation = cells[0].transform.parent.rotation;
        }

        foreach(GameObject cell in cells){
                
            if(_3DManipulationEnabled){

                cell.transform.parent.rotation = lastCubeRotation;
                var rotationAngle = lastRightControllerRotation.eulerAngles - rightControllerRotation.eulerAngles;
                rotationAngle.x = -rotationAngle.x;
                rotationAngle.z = -rotationAngle.z;
                cell.transform.parent.Rotate(rotationAngle, Space.World);

            }
        }

        last_3DManipulationEnabled = _3DManipulationEnabled;
    }
    */
}
