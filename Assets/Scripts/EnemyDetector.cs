using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    //Trigger를 통해서 찾고자 하는 목표 Tag 설정
    public string targetTag = string.Empty;
    public float shpereSize;

    //Trigger 안에 들어왔냐?
    private void Start()
    {
        transform.position = Vector3.zero; 
    }
    /*private void OnTriggerEnter(Collider other)
    {
        //Tag가 찾던 Tag가 맞냐?
        //두 오브젝트 거리 알기
        if (other.gameObject.CompareTag(targetTag) == true)
        {
            //알림 메시지
            gameObject.SendMessageUpwards("OnCKTarget", other.gameObject, SendMessageOptions.DontRequireReceiver);
        }
    }*/
    private void Update()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, shpereSize);
        foreach (Collider col in cols)
        {
            if (col.gameObject.CompareTag(targetTag))
            {
                gameObject.SendMessageUpwards("OnCKTarget", col.gameObject, SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}
