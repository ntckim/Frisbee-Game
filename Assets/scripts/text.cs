using UnityEngine;
using TMPro; 

public class TextOutlineController : MonoBehaviour
{
    private TextMeshProUGUI textmeshPro;

    void Awake()
    {
        textmeshPro = GetComponent<TextMeshProUGUI>();

        textmeshPro.outlineWidth = 0.2f;

        textmeshPro.outlineColor = new Color32(0, 0, 0, 255);
        
        textmeshPro.UpdateMeshPadding();
    }
}