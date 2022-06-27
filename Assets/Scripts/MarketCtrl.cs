using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarketCtrl : MonoBehaviour
{
    public GameObject marketPanel;
    private bool isCanvas = false;
    public float cactusGrenadeBuyGold = 50f;

    void Update()
    {
        OnOff();
    }

    void OnOff()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if (Time.timeScale == 1.0f)
            {
                Time.timeScale = 0.0f;
                isCanvas = (isCanvas == true) ? isCanvas = false : isCanvas = true;
                marketPanel.gameObject.SetActive(isCanvas);
            }
            else
            {
                Time.timeScale = 1.0f;
                isCanvas = (isCanvas == true) ? isCanvas = false : isCanvas = true;
                marketPanel.gameObject.SetActive(isCanvas);
            }
        }
    }

    public static MarketCtrl instance;

    public static MarketCtrl Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = FindObjectOfType<MarketCtrl>();
                if (obj != null)
                {
                    instance = obj;
                }
                else
                {
                    var newSingleton = new GameObject("Singleton Class").AddComponent<MarketCtrl>();
                    instance = newSingleton;
                }
            }
            return instance;
        }
        private set
        {
            instance = value;
        }
    }
}
