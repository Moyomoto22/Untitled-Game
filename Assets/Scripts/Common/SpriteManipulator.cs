using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class SpriteManipulator : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Image image;

    public async UniTask Shake(float duration, float magnitude)
    {
        Vector3 originalPosition = transform.localPosition;

        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            float x = UnityEngine.Random.Range(-1f, 1f) * magnitude;
            float y = UnityEngine.Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

            // Wait until next frame
            await UniTask.Yield();
            elapsed += Time.deltaTime;
        }

        transform.localPosition = originalPosition;
    }

    public async UniTask Flash(float duration, Color color)
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            // �o�ߎ��ԂɊ�Â��ĐF���Ԃ���
            float t = elapsed / duration;  // 0����1�̒l
            Color currentColor = Color.Lerp(color, Color.white, t);

            if (spriteRenderer != null)
            {
                spriteRenderer.color = currentColor;
            }
            else if (image != null)
            {
                image.color = currentColor;
            }

            await UniTask.Yield(PlayerLoopTiming.Update); // �t���[���̍X�V���ɑҋ@
            elapsed += Time.deltaTime;
        }

        // ���[�v�I����A�F�����ɖ߂�
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }
        else if (image != null)
        {
            image.color = Color.white;
        }
    }

    public async UniTask FadeIn(float duration)
    {
        image = gameObject.GetComponent<Image>();

        // Image�R���|�[�l���g�����݂��邱�Ƃ��m�F
        if (image == null)
        {
            Debug.LogError("Image component not found!");
            return;
        }

        // ���������x��0�ɐݒ�
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);

        // �����x��0����1�֕ω�������
        await image.DOFade(0.5f, duration).SetEase(Ease.InOutQuad).SetUpdate(true).ToUniTask();

        //float elapsed = 0.0f;

        //while (elapsed < duration)
        //{
        //    // �o�ߎ��ԂɊ�Â��ĐF���Ԃ���
        //    float t = elapsed / duration;  // 0����1�̒l
        //    Color currentColor = Color.Lerp(Color.clear, Color.white, t);

        //    if (spriteRenderer != null)
        //    {
        //        spriteRenderer.color = currentColor;
        //    }
        //    else if (image != null)
        //    {
        //        image.color = currentColor;
        //    }

        //    await UniTask.Yield(PlayerLoopTiming.Update); // �t���[���̍X�V���ɑҋ@
        //    elapsed += Time.deltaTime;
        //}
    }
}