using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class GazePointer : MonoBehaviour
{
    
    LineRenderer lineRenderer;

    [SerializeField]
    Transform headTransform;

    [SerializeField]
    float lineDistance;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        lineRenderer.SetPosition(0, headTransform.position);
        lineRenderer.SetPosition(1, headTransform.forward * lineDistance);
    }
}
