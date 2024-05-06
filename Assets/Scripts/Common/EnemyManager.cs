using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EnemyManager
{
    private static EnemyManager instance;
    public List<EnemyPartyStatus.PartyMember> EnemyPartyMembers { get; private set; }

    public List<GameObject> InstantiatedEnemies { get; private set; } = new List<GameObject>();

    public List<int> AliveEnemyIndexes { get; private set; } = new List<int>();

    // �V���O���g���̃C���X�^���X���擾����v���p�e�B
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

    // �R���X�g���N�^��private�ɂ��ĊO������̃C���X�^���X����h��
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

    // �G�p�[�e�B�[�����o�[�����X�g�ɒǉ����郁�\�b�h
    public void AddEnemy(EnemyPartyStatus.PartyMember enemyMember)
    {
        EnemyPartyMembers.Add(enemyMember);
    }

    // �G�p�[�e�B�[�����o�[�����X�g����폜���郁�\�b�h
    public void RemoveEnemy(EnemyPartyStatus.PartyMember enemyMember)
    {
        EnemyPartyMembers.Remove(enemyMember);
    }

    public void AddEnemyIns(GameObject enemyIns)
    {
        InstantiatedEnemies.Add(enemyIns);
        AliveEnemyIndexes.Add(enemyIns.GetComponent<EnemyBehaviour>().indexInBattle);
    }

    // �G�p�[�e�B�[�����o�[�����X�g����폜���郁�\�b�h
    public void RemoveEnemyIns(GameObject enemyIns)
    { 
        enemyIns.GetComponent<SpriteRenderer>().DOFade(0f, 0.5f).From(1f); // �t�F�[�h�A�E�g
        InstantiatedEnemies.Remove(enemyIns);
    }

    public void removeIndexFromAliveEnemyIndexes(int indexToRemove)
    {
        AliveEnemyIndexes.Remove(indexToRemove);
    }

    /// <summary>
    /// �G�C���X�^���X���t�F�[�h�A�E�g������
    /// </summary>
    /// <param name="enemyIns"></param>
    public void FadeOutEnemyIns(GameObject enemyIns)
    {
        enemyIns.GetComponent<SpriteRenderer>().DOFade(0f, 0.5f).From(1f); // �t�F�[�h�A�E�g
        enemyIns.GetComponent<EnemyBehaviour>().isFadedOut = true;
    }

    public void Initialize()
    {
        if(EnemyPartyMembers != null && InstantiatedEnemies != null)
        {
            EnemyPartyMembers.Clear();
            InstantiatedEnemies.Clear();
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

    public EnemyStatus GetEnemyStatus(EnemyPartyStatus.PartyMember partyMember)
    {
        var status = partyMember.enemy.GetComponent<EnemyBehaviour>().status;
        return status;
    }

    public EnemyStatus GetEnemyStatusByIndex(int index)
    {
        var enemy = GetEnemyIns(index);
        if (enemy != null)
        {
            var status = enemy.GetComponent<EnemyBehaviour>().status;
            return status;
        }
        return null;
    }

    public List<EnemyStatus> GetAllEnemiesStatus(bool exceptKnockedOut = false)
    {
        List<EnemyStatus> enemies = new List<EnemyStatus>(); 
        foreach(var enemy in InstantiatedEnemies)
        {
            var e = enemy.GetComponent<EnemyBehaviour>().status;
            if(e != null)
            {
                if (!(exceptKnockedOut && e.knockedOut))
                {
                    enemies.Add(e);
                }
            }
        }
        return enemies;
    }

    public List<GameObject> GetEnemiesInsExceptKnockedOut()
    {
        var aliveEnemieIns = InstantiatedEnemies.Where(x => !x.GetComponent<EnemyBehaviour>().status.knockedOut).ToList();
        return aliveEnemieIns;
    }

    private void InithializeStatus()
    {

    }
}