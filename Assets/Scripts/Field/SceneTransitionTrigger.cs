using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Cinemachine;
using Cysharp.Threading.Tasks;

/// <summary>
/// シーン遷移トリガー
/// TriggerColliderにアタッチし、接触判定時発動
/// </summary>
public class SceneTransitionTrigger : MonoBehaviour
{
    public string targetScene; // 移動先のシーン名
    public Vector3 targetPosition; // 移動先の座標
    public float rotationY; // 移動後のプレイヤーの向き
    public Vector3 mainCameraPosition; // 移動後のカメラの座標
    public Vector3 mainCameraRotation; // 移動後のカメラの回転

    // -7 4 12
    // 27 124 0

    // -3 -8 -92
    // 30 40 0

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // プレイヤーがトリガーに入ったかどうかを確認
        {
            // シーンを非同期でロードし、プレイヤーを指定の地点に移動
            LoadSceneAndMovePlayer().Forget();
        }
    }

    private async UniTaskVoid LoadSceneAndMovePlayer()
    {
        // シーンを非同期でロード
        //await SceneManager.LoadSceneAsync(targetScene);
        await SceneController.Instance.TransitionToNextScene(targetScene, targetPosition);

        // プレイヤーを新しいシーンの指定の地点に移動
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = targetPosition;
            player.transform.rotation = Quaternion.Euler(player.transform.rotation.x, rotationY, player.transform.rotation.z);
        }

        // カメラ操作
        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        GameObject CMFreeLook = GameObject.FindGameObjectWithTag("CMFreeLook");
        if (mainCamera != null && CMFreeLook != null)
        {
            // 一時的にCinemachineのカメラ追従をオフ
            CinemachineBrain cinemachineBrain = mainCamera.GetComponent<CinemachineBrain>();
            if (cinemachineBrain != null)
            {
                //cinemachineBrain.enabled = false;
            }

            // CinemachineFreeLookの追従を一時的に解除
            CinemachineFreeLook freeLookCamera = CMFreeLook.GetComponent<CinemachineFreeLook>();
            Transform originalFollow = freeLookCamera.Follow;
            Transform originalLookAt = freeLookCamera.LookAt;

            freeLookCamera.Follow = null;
            freeLookCamera.LookAt = null;

            // カメラの位置と回転を調整
            mainCamera.transform.position = mainCameraPosition;
            mainCamera.transform.rotation = Quaternion.Euler(mainCameraRotation);
            CMFreeLook.transform.position = mainCameraPosition;
            CMFreeLook.transform.rotation = Quaternion.Euler(mainCameraRotation);

            // 120フレーム待機
            await UniTask.DelayFrame(120);

            // CinemachineFreeLookの追従を再設定
            freeLookCamera.Follow = originalFollow;
            freeLookCamera.LookAt = originalLookAt;

            // カメラ追従を再開
            if (cinemachineBrain != null)
            {
                //cinemachineBrain.enabled = true;
            }
        }
    }
}
