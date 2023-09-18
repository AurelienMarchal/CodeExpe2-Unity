using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;


[RequireComponent(typeof(LineRenderer))]
public class EyeTrackingRay : MonoBehaviour
{

    [SerializeField]
    private float rayDistance = 1.0f;

    [SerializeField]
    private float rayWidth = 0.01f;

    [SerializeField]
    private LayerMask layersToInclude;

    [SerializeField]
    private float sphereCastRadius = 0.1f;

    [SerializeField]
    private Color rayColorDefaultState = Color.yellow;

    [SerializeField]
    private Color rayColorHoverState = Color.red;

    private LineRenderer lineRenderer;

    private List<ObjectData> objectDatas = new List<ObjectData>();

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        SetupRay();
    }

    private void SetupRay()
    {
        lineRenderer.useWorldSpace = false;
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = rayWidth;
        lineRenderer.endWidth = rayWidth;
        lineRenderer.startColor = rayColorDefaultState;
        lineRenderer.endColor = rayColorDefaultState;
        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, new Vector3(transform.position.x, transform.position.y, transform.position.z + rayDistance));

    }

    void FixedUpdate(){
        RaycastHit hit;

        Vector3 rayCastDirection = transform.TransformDirection(Vector3.forward) * rayDistance;

        if(Physics.SphereCast(transform.position, sphereCastRadius, rayCastDirection, out hit, Mathf.Infinity, layersToInclude)){
            UnSelect();
        
            lineRenderer.startColor = rayColorHoverState;
            lineRenderer.endColor = rayColorHoverState;

            var objectData = hit.transform.GetComponentInParent<ObjectData>();
            if(objectData != null){

                objectDatas.Add(objectData);
                objectData.isEyeHovered = true;
            }
            
        }
        else{
            lineRenderer.startColor = rayColorDefaultState;
            lineRenderer.endColor = rayColorDefaultState;
            UnSelect(true);
        }
    }

    private void UnSelect(bool clear = true)
    {
        foreach(ObjectData objectData in objectDatas){
            objectData.isEyeHovered = false;
        }

        if(clear){
            objectDatas.Clear();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
