using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public GameObject mainCamera;
    [SerializeField]
    public CinemachineVirtualCamera vCamera;

    private Animator anim;
    Quaternion targetRotation;

    public float Speed;
    public float Motion = 0;

    private bool isMoving;

    private List<EnemyPartyStatus> baseEnemySeeing;
    private List<EnemyPartyStatus> baseEnemyHearing;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        vCamera = vCamera.gameObject.GetComponent<CinemachineVirtualCamera>();
        baseEnemySeeing = GameObject.Find("BaseEnemySeeing").GetComponent<SeeingEnemyController>().EnemyPartyStatuses;
        baseEnemyHearing = GameObject.Find("BaseEnemyHearing").GetComponent<HearingEnemyController>().EnemyPartyStatuses;


        // �J�����̈ʒu�𕜌�
        //vCamera.GetCinemachineComponent(CinemachineCore.Stage.Aim).GetComponent<CinemachinePOV>().m_VerticalAxis.Value = CommonVariableManager.Getvh()[0];
        //vCamera.GetCinemachineComponent(CinemachineCore.Stage.Aim).GetComponent<CinemachinePOV>().m_HorizontalAxis.Value = CommonVariableManager.Getvh()[1];

        // �v���C���[�̈ʒu�𕜌�
        gameObject.transform.position = CommonVariableManager.GetPlayerPosition();
        gameObject.transform.eulerAngles = CommonVariableManager.GetPlayerRotation();
        
        // �G�I�u�W�F�N�g�̏�Ԃ𕜌�
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Dictionary<string, Vector3> enemyPositions = CommonVariableManager.GetEnemyPositions();
        Dictionary<string, Quaternion> enemyRotations = CommonVariableManager.GetEnemyRotations();
        List<string> deactivateEnemyName = CommonVariableManager.GetDeactiveEnemyName();
        foreach (GameObject e in enemies)
        {
            if (enemyPositions != null && enemyPositions.TryGetValue(e.name, out Vector3 ep))
            {
                e.transform.position = ep;
            }
            
            if (enemyRotations != null && enemyRotations.TryGetValue(e.name, out Quaternion er))
            {
                e.transform.rotation = er;
            }

            if (deactivateEnemyName != null && deactivateEnemyName.Contains(e.name))
            {
                GameObject.Find(e.name).SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(mainCamera.transform.eulerAngles + " " + vCamera.transform.eulerAngles + " / " + mainCamera.transform.position + " " + vCamera.transform.position);
        if (Input.GetKey(KeyCode.Space))
        {
            mainCamera.transform.position = CommonVariableManager.GetCameraPosition()[0];
            mainCamera.transform.eulerAngles = CommonVariableManager.GetCameraRotation()[0];
            vCamera.transform.position = CommonVariableManager.GetCameraPosition()[1];
            vCamera.transform.eulerAngles = CommonVariableManager.GetCameraRotation()[1];

        }

        if (CommonVariableManager.playerCanMove)
        {
            //var velocity = new Vector3();

            var h = Input.GetAxisRaw("Horizontal");
            var v = Input.GetAxisRaw("Vertical");

            var hRotation = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up);
            var velocity = hRotation * new Vector3(h, 0, v).normalized;

            float speed;

            if (Input.GetKey(KeyCode.LeftShift))
            {
                speed = Speed / 2;
                Motion = 0.5f;
            }
            else
            {
                speed = Speed;
                Motion = 1;
            }
            var rotationSpeed = 600 * Time.deltaTime;

            anim.SetFloat("speed", velocity.magnitude * Motion, 0.1f, Time.deltaTime);
            transform.position += velocity * speed * Time.deltaTime;

            if ((h != 0 || v != 0) && velocity.magnitude > 0.5f)
            {
                targetRotation = Quaternion.LookRotation(velocity, Vector3.up);
            }
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed);
        }
    }

    /// <summary>
    /// �G�Ƃ̐ڐG���C�x���g
    /// </summary>
    /// <param name="collision"></param>
    private async void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            // �J�����̈ʒu��ۑ�
            //float v = vCamera.GetCinemachineComponent(CinemachineCore.Stage.Aim).GetComponent<CinemachinePOV>().m_VerticalAxis.Value;
            //float h = vCamera.GetCinemachineComponent(CinemachineCore.Stage.Aim).GetComponent<CinemachinePOV>().m_HorizontalAxis.Value;
            //CommonVariableManager.Setvh(v, h);

            // �v���C���[�̓������~�߂�
            CommonVariableManager.playerCanMove = false;
            
            // �ڐG�����G�I�u�W�F�N�g�̖��̂����X�g�ɒǉ�
            CommonVariableManager.SetDeactiveEnemyName(collision.gameObject.name);

            List<EnemyPartyStatus> enemyPartyList = null;

            // �G���J�E���g����G�p�[�e�B�̔ԍ������X�g���烉���_���ɐݒ�
            if (collision.gameObject.GetComponent<SeeingEnemyController>())
            {
                if (collision.gameObject.GetComponent<SeeingEnemyController>().EnemyPartyStatuses.Count > 0)
                {
                    enemyPartyList = collision.gameObject.GetComponent<SeeingEnemyController>().EnemyPartyStatuses;
                }
                else
                {
                    enemyPartyList = baseEnemySeeing;
                }
            }
            else
            {
                if (collision.gameObject.GetComponent<HearingEnemyController>().EnemyPartyStatuses.Count > 0)
                {
                    enemyPartyList = collision.gameObject.GetComponent<HearingEnemyController>().EnemyPartyStatuses;
                }
                else
                {
                    enemyPartyList = baseEnemyHearing;
                }
            }
            // �G�Ǘ��V���O���g���ɓG�̃��X�g���i�[
            var party = enemyPartyList[Random.Range(0, enemyPartyList.Count)].GetPartyMembers();
            EnemyManager.Instance.Initialize();
            EnemyManager.Instance.SetEnemies(party);

            // �G�I�u�W�F�N�g(�S��)�̏���
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            if (enemies != null && enemies.Length > 0)
            {
                foreach (GameObject e in enemies)
                {
                    // �R���|�[�l���g�𖳌������ē������~�߂�
                    if (e.GetComponent<SeeingEnemyController>())
                    {
                        e.GetComponent<SeeingEnemyController>().enabled = false;
                    }
                    else if (e.GetComponent<HearingEnemyController>())
                    {
                        e.GetComponent<HearingEnemyController>().enabled = false;
                    }
                    e.GetComponent<NavMeshAgent>().enabled = false;

                    // �v���C���[�ƓG�I�u�W�F�N�g�̍��W�E�����E������Ԃ�ۑ�
                    CommonVariableManager.SetPlayerPosition(gameObject.transform.position);
                    CommonVariableManager.SetPlayerRotation(gameObject.transform.eulerAngles);
                    CommonVariableManager.SetEnemyPositions(enemies);
                    CommonVariableManager.SetEnemyRotations(enemies);
                }
            }

            // �g�����V�W�������t�F�[�h�C�����Ó]
            await SceneController.Instance.SwitchFieldAndBattleScene("Battle");
            //StartCoroutine(SceneController.Instance.SwitchScene("Battle"));
        }
    }
}