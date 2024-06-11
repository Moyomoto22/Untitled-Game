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

    public GameObject player;
    public float interactRange = 1f;
    public GameObject interactTextObject;
    private bool canInteract = false;

    public AudioSource OpenMenu;

    void Update()
    {
        CheckForInteractableObjects();
    }

    public void OnPressSubmitButton(InputAction.CallbackContext context)
    {
        if (context.performed && canInteract)
        {
            InteractWithObjects();
            HideInteractText();
        }
    }

    public void OnPressMenuButton(InputAction.CallbackContext context)
    {
        Debug.Log("MenuButton Pressed.");
        
        if (context.performed)
        {
            SoundManager.Instance.PlayMenuOpen();

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

    public void OnPressLoadButton_TEST(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //SaveManager.Instance.LoadGame();
        }
    }
    private void PauseGame()
    {
        // �v���C���[�̓������~�߂�
        CommonVariableManager.playerCanMove = false;
        CommonController.PauseGame();
    }

    void InteractWithObjects()
    {
        Vector3 rayOrigin = player.transform.position + new Vector3(0, 1f, 0); // ���悢�������烌�C���o��
        RaycastHit hit;

        if (Physics.Raycast(rayOrigin, player.transform.forward, out hit, interactRange))
        {
            if (hit.collider.GetComponent<Door>() != null)
            {
                hit.collider.GetComponent<Door>().Interact();
            }
            if (hit.collider.GetComponent<HiddenDoorWall>() != null)
            {
                hit.collider.GetComponent<HiddenDoorWall>().Interact();
            }
            //else if (hit.collider.GetComponent<Chest>() != null)
            //{
            //    hit.collider.GetComponent<Door>().Interact();
            //}
            else
            {
                interactTextObject.SetActive(false);
            }
        }
    }

    void CheckForInteractableObjects()
    {
        Vector3 rayOrigin = player.transform.position + new Vector3(0, 1f, 0);

        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, player.transform.forward, out hit, interactRange))
        {
            // �C���^�[�t�F�[�X IInteractable ���������Ă��邩�m�F
            IInteractable interactableComponent = hit.collider.GetComponent<IInteractable>();
            if (interactableComponent != null)
            {
                ShowInteractText();
                canInteract = true;
            }
        }
        else
        {
            HideInteractText();
            canInteract = false;
        }
    }


    void ShowInteractText()
    {
        interactTextObject.SetActive(true); // �e�L�X�g�I�u�W�F�N�g���A�N�e�B�u��
    }

    void HideInteractText()
    {
        interactTextObject.SetActive(false); // �e�L�X�g�I�u�W�F�N�g���A�N�e�B�u��
    }
}
