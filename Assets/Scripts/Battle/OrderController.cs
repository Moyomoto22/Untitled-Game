using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OrderController : MonoBehaviour
{

    private List<int> sortedOrder;
    public int turnCount = 0;

    /// <summary>
    /// 初期行動順を設定する
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

        // 最大値
        var maxSpeed = speeds.Values.Max();
        // <ID,実行速度>のディクショナリ
        var effectiveSpeeds = new Dictionary<int, double>();
        List<Structs.IDAndSpeed> order = new List<Structs.IDAndSpeed>();

        foreach (var speed in speeds)
        {
            // 実行速度を計算しディクショナリに追加
            var effectiveSpeed = (maxSpeed - (double)speed.Value) / 100 + 1;
            effectiveSpeeds.Add(speed.Key, effectiveSpeed);
        }
        // 実行速度でディクショナリをソート
        var sortedEffectiveSpeeds = effectiveSpeeds.OrderBy(x => x.Value);

        // 10回分の行動順を作成
        for (int i = 1; i < 11; i++)
        {
            foreach(var speed in sortedEffectiveSpeeds)
            {
                var value = new Structs.IDAndSpeed(speed.Key, speed.Value * i);
                order.Add(value);
            }
        }

        // 実行速度でソート
        var sortedOrder = order.OrderBy(x => x.effectiveSpeed).ToList();
    }
}
