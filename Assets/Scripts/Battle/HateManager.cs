using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ヘイト管理クラス
/// </summary>
public class HateManager : MonoBehaviour
{
    private static HateManager _instance;

    private List<Ally> allies;
    private List<double> hates;

    [SerializeField]
    private List<Sprite> sprites;
    [SerializeField]
    private List<GameObject> spriteObjects;

    private int maxHateIndex = 0;

    public static HateManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<HateManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("HateManager");
                    _instance = obj.AddComponent<HateManager>();
                }
            }
            return _instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    void Awake()
    {
        Initialize();
        SetHateSprites();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Initialize()
    {
        List<double> newHates = new List<double>() { 0, 0, 0, 0};

        // 初期ヘイト値を格納するリスト
        List<double> numbers = new List<double> { 0, 1, 2, 3 };

        // ランダムインスタンスを作成
        System.Random random = new System.Random();

        // numbersリストからランダムに値を取り出して、元のリストに設定
        for (int i = 0; i < newHates.Count; i++)
        {
            int randomIndex = random.Next(numbers.Count);
            newHates[i] = numbers[randomIndex];
            numbers.RemoveAt(randomIndex);
        }

        hates = newHates;
    }

    /// <summary>
    /// ヘイトの割合に応じてヘイトアイコンを設定する
    /// </summary>
    private void SetHateSprites()
    {
        // hatesが空の場合は処理しない
        if (hates == null || hates.Count == 0)
        {
            Debug.LogWarning("Hates list is empty or not initialized.");
            return;
        }

        // hatesの最大値を取得
        double maxHate = hates.Max();

        for (int i = 0; i < hates.Count; i++)
        {
            double hateValue = hates[i];
            double r = hateValue / maxHate;
            int h;

            if (r >= 1) { h = 5; maxHateIndex = i; }          // 100%
            else if (r >= 0.75f) { h = 4; } // 99 ~ 75%
            else if (r > 0.5f) { h = 3; }   // 74 ~ 50%
            else if (r >= 0.25f) { h = 2; } // 49 ~ 25%
            else { h = 1; }                 // 24 ~  0%

            if (spriteObjects[i] != null)
            {
                SpriteRenderer spriteRenderer = spriteObjects[i].GetComponent<SpriteRenderer>();
                if (spriteRenderer != null && h > 0 && h <= sprites.Count)
                {
                    spriteRenderer.sprite = sprites[h - 1]; // リストは0から始まるため、h-1にする

                    var spriteManipurator = spriteObjects[i].GetComponent<SpriteManipulator>();
                    if (spriteManipurator != null)
                    {
                        if (h == 5)
                        {
                            // ヘイトが最大の場合、アイコン点滅開始
                            spriteManipurator.StartGlowingEffect(0.001f, 3.0f, 1.0f);
                        }
                        else
                        {
                            // アイコン点滅停止
                            spriteManipurator.StopGlowingEffect();
                        }
                    }
                }
                else
                {
                    Debug.LogWarning($"Sprite index {h - 1} is out of bounds for sprites list.");
                }
            }
            else
            {
                Debug.LogWarning($"Sprite object at index {i} is null.");
            }
        }
    }

    /// <summary>
    /// ヘイトが最大のキャラクターのインデックスを取得する
    /// </summary>
    /// <returns></returns>
    public int GetTargetIndex()
    {
        if (hates == null || hates.Count == 0)
        {
            throw new ArgumentException("The list cannot be null or empty.");
        }

        double maxValue = hates.Max();
        List<int> maxIndices = hates.Select((value, index) => new { value, index })
                                   .Where(x => x.value == maxValue)
                                   .Select(x => x.index)
                                   .ToList();

        System.Random random = new System.Random();

        // 同値が複数人いる場合、ランダムで選択
        int randomIndex = random.Next(maxIndices.Count);

        return maxIndices[randomIndex];
    }

    public Ally GetTargetWithRandom()
    {
        var allies = PartyMembers.Instance.GetAllies();
        
        var random = UnityEngine.Random.value;
        int targetIndex;

        if (random < 0.25f)
        {
            targetIndex = 0;
        }
        else if (random < 0.5f)
        {
            targetIndex = 1;
        }
        else if (random < 0.75f)
        {
            targetIndex = 2;
        }
        else
        {
            targetIndex = 3;
        }

        return allies[targetIndex];
    }

    /// <summary>
    /// 与ダメージによるヘイト値加算
    /// </summary>
    /// <param name="targetIndex"></param>
    /// <param name="damage"></param>
    /// <param name="rate"></param>
    public void IncreaseHate(int targetIndex, int damage, double rate)
    {
        var earnedHate = damage * rate;

        hates[targetIndex] += earnedHate;

        SetHateSprites();
    }

    /// <summary>
    /// 定数ヘイト値増減
    /// </summary>
    /// <param name="targetIndex"></param>
    /// <param name="damage"></param>
    /// <param name="rate"></param>
    public void UpdateHateByConst(int targetIndex, int value)
    {
        hates[targetIndex] += value;

        SetHateSprites();
    }

    public void ReduceHate(int targetIndex, double reduceRate)
    {
        hates[targetIndex] = hates[targetIndex] * reduceRate;

        SetHateSprites();
    }

}
