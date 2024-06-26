using UnityEngine;

public class TurnCharacter
{
    private static TurnCharacter instance;
    public static TurnCharacter Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new TurnCharacter();
            }
            return instance;
        }
    }

    // 現在のプレイヤー情報
    public Character CurrentCharacter { get; private set; }

    public int CurrentCharacterIndex { get; private set; }

    // プレイヤーをターンに設定
    public void SetTurnCharacter(Character player, int index)
    {
        CurrentCharacter = player;
        CurrentCharacterIndex = index;

        CurrentCharacter.RefreshWhenTurnBegins();

        Debug.Log("It's now " + player.name + "(" + index + ")" + "'s turn.");
    }

    public void EndTurn()
    {
        // ターン終了時リフレッシュ
        CurrentCharacter.RefreshWhenEndTurn();
    }
}
