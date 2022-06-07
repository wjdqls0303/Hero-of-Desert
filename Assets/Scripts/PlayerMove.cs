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
        //이동하는 프레임 양
        Vector3 moveAmount = (MoveDirect * spd * Time.deltaTime);
        //실제 이동
        collisionflages = characterCtrl.Move(moveAmount);
    }
}
