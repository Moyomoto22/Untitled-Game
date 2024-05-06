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
            // 経過時間に基づいて色を補間する
            float t = elapsed / duration;  // 0から1の値
            Color currentColor = Color.Lerp(color, Color.white, t);

            if (spriteRenderer != null)
            {
                spriteRenderer.color = currentColor;
            }
            else if (image != null)
            {
                image.color = currentColor;
            }

            await UniTask.Yield(PlayerLoopTiming.Update); // フレームの更新毎に待機
            elapsed += Time.deltaTime;
        }

        // ループ終了後、色を元に戻す
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

        // Imageコンポーネントが存在することを確認
        if (image == null)
        {
            Debug.LogError("Image component not found!");
            return;
        }

        // 初期透明度を0に設定
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);

        // 透明度を0から1へ変化させる
        await image.DOFade(0.5f, duration).SetEase(Ease.InOutQuad).SetUpdate(true).ToUniTask();

        //float elapsed = 0.0f;

        //while (elapsed < duration)
        //{
        //    // 経過時間に基づいて色を補間する
        //    float t = elapsed / duration;  // 0から1の値
        //    Color currentColor = Color.Lerp(Color.clear, Color.white, t);

        //    if (spriteRenderer != null)
        //    {
        //        spriteRenderer.color = currentColor;
        //    }
        //    else if (image != null)
        //    {
        //        image.color = currentColor;
        //    }

        //    await UniTask.Yield(PlayerLoopTiming.Update); // フレームの更新毎に待機
        //    elapsed += Time.deltaTime;
        //}
    }
}