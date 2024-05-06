using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class MenuSelectionHandler : MonoBehaviour, ISelectHandler
{
    public TextMeshProUGUI infoText;
    [SerializeField]

    public void OnSelect(BaseEventData eventData)
    {
        //var varName = $"{name}";
        //var message = typeof(Messages).GetField(varName).GetValue(null).ToString();

        //infoText.text = message;
    }
}