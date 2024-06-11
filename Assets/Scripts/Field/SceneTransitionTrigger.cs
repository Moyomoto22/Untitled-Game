using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Cinemachine;
using Cysharp.Threading.Tasks;

/// <summary>
/// �V�[���J�ڃg���K�[
/// TriggerCollider�ɃA�^�b�`���A�ڐG���莞����
/// </summary>
public class SceneTransitionTrigger : MonoBehaviour
{
    public string targetScene; // �ړ���̃V�[����
    public Vector3 targetPosition; // �ړ���̍��W
    public float rotationY; // �ړ���̃v���C���[�̌���
    public Vector3 mainCameraPosition; // �ړ���̃J�����̍��W
    public Vector3 mainCameraRotation; // �ړ���̃J�����̉�]

    // -7 4 12
    // 27 124 0

    // -3 -8 -92
    // 30 40 0

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // �v���C���[���g���K�[�ɓ��������ǂ������m�F
        {
            // �V�[����񓯊��Ń��[�h���A�v���C���[���w��̒n�_�Ɉړ�
            LoadSceneAndMovePlayer().Forget();
        }
    }

    private async UniTaskVoid LoadSceneAndMovePlayer()
    {
        // �V�[����񓯊��Ń��[�h
        //await SceneManager.LoadSceneAsync(targetScene);
        await SceneController.Instance.TransitionToNextScene(targetScene, targetPosition);

        // �v���C���[��V�����V�[���̎w��̒n�_�Ɉړ�
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = targetPosition;
            player.transform.rotation = Quaternion.Euler(player.transform.rotation.x, rotationY, player.transform.rotation.z);
        }

        // �J��������
        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        GameObject CMFreeLook = GameObject.FindGameObjectWithTag("CMFreeLook");
        if (mainCamera != null && CMFreeLook != null)
        {
            // �ꎞ�I��Cinemachine�̃J�����Ǐ]���I�t
            CinemachineBrain cinemachineBrain = mainCamera.GetComponent<CinemachineBrain>();
            if (cinemachineBrain != null)
            {
                //cinemachineBrain.enabled = false;
            }

            // CinemachineFreeLook�̒Ǐ]���ꎞ�I�ɉ���
            CinemachineFreeLook freeLookCamera = CMFreeLook.GetComponent<CinemachineFreeLook>();
            Transform originalFollow = freeLookCamera.Follow;
            Transform originalLookAt = freeLookCamera.LookAt;

            freeLookCamera.Follow = null;
            freeLookCamera.LookAt = null;

            // �J�����̈ʒu�Ɖ�]�𒲐�
            mainCamera.transform.position = mainCameraPosition;
            mainCamera.transform.rotation = Quaternion.Euler(mainCameraRotation);
            CMFreeLook.transform.position = mainCameraPosition;
            CMFreeLook.transform.rotation = Quaternion.Euler(mainCameraRotation);

            // 120�t���[���ҋ@
            await UniTask.DelayFrame(120);

            // CinemachineFreeLook�̒Ǐ]���Đݒ�
            freeLookCamera.Follow = originalFollow;
            freeLookCamera.LookAt = originalLookAt;

            // �J�����Ǐ]���ĊJ
            if (cinemachineBrain != null)
            {
                //cinemachineBrain.enabled = true;
            }
        }
    }
}
