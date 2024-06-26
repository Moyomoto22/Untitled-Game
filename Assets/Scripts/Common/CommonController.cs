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
    /// �Q�[���ꎞ��~
    /// </summary>
    /// <returns></returns>
    public static void PauseGame()
    {
        CommonVariableManager.playerCanMove = false;
        Time.timeScale = 0;
    }

    /// <summary>
    /// �Q�[���ꎞ��~����
    /// </summary>
    /// <returns></returns>
    public static void ResumeGame()
    {
        CommonVariableManager.playerCanMove = true;
        Time.timeScale = 1;
    }

    /// <summary>
    /// �V�[���؂�ւ����t�F�[�h�A�E�g
    /// </summary>
    /// <returns></returns>
    /// 
    //private IEnumerator FadeOutLoaded()
    //{
    //    yield return fade.FadeOut(0.5f, null);
    //}

    /// <summary>
    /// �V�[���؂�ւ�
    /// </summary>
    //public IEnumerator SwitchScene(string sceneName)
    //{
    //    // �؂�ւ��O�̃V�[����ϐ��Ɋi�[
    //    oldScene = SceneManager.GetActiveScene();

    //    // �t�F�[�h�C����؂�ւ���̃V�[�������[�h
    //    yield return fade.FadeIn(0.5f, null);
    //    fade.cutoutRange = 0;
    //    fadeRev.cutoutRange = 0;
    //    yield return LoadNewScene(sceneName);
    //}

    /// <summary>
    /// �V�[���ǂݍ���
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
    /// �V�[���j��
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    IEnumerator UnloadOldScene(string sceneName)
    {
        yield return SceneManager.UnloadSceneAsync(sceneName);
    }

    /// <summary>
    /// Addressable�ȃI�u�W�F�N�g��񓯊��Ń��[�h����
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
        // PlayerInput�R���|�[�l���g���擾
        PlayerInput playerInput = gameObject.GetComponent<PlayerInput>();

        // ����̖��O��InputAction��T���ėL����
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
        // PlayerInput�R���|�[�l���g���擾
        PlayerInput playerInput = gameObject.GetComponent<PlayerInput>();

        // ����̖��O��InputAction��T���ėL����
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
    /// ���[�h����Addressable�ȃI�u�W�F�N�g���A�����[�h����
    /// </summary>
    void UnloadAsset()
    {
        if (!handle.IsValid())
        {
            return;
        }

        Addressables.ReleaseInstance(handle);
    }

    //�J���[�R�[�h�̕����񂩂�Color�N���X�ɕϊ�
    public static Color GetColor(string c)
    {
        Color color = default(Color);
        if (ColorUtility.TryParseHtmlString(c, out color))
        {
            //Color�𐶐��ł����炻���Ԃ�
            return color;
        }
        else
        {
            //���s�����ꍇ�͔���Ԃ�
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
                str = "����A�C�e��";
                break;
            case Constants.ItemCategory.Material:
                str = "�f��";
                break;
            case Constants.ItemCategory.Weapon:
                str = "����";
                break;
            case Constants.ItemCategory.Head:
                str = "��";
                break;
            case Constants.ItemCategory.Body:
                str = "��";
                break;
            case Constants.ItemCategory.Accessary:
                str = "�����i";
                break;
            case Constants.ItemCategory.Misc:
                str = "���̑�";
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
                str = "�Ў茕";
                break;
            case Constants.WeaponCategory.Blade:
                str = "���茕";
                break;
            case Constants.WeaponCategory.Dagger:
                str = "�Z��";
                break;
            case Constants.WeaponCategory.Spear:
                str = "��";
                break;
            case Constants.WeaponCategory.Ax:
                str = "��";
                break;
            case Constants.WeaponCategory.Hammer:
                str = "��";
                break;
            case Constants.WeaponCategory.Fist:
                str = "��";
                break;
            case Constants.WeaponCategory.Bow:
                str = "�|";
                break;
            case Constants.WeaponCategory.Staff:
                str = "��";
                break;
            case Constants.WeaponCategory.Shield:
                str = "��";
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
                str = "���@";
                break;
            case Constants.SkillCategory.Miracle:
                str = "���";
                break;
            case Constants.SkillCategory.Arts:
                str = "�A�[�c";
                break;
            case Constants.SkillCategory.Active:
                str = "�A�N�e�B�u�X�L��";
                break;
            case Constants.SkillCategory.Passive:
                str = "�p�b�V�u�X�L��";
                break;
            default:
                break;
        }
        return str;
    }

    /// <summary>
    /// �A�C�e����ʂ̗񋓌^���琔�l�ɕϊ�����
    /// 0�͑S��(�J�e�S���Ȃ�)�Ȃ̂�1����X�^�[�g
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
    /// GameObject�����q�I�u�W�F�N�g�����ׂĎ擾����
    /// </summary>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static List<GameObject> GetChildrenGameObjects(GameObject parent)
    {
        List<GameObject> children = new List<GameObject>();

        // �e��Transform���擾
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
    /// potentialParent��potentialChild���q�I�u�W�F�N�g�Ƃ��Ď������肷��
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
    /// �Z��I�u�W�F�N�g�̒��ł̎w��I�u�W�F�N�g�̃C���f�b�N�X���擾
    /// �e�I�u�W�F�N�g�����݂��Ȃ��ꍇ�A-1��Ԃ�
    /// </summary>
    /// <param name="targetTransform"></param>
    /// <returns></returns>
    public static int GetSiblingIndexInParent(Transform targetTransform)
    {
        if (targetTransform.parent != null)
        {
            // �e�����݂���ꍇ�A�Z��I�u�W�F�N�g�̒��ł̃C���f�b�N�X���擾
            return targetTransform.GetSiblingIndex();
        }
        else
        {
            // �e�����݂��Ȃ��ꍇ-1��Ԃ�
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
