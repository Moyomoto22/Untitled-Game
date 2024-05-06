using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class SeeingEnemyController : MonoBehaviour
{
    [SerializeField]
    Transform target;
    NavMeshAgent agent;

    private bool isAiStateRunning;

    public float searchDistance;
    public Vector3 position;
    public float searchAngle;

    private float posX;
    private float posZ;

    //�����_���Ō��߂鐔�l�̍ő�l
    [SerializeField] float radius = 3;
    //�ݒ肵���ҋ@����
    [SerializeField] float waitTime = 2;
    //�ҋ@���Ԃ𐔂���
    [SerializeField] float time = 0;
    //�G���J�E���g����G�p�[�e�B���X�g
    [SerializeField]
    public List<EnemyPartyStatus> EnemyPartyStatuses = null;

    public enum EnemyAiState
    {
        Wander = 0,
        MoveToPlayer = 1,
        Stop = 2
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
        position = target.position;
        if (aiState == EnemyAiState.MoveToPlayer)
        {
            agent.SetDestination(position);
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

    public void SetState(int state)
    {
        CommonVariableManager.playerCanMove = false;
        aiState = (EnemyAiState)Enum.ToObject(typeof(EnemyAiState), state);
    }

    protected void SetState()
    {
        //�@��l���̕���
        var playerDirection = target.transform.position - transform.position;
        float angle = Vector3.Angle(transform.forward, playerDirection);

        float dis = Vector3.Distance(target.position, this.transform.position);
        
        if (dis < searchDistance && angle < searchAngle && CommonVariableManager.playerCanMove)
        {
            time = 0;
            agent.autoBraking = false;
            agent.isStopped = false;
            aiState = EnemyAiState.MoveToPlayer;
            posX = 0;
            posZ = 0;
        }
        else if (dis > searchDistance && CommonVariableManager.playerCanMove)
        {
            aiState = EnemyAiState.Wander;
        }
        else if (!CommonVariableManager.playerCanMove)
        {
            aiState = EnemyAiState.Stop;
        }
    }

    private void Wander()
    {
        if (agent.remainingDistance < 0.1f)
        {
            agent.ResetPath();
            StopHere();
        }
    }

    private void GoToNextPoint()
    {
        //NavMeshAgent�̃X�g�b�v������
        agent.isStopped = false;

        //�ڕW�n�_��X���AZ���������_���Ō��߂�
        posX = UnityEngine.Random.Range(-1 * radius, radius);
        posZ = UnityEngine.Random.Range(-1 * radius, radius);

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