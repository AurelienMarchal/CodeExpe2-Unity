
using System;
using UnityEngine;

public class ObjectManagerLogCircle : ObjectManager
{

    [Range(2, 10), SerializeField]
    int logBase;

    protected override void HandleActivation(){
        foreach(GameObject obj in objectList){
            ObjectData objectData = obj.GetComponent<ObjectData>();
            var relativeInd = objectData.number - t;
            var trueRelativeInd = TrueRevativeInd(relativeInd);

            if (trueRelativeInd != 0 || relativeInd == 0){
                obj.transform.parent.gameObject.SetActive(true);
            }
            else if (obj.activeSelf){
                obj.transform.parent.gameObject.SetActive(false);
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

        float halfPerimeter = logBase * Mathf.FloorToInt(Mathf.Log(maxTimeStamp, logBase)) * gap;
        float radius =  halfPerimeter/Mathf.PI;


        foreach(GameObject obj in objectList){
            if(obj.activeSelf){
                ObjectData objectData = obj.GetComponent<ObjectData>();
                var relativeInd = objectData.number - t;
                var trueRelativeInd = TrueRevativeInd(relativeInd);
                float angle = Mathf.PI/2 - ((Mathf.PI*gap/halfPerimeter)*trueRelativeInd);
                float x = Mathf.Cos(angle)*radius;
                float z = Mathf.Sin(angle)*radius;
                Vector3 newPos = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);
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
                obj.transform.parent.LookAt(transform.position, Vector3.up);
                if(_3DManipulationEnabled){
                    Quaternion rightControllerRotation = rightControllerRotationAction.action.ReadValue<Quaternion>();
                    obj.transform.parent.Rotate(rightControllerRotation.eulerAngles);
                }
            }
        }
    }


    private int TrueRevativeInd(int relativeInd){
        
        int sign = Math.Sign(relativeInd);
        int relativeIndAbs = Math.Abs(relativeInd);

        var powOfTen = Mathf.FloorToInt(Mathf.Log(relativeIndAbs, logBase));
        var tenPow = (int)Mathf.Pow(logBase, powOfTen);
        if(tenPow > 0){
            if(relativeIndAbs%tenPow == 0){
                return ((relativeIndAbs/tenPow) + powOfTen*(logBase-1))*sign;
            }
            else{
                return 0;
            }
        }
        else{
            return relativeIndAbs * sign;
        }
    }
}
