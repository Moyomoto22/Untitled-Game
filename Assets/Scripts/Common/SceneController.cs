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

    // シングルトンパターン
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

        // 他のインスタンスが存在しないことを確認し、存在する場合は破棄する
        if (Instance == null)
        {
            Instance = this;
      
            // シーン遷移時に破棄されないようにする
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
    /// 戦闘画面とフィールド画面を切り替え
    /// </summary>
    public async UniTask SwitchFieldAndBattleScene(string sceneName)
    {
        // 切り替え前のシーンを変数に格納
        oldScene = SceneManager.GetActiveScene();
        
        fadeComponent.cutoutRange = 0;

        Debug.Log("SwitchScene started");

        // 切り替え前のシーンを変数に格納
        oldScene = SceneManager.GetActiveScene();

        // フェードインを実行し、完了するまで待機
        Debug.Log("Starting FadeIn");
        await fadeComponent.FadeIn(fadeDuration, mosaic, async () =>
        {
            await LoadNewScene(sceneName);
        });
        Debug.Log("FadeIn completed");

        // シーンがロードされた後にフェードアウトを実行
        Debug.Log("Starting FadeOutLoaded");
        //await FadeOutLoaded();
        Debug.Log("FadeOutLoaded completed");

    }

    /// <summary>
    /// マップ移動時シーン切り替え
    /// </summary>
    public async UniTask TransitionToNextScene(string sceneName, Vector3 position)
    {
        // 切り替え前のシーンを変数に格納
        oldScene = SceneManager.GetActiveScene();

        fadeComponent.cutoutRange = 0;

        Debug.Log("SwitchScene started");

        // 切り替え前のシーンを変数に格納
        oldScene = SceneManager.GetActiveScene();

        // フェードインを実行し、完了するまで待機
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

        // シーンが完全にロードされたことを確認
        await UniTask.WaitUntil(() => SceneManager.GetActiveScene().name == sceneName);
    }

    /// <summary>
    /// マップ移動時シーン切り替え
    /// </summary>
    public async UniTask TransitionToNextSceneNoFade(string sceneName, Vector3 position)
    {
        // 切り替え前のシーンを変数に格納
        oldScene = SceneManager.GetActiveScene();

        Debug.Log("SwitchScene started");
        // 非同期でシーンをロードし、完了を待つ
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        // ロードが完了するまで待機
        while (!asyncLoad.isDone)
        {
            await UniTask.Yield();
            CommonVariableManager.SetCurrentSceneName(sceneName);
        }
        // シーンが完全にロードされたことを確認
        await UniTask.WaitUntil(() => SceneManager.GetActiveScene().name == sceneName);
        Debug.Log(sceneName + "is loaded.");

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = position;
        }
    }

    /// <summary>
    /// シーン切り替え後フェードアウト
    /// </summary>
    /// <returns></returns>
    public async UniTask FadeOutLoaded()
    {
        if (fadeInstance != null && fadeInstance.activeInHierarchy)
        {
            // FadeOutが正しく実行されることを保証
            await fadeComponent.FadeOut(fadeDuration, mosaic, null);
        }
        else
        {
            Debug.LogError("FadeInstance is null or inactive.");
        }
    }


    /// <summary>
    /// シーン読み込み
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    public async UniTask LoadNewScene(string sceneName)
    {
        // 非同期でシーンをロードし、完了を待つ
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        // ロードが完了するまで待機
        while (!asyncLoad.isDone)
        {
            await UniTask.Yield();
            CommonVariableManager.SetCurrentSceneName(sceneName);
        }
        await FadeOutLoaded();
        Debug.Log(sceneName + "is loaded.");
    }

    /// <summary>
    /// フェードインのみ
    /// </summary>
    /// <returns></returns>
    public async UniTask FadeIn()
    {
        await fadeComponent.FadeIn(fadeDuration, null);
    }
}
