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

    // シングルトンパターン
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

        // 他のインスタンスが存在しないことを確認し、存在する場合は破棄する
        if (Instance == null)
        {
            Instance = this; // ピン
      
            // シーン遷移時に破棄されないようにする
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(fadeInstance);  // fadeInstanceも維持する必要がある場合
            DontDestroyOnLoad(fadeRevInstance);
        }
        else if (Instance != this)
        {
           // Destroy(gameObject);
        }
        FadeOutLoaded();
    }

    /// <summary>
    /// シーン切り替え
    /// </summary>
    public IEnumerator SwitchScene(string sceneName)
    {
        // 切り替え前のシーンを変数に格納
        oldScene = SceneManager.GetActiveScene();
        yield return fadeComponent.FadeIn(0.5f, () => LoadNewScene(sceneName));
        fadeComponent.cutoutRange = 0;
        yield return LoadNewScene(sceneName);
        //yield return FadeOutLoaded(); // 確実にシーンロード後に実行

    }

    /// <summary>
    /// シーン切り替え
    /// </summary>
    public IEnumerator SwitchScene2(string sceneName)
    {
        // 切り替え前のシーンを変数に格納
        oldScene = SceneManager.GetActiveScene();

        // フェードイン後切り替え先のシーンをロード
        //yield return fade.GetComponent<Fade>().FadeIn(0.5f, null);
        //fade.GetComponent<Fade>().cutoutRange = 0;
        //fadeRev.cutoutRange = 0;
        yield return LoadNewScene(sceneName);
        FadeOutLoaded();
    }

    /// <summary>
    /// シーン切り替え後フェードアウト
    /// </summary>
    /// <returns></returns>
    public IEnumerator FadeOutLoaded()
    {
        if (fadeInstance != null && fadeInstance.activeInHierarchy)
        {
            // FadeOutが正しく実行されることを保証
            yield return fadeComponent.FadeOut(0.5f, null);
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
    public IEnumerator LoadNewScene(string sceneName)
    {
        // 非同期でシーンをロードし、完了を待つ
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        // ロードが完了するまで待機
        while (!asyncLoad.isDone)
        {
            yield return null;
            Debug.Log(sceneName + "is loaded.");
        }
    }

}
