using DG.Tweening;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using HighlightPlus;

public class SpriteManipulator : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Image image;

    private HighlightEffect hightlightEffect;
    private bool isGlowing = false;

    private void Awake()
    {
        hightlightEffect = gameObject.GetComponent<HighlightEffect>();
    }

    private void OnDestroy()
    {
        isGlowing = false; // ゲームオブジェクトが破棄される際にループを停止
    }

    private void OnDisable()
    {
        isGlowing = false; // スクリプトが無効になる際にループを停止
    }

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

    /// <summary>
    /// durationの間スプライトを左右に往復させる
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="magnitude"></param>
    /// <returns></returns>
    public async UniTask ShakeHorizon(float duration, float magnitude)
    {
        Vector3 originalPosition = transform.localPosition;
        float halfDuration = duration / 2;

        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            // 左に移動
            await transform.DOLocalMoveX(originalPosition.x - magnitude, halfDuration).SetEase(Ease.InOutQuad).AsyncWaitForCompletion();

            // 右に移動
            await transform.DOLocalMoveX(originalPosition.x + magnitude, halfDuration).SetEase(Ease.InOutQuad).AsyncWaitForCompletion();

            elapsed += duration; // ループの終了を確実にするために、経過時間を更新
        }

        // 元の位置に戻す
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

    public async UniTask FlashWithHighlightPlus(float duration, Color color)
    {
        float initialGlow = 0.001f;
        var effect = gameObject.GetComponent<HighlightEffect>();
        if (effect != null)
        {
            float elapsed = initialGlow;
            effect.glowMaskMode = MaskMode.IgnoreMask;

            while (elapsed < duration)
            {
                // 経過時間に基づいて輝度を補間する
                float t = elapsed / duration;  // 0.001から1の値

                effect.glow = t;

                await UniTask.Yield(PlayerLoopTiming.Update); // フレームの更新毎に待機
                elapsed += Time.deltaTime;
            }
            effect.glow = initialGlow;           
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
        await image.DOFade(1.0f, duration).SetEase(Ease.InOutQuad).SetUpdate(true).ToUniTask();

    }

    public async UniTask FadeOut(float duration)
    {
        image = gameObject.GetComponent<Image>();

        // Imageコンポーネントが存在することを確認
        if (image == null)
        {
            Debug.LogError("Image component not found!");
            return;
        }

        // 初期透明度を0に設定
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1);

        // 透明度を0から1へ変化させる
        await image.DOFade(0f, duration).SetEase(Ease.InOutQuad).SetUpdate(true).ToUniTask();

    }

    public async UniTask AnimateColor(Color targetColor, float duration = 0.3f)
    {
        await image.DOColor(targetColor, duration);
    }

    /// <summary>
    /// 【HighlightPlus使用】スプライト点滅開始
    /// </summary>
    /// <param name="minBrightness">最小明度</param>
    /// <param name="maxBrightness">最大明度</param>
    /// <param name="totalDuration">点滅間隔</param>
    /// <param name="cancellationToken">キャンセルトークン</param>
    /// <returns></returns>
    public async void StartGlowingEffect(float minBrightness, float maxBrightness, float totalDuration)
    { 
        if (hightlightEffect != null)
        {
            isGlowing = true;
            float startTime = Time.time;

            while (isGlowing)
            {
                float elapsed = Time.time - startTime;
                float cycleTime = elapsed % totalDuration; // totalDurationごとにリセット
                float value = Mathf.PingPong(cycleTime / totalDuration * 2 * (maxBrightness - minBrightness), maxBrightness - minBrightness) + minBrightness;
                hightlightEffect.glow = value;

                await UniTask.Yield(PlayerLoopTiming.Update); // 次のフレームまで待機
            }
        }     
    }

    /// <summary>
    /// スプライト点滅停止
    /// </summary>
    /// <param name="index">点滅停止させるスプライトの敵パーティ内でのインデックス</param>
    /// <returns></returns>
    public void StopGlowingEffect()
    {
        isGlowing = false;
        if (hightlightEffect != null)
        {
            hightlightEffect.glow = 0.001f;
        }
    }

    /// <summary>
    /// スプライトアウトライン点滅開始
    /// </summary>
    /// <param name="effect">HighlightPlusコンポーネント</param>
    /// <param name="minBrightness">最小明度</param>
    /// <param name="maxBrightness">最大明度</param>
    /// <param name="totalDuration">点滅間隔</param>
    /// <param name="cancellationToken">キャンセルトークン</param>
    /// <returns></returns>
    private async UniTask AnimateGlowBrightness(HighlightEffect effect, float minBrightness, float maxBrightness, float totalDuration, Color color, CancellationToken cancellationToken)
    {
        float startTime = Time.time;
        effect.outlineColor = color;
        while (!cancellationToken.IsCancellationRequested)
        {
            float elapsed = Time.time - startTime;
            float cycleTime = elapsed % totalDuration; // totalDurationごとにリセット
            float value = Mathf.PingPong(cycleTime / totalDuration * 2 * (maxBrightness - minBrightness), maxBrightness - minBrightness) + minBrightness;
            effect.outline = value;

            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken); // 次のフレームまで待機
        }
        effect.outline = 0.001f;
    }

    /// <summary>
    /// スプライトアウトライン点滅停止
    /// </summary>
    /// <param name="index">点滅停止させるスプライトの敵パーティ内でのインデックス</param>
    /// <returns></returns>
    public void StopFlashingGlowEffect(int index, CancellationTokenSource cancellationTokenSource)
    {
        GameObject obj = EnemyManager.Instance.GetEnemyIns(index);
        if (obj != null)
        {
            HighlightEffect effect = obj.GetComponentInChildren<HighlightEffect>();
            if (effect != null)
            {
                effect.outline = 0.001f;
            }
            cancellationTokenSource.Cancel();
        }
    }
}