using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoville.HOTween;

public class FoxMove : MonoBehaviour
{
    private float Delaysecond = 0.665f;
    public string targetTag = string.Empty;
    public string GrenadeTag = string.Empty;
    //���� ����
    public enum FoxState { None, Idle, Move, Wait, GoTarget, Atk, Die }

    //���� �⺻ �Ӽ�
    [Header("�⺻ �Ӽ�")]
    //���� �ʱ� ����
    public FoxState foxState = FoxState.None;
    //���� �̵� �ӵ�
    public float spdMove = 3.5f;
    //������ �� Ÿ��
    public GameObject targetCharactor = null;
    //������ �� Ÿ�� ��ġ���� (�Ź� �� ã������)
    public Transform targetTransform = null;
    //������ �� Ÿ�� ��ġ(�Ź� �� ã����)
    public Vector3 posTarget = Vector3.zero;

    //���� �ִϸ��̼� ������Ʈ ĳ�� 
    private Animation FoxAnimation = null;
    //���� Ʈ������ ������Ʈ ĳ��
    private Transform FoxTransform = null;

    [Header("�ִϸ��̼� Ŭ��")]
    public AnimationClip IdleAnimClip = null;
    public AnimationClip MoveAnimClip = null;
    public AnimationClip AtkAnimClip = null;
    public AnimationClip DieAnimClip = null;

    [Header("�����Ӽ�")]
    //���� ü��
    public int hp = 250;
    //���� ���� �Ÿ�
    public float AtkRange = 4.5f;
    //���� �����Ÿ�
    public float DetectorRange = 10.0f;
    //���� �ǰ� ����Ʈ
    public Material effectDamage = null;
    //���� ���� ����Ʈ
    public GameObject effectDie = null;

