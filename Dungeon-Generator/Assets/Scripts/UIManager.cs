using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager _instance;
    [SerializeField] private TextMeshProUGUI _text;
    private void Awake()
    {
        _instance = this;
    }

    public void UpdateText(string newText)
    {
        _text.text = "Currently Selected: " + newText;
    }

    


}
