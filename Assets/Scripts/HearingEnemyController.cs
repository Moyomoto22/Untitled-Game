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

    //ランダムで決める数値の最大値
    [SerializeField] float radius;
    //設定した待機時間
    [SerializeField] float waitTime;
    //待機時間を数える
    [SerializeField] float time = 0;
    //エンカウントする敵パーティリスト
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
        //主人公のスピード
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

        //NavMeshAgentのストップを解除
        agent.isStopped = false;

        //目標地点のX軸、Z軸をランダムで決める
        float posX = UnityEngine.Random.Range(-1 * radius, radius);
        float posZ = UnityEngine.Random.Range(-1 * radius, radius);

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