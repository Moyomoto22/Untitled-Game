using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rat : EnemyBehaviour
{
    public override async UniTask PerformAction(CharacterStatus objective)
    {
        if (Random.value < 0.2f)
        {
            await Guard(status);
        }
        else
        {
            await Attack(objective);
        }
    }
}
