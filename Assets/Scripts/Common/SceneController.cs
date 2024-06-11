using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public GameObject fade;
    public GameObject fadeRev;

    private const string normal = "001";
    private const string mosaic = "171";

    const float fadeDuration = 0.6f;

    // �V���O���g���p�^�[��
    public static SceneController Instance { get; private set; }

    private GameObject fadeInstance;
    private GameObject fadeRevInstance;

    private FadeUniTask fadeComponent;
    private FadeUniTask fadeRevComponent;

    private Scene oldScene;

    void Start()
    {
        
    }

    async void Awake()
    {
        
        fadeInstance = Instantiate(fade, this.transform);
        fadeRevInstance = Instantiate(fadeRev, this.transform);

        fadeComponent = fadeInstance.GetComponent<FadeUniTask>();
        fadeRevComponent = fadeRevInstance.GetComponent<FadeUniTask>();
        fadeComponent.cutoutRange = 1;

        //Debug.LogError("SceneController is Awaken.");

        // ���̃C���X�^���X�����݂��Ȃ����Ƃ��m�F���A���݂���ꍇ�͔j������
        if (Instance == null)
        {
            Instance = this;
      
            // �V�[���J�ڎ��ɔj������Ȃ��悤�ɂ���
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(fadeInstance);
            DontDestroyOnLoad(fadeRevInstance);
        }
        else if (Instance != this)
        {
           // Destroy(gameObject);
        }
        await FadeOutLoaded();
    }

    /// <summary>
    /// �퓬��ʂƃt�B�[���h��ʂ�؂�ւ�
    /// </summary>
    public async UniTask SwitchFieldAndBattleScene(string sceneName)
    {
        // �؂�ւ��O�̃V�[����ϐ��Ɋi�[
        oldScene = SceneManager.GetActiveScene();
        
        fadeComponent.cutoutRange = 0;

        Debug.Log("SwitchScene started");

        // �؂�ւ��O�̃V�[����ϐ��Ɋi�[
        oldScene = SceneManager.GetActiveScene();

        // �t�F�[�h�C�������s���A��������܂őҋ@
        Debug.Log("Starting FadeIn");
        await fadeComponent.FadeIn(fadeDuration, mosaic, async () =>
        {
            await LoadNewScene(sceneName);
        });
        Debug.Log("FadeIn completed");

        // �V�[�������[�h���ꂽ��Ƀt�F�[�h�A�E�g�����s
        Debug.Log("Starting FadeOutLoaded");
        //await FadeOutLoaded();
        Debug.Log("FadeOutLoaded completed");

    }

    /// <summary>
    /// �}�b�v�ړ����V�[���؂�ւ�
    /// </summary>
    public async UniTask TransitionToNextScene(string sceneName, Vector3 position)
    {
        // �؂�ւ��O�̃V�[����ϐ��Ɋi�[
        oldScene = SceneManager.GetActiveScene();

        fadeComponent.cutoutRange = 0;

        Debug.Log("SwitchScene started");

        // �؂�ւ��O�̃V�[����ϐ��Ɋi�[
        oldScene = SceneManager.GetActiveScene();

        // �t�F�[�h�C�������s���A��������܂őҋ@
        Debug.Log("Starting FadeIn");
        await fadeComponent.FadeIn(fadeDuration, normal, async () =>
        {
            await LoadNewScene(sceneName);

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = position;
            }

        });
        Debug.Log("FadeIn completed");

        // �V�[�������S�Ƀ��[�h���ꂽ���Ƃ��m�F
        await UniTask.WaitUntil(() => SceneManager.GetActiveScene().name == sceneName);
    }

    /// <summary>
    /// �}�b�v�ړ����V�[���؂�ւ�
    /// </summary>
    public async UniTask TransitionToNextSceneNoFade(string sceneName, Vector3 position)
    {
        // �؂�ւ��O�̃V�[����ϐ��Ɋi�[
        oldScene = SceneManager.GetActiveScene();

        Debug.Log("SwitchScene started");
        // �񓯊��ŃV�[�������[�h���A������҂�
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        // ���[�h����������܂őҋ@
        while (!asyncLoad.isDone)
        {
            await UniTask.Yield();
            CommonVariableManager.SetCurrentSceneName(sceneName);
        }
        // �V�[�������S�Ƀ��[�h���ꂽ���Ƃ��m�F
        await UniTask.WaitUntil(() => SceneManager.GetActiveScene().name == sceneName);
        Debug.Log(sceneName + "is loaded.");

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = position;
        }
    }

    /// <summary>
    /// �V�[���؂�ւ���t�F�[�h�A�E�g
    /// </summary>
    /// <returns></returns>
    public async UniTask FadeOutLoaded()
    {
        if (fadeInstance != null && fadeInstance.activeInHierarchy)
        {
            // FadeOut�����������s����邱�Ƃ�ۏ�
            await fadeComponent.FadeOut(fadeDuration, mosaic, null);
        }
        else
        {
            Debug.LogError("FadeInstance is null or inactive.");
        }
    }


    /// <summary>
    /// �V�[���ǂݍ���
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    public async UniTask LoadNewScene(string sceneName)
    {
        // �񓯊��ŃV�[�������[�h���A������҂�
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        // ���[�h����������܂őҋ@
        while (!asyncLoad.isDone)
        {
            await UniTask.Yield();
            CommonVariableManager.SetCurrentSceneName(sceneName);
        }
        await FadeOutLoaded();
        Debug.Log(sceneName + "is loaded.");
    }

    /// <summary>
    /// �t�F�[�h�C���̂�
    /// </summary>
    /// <returns></returns>
    public async UniTask FadeIn()
    {
        await fadeComponent.FadeIn(fadeDuration, null);
    }
}
