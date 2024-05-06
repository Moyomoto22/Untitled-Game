using UnityEngine;

// Stateクラスの基底クラス
public abstract class State
{
    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}