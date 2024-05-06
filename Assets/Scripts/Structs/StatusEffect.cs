using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffect
{
    public Constants.State state;
    public bool isState;
    public string effectName;   // �o�t/�f�o�t�̖��O
    public int effectMagnitude; // ���ʗ�
    public int remainingTurns;  // �c��^�[����

    public StatusEffect(Constants.State state,  string name, int magnitude, int turns)
    {
        this.effectName = name;
        this.effectMagnitude = magnitude;
        this.remainingTurns = turns;
    }



    // �^�[���o�߂ɂ��X�V
    public void DecreaseTurn()
    {
        remainingTurns--;
    }
}
