using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyScriptable/CreateEnemy")]
public class EnemyStatus : CharacterStatus
{
    public Sprite sprite;
    public Sprite eyesSprite;

    public int exp;
    public int gold;

    public Item earnedItem;

    public EnemyStatus()
    {
        hp = maxHp2;
        mp = maxMp2;
    }
}