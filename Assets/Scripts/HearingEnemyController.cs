using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class HearingEnemyController : MonoBehaviour
{
    [SerializeField]
    Transform target;
    NavMeshAgent agent;
    
    public PlayerController player;
    public float searchDistance;

    //�����_���Ō��߂鐔�l�̍ő�l
    [SerializeField] float radius;
    //�ݒ肵���ҋ@����
    [SerializeField] float waitTime;
    //�ҋ@���Ԃ𐔂���
    [SerializeField] float time = 0;
    //�G���J�E���g����G�p�[�e�B���X�g
    [SerializeField]
    public List<EnemyPartyStatus> EnemyPartyStatuses = null;

    public enum EnemyAiState
    {
        Wander,
        MoveToPlayer,
        Stop
    }

    public EnemyAiState aiState = EnemyAiState.Wander;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    // Update is called once per frame
    void Update()
    {
        SetState();

        if (aiState == EnemyAiState.MoveToPlayer)
        {
            agent.SetDestination(target.position);
        }
        else if (aiState == EnemyAiState.Wander)
        {
            Wander();
        }
        else if (aiState == EnemyAiState.Stop)
        {
            StopHere();
        }
    }

    protected void SetState()
    {
        //��l���̃X�s�[�h
        var playerSpeed = player.Motion;

        float dis = Vector3.Distance(target.position, this.transform.position);

        if (dis < searchDistance && playerSpeed > 0.5f && CommonVariableManager.playerCanMove)
        {
            agent.isStopped = false;
            aiState = EnemyAiState.MoveToPlayer;
        }
        else if (CommonVariableManager.playerCanMove)
        {
            aiState = EnemyAiState.Wander;
        }
        else if (!CommonVariableManager.playerCanMove)
        {
            aiState = EnemyAiState.Stop;
        }
    }

    public void SetState(int state)
    {
        CommonVariableManager.playerCanMove = false;
        aiState = (EnemyAiState)Enum.ToObject(typeof(EnemyAiState), state);
    }

    private void Wander()
    {
        if (agent.remainingDistance < 0.5f)
        {
            StopHere();
        }
    }

    private void GoToNextPoint()
    {
        agent.ResetPath();

        //NavMeshAgent�̃X�g�b�v������
        agent.isStopped = false;

        //�ڕW�n�_��X���AZ���������_���Ō��߂�
        float posX = UnityEngine.Random.Range(-1 * radius, radius);
        float posZ = UnityEngine.Random.Range(-1 * radius, radius);

        //CentralPoint�̈ʒu��PosX��PosZ�𑫂�
        Vector3 pos = transform.position;
        pos.x += posX;
        pos.z += posZ;

        //NavMeshAgent�ɖڕW�n�_��ݒ肷��
        agent.destination = pos;
    }

    void StopHere()
    {
        //NavMeshAgent���~�߂�
        agent.isStopped = true;
        //�҂����Ԃ𐔂���
        time += Time.deltaTime;
        //�҂����Ԃ��ݒ肳�ꂽ���l�𒴂���Ɣ���
        if (time > waitTime)
        {
            //�ڕW�n�_��ݒ肵����
            GoToNextPoint();
            time = 0;
        }
    }

}