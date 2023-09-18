using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class PossibleControllerAction : MonoBehaviour, IPointerEnterHandler
{

    public List<UIControllerAction> listControllerAction;
    
    private Selectable selectable = null;

    private void Start() {
        selectable = GetComponent<Selectable>();
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        selectable.Select();
    }

}
