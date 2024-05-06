using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.InputSystem;

public class InputActionManager : MonoBehaviour
{
    public GameObject inputActionParent;
    private PlayerInput playerInput;


    private void Start()
    {
        playerInput = inputActionParent.GetComponent<PlayerInput>();
        //LoadAndEnableInputActions();
    }

    private async void LoadAndEnableInputActions()
    {
        string path = Constants.inputActionPath;

        // Addressable Systemを使用してInputActionAssetを非同期でロード
        AsyncOperationHandle<GameObject> loadOperation = Addressables.LoadAssetAsync<GameObject>(path);
        await loadOperation.Task;

        if (loadOperation.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject obj = loadOperation.Result;
            InputActionAsset inputActionsAsset = obj.GetComponent<PlayerInput>().actions;

            //InputActionAsset inputActionsAsset = loadOperation.Result;

            // InputActionを有効化
            InputActionMap actionMap = inputActionsAsset.actionMaps[0];
            actionMap.Enable();
        }
        else
        {
            Debug.LogError("Failed to load InputActionAsset from Addressable: " + loadOperation.OperationException);
        }
    }

    private void InitializeEvents()
    {

    }

    private void OnDisable()
    {
        // InputActionを無効化
        //InputActionAsset inputActionsAsset = inputActionsReference.Asset as InputActionAsset;
        //if (inputActionsAsset != null)
        //{
        //    InputActionMap actionMap = inputActionsAsset.actionMaps[0];
        //    actionMap.Disable();
        //}
    }
}