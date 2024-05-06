using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ヘイト管理クラス
/// </summary>
public class HateManager : MonoBehaviour
{
    private static HateManager _instance;

    private List<AllyStatus> allies;

    public List<Sprite> sprites;

    public static HateManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<HateManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("HateManager");
                    _instance = obj.AddComponent<HateManager>();
                }
            }
            return _instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    void Awake()
    {
        allies = PartyMembers.Instance.partyMembers;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public AllyStatus GetTargetWithRandom()
    {
        var random = Random.value;
        int targetIndex;

        if (random < 0.25f)
        {
            targetIndex = 0;
        }
        else if (random < 0.5f)
        {
            targetIndex = 1;
        }
        else if (random < 0.75f)
        {
            targetIndex = 2;
        }
        else
        {
            targetIndex = 3;
        }

        return allies[targetIndex];
    }
}
