
using System;
using UnityEngine;

public class ObjectManagerPerspectiveWall : ObjectManager
{

    [SerializeField]
    int numberOfObjectDisplayed;

    [SerializeField]
    protected int minNumberOfObjectDisplayedOnTheFront;

    [Range(0, Mathf.PI/2), SerializeField]
    float wallAngle;

    protected override void HandleActivation(){
        foreach(GameObject obj in completeObjectList){
            ObjectData objectData = obj.GetComponent<ObjectData>();
            if (objectData.number >= t - numberOfObjectDisplayed/2 && objectData.number <= t + numberOfObjectDisplayed/2){
                //Debug.Log($"Activate obj {objectData.number}");
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
                //Debug.Log($"Deactivate obj {objectData.number}");
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

        //Debug.Log($"objectList size : {objectList.Count}");
    }

    protected override void HandleActivatedObjectsPosition(){
        var gap = 0.1f;
        switch(objectType){
            case ObjectType.Cube: gap = gapBetweenTwoCubes; break;
            case ObjectType.SphereCell: gap = gapBetweenTwoSphereCells; break;
            case ObjectType.Cell: gap = gapBetweenTwoCells; break;
        }
        int numberOfObjectDisplayedOnTheFront = minNumberOfObjectDisplayedOnTheFront;

        if(allowZoom){

            var distanceOfDisplayInFront = minNumberOfObjectDisplayedOnTheFront * gap;
            if(Mathf.Abs(maxTimeStamp - zoom) > 0.01f){
                gap *= (maxTimeStamp - zoom)/maxTimeStamp;
            
                numberOfObjectDisplayedOnTheFront = Math.Clamp(
                    Mathf.FloorToInt(zoom),
                    minNumberOfObjectDisplayedOnTheFront,
                    Mathf.FloorToInt(distanceOfDisplayInFront/gap));

            }
        }

        
        foreach(GameObject obj in objectList){
            if(obj.activeSelf){
                ObjectData objectData = obj.GetComponent<ObjectData>();
                var relativeInd = objectData.number - t;
                var dx = 0f;
                var dz = 0f;
                if(Math.Abs(relativeInd) >  numberOfObjectDisplayedOnTheFront/2){
                    var frontDistance = (numberOfObjectDisplayedOnTheFront/2)*gap*Mathf.Sign(relativeInd);
                    var sideDistance = (Math.Abs(relativeInd) - numberOfObjectDisplayedOnTheFront/2)*gap;
                    dx = frontDistance + sideDistance*Mathf.Cos(Mathf.PI/2-wallAngle)*Mathf.Sign(relativeInd);
                    dz = sideDistance*Mathf.Sin(Mathf.PI/2-wallAngle);
                }
                else{
                    dx = gap * relativeInd;
                }
                var newPos = new Vector3(transform.position.x + dx, transform.position.y, transform.position.z + dz);
                if(objectType == ObjectType.Cell){
                    obj.transform.parent.parent.position = newPos;
                }
                else{
                    obj.transform.parent.position = newPos;
                }
            }
        }
        
    }

    protected override void HandleActivatedObjectsSize()
    {
        if(allowZoom){

            var startScale = 1f;
            switch(objectType){
                case ObjectType.Cube: startScale = cubeScale; break;
                case ObjectType.SphereCell: startScale = sphereCellScale; break;
                case ObjectType.Cell: startScale = cellScale; break;
            }
            var scale = startScale;
            if(Mathf.Abs(maxTimeStamp - zoom) > 0.01f){
                scale = startScale * (maxTimeStamp - zoom)/maxTimeStamp;
            }

            foreach(GameObject obj in objectList){
                if(obj.activeSelf){
                    //ObjectData objectData = obj.GetComponent<ObjectData>();
                    obj.transform.localScale = new Vector3(scale, scale, scale);
                }
            }
        }
    }

    protected override void HandleCanBeSelected(){
        var gap = 0.1f;
        switch(objectType){
            case ObjectType.Cube: gap = gapBetweenTwoCubes; break;
            case ObjectType.SphereCell: gap = gapBetweenTwoSphereCells; break;
            case ObjectType.Cell: gap = gapBetweenTwoCells; break;
        }
        numberOfObjectDisplayedOnTheFront = minNumberOfObjectDisplayedOnTheFront;

        if(allowZoom){

            var distanceOfDisplayInFront = minNumberOfObjectDisplayedOnTheFront * gap;
            if(Mathf.Abs(maxTimeStamp - zoom) > 0.01f){
                gap *= (maxTimeStamp - zoom)/maxTimeStamp;
            
                numberOfObjectDisplayedOnTheFront = Math.Clamp(
                    Mathf.FloorToInt(zoom),
                    minNumberOfObjectDisplayedOnTheFront,
                    Mathf.FloorToInt(distanceOfDisplayInFront/gap));
            }
        }

        foreach(GameObject obj in objectList){
            ObjectData objectData = obj.GetComponent<ObjectData>();
            objectData.canBeSelected = (Math.Abs(objectData.number - t) <= numberOfObjectDisplayedOnTheFront/2) && allowSelection;
        }
    }

    protected override void HandleSelectionRectangle(){
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

        numberOfObjectDisplayedOnTheFront = minNumberOfObjectDisplayedOnTheFront;

        if(allowZoom){
            var distanceOfDisplayInFront = minNumberOfObjectDisplayedOnTheFront * gap;
            if(Mathf.Abs(maxTimeStamp - zoom) > 0.01f){
                gap *= (maxTimeStamp - zoom)/maxTimeStamp;
            
                numberOfObjectDisplayedOnTheFront = Math.Clamp(
                    Mathf.FloorToInt(zoom),
                    minNumberOfObjectDisplayedOnTheFront,
                    Mathf.FloorToInt(distanceOfDisplayInFront/gap));
            }
        }

        selectionRectangleCanvasPrefabInstance.GetComponent<RectTransform>().sizeDelta = new Vector2(
            ((minNumberOfObjectDisplayedOnTheFront/2)*2 + 1) * gap  * 100,
            Mathf.Max(60f, gap * 100)
        );
    }

    protected override void HandleFlagActivation(){
        if(generateFlags){

            var gap = 0.1f;
            switch(objectType){
                case ObjectType.Cube: gap = gapBetweenTwoCubes; break;
                case ObjectType.SphereCell: gap = gapBetweenTwoSphereCells; break;
                case ObjectType.Cell: gap = gapBetweenTwoCells; break;
            }

            numberOfObjectDisplayedOnTheFront = minNumberOfObjectDisplayedOnTheFront;

            if(allowZoom){
                var distanceOfDisplayInFront = minNumberOfObjectDisplayedOnTheFront * gap;
                if(Mathf.Abs(maxTimeStamp - zoom) > 0.01f){
                    gap *= (maxTimeStamp - zoom)/maxTimeStamp;
                
                    numberOfObjectDisplayedOnTheFront = Math.Clamp(
                        Mathf.FloorToInt(zoom),
                        minNumberOfObjectDisplayedOnTheFront,
                        Mathf.FloorToInt(distanceOfDisplayInFront/gap));
                }
            }
            if(flagSpacings.Length > 0){
                var flagSpacingsIndex = FindClosestGreaterIndex(zoomThresholdsForFlagSpacings, Mathf.FloorToInt(zoom));
                flagSpacingsIndex = Math.Clamp(flagSpacingsIndex, 0, flagSpacings.Length - 1);
                foreach(GameObject obj in objectList){
                    ObjectData objectData = obj.GetComponent<ObjectData>();
                    foreach(Transform tr in obj.transform.parent){
                        if(tr.tag == "Flag"){
                            //Debug.Log($"Flag Ratio {flagSpacings[flagSpacingsIndex]}");
                            tr.gameObject.SetActive(objectData.number % Mathf.CeilToInt(flagSpacings[flagSpacingsIndex])==0);
                            //tr.gameObject.SetActive(objectData.number % (numberOfObjectDisplayedOnTheFront/5 * 10)==0);
                            //tr.gameObject.SetActive(objectData.number % Mathf.RoundToInt(maxTimeStamp*flagRatio)==0);
                        }
                    }
                }
            }
            
        }
    }
}
