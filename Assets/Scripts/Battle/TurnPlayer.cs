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
    public CharacterStatus CurrentCharacter { get; private set; }

    public int CurrentCharacterIndex { get; private set; }

    // プレイヤーをターンに設定
    public void SetTurnCharacter(CharacterStatus player, int index)
    {
        CurrentCharacter = player;
        CurrentCharacterIndex = index;
        // リフレッシュ
        CurrentCharacter.RefreshEffectsRemainOneTurn();

        Debug.Log("It's now " + player.name + "(" + index + ")" + "'s turn.");
    }
}
