using DG.Tweening;
using UnityEngine;

/// <summary>
/// ドア インタラクト時動作定義クラス
/// </summary>
public class Door : MonoBehaviour, IInteractable
{
    private float rotationDuration = 1f; // 回転の時間
    private float fadeDuration = 1f; // フェードアウトの時間
    private Material material;

    private string doorKey;
    private bool isOpened = false;

    public GameObject doorObject;

    void Start()
    {
        isOpened = false;
    }

    void Awake()
    {
        // ドアのユニークキーを設定
        doorKey = gameObject.name + "_isOpened";

        if (ObjectStateManager.Instance != null)
        {
            // DoorStateManagerから開閉状態を読み込む
            isOpened = ObjectStateManager.Instance.GetDoorState(doorKey);
        }
        
        if (isOpened)
        {
            OpenInstantly();
        }
    }


    public void Interact()
    {
        InteractDoor();
    }

    public void InteractDoor()
    {
        Debug.Log("InteractDoor called");

        Open();
    }

    public void Open()
    {
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        BoxCollider collider = gameObject.GetComponent<BoxCollider>();
        if (meshRenderer != null && collider != null)
        {
            // コリジョンをオフ
            collider.enabled = false;

            SoundManager.Instance.DoorOpen1();

            Quaternion currentRotation = transform.rotation;
            Quaternion targetRotation = currentRotation * Quaternion.Euler(0, 90, 0);

            // Y軸に90度回転
            transform.DORotateQuaternion(targetRotation, rotationDuration);

            isOpened = true;

            // 開閉状態をDoorStateManagerに保存
            ObjectStateManager.Instance.SetState(doorKey, isOpened);
        }
    }

    public void OpenInstantly()
    {
        // ドアを即座に開いた状態にする
        Quaternion currentRotation = transform.rotation;
        Quaternion targetRotation = currentRotation * Quaternion.Euler(0, 90, 0);

        transform.rotation = targetRotation;
    }

    public void Close()
    {
        
    }
}