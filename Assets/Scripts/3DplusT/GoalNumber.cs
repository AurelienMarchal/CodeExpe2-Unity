using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GoalNumber : MonoBehaviour
{

    [SerializeField]
    TextMeshProUGUI textMesh;

    private int numberToLocate_;
    public int numberToLocate{
        get{return numberToLocate_;}
        set{
            numberToLocate_ = value;
            UpdateText();
        }
    }

    public ObjectManager objectManager;

    [SerializeField]
    Image backgroundImage;

    [SerializeField]
    Color backgroundImageColorHighlight;

    Color backgroundImageColorInit;

    // Start is called before the first frame update
    void Start()
    {
        backgroundImageColorInit =  new Color(backgroundImage.color.r, backgroundImage.color.g, backgroundImage.color.b, backgroundImage.color.a);
        numberToLocate = -1;
    }

    // Update is called once per frame
    void UpdateText(){
        textMesh.text = numberToLocate < 0 ? "?" : numberToLocate.ToString();
    }
}
