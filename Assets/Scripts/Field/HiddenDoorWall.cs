using DG.Tweening;
using UnityEngine;

/// <summary>
/// 隠しドア インタラクト時動作定義クラス
/// </summary>
public class HiddenDoorWall : MonoBehaviour, IInteractable
{
    private float duration = 1f;
    public GameObject obj;

    // アニメーション動作タイプ 0:X軸+ ⇒ Z軸+, 1:X軸+ ⇒ Z軸+
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
            // コリジョンをオフ
            //collider.enabled = false;

            Vector3 startPosition = transform.position;

            // シーケンスを作成
            Sequence sequence = DOTween.Sequence();
            
            if (animationType == 0)
            {
                sequence.Append(transform.DOMoveX(startPosition.x + 0.25f, duration)); // X軸方向に移動 
                sequence.Append(transform.DOMoveZ(startPosition.z + 4.0f, duration));  // Z軸方向に移動
            }
            else
            {
                sequence.Append(transform.DOMoveZ(startPosition.z + 0.25f, duration)); // Z軸方向に移動
                sequence.Append(transform.DOMoveX(startPosition.x + 4.0f, duration));  // X軸方向に移動 
            }
            // シーケンスを開始
            sequence.Play();
        }
    }

    public void Close()
    {

    }
}