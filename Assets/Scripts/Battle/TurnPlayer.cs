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
    public Character CurrentCharacter { get; private set; }

    public int CurrentCharacterIndex { get; private set; }

    // �v���C���[���^�[���ɐݒ�
    public void SetTurnCharacter(Character player, int index)
    {
        CurrentCharacter = player;
        CurrentCharacterIndex = index;

        CurrentCharacter.RefreshWhenTurnBegins();

        Debug.Log("It's now " + player.name + "(" + index + ")" + "'s turn.");
    }

    public void EndTurn()
    {
        // �^�[���I�������t���b�V��
        CurrentCharacter.RefreshWhenEndTurn();
    }
}
