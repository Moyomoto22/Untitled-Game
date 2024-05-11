using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class FieldController : MonoBehaviour
{
    public EventSystem eventSystem;
    public GameObject inputActionParent;
    public GameObject mainMenuObject;
    public GameObject mainMenuInstance;

    public void OnPressMenuButton(InputAction.CallbackContext context)
    {
        Debug.Log("MenuButton Pressed.");
        
        if (context.performed)
        {
            MainMenuController mainManuController = mainMenuObject.GetComponent<MainMenuController>();
            if (mainMenuInstance == null)
            {
                CommonController.PauseGame();
                mainMenuInstance = Instantiate(mainMenuObject);
            }
            //else
            //{
            //    Destroy(mainMenuInstance);
            //    mainMenuInstance = null;
            //    CommonController.ResumeGame();
            //}
            //if (!mainMenuObject.activeSelf)
            //{
            //    //CommonController.PauseGame();
            //    mainMenuObject.SetActive(true);
            //}
            //else
            //{
            //    mainMenuObject.SetActive(false);
            //    CommonController.ResumeGame();
            //}

        }
    }

    private void PauseGame()
    {
        // ÉvÉåÉCÉÑÅ[ÇÃìÆÇ´Çé~ÇﬂÇÈ
        CommonVariableManager.playerCanMove = false;
        CommonController.PauseGame();
    }
}
