using System;
using UnityEngine;


public class ObjectDataSphereCell : ObjectData
{

    [SerializeField]
    private MeshRenderer rendererSphere1;

    [SerializeField]
    private MeshRenderer rendererSphere2;

    [SerializeField]
    private MeshRenderer rendererSphere3;

    [SerializeField]
    private MeshRenderer rendererSphere4;

    [SerializeField]
    private MeshRenderer rendererSphere5;

    [SerializeField]
    private MeshRenderer rendererSphere6;

    private SphereCellColors colSphere1_;

    private SphereCellColors colSphere2_;

    private SphereCellColors colSphere3_;

    private SphereCellColors colSphere4_;

    private SphereCellColors colSphere5_;

    private SphereCellColors colSphere6_;

    public override bool validated{
        get{
            return validated_;
        } 
        set{
            validated_ = value;
            if(validated_){
                rendererSphere1.material.color = Singleton.Instance.colorValidated;
                rendererSphere2.material.color = Singleton.Instance.colorValidated;
                rendererSphere3.material.color = Singleton.Instance.colorValidated;
                rendererSphere4.material.color = Singleton.Instance.colorValidated;
                rendererSphere5.material.color = Singleton.Instance.colorValidated;
                rendererSphere6.material.color = Singleton.Instance.colorValidated;
            }
            else{
                colSphere1 = colSphere1_;
                colSphere2 = colSphere2_;
                colSphere3 = colSphere3_;
                colSphere4 = colSphere4_;
                colSphere5 = colSphere5_;
                colSphere6 = colSphere6_;
            }

        }
    }

    public SphereCellColors colSphere1{
        get{return colSphere1_;}
        set{colSphere1_ = value;
            rendererSphere1.material.color=colSphere1_.ToColor();}
    }

    public SphereCellColors colSphere2{
        get{return colSphere2_;}
        set{colSphere2_ = value;
            rendererSphere2.material.color=colSphere2_.ToColor();}
    }

    public SphereCellColors colSphere3{
        get{return colSphere3_;}
        set{colSphere3_ = value;
            rendererSphere3.material.color=colSphere3_.ToColor();}
    }

    public SphereCellColors colSphere4{
        get{return colSphere4_;}
        set{colSphere4_ = value;
            rendererSphere4.material.color=colSphere4_.ToColor();}
    }

    public SphereCellColors colSphere5{
        get{return colSphere5_;}
        set{colSphere5_ = value;
            rendererSphere5.material.color=colSphere5_.ToColor();}
    }

    public SphereCellColors colSphere6{
        get{return colSphere6_;}
        set{colSphere6_ = value;
            rendererSphere6.material.color=colSphere6_.ToColor();}
    }

}

public enum SphereCellColors{
    Color1, Color2, Color3, Color4
}


public static class SphereCellColorsMethods{

    public static string ToString(this SphereCellColors sphereCellColors){
        return "TODO?";
    }

    public static Color ToColor(this SphereCellColors sphereCellColors){
        switch(sphereCellColors){
            case SphereCellColors.Color1: return Singleton.Instance.sphereCellColor1;
            case SphereCellColors.Color2: return Singleton.Instance.sphereCellColor2;
            case SphereCellColors.Color3: return Singleton.Instance.sphereCellColor3;
            case SphereCellColors.Color4: return Singleton.Instance.sphereCellColor4;
            default: return Singleton.Instance.sphereCellColor1;
        }
    }

    public static SphereCellColors RandomSphereCellColor(){
        Array values = SphereCellColors.GetValues(typeof(SphereCellColors));
        return (SphereCellColors)values.GetValue(UnityEngine.Random.Range(0, values.Length));
    }
}
