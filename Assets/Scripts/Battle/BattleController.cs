using Cysharp.Threading.Tasks;
using DG.Tweening;
using SpriteGlow;
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

    public List<GameObject> timelineFaces;

    private Dictionary<int, int> speeds = new Dictionary<int, int>();
    public GameObject MainCamera;

    public GameObject animatedText;

    private CancellationTokenSource cancellationTokenSource;

    private static readonly System.Random random = new System.Random();

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
        await GetAllyStatuses();

        // GetAllyStatusesの処理が完了した後にSetEnemiesを実行
        SetEnemies();

        OrderManager.Instance.Initialize();
        var order = OrderManager.Instance.GetActionOrder(CommonVariableManager.turns);

        SetCommnadHeaderState();
    }

    /// <summary>
    /// 味方キャラクターのステータスを取得する
    /// </summary>
    /// <returns></returns>
    private async Task GetAllyStatuses()
    {
        //PartyMembers.Instance.Initialize();
        List<AllyStatus> allies = PartyMembers.Instance.GetAllies();

        for (int i = 0; i < 4; i++)
        {
            AllyStatus allyStatus = allies[i]; //await CommonController.GetAllyStatus(i + 1);
            allyStatus.spriteObject = faceImages[i];                                    // 画面下部 バストアップ画像
            allyStatus.HPGauge = HPGauges[i];                                           // HPゲージ
            allyStatus.MPGauge = MPGauges[i];                                           // MPゲージ
            allyStatus.TPGauge = TPGauges[i];                                           // TPゲージ

            GaugeManager hpGaugeManager = HPGauges[i].GetComponent<GaugeManager>();     // HPゲージ管理クラス
            GaugeManager mpGaugeManager = MPGauges[i].GetComponent<GaugeManager>();     // MPゲージ管理クラス
            GaugeManager tpGaugeManager = TPGauges[i].GetComponent<GaugeManager>();     // TPゲージ管理クラス

            hpGaugeManager.maxValueText.text = allyStatus.maxHp2.ToString();            // 最大HPテキスト
            hpGaugeManager.currentValueText.text = allyStatus.hp.ToString();            // 現在HPテキスト
            mpGaugeManager.maxValueText.text = allyStatus.maxMp2.ToString();            // 最大MPテキスト
            mpGaugeManager.currentValueText.text = allyStatus.mp.ToString();            // 現在MPテキスト
            tpGaugeManager.currentValueText.text = allyStatus.tp.ToString();            // 現在TPテキスト

            hpGaugeManager.updateGaugeByText();
            mpGaugeManager.updateGaugeByText();
            tpGaugeManager.updateGaugeByText();

            speeds.Add(i, allyStatus.agi2);
            faceImages[i].GetComponent<Image>().sprite = allyStatus.Class.imagesC[i];

            // パーティメンバーのシングルトンに追加
            //PartyMembers.Instance.AddCharacterToParty(allyStatus);   
        }
    }

    /// <summary>
    /// 敵オブジェクトの配置
    /// </summary>
    private void SetEnemies()
    {
        //EnemyManager.Instance.Initialize(); PlayerControllerにて敵との接触時すでに初期化しているのでここでは初期化しない

        Vector3 position;
        GameObject ins;
        List<EnemyPartyStatus.PartyMember> enemyPartyMembers = new List<EnemyPartyStatus.PartyMember>();
        var party = EnemyManager.Instance.EnemyPartyMembers;

        // リストの中の敵プレハブをインスタンス化
        for (int i = 0; i < party.Count; i++)
        {
            var enemy = party[i].enemy;
            position = party[i].position;
            ins = Instantiate<GameObject>(enemy, position, Quaternion.Euler(0, -22, 0));
            ins.GetComponent<EnemyBehaviour>().indexInBattle = i;
            EnemyManager.Instance.AddEnemyIns(ins);

            // 初期表示時、スプライトがバグるのでSpriteGlowを無効化しておく
            var glowEffect = ins.GetComponent<SpriteGlowEffect>();
            if (glowEffect != null)
            {
                glowEffect.enabled = false;
            }
        }
    }

    /// <summary>
    /// 味方ターン開始
    /// </summary>
    public void StartAllysTurn()
    {
        // コマンドウインドウのヘッダーの状態を設定
        SetCommnadHeaderState();
        // コマンドウインドウの操作を有効化
        battleCommandManager.ToggleButtonsInteractable(true);
        // コマンド - 攻撃ボタンを選択
        battleCommandManager.SelectButton(0);
    }

    /// <summary>
    /// 敵ターン開始
    /// </summary>
    /// <param name="index">ターン中の敵のリスト内でのインデックス</param>
    /// <returns></returns>
    public async UniTask StartEnemiesTurn(int index)
    {
        // コマンドウインドウの操作を無効化
        battleCommandManager.ToggleButtonsInteractable(false);

        var enemy = EnemyManager.Instance.InstantiatedEnemies[index];
        var behaviour = enemy.GetComponent<EnemyBehaviour>();

        var target = HateManager.Instance.GetTargetWithRandom();

        await behaviour.PerformAction(target);

        await ChangeNextTurn();
    }

    /// <summary>
    /// ターン移行前準備
    /// </summary>
    public async UniTask ChangeNextTurn()
    {
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
            if (turnCharacter is AllyStatus)
            {
                var c = turnCharacter as AllyStatus;
                tmPro.text = c.characterName;　// キャラクター名称
                image.color = c.color;      　 // 名前背景の色
            }
            else
            {
                tmPro.text = turnCharacter.characterName;
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
            if (ally.hp <= 0 || ally.knockedOut)
            {
                // 念のため戦闘不能状態にしておく
                ally.knockedOut = true;
            }
        }
        // 敵
        var enemies = EnemyManager.Instance.InstantiatedEnemies;
        var count = enemies.Count;
        for (int i = 0; i < count; i++)
        {
            var comp = enemies[i].GetComponent<EnemyBehaviour>();
            var st = comp.status;
            if (st.hp <= 0 || st.knockedOut)
            {
                // 念のため戦闘不能状態にしておく
                st.knockedOut = true;
                if (!comp.isFadedOut)
                {
                    // フェードアウト
                    EnemyManager.Instance.FadeOutEnemyIns(enemies[i]);
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

        GameObject obj = EnemyManager.Instance.GetEnemyIns(index);
        if (obj != null)
        {
            SpriteGlowEffect glowEffect = obj.GetComponent<SpriteGlowEffect>();
            if (glowEffect != null)
            {
                glowEffect.enabled = true;
                if (glowEffect != null)
                {
                    await AnimateGlowBrightness(glowEffect, 1, 2, 0.8f, cancellationTokenSource.Token);
                }
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
    private async UniTask AnimateGlowBrightness(SpriteGlowEffect glowEffect, float minBrightness, float maxBrightness, float totalDuration, CancellationToken cancellationToken)
    {
        float startTime = Time.time;
        while (!cancellationToken.IsCancellationRequested)
        {
            float elapsed = Time.time - startTime;
            float cycleTime = elapsed % totalDuration; // totalDurationごとにリセット
            float value = Mathf.PingPong(cycleTime / totalDuration * 2 * (maxBrightness - minBrightness), maxBrightness - minBrightness) + minBrightness;
            glowEffect.GlowBrightness = value;

            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken); // 次のフレームまで待機
        }
        glowEffect.enabled = false;
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
            var glowEffect = obj.GetComponent<SpriteGlowEffect>();
            if (glowEffect != null)
            {
                glowEffect.enabled = false;
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
        
        foreach(EnemyStatus enemy in enemies)
        {
            exp += enemy.exp;
        }

        return exp;
    }

    private int CalculateEarnedGold()
    {
        int gold = 0;
        var enemies = EnemyManager.Instance.GetAllEnemiesStatus();

        foreach (EnemyStatus enemy in enemies)
        {
            gold += enemy.gold;
        }

        return gold;
    }

    private List<Item> GetDropItems()
    {
        List<Item> earnedItems = new List<Item>();
        var enemies = EnemyManager.Instance.GetAllEnemiesStatus();

        foreach (EnemyStatus enemy in enemies)
        {
            if (enemy.dropItemOne != null)
            {
                int result = random.Next(1);
                if (enemy.dropRateOne > result)
                {
                    earnedItems.Add(enemy.dropItemOne);
                    continue;
                }
            }
            if (enemy.dropItemTwo != null)
            {
                int result = random.Next(1);
                if (enemy.dropRateTwo > result)
                {
                    earnedItems.Add(enemy.dropItemTwo);
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
}