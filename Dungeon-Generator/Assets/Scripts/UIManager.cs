using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [SerializeField] TextMeshProUGUI text;
    private void Awake()
    {
        instance = this;
    }

    public void UpdateText(string newText)
    {
        text.text = "Currently Selected: " + newText;
    }

    


}
