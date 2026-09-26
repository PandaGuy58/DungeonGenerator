using System;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager _instance;
    [SerializeField] private TextMeshProUGUI _text;

    private Action<UpdateUIEvent> onUpdateUIEvent;

    private void OnEnable()
    {
        onUpdateUIEvent = UpdateText;
        EventBus<UpdateUIEvent>.Subscribe(onUpdateUIEvent);
    }

    public void UpdateText(UpdateUIEvent evt)
    {
        _text.text = "Currently Selected: " + evt.Text;
    }
}
