using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : EnemyBehaviour
{
    public override async UniTask PerformAction(Character objective)
    {
        if (Random.value < 0.1f)
        {
            await Guard(enemy);
        }
        else
        {
            await Attack(objective);
        }
    }
}
