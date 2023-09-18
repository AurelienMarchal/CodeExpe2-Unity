using UnityEngine;

public class Interaction : MonoBehaviour
{
    public bool interationEnabled{
        protected set;
        get;
    }

    public bool interactionStarted{
        get;
        protected set;
    }

    public float controllerDistance{
        get;
        protected set;
    }

    public virtual void StartInteraction(){
        interactionStarted = true;
    }

    public virtual void StopInteraction(){
        interactionStarted = false;
    }

    public virtual void EnableInteraction(){
        interationEnabled = true;
        OnInteractionEnabled();
    }

    protected virtual void OnInteractionEnabled(){
        
    }

    public virtual void DisableInteraction(){
        interationEnabled = false;
    }

    protected virtual void OnInteractionDisabled(){

    }


    public void DestroyAllControllerDistanceLine(){
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("ControllerDistanceLine");
        foreach (GameObject go in gameObjects) {
            Destroy(go);
        }
    }
}
