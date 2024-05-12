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
    List<AssetReference> AllyStatuses;

    AsyncOperationHandle handle;

    public List<AllyStatus> allyStatuses;

    public ItemDatabase itemDatabase;
    public SkillDatabase skillDatabase;

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
        //for (int i = 0; i < AllyStatuses.Count; i++)
        //{
        //    handle = AllyStatuses[i].InstantiateAsync();
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

    public static async Task<AllyStatus> GetAllyStatus(int id)
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

        AllyStatus status = await Addressables.LoadAssetAsync<AllyStatus>(path).Task;

        return status;
    }

    /// <summary>
    /// �A�C�e���������\���`�F�b�N����
    /// </summary>
    /// <param name="status"></param>
    /// <param name="item"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static bool CheckEquippable(AllyStatus status, Item item, int index)
    {
        bool isEquippable = false;
        bool isNitouryu = false;

        Class Class = status.Class;
        Equip equip = item as Equip;

        if (equip != null && Class != null)
        {

            switch (index)
            {
                // �E��
                case 0:
                    Weapon weapon = equip as Weapon;
                    if (weapon != null)
                    {
                        // �E��ɏ��͑����s��
                        if (weapon.weaponCategory != Constants.WeaponCategory.Shield)
                        {
                            if (weapon.equipableClasses.Exists(x => x.name == Class.name))
                            {
                                return true;
                            }
                        }
                    }
                    break;
                // ����
                case 1:
                    Weapon weapon2 = equip as Weapon;
                    if (weapon2 != null)
                    {
                        // ����͊�{���̂�
                        if (weapon2.weaponCategory == Constants.WeaponCategory.Shield || isNitouryu)
                        {
                            // �E�葕�������莝���łȂ�
                            if (!status.rightArm.isTwoHanded)
                            {
                                if (weapon2.equipableClasses.Exists(x => x.name == Class.name))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                    break;
                // ���E��
                case 2:
                case 3:
                    if (equip.equipableClasses.Exists(x => x.name == Class.name))
                    {
                        return true;
                    }
                    break;
                // �����i�P�E�Q
                case 4:
                case 5:
                        return true;
                default:
                    return false;
            }
        }

        return isEquippable;
    }

    public static async Task<int> CheckWhoEquiped(Item item)
    {
        List<AllyStatus> statuses = new List<AllyStatus>();

        for (int i = 1; i < 5; i++)
        {
            statuses.Add(await CommonController.GetAllyStatus(i));
        }

        foreach(var status in statuses)
        {
            if (status.rightArm == item || status.leftArm == item || status.head == item || status.body == item || status.accessary1 == item || status.accessary2 == item)
            {
                return status.CharacterID;
            }
        }
        return 0;
    }


    /// <summary>
    /// ������X�L�����l�������X�e�[�^�X���Čv�Z����
    /// </summary>
    public static AllyStatus CalcStatus(AllyStatus status)
    {
        AllyStatus newStatus = status;

        // ��{�X�e�[�^�X
        newStatus.maxHp2 = status.maxHp + (status.rightArm?.maxHp ?? 0) + (status.leftArm?.maxHp ?? 0) + (status.head?.maxHp ?? 0) + (status.body?.maxHp ?? 0) + (status.accessary1?.maxHp ?? 0) + (status.accessary2?.maxHp ?? 0);
        newStatus.maxMp2 = status.maxMp + (status.rightArm?.maxMp ?? 0) + (status.leftArm?.maxMp ?? 0) + (status.head?.maxMp ?? 0) + (status.body?.maxMp ?? 0) + (status.accessary1?.maxMp ?? 0) + (status.accessary2?.maxMp ?? 0);
        newStatus.str2 = status.str + (status.rightArm?.str ?? 0) + (status.leftArm?.str ?? 0) + (status.head?.str ?? 0) + (status.body?.str ?? 0) + (status.accessary1?.str ?? 0) + (status.accessary2?.str ?? 0);
        newStatus.vit2 = status.vit + (status.rightArm?.vit ?? 0) + (status.leftArm?.vit ?? 0) + (status.head?.vit ?? 0) + (status.body?.vit ?? 0) + (status.accessary1?.vit ?? 0) + (status.accessary2?.vit ?? 0);
        newStatus.dex2 = status.dex + (status.rightArm?.dex ?? 0) + (status.leftArm?.dex ?? 0) + (status.head?.dex ?? 0) + (status.body?.dex ?? 0) + (status.accessary1?.dex ?? 0) + (status.accessary2?.dex ?? 0);
        newStatus.agi2 = status.agi + (status.rightArm?.agi ?? 0) + (status.leftArm?.agi ?? 0) + (status.head?.agi ?? 0) + (status.body?.agi ?? 0) + (status.accessary1?.agi ?? 0) + (status.accessary2?.agi ?? 0);
        newStatus.inte2 = status.inte + (status.rightArm?.inte ?? 0) + (status.leftArm?.inte ?? 0) + (status.head?.inte ?? 0) + (status.body?.inte ?? 0) + (status.accessary1?.inte ?? 0) + (status.accessary2?.inte ?? 0);
        newStatus.mnd2 = status.mnd + (status.rightArm?.mnd ?? 0) + (status.leftArm?.mnd ?? 0) + (status.head?.mnd ?? 0) + (status.body?.mnd ?? 0) + (status.accessary1?.mnd ?? 0) + (status.accessary2?.mnd ?? 0);

        // �����U���͈ˑ��l ����ɂ����STR or DEX or INT or MND���U���͂ɉ��Z
        int pAttackCorect = newStatus.str;
        int pAttackCorectLeft = 0;
        if (status.rightArm != null)
        {
            switch (status.rightArm.dependentStatus)
            {
                case 1:
                    pAttackCorect = newStatus.dex2;
                    break;
                case 2:
                    pAttackCorect = newStatus.inte2;
                    break;
                case 3:
                    pAttackCorect = newStatus.mnd2;
                    break;
                default:
                    pAttackCorect = newStatus.str2;
                    break;
            }
        }

        if (status.leftArm != null)
        {
            if (status.leftArm.weaponCategory != Constants.WeaponCategory.Shield)
            {
                switch (status.leftArm.dependentStatus)
                {
                    case 1:
                        pAttackCorectLeft = newStatus.dex2;
                        break;
                    case 2:
                        pAttackCorectLeft = newStatus.inte2;
                        break;
                    case 3:
                        pAttackCorectLeft = newStatus.mnd2;
                        break;
                    default:
                        pAttackCorectLeft = newStatus.str2;
                        break;
                }
            }
        }

        // �T�u�X�e�[�^�X
        newStatus.pAttackLeft = pAttackCorectLeft + (status.leftArm?.pAttack ?? 0);
        newStatus.pAttack = pAttackCorect + (status.rightArm?.pAttack ?? 0) + newStatus.pAttackLeft + (status.head?.pAttack ?? 0) + (status.body?.pAttack ?? 0) + (status.accessary1?.pAttack ?? 0) + (status.accessary2?.pAttack ?? 0);
        newStatus.mAttack = newStatus.inte2 + (status.rightArm?.mAttack ?? 0) + (status.leftArm?.mAttack ?? 0) + (status.head?.mAttack ?? 0) + (status.body?.mAttack ?? 0) + (status.accessary1?.mAttack ?? 0) + (status.accessary2?.mAttack ?? 0);
        newStatus.pDefence = newStatus.vit2 + (status.rightArm?.pDefence ?? 0) + (status.leftArm?.pDefence ?? 0) + (status.head?.pDefence ?? 0) + (status.body?.pDefence ?? 0) + (status.accessary1?.pDefence ?? 0) + (status.accessary2?.pDefence ?? 0);
        newStatus.mDefence = newStatus.mnd / 2 + (status.rightArm?.mDefence ?? 0) + (status.leftArm?.mDefence ?? 0) + (status.head?.mDefence ?? 0) + (status.body?.mDefence ?? 0) + (status.accessary1?.mDefence ?? 0) + (status.accessary2?.mDefence ?? 0);

        return newStatus;
    }

    public static void UseItem(Consumable item)
    {

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

    public static async void GetAllItems()
    {

        ItemDatabase itemDatabase = FindObjectsOfType<CommonController>()[0].itemDatabase;
        List<Item> items = itemDatabase.items;


        // �A�C�e���ꗗ��Adressable����擾
        GameObject obj = await Addressables.LoadAssetAsync<GameObject>(Constants.itemInventoryPath).Task;
        ItemInventory itemInventory = obj.GetComponent<ItemInventory>();

        itemInventory.itemInventory.Clear();

        foreach (Item item in items)
        {
            for (int i = 0; i < 5; i++)
            {
                itemInventory.AddItem(item);

            }
        }
    }

    /// <summary>
    /// �S�ẴX�L�����K������
    /// </summary>
    /// <param name="ally"></param>
    public static async void LearnAllSkills(AllyStatus ally)
    {

        SkillDatabase skillDatabase = FindObjectsOfType<CommonController>()[0].skillDatabase;
        List<Skill> skills = skillDatabase.skills;

        foreach (Skill skill in skills)
        {
            for (int i = 0; i < 5; i++)
            {
                ally.LearnSkill(skill);
            }
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
}
