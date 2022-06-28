using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private bool isText = false;
    public Text cactusGrenadeCount;
    public Text goldCount;
    public GameObject goldRack;
    public Text hpCount;
    public Text boosHpCount;
    public GameObject boosHp;
    public GameObject TutorialText;
    public GameObject EnemyText;
    public Text KillCountText;
    public float waitSeconds = 1.0f;


    private void Start()
    {
        TutorialT();
        EnemyT();
    }

    void Update()
    {
        GoldText();
        cactusGrenadeText();
        HPText();
        KillCount();
    }

    IEnumerator Tutorialview()
    {
        TutorialText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(3.0f);
        TutorialText.gameObject.SetActive(false);
    }

    IEnumerator Enemyview()
    {
        yield return new WaitForSecondsRealtime(3.0f);
        EnemyText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(3.0f);
        EnemyText.gameObject.SetActive(false);
    }

    void EnemyT()
    {
        StartCoroutine(Enemyview());
    }

    void TutorialT()
    {
        StartCoroutine(Tutorialview());
    }

    void cactusGrenadeText()
    {
        cactusGrenadeCount.text = PlayerMove.Instance.cactusGrenade + "��";
    }
    
    void GoldText()
    {
        goldCount.text = PlayerMove.Instance.playerGold + "���";
    }

    void HPText()
    {
        hpCount.text = PlayerMove.Instance.playerHp + " : HP";
    }

    void KillCount()
    {
        KillCountText.text = "10 / " + PlayerMove.Instance.killEnemy;
    }

    public void BossHpText(int hp)
    {
        boosHpCount.text = hp + " : HP";
        Debug.Log(FoxMove.Instance.hp);
    }

    IEnumerator WaitforIt()
    {
        yield return new WaitForSecondsRealtime(waitSeconds);
        goldRack.gameObject.SetActive(false);
        isText = false;
    }

    public void cactusGrenadeBuy()
    {
        if (PlayerMove.Instance.playerGold < MarketCtrl.Instance.cactusGrenadeBuyGold)
        {
            Debug.Log("���� �����մϴ�!");
            isText = (isText == true) ? isText = false : isText = true;
            goldRack.gameObject.SetActive(isText);
            StartCoroutine(WaitforIt());
        }
        else
        {
            Debug.Log("������ ����ź�� �����߽��ϴ�!");
            PlayerMove.Instance.playerGold -= MarketCtrl.Instance.cactusGrenadeBuyGold;
            PlayerMove.Instance.cactusGrenade += 1f;
        }
    }

    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = FindObjectOfType<GameManager>();
                if (obj != null)
                {
                    instance = obj;
                }
                else
                {
                    var newSingleton = new GameObject("Singleton Class").AddComponent<GameManager>();
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
