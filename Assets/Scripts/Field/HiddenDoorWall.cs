using DG.Tweening;
using UnityEngine;

/// <summary>
/// �B���h�A �C���^���N�g�������`�N���X
/// </summary>
public class HiddenDoorWall : MonoBehaviour, IInteractable
{
    private float duration = 1f;
    public GameObject obj;

    // �A�j���[�V��������^�C�v 0:X��+ �� Z��+, 1:X��+ �� Z��+
    public int animationType = 0;

    public void Interact()
    {
        InteractHiddenDoorWall();
    }

    public void InteractHiddenDoorWall()
    {
        Open();
    }

    public void Open()
    {
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        MeshCollider collider = gameObject.GetComponent<MeshCollider>();
        if (meshRenderer != null && collider != null)
        {
            // �R���W�������I�t
            //collider.enabled = false;

            Vector3 startPosition = transform.position;

            // �V�[�P���X���쐬
            Sequence sequence = DOTween.Sequence();
            
            if (animationType == 0)
            {
                sequence.Append(transform.DOMoveX(startPosition.x + 0.25f, duration)); // X�������Ɉړ� 
                sequence.Append(transform.DOMoveZ(startPosition.z + 4.0f, duration));  // Z�������Ɉړ�
            }
            else
            {
                sequence.Append(transform.DOMoveZ(startPosition.z + 0.25f, duration)); // Z�������Ɉړ�
                sequence.Append(transform.DOMoveX(startPosition.x + 4.0f, duration));  // X�������Ɉړ� 
            }
            // �V�[�P���X���J�n
            sequence.Play();
        }
    }

    public void Close()
    {

    }
}