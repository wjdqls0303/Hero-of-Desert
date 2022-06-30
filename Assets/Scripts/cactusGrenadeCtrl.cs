using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cactusGrenadeCtrl : MonoBehaviour
{
    public float throwSpd = 3f;
    public string enemyTag = string.Empty;
    public string grenadeTag = string.Empty;
    public float waitingTime = 0f;
    public float explosionRange = 5f;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("ºÎµúÈû");
        Collider[] colls = Physics.OverlapSphere(transform.position, explosionRange);

        Debug.Log(colls.Length);
        for (int i = 0; i < colls.Length; i++)
        {
            Debug.Log(colls[i].name);
            if (colls[i].CompareTag(enemyTag))
            {
                colls[i].GetComponent<IHittable>().GetHit();
            }

            Destroy(gameObject);
        }

        PlayerMove.Instance.currentCnt--;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange); 
    }

}
