using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ゲージ類管理クラス
/// </summary>
public class GaugeManager : MonoBehaviour
{
    //private int maxValue;
    //private int currentValue;
    public bool isUpdateByText = false;

    public TextMeshProUGUI maxValueText;
    public TextMeshProUGUI currentValueText;

    // ゲージスプライト(UI)
    public Image gaugeSprite;
    // ゲージ(スプライトマテリアル - 敵用)
    public SpriteRenderer renderer;

    // Start is called before the first frame update
    private void Start()
    {
        //gaugeImage = gameObject.GetComponent<Image>();
    }

    private void Update()
    {
        if (isUpdateByText)
        {
            updateGaugeByText();
        }
    }

    private void Awake()
    {
        updateGaugeByText();
    }

    public void updateGauge(int maxValue, int currentValue)
    {
        float per = (float)currentValue / (float)maxValue;

        if (gaugeSprite != null)
        {
            gaugeSprite.fillAmount = per;
        }
    }

    public void updateGaugeByText()
    {
        if (maxValueText != null && currentValueText != null)
        {
            int maxValue = int.Parse(maxValueText.text);
            int currentValue = int.Parse(currentValueText.text);
            float per = (float)currentValue / (float)maxValue;

            if (gaugeSprite != null)
            {
                gaugeSprite.fillAmount = per;
            }
        }
    }

    /// <summary>
    /// テキストとゲージを同時にアニメーションさせる
    /// </summary>
    /// <param name="targetValue"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public async UniTask AnimateTextAndGauge(int targetValue, float duration = 1.0f)
    {
        // キャスト失敗、ゼロ除算時
        if (!int.TryParse(maxValueText.text, out var maxValue) || maxValue <= 0)
        {
            return;
        }

        float targetFillAmount = (float)targetValue / maxValue;

        await UniTask.WhenAll(
            CountText(targetValue, duration),
            AnimateGauge(targetFillAmount, duration)
            );
    }

    /// <summary>
    /// テキストを指定の値までカウントアップ・ダウンさせる
    /// </summary>
    /// <param name="targetValue">カウントさせる目標値</param>
    /// <param name="duration">カウントさせる時間間隔</param>
    /// <returns></returns>
    public async UniTask CountText(int targetValue, float duration = 1.0f)
    {
        if (!int.TryParse(currentValueText.text, out var currentValue))
        {
            return;
        }
        await DOTween.To(
            () => currentValue,                        // 開始値
            x => currentValueText.text = x.ToString(), // テキスト更新のアクション
            targetValue,                            　 // 目標値
            duration                                　 // 継続時間
        ).SetEase(Ease.OutQuad).SetUpdate(true);                       // アニメーションのイージング設定
    }

    /// <summary>
    /// ゲージ画像のFillAmountを指定の値までアニメーションさせる
    /// </summary>
    /// <param name="targetFillAmount">目標FillAmount</param>
    /// <param name="duration">時間間隔</param>
    /// <returns></returns>
    public async UniTask AnimateGauge(float targetFillAmount, float duration = 1.0f)
    {
        await gaugeSprite.DOFillAmount(targetFillAmount, duration).SetEase(Ease.OutQuad).SetUpdate(true);
    }

    /// <summary>
    /// 敵用ゲージをアニメーションさせる
    /// </summary>
    /// <param name="targetFillAmount"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public async UniTask AnimateGaugeRenderer(float targetFillAmount, float duration = 1.0f)
    {
        Material material = renderer.material;

        if (material != null)
        {
            float currentAmount = material.GetFloat("_FillAmount");
            await DOTween.To(() => material.GetFloat("_FillAmount"), x => material.SetFloat("_FillAmount", x), targetFillAmount, duration);
        }
    }
}
