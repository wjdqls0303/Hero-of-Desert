using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoville.HOTween;

public class EnemyMove : MonoBehaviour
{
    private float Delaysecond = 1.11f;
    private float posY = 0.5f;
    public string targetTag = string.Empty;
    //좀비 상태
    public enum ZombieState { None, Idle, Move, Wait, GoTarget, Atk, Die }

    //좀비 기본 속성
    [Header("기본 속성")]
    //좀비 초기 상태
    public ZombieState zombieState = ZombieState.None;
    //좀비 이동 속도
    public float spdMove = 3.5f;
    //좀비이 본 타겟
    public GameObject targetCharactor = null;
    //좀비이 본 타겟 위치정보 (매번 안 찾을려고)
    public Transform targetTransform = null;
    //좀비이 본 타겟 위치(매번 안 찾을려)
    public Vector3 posTarget = Vector3.zero;

    //좀비 애니메이션 컴포넌트 캐싱 
    private Animation ZombieAnimation = null;
    //좀비 트랜스폼 컴포넌트 캐싱
    private Transform ZombieTransform = null;

    [Header("애니메이션 클립")]
    public AnimationClip IdleAnimClip = null;
    public AnimationClip MoveAnimClip = null;
    public AnimationClip AtkAnimClip = null;
    public AnimationClip DieAnimClip = null;

    [Header("전투속성")]
    //좀비 체력
    public int hp = 100;
    //좀비 공격 거리
    public float AtkRange = 1.5f;
    //좀비 감지거리
    public float DetectorRange = 10.0f;
    //좀비 피격 이펙트
    public GameObject effectDamage = null;
    //좀비 다이 이펙트
    public GameObject effectDie = null;

    private Tweener effectTweener = null;

    [Header("캐릭터 속성")]
    public float enemyGold = 10f;

    void OnAtkAnmationFinished()
    {
        //Debug.Log("Atk Animation finished");
    }

    void OnDmgAnmationFinished()
    {
        //Debug.Log("Dmg Animation finished");
    }

    void OnDieAnmationFinished()
    {
        //Debug.Log("Die Animation finished");
        StartCoroutine(Die());
    }

    IEnumerator Die()
    {
        yield return new WaitForSeconds(Delaysecond);
        //아이템 얻기
        Refresh();
        zombieState = ZombieState.Idle;
    }

    /// <summary>
    /// 좀비 재활용
    /// </summary>
    void Refresh()
    {
        float posX = Random.Range(10f, -10f);
        float posZ = Random.Range(10f, -10f);
        transform.position = new Vector3(posX, 15, posZ);
    }

    /// <summary>
    /// 애니메이션 이벤트를 추가해주는 함. 
    /// </summary>
    /// <param name="clip">애니메이션 클립 </param>
    /// <param name="funcName">함수명 </param>
    void OnAnimationEvent(AnimationClip clip, string funcName)
    {
        //애니메이션 이벤트를 만들어 준다
        AnimationEvent retEvent = new AnimationEvent();
        //애니메이션 이벤트에 호출 시킬 함수명
        retEvent.functionName = funcName;
        //애니메이션 클립 끝나기 바로 직전에 호출
        retEvent.time = clip.length - 0.1f;
        //위 내용을 이벤트에 추가 하여라
        clip.AddEvent(retEvent);
    }


    // Start is called before the first frame update
    void Start()
    {
        //처음 상태 대기상태
        zombieState = ZombieState.Idle;

        //애니메이, 트랜스폼 컴포넌트 캐싱 : 쓸때마다 찾아 만들지 않게
        ZombieAnimation = GetComponent<Animation>();
        ZombieTransform = GetComponent<Transform>();

        //애니메이션 클립 재생 모드 비중
        ZombieAnimation[IdleAnimClip.name].wrapMode = WrapMode.Loop;
        ZombieAnimation[MoveAnimClip.name].wrapMode = WrapMode.Loop;
        ZombieAnimation[AtkAnimClip.name].wrapMode = WrapMode.Once;
        //ZombieAnimation[DamageAnimClip.name].wrapMode = WrapMode.Once;

        //애니메이션 블랜딩 위해 크게 올림
        ZombieAnimation[DieAnimClip.name].wrapMode = WrapMode.Once;
        ZombieAnimation[DieAnimClip.name].layer = 10;

        //공격 애니메이션 이벤트 추가
        OnAnimationEvent(AtkAnimClip, "OnAtkAnmationFinished");
        OnAnimationEvent(DieAnimClip, "OnDieAnmationFinished");
    }

