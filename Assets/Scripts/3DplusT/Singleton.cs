using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton : MonoBehaviour
{   

    public Color sphereCellColor1;

    public Color sphereCellColor2;

    public Color sphereCellColor3;

    public Color sphereCellColor4;

    public Color colorValidated;

    public static Singleton Instance { get; private set; }

    private void Awake(){ 
    // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this){ 
            Destroy(this); 
        } 
        else{ 
            Instance = this; 
        } 
    }
}
