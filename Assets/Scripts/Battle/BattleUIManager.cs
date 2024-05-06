using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class BattleUIManager : MonoBehaviour
{
    public List<GameObject> faceImages;
    public List<GameObject> maxHPs;
    public List<GameObject> HPs;
    public List<GameObject> maxMPs;
    public List<GameObject> MPs;
    public List<GameObject> TPs;

    private List<AllyStatus> allyStatuses;

    // Start is called before the first frame update
    void Start()
    {
        GetAllyStatuses();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private async void GetAllyStatuses()
    {
        for (int i = 0; i < 4; i++)
        {
            allyStatuses[i] = await CommonController.GetAllyStatus(i + 1);
            faceImages[i].GetComponent<Image>().sprite = allyStatuses[i].Class.imagesC[i];
            maxHPs[i].GetComponent<TextMeshProUGUI>().text = allyStatuses[i].maxHp2.ToString();
            HPs[i].GetComponent<TextMeshProUGUI>().text = allyStatuses[i].hp.ToString();
            maxMPs[i].GetComponent<TextMeshProUGUI>().text = allyStatuses[i].maxMp2.ToString();
            MPs[i].GetComponent<TextMeshProUGUI>().text = allyStatuses[i].mp.ToString();
            TPs[i].GetComponent<TextMeshProUGUI>().text = allyStatuses[i].tp.ToString();
        }
    }
}
