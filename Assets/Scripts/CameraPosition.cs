using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPosition : MonoBehaviour
{
    public Transform playerCamTrans;

    public float xDelta = 10f;
    public float yDelta = 10f;

    float rotationX;
    float rotationY;

    void Update()
    {
        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");
        rotationX = rotationX + x * xDelta;

        rotationY = rotationY + y * yDelta;
        rotationY = Mathf.Clamp(rotationY, -40, 80);
        // eulerangle¿Í roatationÂ÷ÀÌ 
        transform.rotation = Quaternion.Euler(new Vector3(-rotationY, rotationX, 0));
        transform.position = playerCamTrans.position;
    }
}
