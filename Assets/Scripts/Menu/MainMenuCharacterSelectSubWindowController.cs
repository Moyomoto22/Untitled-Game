using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuCharacterSelectSubWindowController : MonoBehaviour
{
    private EventSystem eventSystem;
    public MainMenuController mainMenuController;

    public int destinationMenuIndex;

    public List<GameObject> buttons;

    private void Awake()
    {
        eventSystem = FindObjectOfType<EventSystem>();
        SelectButton();
    }

    private void OnDestroy()
    {
        mainMenuController.SelectButton(destinationMenuIndex);
        mainMenuController.ToggleButtonsInteractable(true);
    }

    public void SelectButton(int number = 0)
    {
        if (eventSystem != null)
        {
            var buttonToSelect = buttons[number];
            eventSystem.SetSelectedGameObject(buttonToSelect);
        }
    }

    public async void OnPressButton(int index)
    {
        SoundManager.Instance.PlaySubmit();
        switch (destinationMenuIndex)
        {
            case 1:
                await mainMenuController.GoToEquipMenu(index);
                break;
            case 2:
                await mainMenuController.GoToSkillMenu(index);
                break;
            case 3:
                await mainMenuController.GoToClassMenu(index);
                break;
            case 4:
                await mainMenuController.GoToStatusMenu(index);
                break;
            default:
                return;
        }
    }

    /// <summary>
    /// É{É^ÉìÇÃInteractableÇêÿÇËë÷Ç¶ÇÈ
    /// </summary>
    /// <param name="interactable">óLå¯/ñ≥å¯</param>
    public void ToggleButtonsInteractable(bool interactable)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            var button = buttons[i].GetComponent<Button>();
            if (button != null)
            {
                button.interactable = interactable;
            }
        }
    }
}
