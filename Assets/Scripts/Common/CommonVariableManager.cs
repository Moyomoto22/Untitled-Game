using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���ʕϐ��}�l�[�W��
public class CommonVariableManager : MonoBehaviour
{
    #region �v���C���[�̈ړ���
    public static bool playerCanMove = true;

    // setter
    public static void SetPlayerCanMove(bool canMove)
    {
        playerCanMove = canMove;
    }

    // getter
    public static bool GetPlayerCanMove()
    {
        return playerCanMove;
    }
    #endregion

    #region �v���C���[�̍��W
    public static Vector3 playerPosition = new Vector3();

    // setter
    public static void SetPlayerPosition(Vector3 position)
    {
        playerPosition = position;
    }

    // getter
    public static Vector3 GetPlayerPosition()
    {
        return playerPosition;
    }
    #endregion

    #region �v���C���[�̌���
    public static Vector3 playerRotation = new Vector3();

    // setter
    public static void SetPlayerRotation(Vector3 rotation)
    {
        playerRotation = rotation;
    }

    // getter
    public static Vector3 GetPlayerRotation()
    {
        return playerRotation;
    }
    #endregion

    #region �G�I�u�W�F�N�g�̍��W
    public static Dictionary<string, Vector3> enemyPositions = new Dictionary<string, Vector3>();

    // setter
    public static void SetEnemyPositions(GameObject[] enemies)
    {
        enemyPositions = new Dictionary<string, Vector3>();
        foreach (GameObject e in enemies)
        {
            // Position��ۑ�
            enemyPositions.Add(e.name, e.transform.position);
        }
    }

    // getter
    public static Dictionary<string, Vector3> GetEnemyPositions()
    {
        return enemyPositions;
    }
    #endregion

    #region �G�I�u�W�F�N�g�̌���
    public static Dictionary<string, Quaternion> enemyRotations = new Dictionary<string, Quaternion>();

    // setter
    public static void SetEnemyRotations(GameObject[] enemies)
    {
        enemyRotations = new Dictionary<string, Quaternion>();
        foreach (GameObject e in enemies)
        {
            // Rotation��ۑ�
            enemyRotations.Add(e.name, e.transform.rotation);
        }
    }

    // getter
    public static Dictionary<string, Quaternion> GetEnemyRotations()
    {
        return enemyRotations;
    }
    #endregion

    #region �J�����̉�]
    public static Vector3[] MainCamRotation = new Vector3[2];

    // setter
    public static void SetCameraRotation(Vector3 mainCamera, Vector3 vCamera)
    {
        MainCamRotation[0] = mainCamera;
        MainCamRotation[1] = vCamera;
    }

    // getter
    public static Vector3[] GetCameraRotation()
    {
            return MainCamRotation;
    }
    #endregion

    #region �J�����̈ʒu
    public static Vector3[] CameraPosition = new Vector3[2];

    // setter
    public static void SetCameraPosition(Vector3 mainCam, Vector3 vCam)
    {
        CameraPosition[0] = mainCam;
        CameraPosition[1] = vCam;
    }

    // getter
    public static Vector3[] GetCameraPosition()
    {
        return CameraPosition;
    }
    #endregion

    public static float[] vh = new float[2];
    public static void Setvh(float v, float h)
    {
        vh[0] = v;
        vh[1] = h;
    }

    // getter
    public static float[] Getvh()
    {
        return vh;
    }

    #region ��A�N�e�B�u�ȓG�I�u�W�F�N�g����
    public static List<string> DeactiveEnemyName = new List<string>();

    // setter
    public static void SetDeactiveEnemyName(string name)
    {
        
        DeactiveEnemyName.Add(name);
    }

    // getter
    public static List<string> GetDeactiveEnemyName()
    {
        return DeactiveEnemyName;
    }
    #endregion

    #region �ڐG�����G�I�u�W�F�N�g��
    public static string ContactEnemyName;

    // setter
    public static void SetContactEnemyName(string enemyName)
    {
        ContactEnemyName = enemyName;
    }

    // getter
    public static string GetContactEnemyName()
    {
        return ContactEnemyName;
    }

    #endregion

    #region �G���J�E���g�����G�p�[�e�B
    public static List<EnemyPartyStatus.PartyMember> PartyMembers;

    // setter
    public static void SetPartyMembers(List<EnemyPartyStatus.PartyMember> partyMembers)
    { 
        PartyMembers = partyMembers;
    }

    // getter
    public static List<EnemyPartyStatus.PartyMember> GetPartyMembers()
    {
        return PartyMembers;
    }
    #endregion

    public static Constants.MenuState ShowingMenuState { get; set; }

    #region �^�[����
    public static int turns;
    #endregion

    #region �\�L�_���[�W
    public static List<string> DamageStrings;
    public static List<string> HealHPStrings;
    public static List<string> HealMPStrings;
    #endregion
}
