using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OrderController : MonoBehaviour
{

    private List<int> sortedOrder;
    public int turnCount = 0;

    /// <summary>
    /// �����s������ݒ肷��
    /// </summary>
    /// <param name="speeds"></param>
    /// <returns></returns>
    public void initializeOrder()
    {
        Dictionary<int, int> speeds = new Dictionary<int, int>();
        var allies = PartyMembers.Instance.GetAllies();

        for(int i = 0; i < allies.Count; i++)
        {
            speeds.Add(i, allies[i].agi2);
        }

        var enemies = EnemyManager.Instance.GetAllEnemiesStatus();
        for (int i = 4; i < enemies.Count + 4; i++)
        {
            speeds.Add(i, enemies[i - 4].agi2);
        }

        // �ő�l
        var maxSpeed = speeds.Values.Max();
        // <ID,���s���x>�̃f�B�N�V���i��
        var effectiveSpeeds = new Dictionary<int, double>();
        List<Structs.IDAndSpeed> order = new List<Structs.IDAndSpeed>();

        foreach (var speed in speeds)
        {
            // ���s���x���v�Z���f�B�N�V���i���ɒǉ�
            var effectiveSpeed = (maxSpeed - (double)speed.Value) / 100 + 1;
            effectiveSpeeds.Add(speed.Key, effectiveSpeed);
        }
        // ���s���x�Ńf�B�N�V���i�����\�[�g
        var sortedEffectiveSpeeds = effectiveSpeeds.OrderBy(x => x.Value);

        // 10�񕪂̍s�������쐬
        for (int i = 1; i < 11; i++)
        {
            foreach(var speed in sortedEffectiveSpeeds)
            {
                var value = new Structs.IDAndSpeed(speed.Key, speed.Value * i);
                order.Add(value);
            }
        }

        // ���s���x�Ń\�[�g
        var sortedOrder = order.OrderBy(x => x.effectiveSpeed).ToList();
    }
}
