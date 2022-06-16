using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarketCtrl : MonoBehaviour
{
    public GameObject marketPanel;
    private bool isCanvas = false;
    private float cactusGrenadeBuyGold = 50f;

    void Start()
    {
        
    }


    void Update()
    {
        OnOff();
    }

    void OnOff()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            isCanvas = (isCanvas == true) ? isCanvas = false : isCanvas = true;
            marketPanel.gameObject.SetActive(isCanvas);
        }
    }

    public void cactusGrenadeBuy()
    {
        if(PlayerMove.Instance.playerGold < cactusGrenadeBuyGold)
        {
            Debug.Log("���� �����մϴ�!");
        }
        else
        {
            Debug.Log("������ ����ź�� �����߽��ϴ�!");
            PlayerMove.Instance.playerGold -= cactusGrenadeBuyGold;
            PlayerMove.Instance.cactusGrenade += 1f;
        }
    }
}
