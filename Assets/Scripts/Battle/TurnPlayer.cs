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

    // ���݂̃v���C���[���
    public CharacterStatus CurrentCharacter { get; private set; }

    public int CurrentCharacterIndex { get; private set; }

    // �v���C���[���^�[���ɐݒ�
    public void SetTurnCharacter(CharacterStatus player, int index)
    {
        CurrentCharacter = player;
        CurrentCharacterIndex = index;
        // ���t���b�V��
        CurrentCharacter.RefreshEffectsRemainOneTurn();

        Debug.Log("It's now " + player.name + "(" + index + ")" + "'s turn.");
    }
}
