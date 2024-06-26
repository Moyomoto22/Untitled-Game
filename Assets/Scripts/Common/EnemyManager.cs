using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class EnemyManager
{
    private static EnemyManager instance;
    public List<EnemyPartyStatus.PartyMember> EnemyPartyMembers { get; private set; }

    public List<GameObject> InstantiatedEnemies { get; private set; } = new List<GameObject>();

    public List<int> AliveEnemyIndexes { get; private set; } = new List<int>();

    // シングルトンのインスタンスを取得するプロパティ
    public static EnemyManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new EnemyManager();
            }
            return instance;
        }
    }

    // コンストラクタをprivateにして外部からのインスタンス化を防ぐ
    private EnemyManager()
    {
        EnemyPartyMembers = new List<EnemyPartyStatus.PartyMember>();
    }

    public List<EnemyPartyStatus.PartyMember> GetEnemyPartyMembers()
    {
        return EnemyPartyMembers;
    }

    public void SetEnemies(List<EnemyPartyStatus.PartyMember> partyMembers)
    {
        EnemyPartyMembers = partyMembers;
    }

    // 敵パーティーメンバーをリストに追加するメソッド
    public void AddEnemy(EnemyPartyStatus.PartyMember enemyMember)
    {
        EnemyPartyMembers.Add(enemyMember);
    }

    // 敵パーティーメンバーをリストから削除するメソッド
    public void RemoveEnemy(EnemyPartyStatus.PartyMember enemyMember)
    {
        EnemyPartyMembers.Remove(enemyMember);
    }

    public void AddEnemyIns(GameObject enemyIns)
    {
        InstantiatedEnemies.Add(enemyIns);
        AliveEnemyIndexes.Add(enemyIns.GetComponent<EnemyComponent>().indexInBattle);
    }

    // 敵パーティーメンバーをリストから削除するメソッド
    public void RemoveEnemyIns(GameObject enemyIns)
    { 
        enemyIns.GetComponent<SpriteRenderer>().DOFade(0f, 0.5f).From(1f); // フェードアウト
        InstantiatedEnemies.Remove(enemyIns);
    }

    public void removeIndexFromAliveEnemyIndexes(int indexToRemove)
    {
        AliveEnemyIndexes.Remove(indexToRemove);
    }

    /// <summary>
    /// 敵インスタンスをフェードアウトさせる
    /// </summary>
    /// <param name="enemyIns"></param>
    public async UniTask FadeOutEnemyIns(GameObject enemyIns)
    {
        var comp = enemyIns.GetComponent<EnemyComponent>();
        if (comp != null)
        {
            await DOTween.Sequence()
            // スプライトのフェードアウト
            .Append(comp.mainSprite.DOFade(0f, 0.5f).From(1f))
            .Join(comp.shadowSprite.DOFade(0f, 0.5f).From(1f));
               
        }
        enemyIns.GetComponent<EnemyComponent>().isFadedOut = true;
    }

    public void Initialize()
    {
        if(EnemyPartyMembers != null && InstantiatedEnemies != null)
        {
            EnemyPartyMembers.Clear();
            InstantiatedEnemies.Clear();
            AliveEnemyIndexes.Clear();
        } 
    }

    public GameObject GetEnemyIns(int index)
    {
        if (InstantiatedEnemies != null) {

            if (index >= 0 && index < InstantiatedEnemies.Count)
            {
                return InstantiatedEnemies[index];
            }
            return null;
        }
        return null;
    }

    public Enemy GetEnemy(EnemyPartyStatus.PartyMember partyMember)
    {
        var status = partyMember.enemy.GetComponent<EnemyComponent>().status;
        return status;
    }

    public Enemy GetEnemyByIndex(int index)
    {
        var enemy = GetEnemyIns(index);
        if (enemy != null)
        {
            var status = enemy.GetComponent<EnemyComponent>().status;
            return status;
        }
        return null;
    }

    public List<Enemy> GetAllEnemiesStatus(bool exceptKnockedOut = false)
    {
        List<Enemy> enemies = new List<Enemy>(); 
        foreach(var enemy in InstantiatedEnemies)
        {
            var e = enemy.GetComponent<EnemyComponent>().status;
            if(e != null)
            {
                if (!(exceptKnockedOut && e.KnockedOut))
                {
                    enemies.Add(e);
                }
            }
        }
        return enemies;
    }

    public List<GameObject> GetEnemiesInsExceptKnockedOut()
    {
        var aliveEnemieIns = InstantiatedEnemies.Where(x => !x.GetComponent<EnemyComponent>().status.KnockedOut).ToList();
        return aliveEnemieIns;
    }

    public void ShowHPGauges()
    {
        foreach(var enemyObject in InstantiatedEnemies)
        {
            GaugeManager gaugeManager = enemyObject.GetComponentInChildren<GaugeManager>(true);
            
            if (gaugeManager != null)
            {
                GameObject gaugeObject = gaugeManager.transform.gameObject;

                if (gaugeObject != null)
                {
                    gaugeObject.SetActive(true);
                }
            }
        }
    }
}