    [Header("ĳ���� �Ӽ�")]
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
        StartCoroutine(wait());
    }

    IEnumerator wait()
    {
        yield return new WaitForSeconds(Delaysecond);
        Destroy(gameObject);
    }

    /// <summary>
    /// �ִϸ��̼� �̺�Ʈ�� �߰����ִ� ��. 
    /// </summary>
    /// <param name="clip">�ִϸ��̼� Ŭ�� </param>
    /// <param name="funcName">�Լ��� </param>
    void OnAnimationEvent(AnimationClip clip, string funcName)
    {
        //�ִϸ��̼� �̺�Ʈ�� ����� �ش�
        AnimationEvent retEvent = new AnimationEvent();
        //�ִϸ��̼� �̺�Ʈ�� ȣ�� ��ų �Լ���
        retEvent.functionName = funcName;
        //�ִϸ��̼� Ŭ�� ������ �ٷ� ������ ȣ��
        retEvent.time = clip.length - 0.1f;
        //�� ������ �̺�Ʈ�� �߰� �Ͽ���
        clip.AddEvent(retEvent);
    }


    // Start is called before the first frame update
    void Start()
    {
        //ó�� ���� ������
        foxState = FoxState.Idle;

        //�ִϸ���, Ʈ������ ������Ʈ ĳ�� : �������� ã�� ������ �ʰ�
        FoxAnimation = GetComponent<Animation>();
        FoxTransform = GetComponent<Transform>();

        //�ִϸ��̼� Ŭ�� ��� ��� ���� 
        FoxAnimation[IdleAnimClip.name].wrapMode = WrapMode.Loop;
        FoxAnimation[MoveAnimClip.name].wrapMode = WrapMode.Loop;
        FoxAnimation[AtkAnimClip.name].wrapMode = WrapMode.Once;
        //FoxAnimation[DamageAnimClip.name].wrapMode = WrapMode.Once;

        //�ִϸ��̼� ���� ���� ũ�� �ø�
        FoxAnimation[DieAnimClip.name].wrapMode = WrapMode.Once;
        FoxAnimation[DieAnimClip.name].layer = 10;

        //���� �ִϸ��̼� �̺�Ʈ �߰�
        OnAnimationEvent(AtkAnimClip, "OnAtkAnmationFinished");
        OnAnimationEvent(DieAnimClip, "OnDieAnmationFinished");
    }

    /// <summary>
    /// ���� ���¿� ���� ������ �����ϴ� �Լ� 
    /// </summary>
    void CkState()
    {
        switch (foxState)
        {
            case FoxState.Idle:
                //�̵��� ���õ� RayCast��
                setIdle();
                break;
            case FoxState.GoTarget:
            case FoxState.Move:
                setMove();
                break;
            case FoxState.Atk:
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
    /// ���� ���°� ��� �� �� ���� 
    /// </summary>
    void setIdle()
    {
        if (targetCharactor == null)
        {
            posTarget = new Vector3(FoxTransform.position.x + Random.Range(-10f, 10f),
                                    FoxTransform.position.y,
                                    FoxTransform.position.z + Random.Range(-10f, 10f)
                );
            Ray ray = new Ray(posTarget, Vector3.down);
            RaycastHit infoRayCast = new RaycastHit();
            if (Physics.Raycast(ray, out infoRayCast, Mathf.Infinity) == true)
            {
                posTarget.y = infoRayCast.point.y;
            }
            foxState = FoxState.Move;
        }
        else
        {
            foxState = FoxState.GoTarget;
        }
    }

    /// <summary>
    /// ���� ���°� �̵� �� �� �� 
    /// </summary>
    void setMove()
    {
        //����� ������ �� ������ ���� 
        Vector3 distance = Vector3.zero;
        //��� ������ �ٶ󺸰� ���� �ִ��� 
        Vector3 posLookAt = Vector3.zero;

        //���� ����
        switch (foxState)
        {
            //������ ���ƴٴϴ� ���
            case FoxState.Move:
                //���� ���� ��ġ ���� ���ΰ� �ƴϸ�
                if (posTarget != Vector3.zero)
                {
                    //��ǥ ��ġ���� ���� �ִ� ��ġ ���� ���ϰ�
                    distance = posTarget - FoxTransform.position;

                    //���࿡ �����̴� ���� ������ ��ǥ�� �� ���� ���� ���� 
                    if (distance.magnitude < AtkRange)
                    {
                        //��� ���� �Լ��� ȣ��
                        StartCoroutine(setWait());
                        //���⼭ ����
                        //Debug.Log("111");
                        setIdle();
                        return;
                    }

                    //��� ������ �ٶ� �� ����. ���� ����
                    posLookAt = new Vector3(posTarget.x,
                                            //Ÿ���� ���� ���� ��찡 ������ y�� üũ
                                            FoxTransform.position.y,
                                            posTarget.z);
                }
                break;
            //ĳ���͸� ���ؼ� ���� ���ƴٴϴ�  ���
            case FoxState.GoTarget:
                //��ǥ ĳ���Ͱ� ���� ��
                if (targetCharactor != null)
                {
                    //��ǥ ��ġ���� ���� �ִ� ��ġ ���� ���ϰ�
                    distance = targetCharactor.transform.position - FoxTransform.position;
                    //���࿡ �����̴� ���� ������ ��ǥ�� �� ���� ���� ���� 
                    if (distance.magnitude < AtkRange)
                    {
                        //���ݻ��·� �����մ�.
                        foxState = FoxState.Atk;
                        //���⼭ ����
                        return;
                    }
                    //��� ������ �ٶ� �� ����. ���� ����
                    posLookAt = new Vector3(targetCharactor.transform.position.x,
                                            //Ÿ���� ���� ���� ��찡 ������ y�� üũ
                                            FoxTransform.position.y,
                                            targetCharactor.transform.position.z);
                }
                break;
            default:
                break;
        }

        //���� �̵��� ���⿡ ũ�⸦ ���ְ� ���⸸ ����(normalized)
        Vector3 direction = distance.normalized;

        //������ x,z ��� y�� ���� �İ� ���Ŷ� ����
        direction = new Vector3(direction.x, 0f, direction.z);

        //�̵��� ���� ���ϱ�
        Vector3 amount = direction * spdMove * Time.deltaTime;

        //ĳ���� ��Ʈ���� �ƴ� Ʈ���������� ���� ��ǥ �̿��Ͽ� �̵�
        FoxTransform.Translate(amount, Space.World);
        //ĳ���� ���� ���ϱ�
        FoxTransform.LookAt(posLookAt);

    }

    /// <summary>
    /// ��� ���� ���� �� 
    /// </summary>
    /// <returns></returns>
    IEnumerator setWait()
    {
        //���� ���¸� ��� ���·� �ٲ�
        foxState = FoxState.Wait;
        //����ϴ� �ð��� �������� �ʰ� ����
        float timeWait = Random.Range(1f, 3f);
        //��� �ð��� �־� ��.
        yield return new WaitForSeconds(timeWait);
        //��� �� �ٽ� �غ� ���·� ����
        foxState = FoxState.Move;
    }

    /// <summary>
    /// �ִϸ��̼��� ��������ִ� �� 
    /// </summary>
    void AnimationCtrl()
    {
        //������ ���¿� ���� �ִϸ��̼� ����
        switch (foxState)
        {
            //���� �غ��� �� �ִϸ��̼� ��.
            case FoxState.Wait:
            case FoxState.Idle:
                //�غ� �ִϸ��̼� ����
                FoxAnimation.CrossFade(IdleAnimClip.name);
                break;
            //������ ��ǥ �̵��� �� �ִϸ��̼� ��.
            case FoxState.GoTarget:
                FoxAnimation.CrossFade(MoveAnimClip.name);
                //�̵� �ִϸ��̼� ����

                break;
            //������ ��
            case FoxState.Atk:
                //���� �ִϸ��̼� ����
                FoxAnimation.CrossFade(AtkAnimClip.name);
                break;
            //�׾��� ��
            case FoxState.Die:
                //���� ���� �ִϸ��̼� ����
                FoxAnimation.CrossFade(DieAnimClip.name);
                OnDieAnmationFinished();
                break;
            default:
                break;

        }
    }

    /// <summary>
    /// �þ� ���� �ȿ� �ٸ� Trigger �Ǵ� ĳ���Ͱ� ������ ȣ�� �ȴ�.
    /// �Լ� ������ ��ǥ���� ������ ��ǥ���� �����ϰ� ������ Ÿ�� ��ġ�� �̵���Ų��.
    /// </summary>
    /// <param name="target"></param>
    void OnCKTarget(GameObject target)
    {
        //��ǥ ĳ���Ϳ� �Ķ���ͷ� ����� ������Ʈ�� �ִ´�.
        targetCharactor = target;

        //��ǥ ��ġ�� ��ǥ ĳ������ ��ġ ���� �ֽ��ϴ�.
        targetTransform = targetCharactor.transform;

        //��ǥ���� ���� ������ �̵��ϴ� ���·� ����
        foxState = FoxState.GoTarget;
    }

    /// <summary>
    /// ���� ���� ���� ���
    /// </summary>
    void setAtk()
    {
        //����� ĳ���Ͱ��� ��ġ �Ÿ�
        float distance = Vector3.Distance(targetTransform.position, FoxTransform.position);

        //���� �Ÿ����� �� ���� �Ÿ��� �־����ٸ�
        if (distance > AtkRange + 0.5f)
        {
            //Ÿ�ٰ��� �Ÿ��� �־����ٸ� �ٽ� Ÿ������ �̵�
            foxState = FoxState.GoTarget;
        }
    }

    /// <summary>
    /// ���� �ǰ� �浹 ����
    /// </summary>
    /// <param name="other"></param>
    private void OnCollisionEnter(Collision collision)
    {
        //���࿡ ������ ĳ���� ���ݿ� �¾Ҵٸ�
        if (collision.gameObject.CompareTag(targetTag))
        {
            //���� ü���� 10����
            hp -= 10;
            GameManager.Instance.BossHpText(hp);
            Debug.Log("����");
            if (hp > 0)
                //Instantiate(effectDamage, collision.transform.position, Quaternion.identity);
                return;
            else
            {
                foxState = FoxState.Die;
                Debug.Log("���ݴ��ߵ�");
                PlayerMove.Instance.playerGold += enemyGold;
            }
        }
    }

    private static FoxMove instance;
    public static FoxMove Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = FindObjectOfType<FoxMove>();
                if (obj != null)
                {
                    instance = obj;
                }
                else
                {
                    var newSingleton = new GameObject("Singleton Class").AddComponent<FoxMove>();
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
}