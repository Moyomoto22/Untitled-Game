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

    // �V���O���g���p�^�[��
    public static SceneController Instance { get; private set; }

    private GameObject fadeInstance;
    private GameObject fadeRevInstance;

    private Fade fadeComponent;
    private Fade fadeRevComponent;

    private Scene oldScene;

    void Start()
    {
        
    }

    void Awake()
    {
        fadeInstance = Instantiate(fade, this.transform);
        fadeRevInstance = Instantiate(fadeRev, this.transform);

        fadeComponent = fadeInstance.GetComponent<Fade>();
        fadeRevComponent = fadeRevInstance.GetComponent<Fade>();
        fadeComponent.cutoutRange = 1;

        //Debug.LogError("SceneController is Awaken.");

        // ���̃C���X�^���X�����݂��Ȃ����Ƃ��m�F���A���݂���ꍇ�͔j������
        if (Instance == null)
        {
            Instance = this; // �s��
      
            // �V�[���J�ڎ��ɔj������Ȃ��悤�ɂ���
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(fadeInstance);  // fadeInstance���ێ�����K�v������ꍇ
            DontDestroyOnLoad(fadeRevInstance);
        }
        else if (Instance != this)
        {
           // Destroy(gameObject);
        }
        FadeOutLoaded();
    }

    /// <summary>
    /// �V�[���؂�ւ�
    /// </summary>
    public IEnumerator SwitchScene(string sceneName)
    {
        // �؂�ւ��O�̃V�[����ϐ��Ɋi�[
        oldScene = SceneManager.GetActiveScene();
        yield return fadeComponent.FadeIn(0.5f, () => LoadNewScene(sceneName));
        fadeComponent.cutoutRange = 0;
        yield return LoadNewScene(sceneName);
        //yield return FadeOutLoaded(); // �m���ɃV�[�����[�h��Ɏ��s

    }

    /// <summary>
    /// �V�[���؂�ւ�
    /// </summary>
    public IEnumerator SwitchScene2(string sceneName)
    {
        // �؂�ւ��O�̃V�[����ϐ��Ɋi�[
        oldScene = SceneManager.GetActiveScene();

        // �t�F�[�h�C����؂�ւ���̃V�[�������[�h
        //yield return fade.GetComponent<Fade>().FadeIn(0.5f, null);
        //fade.GetComponent<Fade>().cutoutRange = 0;
        //fadeRev.cutoutRange = 0;
        yield return LoadNewScene(sceneName);
        FadeOutLoaded();
    }

    /// <summary>
    /// �V�[���؂�ւ���t�F�[�h�A�E�g
    /// </summary>
    /// <returns></returns>
    public IEnumerator FadeOutLoaded()
    {
        if (fadeInstance != null && fadeInstance.activeInHierarchy)
        {
            // FadeOut�����������s����邱�Ƃ�ۏ�
            yield return fadeComponent.FadeOut(0.5f, null);
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
    public IEnumerator LoadNewScene(string sceneName)
    {
        // �񓯊��ŃV�[�������[�h���A������҂�
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        // ���[�h����������܂őҋ@
        while (!asyncLoad.isDone)
        {
            yield return null;
            Debug.Log(sceneName + "is loaded.");
        }
    }

}
