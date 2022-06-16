using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoville.HOTween;

public class EnemyMove : MonoBehaviour
{
    private float Delaysecond = 1.11f;
    private float posY = 0.5f;
    public string targetTag = string.Empty;
    //���� ����
    public enum ZombieState { None, Idle, Move, Wait, GoTarget, Atk, Die }

    //���� �⺻ �Ӽ�
    [Header("�⺻ �Ӽ�")]
    //���� �ʱ� ����
    public ZombieState zombieState = ZombieState.None;
    //���� �̵� �ӵ�
    public float spdMove = 3.5f;
    //������ �� Ÿ��
    public GameObject targetCharactor = null;
    //������ �� Ÿ�� ��ġ���� (�Ź� �� ã������)
    public Transform targetTransform = null;
    //������ �� Ÿ�� ��ġ(�Ź� �� ã����)
    public Vector3 posTarget = Vector3.zero;

    //���� �ִϸ��̼� ������Ʈ ĳ�� 
    private Animation ZombieAnimation = null;
    //���� Ʈ������ ������Ʈ ĳ��
    private Transform ZombieTransform = null;

    [Header("�ִϸ��̼� Ŭ��")]
    public AnimationClip IdleAnimClip = null;
    public AnimationClip MoveAnimClip = null;
    public AnimationClip AtkAnimClip = null;
    public AnimationClip DieAnimClip = null;

    [Header("�����Ӽ�")]
    //���� ü��
    public int hp = 100;
    //���� ���� �Ÿ�
    public float AtkRange = 1.5f;
    //���� �����Ÿ�
    public float DetectorRange = 10.0f;
    //���� �ǰ� ����Ʈ
    public GameObject effectDamage = null;
    //���� ���� ����Ʈ
    public GameObject effectDie = null;

