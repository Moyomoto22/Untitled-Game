using DG.Tweening;
using UnityEngine;

/// <summary>
/// �h�A �C���^���N�g�������`�N���X
/// </summary>
public class Door : MonoBehaviour, IInteractable
{
    private float rotationDuration = 1f; // ��]�̎���
    private float fadeDuration = 1f; // �t�F�[�h�A�E�g�̎���
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
        // �h�A�̃��j�[�N�L�[��ݒ�
        doorKey = gameObject.name + "_isOpened";

        if (ObjectStateManager.Instance != null)
        {
            // DoorStateManager����J��Ԃ�ǂݍ���
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
            // �R���W�������I�t
            collider.enabled = false;

            SoundManager.Instance.DoorOpen1();

            Quaternion currentRotation = transform.rotation;
            Quaternion targetRotation = currentRotation * Quaternion.Euler(0, 90, 0);

            // Y����90�x��]
            transform.DORotateQuaternion(targetRotation, rotationDuration);

            isOpened = true;

            // �J��Ԃ�DoorStateManager�ɕۑ�
            ObjectStateManager.Instance.SetState(doorKey, isOpened);
        }
    }

    public void OpenInstantly()
    {
        // �h�A�𑦍��ɊJ������Ԃɂ���
        Quaternion currentRotation = transform.rotation;
        Quaternion targetRotation = currentRotation * Quaternion.Euler(0, 90, 0);

        transform.rotation = targetRotation;
    }

    public void Close()
    {
        
    }
}