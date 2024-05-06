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

    //ランダムで決める数値の最大値
    [SerializeField] float radius = 3;
    //設定した待機時間
    [SerializeField] float waitTime = 2;
    //待機時間を数える
    [SerializeField] float time = 0;
    //エンカウントする敵パーティリスト
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
        //　主人公の方向
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
        //NavMeshAgentのストップを解除
        agent.isStopped = false;

        //目標地点のX軸、Z軸をランダムで決める
        posX = UnityEngine.Random.Range(-1 * radius, radius);
        posZ = UnityEngine.Random.Range(-1 * radius, radius);

        //CentralPointの位置にPosXとPosZを足す
        Vector3 pos = transform.position;
        pos.x += posX;
        pos.z += posZ;

        //NavMeshAgentに目標地点を設定する
        agent.destination = pos;
    }

    void StopHere()
    {
        //NavMeshAgentを止める
        agent.isStopped = true;
        //待ち時間を数える
        time += Time.deltaTime;
        //待ち時間が設定された数値を超えると発動
        if (time > waitTime)
        {
            //目標地点を設定し直す
            GoToNextPoint();
            time = 0;
        }
    }
}