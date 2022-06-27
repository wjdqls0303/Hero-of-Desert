using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("속성")]
    //캐릭터 이동속도 설정
    [Tooltip("캐릭터 이동속도 설정")]
    public float moveSpd = 5.0f;
    //캐릭터 이동속도 설정
    public float runMoveSpd = 10.0f;
    //캐릭터 이동방향 회전 속도 설정
    public float DirectionRotateSpd = 100.0f;
    //캐릭터 몸을 움직이는 회전 속도 설정
    public float BodyRotateSpd = 3.5f;
    //캐릭터 속도 변경 증가 값
    [Range(0.1f, 50.0f)]
    public float VelocityChangeSpd = 0.1f;
    //캐릭터 현재 이동 속도 설정 초기값
    private Vector3 CurrentVelocitySpd = Vector3.zero;
    //캐릭터 현재 이동 방향 초기값 설정
    private Vector3 MoveDirect = Vector3.zero;
    //CharacterController 캐싱 준비
    private CharacterController characterCtrl = null;
    //캐릭터 CollisionFlags 초기값 설정
    private CollisionFlags collisionflages = CollisionFlags.None;
    [Header("애니메이션 속성")]
    public AnimationClip animationClipIdle = null;
    public AnimationClip animationClipWalk = null;
    public AnimationClip animationClipRun = null;
    public AnimationClip animationClipAtk_1 = null;

    //animation component 캐싱 준비
    private Animation animationPlayer = null;
    //캐릭터 상태
    public enum PlayerState { None, Idle, Walk, Run, Atk }

    [Header("캐릭터 상태")]
    public PlayerState playerState = PlayerState.None;

    [Header("전투관련")]
    //공격할 때만 켜지게
    public TrailRenderer AtkTrailRenderer = null;
    public string EnemyTag = string.Empty;

    //무기에 있는 콜라이더 캐싱
    public CapsuleCollider AtkCapsuleCollider = null;

    [Header("캐릭터 속성")]
    public float playerGold = 0f;
    public float playerHp = 100f;

    [Header("아이템 속성")]
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
        //CharacterController 캐싱
        characterCtrl = GetComponent<CharacterController>();
        //animation 캐싱
        animationPlayer = GetComponent<Animation>();
        //자동 재생 off
        animationPlayer.playAutomatically = false;
        //현재 재생중인 애니메이션을 STOP
        animationPlayer.Stop();
        //상태값을 기본값
        playerState = PlayerState.Idle;
        //애니메이션 상태에 클립 연결
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
    /// 캐릭터 이동 함수
    /// </summary>
    void Move()
    {
        //메인카메라 Transform
        Transform cameraTransform = Camera.main.transform;
        //메인카메라가 바라보는 방향이 월드상에서 어떤 방향인가
        Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);
        forward.y = 0.0f;
        //벡터 내적
        Vector3 right = new Vector3(forward.z, 0.0f, -forward.x);
        //키 값
        float vertical = Input.GetAxis("Vertical");
        float Horizontal = Input.GetAxis("Horizontal");
        //방향 벡터 이자 목표점
        Vector3 amount = vertical * forward + Horizontal * right;
        //이동 방향
        MoveDirect = Vector3.RotateTowards(MoveDirect, amount, DirectionRotateSpd * Mathf.Deg2Rad * Time.deltaTime, 1000.0f);
        MoveDirect = MoveDirect.normalized;
        //이동 속도
        float spd = moveSpd;
        //만약에 playerState가 Run이면
        if (playerState == PlayerState.Run)
        {
            spd = runMoveSpd;
        }

        Vector3 gravitySpd = new Vector3(0, verticalSpd, 0);

        //이동하는 프레임 양
        Vector3 moveAmount = (MoveDirect * spd * Time.deltaTime) + gravitySpd;
        //실제 이동
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
    /// 몸통 방향 함수 작성
    /// </summary>
    void BodyDirectionChange()
    {
        //움직이고 있는가?
        if (GetVelocitySpd() > 0.0f)
        {
            //캐릭터 몸통이 바라볼 전방은? 캐릭터 속도 방향
            Vector3 newForward = characterCtrl.velocity;
            newForward.y = 0.0f;
            //캐릭터를 전방으로 방향을 설정한다.
            transform.forward = Vector3.Lerp(transform.forward, newForward, BodyRotateSpd);
        }
    }

    /// <summary>
    /// 애니메이션 재생 함수
    /// </summary>
    /// <param name="clip">애니메이션 클립</param>
    void playAnimationByClip(AnimationClip clip)
    {
        animationPlayer.clip = clip;
        animationPlayer.GetClip(clip.name);
        animationPlayer.CrossFade(clip.name);
    }

    /// <summary>
    /// 상태에 따른 변경해주는 함수
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
                    Debug.Log("뛴다");
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
                    Debug.Log("걷는다");
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
    /// 공격 버튼 체크 함수
    /// </summary>
    void InputAttackCtrl()
    {
        //마우스 클릭 감지
        if (Input.GetMouseButton(0) == true)
        {
            playerState = PlayerState.Atk;
        }
    }

    /// <summary>
    /// CallBack 공격 애니메이션 재생이 끝나면 호출 되는 애니메이션 이벤트 함수
    /// </summary>
    void OnPlayerAttackFinshed()
    {
        playerState = PlayerState.Idle;
    }

    /// <summary>
    /// 애니메이션 클립 재생이 끝날때 애니메이션 이벤트 함수를 호출
    /// </summary>
    /// <param name="clip">애니메이션 클립</param>
    /// <param name="funcName">이벤트 함수</param>
    void SetAnimationEvent(AnimationClip clip, string funcName)
    {
        //새로운 이벤트 선언
        AnimationEvent newAnimationEvent = new AnimationEvent();

        //해당 이벤트의 호출은 funcName
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

    //스킬 애니메이션 재생이 끝났을 때 이벤트
    void OnSkillAnimFinished()
    {
        //현재 캐릭터 위치 저장
        Vector3 pos = transform.position;

        //캐릭터 앞 방향 2.0정도 떨어진 거리
        pos += transform.forward * 2f;

        //끝났으면 대기 상태로 둔다.
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
                Debug.Log("아이템이 없습니다!");
            }
            else
            {
                Spawn();
                Debug.Log("아이템을 사용했습니다!");
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

        //생성할 위치를 지정한다. 초기 높이만 1000 나머지 .x,z는 랜덤 
        Vector3 vecSpawn = new Vector3(this.transform.position.x + Random.Range(rndPos, -rndPos), 10f, this.transform.position.z + Random.Range(rndPos, -rndPos));

        //생성할 임시 높이에서 아래방향으로 Raycast를 통해 지형까지 높이 구하기
        Ray ray = new Ray(vecSpawn, Vector3.down);

        //생성할 새로운 몬스터를 Instantiate로 clone을 만든다.
        GameObject newMonster = Instantiate(cactusGrenadeIns, vecSpawn, Quaternion.identity);

        //아이템 목록에 새로운 몬스터를 추가
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