using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("�Ӽ�")]
    //ĳ���� �̵��ӵ� ����
    [Tooltip("ĳ���� �̵��ӵ� ����")]
    public float moveSpd = 5.0f;
    //ĳ���� �̵��ӵ� ����
    public float runMoveSpd = 10.0f;
    //ĳ���� �̵����� ȸ�� �ӵ� ����
    public float DirectionRotateSpd = 100.0f;
    //ĳ���� ���� �����̴� ȸ�� �ӵ� ����
    public float BodyRotateSpd = 3.5f;
    //ĳ���� �ӵ� ���� ���� ��
    [Range(0.1f, 50.0f)]
    public float VelocityChangeSpd = 0.1f;
    //ĳ���� ���� �̵� �ӵ� ���� �ʱⰪ
    private Vector3 CurrentVelocitySpd = Vector3.zero;
    //ĳ���� ���� �̵� ���� �ʱⰪ ����
    private Vector3 MoveDirect = Vector3.zero;
    //CharacterController ĳ�� �غ�
    private CharacterController characterCtrl = null;
    //ĳ���� CollisionFlags �ʱⰪ ����
    private CollisionFlags collisionflages = CollisionFlags.None;
    [Header("�ִϸ��̼� �Ӽ�")]
    public AnimationClip animationClipIdle = null;
    public AnimationClip animationClipWalk = null;
    public AnimationClip animationClipRun = null;
    public AnimationClip animationClipAtk_1 = null;

    //animation component ĳ�� �غ�
    private Animation animationPlayer = null;
    //ĳ���� ����
    public enum PlayerState { None, Idle, Walk, Run, Atk }

    [Header("ĳ���� ����")]
    public PlayerState playerState = PlayerState.None;

    [Header("��������")]
    //������ ���� ������
    public TrailRenderer AtkTrailRenderer = null;
    public string EnemyTag = string.Empty;

    //���⿡ �ִ� �ݶ��̴� ĳ��
    public CapsuleCollider AtkCapsuleCollider = null;

    [Header("ĳ���� �Ӽ�")]
    public float playerGold = 0f;
    public float playerHp = 100f;

    [Header("������ �Ӽ�")]
    public GameObject cactusGrenadeIns = null;
    public float cactusGrenade = 0f;
    //public List<GameObject> Items = new List<GameObject>();
    public int currentCnt;
    public float killEnemy = 0f;
    public int spawnMaxCnt = 10;
    public float rndPos = 100f;
    public Vector3 pos = Vector3.zero;

    private static PlayerMove instance;
    public static PlayerMove Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = FindObjectOfType<PlayerMove>();
                if (obj != null)
                {
                    instance = obj;
                }
                else
                {
                    var newSingleton = new GameObject("Singleton Class").AddComponent<PlayerMove>();
                    instance = newSingleton;
                }
            }
            return instance;
        }
        private set
        {
            instance = value;
        }
    }
    private void Awake()
    {
        var objs = FindObjectsOfType<PlayerMove>();
        if (objs.Length != 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        //CharacterController ĳ��
        characterCtrl = GetComponent<CharacterController>();
        //animation ĳ��
        animationPlayer = GetComponent<Animation>();
        //�ڵ� ��� off
        animationPlayer.playAutomatically = false;
        //���� ������� �ִϸ��̼��� STOP
        animationPlayer.Stop();
        //���°��� �⺻��
        playerState = PlayerState.Idle;
        //�ִϸ��̼� ���¿� Ŭ�� ����
        animationPlayer[animationClipIdle.name].wrapMode = WrapMode.Loop;
        animationPlayer[animationClipWalk.name].wrapMode = WrapMode.Loop;
        animationPlayer[animationClipRun.name].wrapMode = WrapMode.Loop;
        animationPlayer[animationClipAtk_1.name].wrapMode = WrapMode.Once;

        SetAnimationEvent(animationClipAtk_1, "OnPlayerAttackFinshed");
    }

    void Update()
    {
        Move();
        gravitySet();
        BodyDirectionChange();
        InputAttackCtrl();
        AtkComponentCtrl();
        ckAnimationState();
        ItemNum();
    }
    /// <summary>
    /// ĳ���� �̵� �Լ�
    /// </summary>
    void Move()
    {
        //����ī�޶� Transform
        Transform cameraTransform = Camera.main.transform;
        //����ī�޶� �ٶ󺸴� ������ ����󿡼� � �����ΰ�
        Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);
        forward.y = 0.0f;
        //���� ����
        Vector3 right = new Vector3(forward.z, 0.0f, -forward.x);
        //Ű ��
        float vertical = Input.GetAxis("Vertical");
        float Horizontal = Input.GetAxis("Horizontal");
        //���� ���� ���� ��ǥ��
        Vector3 amount = vertical * forward + Horizontal * right;
        //�̵� ����
        MoveDirect = Vector3.RotateTowards(MoveDirect, amount, DirectionRotateSpd * Mathf.Deg2Rad * Time.deltaTime, 1000.0f);
        MoveDirect = MoveDirect.normalized;
        //�̵� �ӵ�
        float spd = moveSpd;
        //���࿡ playerState�� Run�̸�
        if (playerState == PlayerState.Run)
        {
            spd = runMoveSpd;
        }

        Vector3 gravitySpd = new Vector3(0, verticalSpd, 0);

        //�̵��ϴ� ������ ��
        Vector3 moveAmount = (MoveDirect * spd * Time.deltaTime) + gravitySpd;
        //���� �̵�
        collisionflages = characterCtrl.Move(moveAmount);
    }

    float GetVelocitySpd()
    {
        if (characterCtrl.velocity == Vector3.zero)
        {
            CurrentVelocitySpd = Vector3.zero;
        }
        else
        {
            Vector3 retVelocitySpd = characterCtrl.velocity;
            retVelocitySpd.y = 0;
            CurrentVelocitySpd = Vector3.Lerp(CurrentVelocitySpd, retVelocitySpd, VelocityChangeSpd * Time.fixedDeltaTime);
        }

        return CurrentVelocitySpd.magnitude;
    }

    /// <summary>
    /// ���� ���� �Լ� �ۼ�
    /// </summary>
    void BodyDirectionChange()
    {
        //�����̰� �ִ°�?
        if (GetVelocitySpd() > 0.0f)
        {
            //ĳ���� ������ �ٶ� ������? ĳ���� �ӵ� ����
            Vector3 newForward = characterCtrl.velocity;
            newForward.y = 0.0f;
            //ĳ���͸� �������� ������ �����Ѵ�.
            transform.forward = Vector3.Lerp(transform.forward, newForward, BodyRotateSpd);
        }
    }

    /// <summary>
    /// �ִϸ��̼� ��� �Լ�
    /// </summary>
    /// <param name="clip">�ִϸ��̼� Ŭ��</param>
    void playAnimationByClip(AnimationClip clip)
    {
        animationPlayer.clip = clip;
        animationPlayer.GetClip(clip.name);
        animationPlayer.CrossFade(clip.name);
    }

    /// <summary>
    /// ���¿� ���� �������ִ� �Լ�
    /// </summary>
    void ckAnimationState()
    {
        float nowSpd = GetVelocitySpd();
        switch (playerState)
        {
            case PlayerState.Idle:
                playAnimationByClip(animationClipIdle);
                if (nowSpd > 0.0f)
                {
                    playerState = PlayerState.Walk;
                }
                break;
            case PlayerState.Walk:
                playAnimationByClip(animationClipWalk);
                if (Input.GetKey(KeyCode.LeftShift) == true)
                {
                    Debug.Log("�ڴ�");
                    playerState = PlayerState.Run;
                }
                else if (nowSpd < 0.1f)
                {
                    playerState = PlayerState.Idle;
                }
                break;
            case PlayerState.Run:
                playAnimationByClip(animationClipRun);
                if (Input.GetKey(KeyCode.LeftShift) != true)
                {
                    Debug.Log("�ȴ´�");
                    playerState = PlayerState.Walk;
                }
                if (nowSpd < 0.1f)
                {
                    playerState = PlayerState.Idle;
                }
                break;
            case PlayerState.Atk:
                playAnimationByClip(animationClipAtk_1);
                break;
        }
    }

    /// <summary>
    /// ���� ��ư üũ �Լ�
    /// </summary>
    void InputAttackCtrl()
    {
        //���콺 Ŭ�� ����
        if (Input.GetMouseButton(0) == true)
        {
            playerState = PlayerState.Atk;
        }
    }

    /// <summary>
    /// CallBack ���� �ִϸ��̼� ����� ������ ȣ�� �Ǵ� �ִϸ��̼� �̺�Ʈ �Լ�
    /// </summary>
    void OnPlayerAttackFinshed()
    {
        playerState = PlayerState.Idle;
    }

    /// <summary>
    /// �ִϸ��̼� Ŭ�� ����� ������ �ִϸ��̼� �̺�Ʈ �Լ��� ȣ��
    /// </summary>
    /// <param name="clip">�ִϸ��̼� Ŭ��</param>
    /// <param name="funcName">�̺�Ʈ �Լ�</param>
    void SetAnimationEvent(AnimationClip clip, string funcName)
    {
        //���ο� �̺�Ʈ ����
        AnimationEvent newAnimationEvent = new AnimationEvent();

        //�ش� �̺�Ʈ�� ȣ���� funcName
        newAnimationEvent.functionName = funcName;
        //newAnimationEvent.time = clip.length - 0.1f;
        clip.AddEvent(newAnimationEvent);
    }

    private float gravity = 9.8f;
    private float verticalSpd = 0f;
    void gravitySet()
    {
        if ((collisionflages & CollisionFlags.CollidedBelow) != 0)
        {
            verticalSpd = 0f;
        }
        else
        {
            verticalSpd -= gravity + Time.deltaTime;
        }
    }

    //��ų �ִϸ��̼� ����� ������ �� �̺�Ʈ
    void OnSkillAnimFinished()
    {
        //���� ĳ���� ��ġ ����
        Vector3 pos = transform.position;

        //ĳ���� �� ���� 2.0���� ������ �Ÿ�
        pos += transform.forward * 2f;

        //�������� ��� ���·� �д�.
        playerState = PlayerState.Idle;
    }
    void AtkComponentCtrl()
    {
        switch (playerState)
        {
            case PlayerState.Atk:
                AtkCapsuleCollider.enabled = true;
                break;
            default:
                AtkCapsuleCollider.enabled = false;
                break;
        }
    }

    void ItemNum()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (cactusGrenade <= 0f)
            {
                Debug.Log("�������� �����ϴ�!");
            }
            else
            {
                Spawn();
                Debug.Log("�������� ����߽��ϴ�!");
                cactusGrenade -= 1f;
            }
        }
    }

    void Spawn()
    {
        pos = this.gameObject.transform.position;
        if (currentCnt >= spawnMaxCnt)
        {

            return;
        }

        //������ ��ġ�� �����Ѵ�. �ʱ� ���̸� 1000 ������ .x,z�� ���� 
        Vector3 vecSpawn = new Vector3(this.transform.position.x + Random.Range(rndPos, -rndPos), 10f, this.transform.position.z + Random.Range(rndPos, -rndPos));

        //������ �ӽ� ���̿��� �Ʒ��������� Raycast�� ���� �������� ���� ���ϱ�
        Ray ray = new Ray(vecSpawn, Vector3.down);

        //������ ���ο� ���͸� Instantiate�� clone�� �����.
        GameObject newMonster = Instantiate(cactusGrenadeIns, vecSpawn, Quaternion.identity);

        //������ ��Ͽ� ���ο� ���͸� �߰�
        currentCnt++;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag(EnemyTag))
        {
            Debug.Log("Atk");
            playerHp -= 10f;
        }
    }
}