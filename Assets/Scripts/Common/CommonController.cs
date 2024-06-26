using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.AsyncOperations;

public class CommonController : MonoBehaviour
{

    //[SerializeField]
    //Fade fade;
    //[SerializeField]
    //Fade fadeRev;

    [SerializeField]
    List<AssetReference> Allyes;

    AsyncOperationHandle handle;

    public List<Ally> Allies;

    private Scene oldScene;
    void Start()
    {
        //StartCoroutine(SceneController.Instance.FadeOutLoaded());

        CommonVariableManager.playerCanMove = true;

        LoadAsset();
    }

    //private void Start()
    //{
    //    //SceneManager.sceneLoaded += SceneLoaded;
    //}
    private void Update()
    {

    }

    /// <summary>
    /// ゲーム一時停止
    /// </summary>
    /// <returns></returns>
    public static void PauseGame()
    {
        CommonVariableManager.playerCanMove = false;
        Time.timeScale = 0;
    }

    /// <summary>
    /// ゲーム一時停止解除
    /// </summary>
    /// <returns></returns>
    public static void ResumeGame()
    {
        CommonVariableManager.playerCanMove = true;
        Time.timeScale = 1;
    }

    /// <summary>
    /// シーン切り替え時フェードアウト
    /// </summary>
    /// <returns></returns>
    /// 
    //private IEnumerator FadeOutLoaded()
    //{
    //    yield return fade.FadeOut(0.5f, null);
    //}

    /// <summary>
    /// シーン切り替え
    /// </summary>
    //public IEnumerator SwitchScene(string sceneName)
    //{
    //    // 切り替え前のシーンを変数に格納
    //    oldScene = SceneManager.GetActiveScene();

    //    // フェードイン後切り替え先のシーンをロード
    //    yield return fade.FadeIn(0.5f, null);
    //    fade.cutoutRange = 0;
    //    fadeRev.cutoutRange = 0;
    //    yield return LoadNewScene(sceneName);
    //}

