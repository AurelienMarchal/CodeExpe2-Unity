using UnityEngine;

public class ObjectManagerHelice : ObjectManager
{
    
    [SerializeField]
    float radius;

    [SerializeField]
    int numberOfObjectDisplayed;

    [SerializeField]
    float heightIncrement;

    protected override void HandleActivation(){
        foreach(GameObject obj in completeObjectList){
            ObjectData objectData = obj.GetComponent<ObjectData>();
            if (objectData.number >= t - numberOfObjectDisplayed/2 && objectData.number <= t + numberOfObjectDisplayed/2){
                if(objectType == ObjectType.Cell){
                    obj.transform.parent.parent.gameObject.SetActive(true);
                }
                else{
                    obj.transform.parent.gameObject.SetActive(true);
                }

                if(!objectList.Contains(obj)){
                    objectList.Add(obj);
                }
            }

            else if (obj.activeSelf){
                if(objectType == ObjectType.Cell){
                    obj.transform.parent.parent.gameObject.SetActive(false);
                }
                else{
                    obj.transform.parent.gameObject.SetActive(false);
                }
                if(objectList.Contains(obj)){
                    objectList.Remove(obj);
                }
            }
        }
    }

    protected override void HandleActivatedObjectsPosition(){
        var gap = 0f;
        switch(objectType){
            case ObjectType.Cube: gap = gapBetweenTwoCubes; break;
            case ObjectType.SphereCell: gap = gapBetweenTwoSphereCells; break;
            case ObjectType.Cell: gap = gapBetweenTwoCells; break;
        }
        var halfPerimeter = Mathf.PI * radius;
        foreach(GameObject obj in objectList){
            if(obj.activeSelf){
                ObjectData objectData = obj.GetComponent<ObjectData>();
                var relativeInd = objectData.number - t;
                
                float angle = Mathf.PI/2 - ((Mathf.PI*gap/halfPerimeter)*relativeInd);
                float x = Mathf.Cos(angle)*radius;
                float y = angle * heightIncrement;
                float z = Mathf.Sin(angle)*radius;
                Vector3 newPos = new Vector3(transform.position.x + x, transform.position.y + y, transform.position.z + z);
                if(objectType == ObjectType.Cell){
                    obj.transform.parent.parent.position = newPos;
                }
                else{
                    obj.transform.parent.position = newPos;
                }
            }
        }
    }

    protected override void HandleActivatedObjectsRotation(){
        foreach(GameObject obj in objectList){
            if(obj.activeSelf){
                ObjectData objectData = obj.GetComponent<ObjectData>();
                Vector3 vectorToLookAt = new Vector3(transform.position.x, obj.transform.position.y, transform.position.z);
                obj.transform.parent.LookAt(vectorToLookAt, Vector3.up);
                
                if(_3DManipulationEnabled){
                    Quaternion rightControllerRotation = rightControllerRotationAction.action.ReadValue<Quaternion>();
                    obj.transform.parent.Rotate(rightControllerRotation.eulerAngles);
                }
            }
        }
    }
}
