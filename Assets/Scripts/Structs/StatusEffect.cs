using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffect
{
    public Constants.State state;
    public bool isState;
    public string effectName;   // バフ/デバフの名前
    public int effectMagnitude; // 効果量
    public int remainingTurns;  // 残りターン数

    public StatusEffect(Constants.State state,  string name, int magnitude, int turns)
    {
        this.effectName = name;
        this.effectMagnitude = magnitude;
        this.remainingTurns = turns;
    }



    // ターン経過による更新
    public void DecreaseTurn()
    {
        remainingTurns--;
    }
}