    /// <summary>
    /// 좀비 상태에 따라 동작을 제어하는 함수 
    /// </summary>
    void CkState()
    {
        switch (zombieState)
        {
            case ZombieState.Idle:
                //이동에 관련된 RayCast값
                setIdle();
                break;
            case ZombieState.GoTarget:
            case ZombieState.Move:
                setMove();
                break;
            case ZombieState.Atk:
                setAtk();
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CkState();
        AnimationCtrl();
    }

    /// <summary>
    /// 좀비 상태가 대기 일 때 동작 
    /// </summary>
    void setIdle()
    {
        if (targetCharactor == null)
        {
            posTarget = new Vector3(ZombieTransform.position.x + Random.Range(-10f, 10f),
                                    ZombieTransform.position.y,
                                    ZombieTransform.position.z + Random.Range(-10f, 10f)
                );
            Ray ray = new Ray(posTarget, Vector3.down);
            RaycastHit infoRayCast = new RaycastHit();
            if (Physics.Raycast(ray, out infoRayCast, Mathf.Infinity) == true)
            {
                posTarget.y = infoRayCast.point.y;
            }
            zombieState = ZombieState.Move;
        }
        else
        {
            zombieState = ZombieState.GoTarget;
        }
    }

    /// <summary>
    /// 좀비 상태가 이동 일 때 동 
    /// </summary>
    void setMove()
    {
        //출발점 도착점 두 벡터의 차이 
        Vector3 distance = Vector3.zero;
        //어느 방향을 바라보고 가고 있느냐 
        Vector3 posLookAt = Vector3.zero;

        //좀비 상태
        switch (zombieState)
        {
            //좀비이 돌아다니는 경우
            case ZombieState.Move:
                //만약 랜덤 위치 값이 제로가 아니면
                if (posTarget != Vector3.zero)
                {
                    //목표 위치에서 좀비 있는 위치 차를 구하고
                    distance = posTarget - ZombieTransform.position;

                    //만약에 움직이는 동안 좀비이 목표로 한 지점 보다 작으 
                    if (distance.magnitude < AtkRange)
                    {
                        //대기 동작 함수를 호출
                        StartCoroutine(setWait());
                        //여기서 끝냄
                        Debug.Log("111");
                        setIdle();
                        return;
                    }

                    //어느 방향을 바라 볼 것인. 랜덤 지역
                    posLookAt = new Vector3(posTarget.x,
                                            //타겟이 높이 있을 경우가 있으니 y값 체크
                                            ZombieTransform.position.y,
                                            posTarget.z);
                }
                break;
            //캐릭터를 향해서 가는 돌아다니는  경우
            case ZombieState.GoTarget:
                //목표 캐릭터가 있을 땟
                if (targetCharactor != null)
                {
                    //목표 위치에서 좀비 있는 위치 차를 구하고
                    distance = targetCharactor.transform.position - ZombieTransform.position;
                    //만약에 움직이는 동안 좀비이 목표로 한 지점 보다 작으 
                    if (distance.magnitude < AtkRange)
                    {
                        //공격상태로 변경합니.
                        zombieState = ZombieState.Atk;
                        //여기서 끝냄
                        return;
                    }
                    //어느 방향을 바라 볼 것인. 랜덤 지역
                    posLookAt = new Vector3(targetCharactor.transform.position.x,
                                            //타겟이 높이 있을 경우가 있으니 y값 체크
                                            ZombieTransform.position.y,
                                            targetCharactor.transform.position.z);
                }
                //if (distance.magnitude > DetectorRange)
                //{
                //    targetCharactor = null;
                //    targetTransform = null;
                //    posTarget = Vector3.zero;
                //}
                break;
            default:
                break;
        }

        //좀비 이동할 방향에 크기를 없애고 방향만 가진(normalized)
        Vector3 direction = distance.normalized;

        //방향은 x,z 사용 y는 땅을 파고 들어갈거라 안함
        direction = new Vector3(direction.x, 0f, direction.z);

        //이동량 방향 구하기
        Vector3 amount = direction * spdMove * Time.deltaTime;

        //캐릭터 컨트롤이 아닌 트랜스폼으로 월드 좌표 이용하여 이동
        ZombieTransform.Translate(amount, Space.World);
        //캐릭터 방향 정하기
        ZombieTransform.LookAt(posLookAt);

    }

    /// <summary>
    /// 대기 상태 동작 함 
    /// </summary>
    /// <returns></returns>
    IEnumerator setWait()
    {
        //좀비 상태를 대기 상태로 바꿈
        zombieState = ZombieState.Wait;
        //대기하는 시간이 오래되지 않게 설정
        float timeWait = Random.Range(1f, 3f);
        //대기 시간을 넣어 준.
        yield return new WaitForSeconds(timeWait);
        //대기 후 다시 준비 상태로 변경
        zombieState = ZombieState.Move;
    }

    /// <summary>
    /// 애니메이션을 재생시켜주는 함 
    /// </summary>
    void AnimationCtrl()
    {
        //좀비의 상태에 따라서 애니메이션 적용
        switch (zombieState)
        {
            //대기와 준비할 때 애니메이션 같.
            case ZombieState.Wait:
            case ZombieState.Idle:
                //준비 애니메이션 실행
                ZombieAnimation.CrossFade(IdleAnimClip.name);
                break;
            //랜덤과 목표 이동할 때 애니메이션 같.
            case ZombieState.Move:
            case ZombieState.GoTarget:
                ZombieAnimation.CrossFade(MoveAnimClip.name);
                //이동 애니메이션 실행
                
                break;
            //공격할 때
            case ZombieState.Atk:
                //공격 애니메이션 실행
                ZombieAnimation.CrossFade(AtkAnimClip.name);
                break;
            //죽었을 때
            case ZombieState.Die:
                //죽을 때도 애니메이션 실행
                ZombieAnimation.CrossFade(DieAnimClip.name);
                OnDieAnmationFinished();
                break;
            default:
                break;

        }
    }

    /// <summary>
    /// 시야 범위 안에 다른 Trigger 또는 캐릭터가 들어오면 호출 된다.
    /// 함수 동작은 목표물이 들어오면 목표물을 설정하고 좀비을 타겟 위치로 이동시킨다.
    /// </summary>
    /// <param name="target"></param>
    void OnCKTarget(GameObject target)
    {
        //목표 캐릭터에 파라메터로 검출된 오브젝트를 넣는다.
        targetCharactor = target;

        //목표 위치에 목표 캐릭터의 위치 값을 넣습니다.
        targetTransform = targetCharactor.transform;

        //목표물을 향해 좀비이 이동하는 상태로 변경
        zombieState = ZombieState.GoTarget;
    }

    /// <summary>
    /// 좀비 상태 공격 모드
    /// </summary>
    void setAtk()
    {
        //좀비과 캐릭터간의 위치 거리
        float distance = Vector3.Distance(targetTransform.position, ZombieTransform.position);

        //공격 거리보다 둘 간의 거리가 멀어졌다면
        if (distance > AtkRange + 0.5f)
        {
            //타겟과의 거리가 멀어졌다면 다시 타겟으로 이동
            zombieState = ZombieState.GoTarget;
        }
    }

    /// <summary>
    /// 좀비 피격 충돌 검출
    /// </summary>
    /// <param name="other"></param>
    private void OnCollisionEnter(Collision collision)
    {
        //만약에 좀비이 캐릭터 공격에 맞았다면
        if (collision.gameObject.CompareTag(targetTag) == true)
        {
            //Debug.Log("Atk");
            //좀비 체력을 10빼고
            hp -= 10;
            if (hp > 0)
            {
                //피격 이벤트
                Instantiate(effectDamage, collision.transform.position, Quaternion.identity);

                //피격 트위닝 이펙트
                //effectDamageTween();
            }
            else
            {
                //0 보다 작으면 좀비이 죽음 상태로 바꾸어라
                zombieState = ZombieState.Die;
                gameObject.SendMessageUpwards("AddGold", gameObject, SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}