using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using HighlightPlus;

/// <summary>
/// 戦闘画面コントローラー
/// </summary>
public class BattleController : MonoBehaviour
{

    public EventSystem eventSystem;
    public BattleCommandManager battleCommandManager;

    public GameObject mainCanvas;
    public GameObject resultCanvas;
    public BattleResultControler resultController;

    public GameObject currentName;
    public GameObject nameBackground;

    public GameObject attackButton;
    public GameObject skillButton;
    public GameObject itemButton;
    public GameObject blockButton;
    public GameObject runButton;

    public List<GameObject> faceImages;
    public List<GameObject> HPGauges;
    public List<GameObject> MPGauges;
    public List<GameObject> TPGauges;
    public List<GameObject> hates;
    public List<GameObject> effectSpriteObjectsParents;

    private Dictionary<int, int> speeds = new Dictionary<int, int>();
    public GameObject MainCamera;

    public GameObject animatedText;

    private CancellationTokenSource cancellationTokenSource;

    private static readonly System.Random random = new System.Random();

    public EnemyPartyStatus dummyEnemyParty;

    async void Start()
    {
        // 初期化処理
        await Initialize();
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    /// <returns></returns>
    private async Task Initialize()
    {
        CommonVariableManager.turns = 0;
        // 味方のステータス
        await GetAllyes();

        // GetAllyesの処理が完了した後にSetEnemiesを実行
        SetEnemies();

        OrderManager.Instance.Initialize();
        var order = OrderManager.Instance.GetActionOrder(CommonVariableManager.turns);

        SetCommnadHeaderState();
    }

    /// <summary>
    /// 味方キャラクターのステータスを取得する
    /// </summary>
    /// <returns></returns>
    private async Task GetAllyes()
    {
        //await PartyMembers.Instance.Initialize();
        List<Ally> allies = PartyMembers.Instance.GetAllies();

        for (int i = 0; i < 4; i++)
        {
            Ally Ally = allies[i]; //await CommonController.GetAlly(i + 1);
            Ally.spriteObject = faceImages[i];                                    // 画面下部 バストアップ画像
            Ally.HPGauge = HPGauges[i];                                           // HPゲージ
            Ally.MPGauge = MPGauges[i];                                           // MPゲージ
            Ally.TPGauge = TPGauges[i];                                           // TPゲージ

            GaugeManager hpGaugeManager = HPGauges[i].GetComponent<GaugeManager>();     // HPゲージ管理クラス
            GaugeManager mpGaugeManager = MPGauges[i].GetComponent<GaugeManager>();     // MPゲージ管理クラス
            GaugeManager tpGaugeManager = TPGauges[i].GetComponent<GaugeManager>();     // TPゲージ管理クラス

            hpGaugeManager.maxValueText.text = Ally.MaxHp.ToString();            // 最大HPテキスト
            hpGaugeManager.currentValueText.text = Ally.HP.ToString();            // 現在HPテキスト
            mpGaugeManager.maxValueText.text = Ally.MaxMp.ToString();            // 最大MPテキスト
            mpGaugeManager.currentValueText.text = Ally.MP.ToString();            // 現在MPテキスト
            tpGaugeManager.currentValueText.text = Ally.TP.ToString();            // 現在TPテキスト

            hpGaugeManager.updateGaugeByText();
            mpGaugeManager.updateGaugeByText();
            tpGaugeManager.updateGaugeByText();

            speeds.Add(i, Ally.Agi);
            faceImages[i].GetComponent<SpriteRenderer>().sprite = Ally.CharacterClass.imagesC[i];
            Ally.positionInScreen = GetPositionInScreen(Ally.spriteObject.transform.parent.gameObject);

            
            if (effectSpriteObjectsParents[i] != null)
            {
                int childCount = effectSpriteObjectsParents[i].transform.childCount;
                Ally.effectSpriteObjects = new List<GameObject>(childCount);
                for (int j = 0; j < childCount; j++)
                {
                    var child = effectSpriteObjectsParents[i].transform.GetChild(j);
                    if (child != null)
                    {
                        Ally.effectSpriteObjects.Add(child.gameObject);
                    }
                }

                //foreach (Transform child in effectSpriteObjectsParents[i].transform)
                //{
                //    Ally.effectSpriteObjects.Add(child.gameObject);
                //}
            }
            // パーティメンバーのシングルトンに追加
            //PartyMembers.Instance.AddCharacterToParty(Ally);   
        }
    }

    /// <summary>
    /// 敵オブジェクトの配置
    /// </summary>
    private void SetEnemies()
    {
        //EnemyManager.Instance.Initialize(); //PlayerControllerにて敵との接触時すでに初期化しているのでここでは初期化しない
        //SetTestEnemies();

        Vector3 position;
        GameObject ins;
        var party = EnemyManager.Instance.EnemyPartyMembers;

        // リストの中の敵プレハブをインスタンス化
        for (int i = 0; i < party.Count; i++)
        {
            var enemy = party[i].enemy;
            var component = enemy.GetComponent<EnemyComponent>();
            component.indexInBattle = i;

            position = party[i].position;
            ins = Instantiate<GameObject>(enemy, position, Quaternion.Euler(0, -0, 0));
            EnemyManager.Instance.AddEnemyIns(ins);

            // スキル「オウルアイ」装備中のキャラクターがいる場合、敵のHPゲージを表示
            if (PartyMembers.Instance.IsOwlEyeActivate())
            {
                EnemyManager.Instance.ShowHPGauges();
            }

            // 初期表示時、スプライトがバグるのでSpriteGlowを無効化しておく
            //var glowEffect = ins.GetComponent<SpriteGlowEffect>();
            //if (glowEffect != null)
            //{
            //    glowEffect.enabled = false;
            //}
        }
    }

    /// <summary>
    /// 味方ターン開始
    /// </summary>
    public void StartAllysTurn()
    {
        // コマンドウインドウのヘッダーの状態を設定
        SetCommnadHeaderState();

        // キャラクター画像スプライトをハイライト
        var turnCharacterIndex = TurnCharacter.Instance.CurrentCharacterIndex;
        for (int i = 0; i < 4; i++)
        {
            var manipulator = faceImages[i].GetComponent<SpriteManipulator>();          
            if (manipulator != null)
            {
                manipulator.StopGlowingEffect();
                if (i == turnCharacterIndex)
                {
                    manipulator.StartGlowingEffect(0.001f, 3.0f, 2.0f);
                }  
            }
        }

        var turnCharacter = TurnCharacter.Instance.CurrentCharacter;
        // 行動可能な場合
        if (turnCharacter.CanAct())
        {
            // コマンドウインドウの操作を有効化
            battleCommandManager.ToggleButtonsInteractable(true);
            // コマンド - 攻撃ボタンを選択
            battleCommandManager.SelectButton(0);
        }
    }

    /// <summary>
    /// 敵ターン開始
    /// </summary>
    /// <param name="index">ターン中の敵のリスト内でのインデックス</param>
    /// <returns></returns>
    public async UniTask StartEnemiesTurn(int index)
    {
        var targetIndex = HateManager.Instance.GetTargetIndex();
        var target = PartyMembers.Instance.GetAllyByIndex(targetIndex);

        // コマンドウインドウの操作を無効化
        battleCommandManager.ToggleButtonsInteractable(false);

        var enemy = EnemyManager.Instance.InstantiatedEnemies[index];
        var component = enemy.GetComponent<EnemyComponent>();

        if (component != null)
        {
            var behaviour = component.behaviour;
            if (behaviour != null)
            {
                var turnCharacter = TurnCharacter.Instance.CurrentCharacter;
                // 行動可能な場合
                if (turnCharacter.CanAct())
                {
                    await behaviour.PerformAction(target);
                }
                else
                {
                    await behaviour.Stunned();
                }
            }
            else
            {
                Debug.LogError(enemy.name + "has no EnemyBehaviour!");
            }
        }
        else
        {
            Debug.LogError(enemy.name + "has no EnemyComponent!");
        }
        await ChangeNextTurn();
    }

    /// <summary>
    /// ターン移行前準備
    /// </summary>
    public async UniTask ChangeNextTurn()
    {
        TurnCharacter.Instance.EndTurn();
        
        // キャラクターが生存しているかチェック
        CheckCharactersAreAlive();

        var aliveEnemies = EnemyManager.Instance.GetEnemiesInsExceptKnockedOut();

        // 生存している敵がいれば続行
        if (aliveEnemies.Count > 0)
        {

            // ターンキャラクターの行動回数をインクリメント
            var index = TurnCharacter.Instance.CurrentCharacterIndex;
            OrderManager.Instance.IncrementActionsTaken(index);

            // ターン数をインクリメント
            CommonVariableManager.turns += 1;


            // 行動順取得・ターンキャラクター切り替え
            OrderManager.Instance.GetActionOrder(CommonVariableManager.turns);

            var turnCharacterIndex = TurnCharacter.Instance.CurrentCharacterIndex;

            if (turnCharacterIndex < 4)
            {
                StartAllysTurn();
            }
            else
            {
                var enemyIndex = turnCharacterIndex - 4;
                await StartEnemiesTurn(enemyIndex);
            }
        }
        else
        {
            await UniTask.DelayFrame(120);
            // 終了処理
            EndBattle();
        }
    }

    /// <summary>
    /// コマンドウインドウのヘッダーの状態を設定する
    /// </summary>
    public void SetCommnadHeaderState()
    {
        var turnCharacter = TurnCharacter.Instance.CurrentCharacter;
        var tmPro = currentName.GetComponent<TextMeshProUGUI>();
        var image = nameBackground.GetComponent<Image>();

        if (turnCharacter != null && tmPro != null && image != null)
        {
            if (turnCharacter is Ally)
            {
                var c = turnCharacter as Ally;
                var index = PartyMembers.Instance.GetIndex(c);

                tmPro.text = c.CharacterName;　// キャラクター名称
                image.color = CommonController.GetCharacterColorByIndex(index);      　 // 名前背景の色
            }
            else
            {
                tmPro.text = turnCharacter.CharacterName;
                image.color = CommonController.GetColor(Constants.gray);
            }
        }
    }

    /// <summary>
    /// キャラクターが戦闘不能かチェックする
    /// </summary>
    private void CheckCharactersAreAlive()
    {
        // 味方
        var allies = PartyMembers.Instance.GetAllies();
        foreach (var ally in allies)
        {
            if (ally.HP <= 0 || ally.KnockedOut)
            {
                // 念のため戦闘不能状態にしておく
                ally.KnockedOut = true;
            }
        }
        // 敵
        var enemies = EnemyManager.Instance.InstantiatedEnemies;
        var count = enemies.Count;
        for (int i = 0; i < count; i++)
        {
            var comp = enemies[i].GetComponent<EnemyComponent>();
            var st = comp.status;
            if (st.HP <= 0 || st.KnockedOut)
            {
                // 念のため戦闘不能状態にしておく
                st.KnockedOut = true;
                if (!comp.isFadedOut)
                {
                    // フェードアウト
                    _ = EnemyManager.Instance.FadeOutEnemyIns(enemies[i]);
                    EnemyManager.Instance.removeIndexFromAliveEnemyIndexes(i);
                }

            }
        }
    }

    /// <summary>
    /// 敵スプライト点滅準備
    /// </summary>
    /// <param name="index">点滅させるスプライトの敵パーティ内でのインデックス</param>
    /// <returns></returns>
    public async UniTask StartFlashingGlowEffect(int index)
    {
        cancellationTokenSource?.Cancel();  // 前のタスクをキャンセル
        cancellationTokenSource = new CancellationTokenSource();

        var color = nameBackground.GetComponent<Image>().color;

        GameObject obj = EnemyManager.Instance.GetEnemyIns(index);
        if (obj != null)
        {
            HighlightEffect effect = obj.GetComponentInChildren<HighlightEffect>();
            if (effect != null)
            {
                await AnimateGlowBrightness(effect, 0.001f, 0.5f, 0.8f, color, cancellationTokenSource.Token);
            }
        }
    }

    /// <summary>
    /// 敵スプライト点滅開始
    /// </summary>
    /// <param name="glowEffect">SpriteGlowコンポーネント</param>
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
    /// 敵スプライト点滅停止
    /// </summary>
    /// <param name="index">点滅停止させるスプライトの敵パーティ内でのインデックス</param>
    /// <returns></returns>
    public void StopFlashingGlowEffect(int index)
    {
        GameObject obj = EnemyManager.Instance.GetEnemyIns(index);
        if (obj != null)
        {
            HighlightEffect effect = obj.GetComponentInChildren<HighlightEffect>();
            if (effect != null)
            {
                effect.outline = 0.001f;
            }
            cancellationTokenSource?.Cancel();
        }
    }

    private async void EndBattle()
    {
        await FadeOut(mainCanvas);

        resultController.earnedExp = CalculateEarnedExp();
        resultController.earnedGold = CalculateEarnedGold();
        resultController.earnedItems = GetDropItems();

        resultCanvas.SetActive(true);
    }

    private int CalculateEarnedExp()
    {
        int exp = 0;
        var enemies = EnemyManager.Instance.GetAllEnemiesStatus();
        
        foreach(Enemy enemy in enemies)
        {
            exp += enemy.Exp;
        }

        return exp;
    }

    private int CalculateEarnedGold()
    {
        int gold = 0;
        var enemies = EnemyManager.Instance.GetAllEnemiesStatus();

        foreach (Enemy enemy in enemies)
        {
            gold += enemy.Gold;
        }

        return gold;
    }

    private List<Item> GetDropItems()
    {
        List<Item> earnedItems = new List<Item>();
        var enemies = EnemyManager.Instance.GetAllEnemiesStatus();

        foreach (Enemy enemy in enemies)
        {
            if (enemy.DropItemOne != null)
            {
                int result = random.Next(1);
                if (enemy.DropRateOne > result)
                {
                    earnedItems.Add(enemy.DropItemOne);
                    continue;
                }
            }
            if (enemy.DropItemTwo != null)
            {
                int result = random.Next(1);
                if (enemy.DropRateTwo > result)
                {
                    earnedItems.Add(enemy.DropItemTwo);
                }
            }
        }
        return earnedItems;
    }

    public async UniTask FadeOut(GameObject gameObject, float duration = 0.3f)
    {
        // ゲームオブジェクトと CanvasGroup の存在を確認
        if (gameObject != null && gameObject.activeInHierarchy)
        {
            CanvasGroup canvasGroup = gameObject.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                // 透明度を0にアニメーション
                await canvasGroup.DOFade(0, duration).SetEase(Ease.InOutQuad).SetUpdate(true).ToUniTask();
                canvasGroup.interactable = false;
            }
            // アニメーション完了後にゲームオブジェクトを破棄
            if (gameObject != null)
            {
                //Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// シーン破棄
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    IEnumerator UnloadOldScene(string sceneName)
    {
        yield return SceneManager.UnloadSceneAsync(sceneName);
    }

    private Vector2 GetPositionInScreen(GameObject obj)
    {
        if (Camera.main != null)
        {
            Canvas canvas = FindObjectOfType<Canvas>(); // Canvasを見つける
            //Canvas canvas = mainCanvas.GetComponentInParent<Canvas>(); // Canvasを見つける
            if (canvas.renderMode != RenderMode.WorldSpace) // World Space以外のCanvasを想定
            {

                // カメラを通してワールド座標からスクリーン座標へ変換
                Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, obj.transform.position);

                // スクリーン座標をCanvas内のアンカー座標に変換
                Vector2 canvasPosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), screenPosition, canvas.worldCamera, out canvasPosition);

                // RectTransformの位置を設定
                return new Vector2(canvasPosition.x, canvasPosition.y);// - spriteHeight);// / 2);
            }
        }
        return new Vector3(0.0f, 0.0f, 1);
    }

    private void SetTestEnemies()
    {
        // 敵管理シングルトンに敵のリストを格納
        var party = dummyEnemyParty.GetPartyMembers();
        EnemyManager.Instance.Initialize();
        EnemyManager.Instance.SetEnemies(party);
    }
}