    private Tweener effectTweener = null;

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
        StartCoroutine(Die());
    }

    IEnumerator Die()
    {
        yield return new WaitForSeconds(Delaysecond);
        //������ ���
        Refresh();
        zombieState = ZombieState.Idle;
    }

    /// <summary>
    /// ���� ��Ȱ��
    /// </summary>
    void Refresh()
    {
        float posX = Random.Range(10f, -10f);
        float posZ = Random.Range(10f, -10f);
        transform.position = new Vector3(posX, 15, posZ);
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
        zombieState = ZombieState.Idle;

        //�ִϸ���, Ʈ������ ������Ʈ ĳ�� : �������� ã�� ������ �ʰ�
        ZombieAnimation = GetComponent<Animation>();
        ZombieTransform = GetComponent<Transform>();

        //�ִϸ��̼� Ŭ�� ��� ��� ����
        ZombieAnimation[IdleAnimClip.name].wrapMode = WrapMode.Loop;
        ZombieAnimation[MoveAnimClip.name].wrapMode = WrapMode.Loop;
        ZombieAnimation[AtkAnimClip.name].wrapMode = WrapMode.Once;
        //ZombieAnimation[DamageAnimClip.name].wrapMode = WrapMode.Once;

        //�ִϸ��̼� ���� ���� ũ�� �ø�
        ZombieAnimation[DieAnimClip.name].wrapMode = WrapMode.Once;
        ZombieAnimation[DieAnimClip.name].layer = 10;

        //���� �ִϸ��̼� �̺�Ʈ �߰�
        OnAnimationEvent(AtkAnimClip, "OnAtkAnmationFinished");
        OnAnimationEvent(DieAnimClip, "OnDieAnmationFinished");
    }

    /// <summary>
    /// ���� ���¿� ���� ������ �����ϴ� �Լ� 
    /// </summary>
    void CkState()
    {
        switch (zombieState)
        {
            case ZombieState.Idle:
                //�̵��� ���õ� RayCast��
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
    /// ���� ���°� ��� �� �� ���� 
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
    /// ���� ���°� �̵� �� �� �� 
    /// </summary>
    void setMove()
    {
        //����� ������ �� ������ ���� 
        Vector3 distance = Vector3.zero;
        //��� ������ �ٶ󺸰� ���� �ִ��� 
        Vector3 posLookAt = Vector3.zero;

        //���� ����
        switch (zombieState)
        {
            //������ ���ƴٴϴ� ���
            case ZombieState.Move:
                //���� ���� ��ġ ���� ���ΰ� �ƴϸ�
                if (posTarget != Vector3.zero)
                {
                    //��ǥ ��ġ���� ���� �ִ� ��ġ ���� ���ϰ�
                    distance = posTarget - ZombieTransform.position;

                    //���࿡ �����̴� ���� ������ ��ǥ�� �� ���� ���� ���� 
                    if (distance.magnitude < AtkRange)
                    {
                        //��� ���� �Լ��� ȣ��
                        StartCoroutine(setWait());
                        //���⼭ ����
                        Debug.Log("111");
                        setIdle();
                        return;
                    }

                    //��� ������ �ٶ� �� ����. ���� ����
                    posLookAt = new Vector3(posTarget.x,
                                            //Ÿ���� ���� ���� ��찡 ������ y�� üũ
                                            ZombieTransform.position.y,
                                            posTarget.z);
                }
                break;
            //ĳ���͸� ���ؼ� ���� ���ƴٴϴ�  ���
            case ZombieState.GoTarget:
                //��ǥ ĳ���Ͱ� ���� ��
                if (targetCharactor != null)
                {
                    //��ǥ ��ġ���� ���� �ִ� ��ġ ���� ���ϰ�
                    distance = targetCharactor.transform.position - ZombieTransform.position;
                    //���࿡ �����̴� ���� ������ ��ǥ�� �� ���� ���� ���� 
                    if (distance.magnitude < AtkRange)
                    {
                        //���ݻ��·� �����մ�.
                        zombieState = ZombieState.Atk;
                        //���⼭ ����
                        return;
                    }
                    //��� ������ �ٶ� �� ����. ���� ����
                    posLookAt = new Vector3(targetCharactor.transform.position.x,
                                            //Ÿ���� ���� ���� ��찡 ������ y�� üũ
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

        //���� �̵��� ���⿡ ũ�⸦ ���ְ� ���⸸ ����(normalized)
        Vector3 direction = distance.normalized;

        //������ x,z ��� y�� ���� �İ� ���Ŷ� ����
        direction = new Vector3(direction.x, 0f, direction.z);

        //�̵��� ���� ���ϱ�
        Vector3 amount = direction * spdMove * Time.deltaTime;

        //ĳ���� ��Ʈ���� �ƴ� Ʈ���������� ���� ��ǥ �̿��Ͽ� �̵�
        ZombieTransform.Translate(amount, Space.World);
        //ĳ���� ���� ���ϱ�
        ZombieTransform.LookAt(posLookAt);

    }

    /// <summary>
    /// ��� ���� ���� �� 
    /// </summary>
    /// <returns></returns>
    IEnumerator setWait()
    {
        //���� ���¸� ��� ���·� �ٲ�
        zombieState = ZombieState.Wait;
        //����ϴ� �ð��� �������� �ʰ� ����
        float timeWait = Random.Range(1f, 3f);
        //��� �ð��� �־� ��.
        yield return new WaitForSeconds(timeWait);
        //��� �� �ٽ� �غ� ���·� ����
        zombieState = ZombieState.Move;
    }

    /// <summary>
    /// �ִϸ��̼��� ��������ִ� �� 
    /// </summary>
    void AnimationCtrl()
    {
        //������ ���¿� ���� �ִϸ��̼� ����
        switch (zombieState)
        {
            //���� �غ��� �� �ִϸ��̼� ��.
            case ZombieState.Wait:
            case ZombieState.Idle:
                //�غ� �ִϸ��̼� ����
                ZombieAnimation.CrossFade(IdleAnimClip.name);
                break;
            //������ ��ǥ �̵��� �� �ִϸ��̼� ��.
            case ZombieState.Move:
            case ZombieState.GoTarget:
                ZombieAnimation.CrossFade(MoveAnimClip.name);
                //�̵� �ִϸ��̼� ����
                
                break;
            //������ ��
            case ZombieState.Atk:
                //���� �ִϸ��̼� ����
                ZombieAnimation.CrossFade(AtkAnimClip.name);
                break;
            //�׾��� ��
            case ZombieState.Die:
                //���� ���� �ִϸ��̼� ����
                ZombieAnimation.CrossFade(DieAnimClip.name);
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
        zombieState = ZombieState.GoTarget;
    }

    /// <summary>
    /// ���� ���� ���� ���
    /// </summary>
    void setAtk()
    {
        //����� ĳ���Ͱ��� ��ġ �Ÿ�
        float distance = Vector3.Distance(targetTransform.position, ZombieTransform.position);

        //���� �Ÿ����� �� ���� �Ÿ��� �־����ٸ�
        if (distance > AtkRange + 0.5f)
        {
            //Ÿ�ٰ��� �Ÿ��� �־����ٸ� �ٽ� Ÿ������ �̵�
            zombieState = ZombieState.GoTarget;
        }
    }

    /// <summary>
    /// ���� �ǰ� �浹 ����
    /// </summary>
    /// <param name="other"></param>
    private void OnCollisionEnter(Collision collision)
    {
        //���࿡ ������ ĳ���� ���ݿ� �¾Ҵٸ�
        if (collision.gameObject.CompareTag(targetTag) == true)
        {
            //Debug.Log("Atk");
            //���� ü���� 10����
            hp -= 10;
            if (hp > 0)
            {
                //�ǰ� �̺�Ʈ
                Instantiate(effectDamage, collision.transform.position, Quaternion.identity);

                //�ǰ� Ʈ���� ����Ʈ
                //effectDamageTween();
            }
            else
            {
                //0 ���� ������ ������ ���� ���·� �ٲپ��
                zombieState = ZombieState.Die;
                gameObject.SendMessageUpwards("AddGold", gameObject, SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}