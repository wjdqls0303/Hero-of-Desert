using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    //Trigger�� ���ؼ� ã���� �ϴ� ��ǥ Tag ����
    public string targetTag = string.Empty;
    public float shpereSize;

    //Trigger �ȿ� ���Գ�?
    private void Start()
    {
        transform.position = Vector3.zero; 
    }
    /*private void OnTriggerEnter(Collider other)
    {
        //Tag�� ã�� Tag�� �³�?
        //�� ������Ʈ �Ÿ� �˱�
        if (other.gameObject.CompareTag(targetTag) == true)
        {
            //�˸� �޽���
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
