using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cactusGrenadeCtrl : MonoBehaviour
{
    public float throwSpd = 3f;
    public string enemyTag = string.Empty;
    public float waitingTime = 0f;
    void Start()
    {
        
    }


    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
