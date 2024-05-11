using UnityEngine;
using DG.Tweening;
using TMPro;
using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

/// <summary>
/// テキストアニメーション管理クラス
/// </summary>
public class TextAnimationManager : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    public List<GameObject> objects;

    private List<TextMeshProUGUI> TMPros;

    public float duration = 0.2f;                        // アニメーションの総時間
    public Vector3 startScale = new Vector3(4f, 4f, 4f); // 開始時のスケール
    public float endScale = 1.0f;                        // 終了時のスケール

    public string text;

    void Start()
    {
        gameObject.SetActive(true);
    }

    private async void Awake()
    {
        TMPros = objects.Select(obj => obj.GetComponent<TextMeshProUGUI>()).ToList();
    }

    /// <summary>
    /// ダメージテキストアニメーション準備
    /// </summary>
    /// <param name="target"></param>
    /// <param name="damageStrings"></param>
    /// <returns></returns>
    public async UniTask ShowDamageTextAnimation(CharacterStatus target, List<string> damageStrings, SpriteManipulator manip)
    {
        if (damageStrings.Count > 0)
        {
            // 戦闘画面のキャンバスを取得
            Canvas canvas = GameObject.Find("BattleSceneCanvas").GetComponent<Canvas>();
            // 次のテキスト表示までのディレイ
            float delay = 0.5f / damageStrings.Count;

            for (int i = 0; i < damageStrings.Count; i++)
            {
                GameObject at = Instantiate(gameObject, canvas.transform);
                RectTransform rectTransform = at.GetComponent<RectTransform>();

                // 事前に取得した座標をキャンバス上での座標に変換
                rectTransform.anchoredPosition = target.positionInScreen;
                rectTransform.localRotation = Quaternion.identity;

                if (rectTransform != null)
                {
                    // 2ループ目以降、上下左右にランダムに表示位置をずらす
                    System.Random random = new System.Random();
                    var randomOffsetX = 0.0f;
                    var randomOffsetY = 0.0f;

                    if (i > 0)
                    {
                        randomOffsetX = (float)random.NextDouble() * 100.0f - 50.0f;
                        randomOffsetY = (float)random.NextDouble() * 100.0f - 50.0f;
                    }

                    // RectTransformの位置を設定
                    rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x + randomOffsetX, rectTransform.anchoredPosition.y + randomOffsetY);
                }
                // アニメーション開始
                _ = UniTask.WhenAll(
                    at.GetComponent<TextAnimationManager>().RevealDamageTextAnimation(damageStrings[i]),
                    manip.Shake(0.2f, 0.1f),
                    manip.Flash(0.2f, Color.red));

                // 次ループまでディレイ
                await UniTask.Delay(TimeSpan.FromSeconds(delay));
            }
            await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
        }
    }

    /// <summary>
    /// 回復テキストアニメーション準備
    /// </summary>
    /// <param name="target"></param>
    /// <param name="damageStrings"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    public async UniTask ShowHealTextAnimation(CharacterStatus target, List<string> damageStrings, Color color)
    {
        GameObject canvasObject = GameObject.Find("BattleSceneCanvas");  
        if (canvasObject != null)
        {
            Canvas canvas = canvasObject.GetComponent<Canvas>();
            if (damageStrings.Count > 0 && canvas != null)
            {
                float delay = 0.1f / damageStrings.Count;

                for (int i = 0; i < damageStrings.Count; i++)
                {
                    GameObject at = Instantiate(gameObject, canvas.transform);
                    RectTransform rectTransform = at.GetComponent<RectTransform>();

                    // 事前に取得した座標をキャンバス上での座標に変換
                    rectTransform.anchoredPosition = target.positionInScreen;
                    rectTransform.localRotation = Quaternion.identity;

                    if (rectTransform != null)
                    {
                        // 上下左右にランダムに表示位置をずらす
                        System.Random random = new System.Random();
                        var randomOffsetX = 0.0f;
                        var randomOffsetY = 0.0f;

                        if (i > 0)
                        {
                            randomOffsetX = (float)random.NextDouble() * 100.0f - 50.0f;
                            randomOffsetY = (float)random.NextDouble() * 100.0f - 50.0f;
                        }
                        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x + randomOffsetX, rectTransform.anchoredPosition.y + randomOffsetY);
                    }
                    var ta = at.GetComponent<TextAnimationManager>();
                    if (ta != null)
                    {
                        await ta.RevealHealTextAnimation(damageStrings[i], color);
                    }
                    await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
                }
            }
        }
    }

    /// <summary>
    /// テキストアニメーション表示開始
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public async UniTask RevealDamageTextAnimation(string text)
    {
        System.Random random = new System.Random();
        var array = TextToArray(text);

        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].SetActive(true); // オブジェクトをアクティブ化

            var textMesh = objects[i].GetComponent<TextMeshProUGUI>();
            textMesh.text = array[i];
            var rectTransform = objects[i].GetComponent<RectTransform>();

            // ランダムな回転、オフセット、スケールを生成
            var randomRotationZ = (float)random.NextDouble() * 20.0f - 10.0f;
            var randomOffsetX = (float)random.NextDouble() * 10.0f - 5.0f;
            var randomOffsetY = (float)random.NextDouble() * 10.0f - 5.0f;
            var randomScale = (float)random.NextDouble() * 1.5f + 1.0f;

            if (textMesh != null)
            {
                if (rectTransform != null)
                {
                    // ランダムにテキスト変形
                    rectTransform.rotation = Quaternion.Euler(0, 0, randomRotationZ);
                    rectTransform.position = new Vector3(rectTransform.position.x + randomOffsetX, rectTransform.position.y + randomOffsetY, 0);
                    rectTransform.localScale = new Vector3(randomScale, randomScale, 1);
                }

                textMesh.transform.localScale = startScale; // 初期スケール設定
                await textMesh.DOFade(1f, 0.05f).From(0f); // フェードイン
                await textMesh.transform.DOScale(Vector3.one * randomScale, 0.05f); // スケールのアニメーション
            }
            await UniTask.Delay(TimeSpan.FromSeconds(0.01f));
        }
        // フェードアウト
        await FadeOutAllTexts();
        Destroy(gameObject);
    }

    /// <summary>
    /// テキストアニメーション表示開始
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public async UniTask RevealHealTextAnimation(string text, Color color)
    {
        var array = TextToArray(text);

        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].SetActive(true); // オブジェクトをアクティブ化

            var textMesh = objects[i].GetComponent<TextMeshProUGUI>();
            textMesh.text = array[i];
            var rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.localScale = new Vector3(1.5f, 1.5f, 1);

            if (textMesh != null)
            {
                textMesh.color = color;  // テキストの色を変更
                _ = textMesh.DOFade(1f, 0.05f).From(0f); // フェードイン
                _ = textMesh.rectTransform.DOAnchorPosY(-20f, 0.05f).SetEase(Ease.InOutQuad); // 上から落下
            }
            await UniTask.Delay(TimeSpan.FromSeconds(0.05f));
        }
        await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
        await FadeOutAllTexts();
    }

    /// <summary>
    /// 文字列を一文字づつの配列に変換する
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private string[] TextToArray(string text)
    {
        string[] array = new string[5];

        // 文字列の各文字を配列に格納
        for (int i = 0; i < text.Length; i++)
        {
            array[i + 1] = text[i].ToString();
        }

        // 配列の未使用部分を空文字で埋める
        for (int i = text.Length + 1; i < array.Length; i++)
        {
            array[i] = "";
        }
        return array;
    }

    /// <summary>
    /// テキストを全てフェードアウトさせる
    /// </summary>
    /// <returns></returns>
    private async UniTask FadeOutAllTexts()
    {
        // 各テキストコンポーネントにDOFadeを適用し、それぞれのUniTaskをリストに格納
        var fadingTasks = new UniTask[TMPros.Count];
        for (int i = 0; i < TMPros.Count; i++)
        {
            var text = TMPros[i];
            fadingTasks[i] = FadeText(text);
        }

        await UniTask.WhenAll(fadingTasks);
    }

    /// <summary>
    /// テキストをフェードアウトさせる
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    UniTask FadeText(TextMeshProUGUI text)
    {
        // DOTweenのアニメーションをUniTaskとして返す
        return text.DOFade(0, 0.2f).ToUniTask();
    }
}
