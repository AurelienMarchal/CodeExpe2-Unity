
using UnityEngine;

public class ObjectManagerStandard : ObjectManager
{

    [SerializeField]
    int numberOfObjectDisplayed;

    protected override void HandleActivation(){
        foreach(GameObject obj in objectList){
            ObjectData objectData = obj.GetComponent<ObjectData>();
            if (objectData.number >= t - numberOfObjectDisplayed/2 && objectData.number <= t + numberOfObjectDisplayed/2){
                if(objectType == ObjectType.Cell){
                    obj.transform.parent.parent.gameObject.SetActive(true);
                }
                else{
                    obj.transform.parent.gameObject.SetActive(true);
                }
                
            }
            else if (obj.activeSelf){
                if(objectType == ObjectType.Cell){
                    obj.transform.parent.parent.gameObject.SetActive(false);
                }
                else{
                    obj.transform.parent.gameObject.SetActive(false);
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

        foreach(GameObject obj in objectList){
            if(obj.activeSelf){
                ObjectData objectData = obj.GetComponent<ObjectData>();
                var relativeInd = objectData.number - t;
                float x = gap * relativeInd;

                Vector3 newPos = new Vector3(transform.position.x + x, transform.position.y, transform.position.z);
                if(objectType == ObjectType.Cell){
                    obj.transform.parent.parent.position = newPos;
                }
                else{
                    obj.transform.parent.position = newPos;
                }
            }
        }
    }
}