    /// <summary>
    /// シーン読み込み
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    public IEnumerator LoadNewScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        yield return null;
    }

    //public IEnumerator FadeIn(int time)
    //{
    //    yield return fade.FadeIn(time);
    //}

    /// <summary>
    /// シーン破棄
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    IEnumerator UnloadOldScene(string sceneName)
    {
        yield return SceneManager.UnloadSceneAsync(sceneName);
    }

    /// <summary>
    /// Addressableなオブジェクトを非同期でロードする
    /// </summary>
    void LoadAsset()
    {
        //for (int i = 0; i < Allyes.Count; i++)
        //{
        //    handle = Allyes[i].InstantiateAsync();
        //}
    }

    public static void EnableInputActionMap(GameObject gameObject, string mapName)
    {
        // PlayerInputコンポーネントを取得
        PlayerInput playerInput = gameObject.GetComponent<PlayerInput>();

        // 特定の名前のInputActionを探して有効化
        InputActionMap map = playerInput.actions.FindActionMap(mapName);

        if (map != null)
        {
            map.Enable();
            Debug.Log(mapName + " is enabled.");
        }
        else
        {
            Debug.LogWarning($"InputAction '{mapName}' not found!");
        }
    }

    public static void DisableInputActionMap(GameObject gameObject, string mapName)
    {
        // PlayerInputコンポーネントを取得
        PlayerInput playerInput = gameObject.GetComponent<PlayerInput>();

        // 特定の名前のInputActionを探して有効化
        InputActionMap map = playerInput.actions.FindActionMap(mapName);

        if (map != null)
        {
            map.Disable();
            Debug.Log(mapName + " is disabled.");
        }
        else
        {
            Debug.LogWarning($"InputAction '{mapName}' not found!");
        }
    }

    public static void SwitchInputSystemMap(EventSystem eventSystem)
    {

    }

    /// <summary>
    /// ロードしたAddressableなオブジェクトをアンロードする
    /// </summary>
    void UnloadAsset()
    {
        if (!handle.IsValid())
        {
            return;
        }

        Addressables.ReleaseInstance(handle);
    }

    //カラーコードの文字列からColorクラスに変換
    public static Color GetColor(string c)
    {
        Color color = default(Color);
        if (ColorUtility.TryParseHtmlString(c, out color))
        {
            //Colorを生成できたらそれを返す
            return color;
        }
        else
        {
            //失敗した場合は白を返す
            return new Color32(255, 255, 255, 255);
        }
    }

    public static async Task<Ally> GetAlly(int id)
    {
        string path;

        switch (id)
        {
            case 1:
                path = Constants.alexPath;
                break;
            case 2:
                path = Constants.nicoPath;
                break;
            case 3:
                path = Constants.tabathaPath;
                break;
            case 4:
                path = Constants.aliciaPath;
                break;
            default:
                path = Constants.alexPath;
                break;
        }

        Ally status = await Addressables.LoadAssetAsync<Ally>(path).Task;

        return status;
    }

    public static string GetItemCategoryString(Constants.ItemCategory category)
    {
        string str = "";

        switch (category)
        {
            case Constants.ItemCategory.Consumable:
                str = "消費アイテム";
                break;
            case Constants.ItemCategory.Material:
                str = "素材";
                break;
            case Constants.ItemCategory.Weapon:
                str = "武器";
                break;
            case Constants.ItemCategory.Head:
                str = "頭";
                break;
            case Constants.ItemCategory.Body:
                str = "胴";
                break;
            case Constants.ItemCategory.Accessary:
                str = "装飾品";
                break;
            case Constants.ItemCategory.Misc:
                str = "その他";
                break;
            default:
                break;
        }
        return str;
    }

    public static string GetWeaponCategoryString(Constants.WeaponCategory category)
    {
        string str = "";

        switch (category)
        {
            case Constants.WeaponCategory.Sword:
                str = "片手剣";
                break;
            case Constants.WeaponCategory.Blade:
                str = "両手剣";
                break;
            case Constants.WeaponCategory.Dagger:
                str = "短剣";
                break;
            case Constants.WeaponCategory.Spear:
                str = "槍";
                break;
            case Constants.WeaponCategory.Ax:
                str = "斧";
                break;
            case Constants.WeaponCategory.Hammer:
                str = "槌";
                break;
            case Constants.WeaponCategory.Fist:
                str = "拳";
                break;
            case Constants.WeaponCategory.Bow:
                str = "弓";
                break;
            case Constants.WeaponCategory.Staff:
                str = "杖";
                break;
            case Constants.WeaponCategory.Shield:
                str = "盾";
                break;
            default:
                break;
        }
        return str;
    }

    public static string GetSkillCategoryString(Constants.SkillCategory category)
    {
        string str = "";

        switch (category)
        {
            case Constants.SkillCategory.Magic:
                str = "魔法";
                break;
            case Constants.SkillCategory.Miracle:
                str = "奇跡";
                break;
            case Constants.SkillCategory.Arts:
                str = "アーツ";
                break;
            case Constants.SkillCategory.Active:
                str = "アクティブスキル";
                break;
            case Constants.SkillCategory.Passive:
                str = "パッシブスキル";
                break;
            default:
                break;
        }
        return str;
    }

    /// <summary>
    /// アイテム種別の列挙型から数値に変換する
    /// 0は全て(カテゴリなし)なので1からスタート
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    public static Constants.ItemCategory GetItemCategoryValue(int val)
    {
        Constants.ItemCategory category = Constants.ItemCategory.Consumable;

        switch (val)
        {
            case 0:
                category = Constants.ItemCategory.Consumable;
                break;
            case 1:
                category = Constants.ItemCategory.Material;
                break;
            case 2:
                category = Constants.ItemCategory.Weapon;
                break;
            case 3:
                category = Constants.ItemCategory.Body;
                break;
            case 4:
                category = Constants.ItemCategory.Head;
                break;
            case 5:
                category = Constants.ItemCategory.Accessary;
                break;
            case 6:
                category = Constants.ItemCategory.Misc;
                break;
            default:
                break;
        }
        return category;
    }

    public static string TranslateRarity(Constants.Rarity category)
    {
        string str = "";

        switch (category)
        {
            case Constants.Rarity.Common:
                str = "C";
                break;
            case Constants.Rarity.Uncommon:
                str = "UC";
                break;
            case Constants.Rarity.Rare:
                str = "R";
                break;
            case Constants.Rarity.Legendary:
                str = "L";
                break;
            default:
                break;
        }
        return str;
    }

    /// <summary>
    /// GameObjectが持つ子オブジェクトをすべて取得する
    /// </summary>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static List<GameObject> GetChildrenGameObjects(GameObject parent)
    {
        List<GameObject> children = new List<GameObject>();

        // 親のTransformを取得
        Transform parentTransform = parent.transform;

        int childCount = parentTransform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform childTransform = parentTransform.GetChild(i);
            children.Add(childTransform.gameObject);
        }

        return children;
    }

    /// <summary>
    /// potentialParentがpotentialChildを子オブジェクトとして持つか判定する
    /// </summary>
    /// <param name="potentialParent"></param>
    /// <param name="potentialChild"></param>
    /// <returns></returns>
    public static bool IsChildOf(GameObject potentialParent, GameObject potentialChild)
    {
        return potentialChild != null &&
               potentialParent != null &&
               potentialChild.transform.IsChildOf(potentialParent.transform);
    }

    /// <summary>
    /// 兄弟オブジェクトの中での指定オブジェクトのインデックスを取得
    /// 親オブジェクトが存在しない場合、-1を返す
    /// </summary>
    /// <param name="targetTransform"></param>
    /// <returns></returns>
    public static int GetSiblingIndexInParent(Transform targetTransform)
    {
        if (targetTransform.parent != null)
        {
            // 親が存在する場合、兄弟オブジェクトの中でのインデックスを取得
            return targetTransform.GetSiblingIndex();
        }
        else
        {
            // 親が存在しない場合-1を返す
            return -1;
        }
    }

    public static Color GetCharacterColorByIndex(int index)
    {
        string colorCode = Constants.gradationBlue;

        switch (index)
        {
            case 0:
                colorCode = Constants.gradationBlue;
                break;
            case 1:
                colorCode = Constants.gradationRed;
                break;
            case 2:
                colorCode = Constants.gradationPurple;
                break;
            case 3:
                colorCode = Constants.gradationGreen;
                break;
            default:
                colorCode = Constants.gradationBlue;
                break;
        }

        return GetColor(colorCode);
    }

    public static Sprite GetSpriteForEffect(string effectName)
    {
        var sprite = Resources.Load<Sprite>("UI/Icon/StatusEffect/" + effectName);
        return sprite;
    }

}
