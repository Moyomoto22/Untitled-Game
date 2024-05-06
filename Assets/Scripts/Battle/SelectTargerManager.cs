using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectTargerManager : MonoBehaviour
{
    public GameObject window;

    //private const int[] windowHeight = []; 

    private void OnEnable()
    {
        var enemyCount = EnemyManager.Instance.GetEnemyPartyMembers().Count;

        var newHeight = 100 + (enemyCount - 1) * 76;
        var ratio = newHeight / window.transform.localScale.y;
        window.transform.localScale = new Vector3(window.transform.localScale.x, ratio, window.transform.localScale.z);
    }
}
