using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPosition : MonoBehaviour
{
    [Header("ī�޶� �⺻ �Ӽ�")]
    //ī�޶� ��ġ ĳ�� �غ�
    private Transform cameraTransform = null;

    //target
    public GameObject objTarget = null;

    //player transform ĳ��
    private Transform objTargetTransform = null;

    //ī�޶� 3������
    public enum CameraTypeState { First, Second, Third }

    //ī�޶� �⺻ 3��Ī 
    public CameraTypeState cameraState = CameraTypeState.Third;

    [Header("3��Ī ī�޶�")]
    //������ �Ÿ�
    public float distance = 6.0f;

    //�߰� ����
    public float height = 5f;

    //smooth time
    public float heightDamp = 2f;
    public float rotationDamping = 3f;

    [Header("2��Īī�޶�")]
    public float rotationSpd = 10f;

    [Header("1��Īī�޶�")]
    //���콺 ī�޶� ���� ������ ��ǥ
    public float detailX = 5f;
    public float detailY = 5f;

    //���콺 ȸ�� ��
    private float rotationX = 0f;
    private float rotationY = 0f;

    //ĳ��
    public Transform posFirstTarget = null;

    void FirstCamera()
    {
        //���콺 ��ǥ ��������
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        rotationX = cameraTransform.localEulerAngles.y + mouseX * detailX;
        rotationX = (rotationX > 180.0f) ? rotationX - 360.0f : rotationX;
        rotationY = rotationY + mouseY * detailY;
        rotationY = (rotationY > 180.0f) ? rotationY - 360.0f : rotationY;

        cameraTransform.localEulerAngles = new Vector3(-rotationY, rotationX, 0f);
        cameraTransform.position = posFirstTarget.position;
    }

    /// <summary>
    /// 2��Ī ī�޶� ����
    /// </summary>
    void SecondCamera()
    {
        cameraTransform.RotateAround(objTargetTransform.position, Vector3.up, rotationSpd * Time.deltaTime);

        cameraTransform.LookAt(objTargetTransform.position);
    }

    private void LateUpdate()
    {
        if (objTarget == null)
        {
            return;
        }

        if (objTargetTransform == null)
        {
            objTargetTransform = objTarget.transform;
        }

        switch (cameraState)
        {
            case CameraTypeState.Third:
                ThirdCamera();
                break;
            case CameraTypeState.Second:
                SecondCamera();
                break;
            case CameraTypeState.First:
                FirstCamera();
                break;
            default:
                break;
        }
    }

    void Start()
    {
        cameraTransform = GetComponent<Transform>();

        //Ÿ���� �ֳ�?
        if (objTarget != null)
        {
            objTargetTransform = objTarget.transform;
        }
    }

    /// <summary>
    /// 3��Ī ī�޶� �⺻ ���� �Լ�
    /// </summary>
    void ThirdCamera()
    {
        //���� Ÿ�� y�� ���� ��
        float objTargetRotationAngle = objTargetTransform.eulerAngles.y;

        //���� Ÿ�� ���� + ī�޶� ��ġ�� ���� �߰� ����
        float objHeight = objTargetTransform.position.y + height;

        //���� ���� ����
        float nowRotationAngle = cameraTransform.eulerAngles.y;
        float nowHeight = cameraTransform.position.y;

        //���� ������ ���� DAMP
        nowRotationAngle = Mathf.LerpAngle(nowRotationAngle, objTargetRotationAngle, rotationDamping * Time.deltaTime);

        //���� ���̿� ���� DAMP
        nowHeight = Mathf.Lerp(nowHeight, objHeight, heightDamp * Time.deltaTime);

        //����Ƽ ������ ����
        Quaternion nowRotation = Quaternion.Euler(0f, nowRotationAngle, 0f);

        //ī�޶� ��ġ ������ �̵�
        cameraTransform.position = objTargetTransform.position;
        cameraTransform.position -= nowRotation * Vector3.forward * distance;

        //�����̵�
        cameraTransform.position = new Vector3(cameraTransform.position.x, nowHeight, cameraTransform.position.z);

        cameraTransform.LookAt(objTargetTransform);
    }
}
