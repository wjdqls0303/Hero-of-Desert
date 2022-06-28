using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TextFade : MonoBehaviour
{
    public Text text;
    public float fadeA = 0f;

    private void Start()
    {
        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        while(fadeA < 1.0f)
        {
            fadeA += 0.01f;
            yield return new WaitForSeconds(0.01f);
            text.color = new Color(0, 0, 0, fadeA);
        }
        //StartCoroutine(Load());
        Application.Quit();
    }
    //IEnumerator Load()
    //{
    //    yield return new WaitForSeconds(2.0f);
    //    SceneManager.LoadScene("Main");
    //}
}
