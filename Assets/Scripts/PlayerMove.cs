using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float PlayerSpd = 10f;
    public float PlayerGrv = 9.8f;
    public float rotSpd = 10f;
    private Vector3 MoveDirect = Vector3.zero;

    private Vector3 moveDir;

    private CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        Moving();
    }

    void Moving()
    {
        //Transform cameraTransform = Camera.main.transform;
        //Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);

        //float horizontal = Input.GetAxisRaw("Horizontal");
        //float vertical = Input.GetAxisRaw("Vertical");

        //moveDir = new Vector3(horizontal, 0, vertical);

        //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveDir), rotSpd * Time.deltaTime);

        //moveDir *= PlayerSpd;

        //moveDir.y += Physics.gravity.y * Time.deltaTime;

        //characterController.Move(moveDir * Time.deltaTime);

        //transform.position += new Vector3(horizontal, 0, vertical) * PlayerSpd * Time.deltaTime ;
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
        //�̵��ϴ� ������ ��
        Vector3 moveAmount = (MoveDirect * spd * Time.deltaTime);
        //���� �̵�
        collisionflages = characterCtrl.Move(moveAmount);
    }
}
