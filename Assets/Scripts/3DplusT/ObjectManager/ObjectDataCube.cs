using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine;
using TMPro;
using System;

public class ObjectDataCube : ObjectData
{

    private Color col_;

    public Color col{
        get{return col_;}
        set{col_ = value;
            renderer_.material.color=col_;}
    }

    [SerializeField]
    private MeshRenderer renderer_;

}
