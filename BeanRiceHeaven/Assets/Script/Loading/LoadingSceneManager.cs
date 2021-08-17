using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LoadingSceneManager : MonoBehaviour
{
    public static string nextScene;
    public Image backgroundBlur;
    
    public Image loadingIcon;
    public GameObject[] cutImages = new GameObject[2];
    private float testTimer;
    

    private void Start()
    {
        int r = Random.Range(0, 2);
        testTimer = Random.Range(4.0f, 7.0f);
        backgroundBlur.color = new Color(0, 0, 0, 1);
        int i = 0;
        foreach (var g in cutImages)
        {
            g.SetActive(false);
            if(i++ == r)
                g.SetActive(true);
        }
        StartCoroutine(LoadNextScene());
    }

    public static void LoadNextScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }

    IEnumerator LoadNextScene()
    {
        yield return null;
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;
        Debug.Log(nextScene + " Name");

        float t = 0.0f;
        while (t < testTimer)
        {
            yield return null;
            t += Time.deltaTime;
            float p = t / testTimer;
            
            loadingIcon.transform.Rotate(new Vector3(0, 0, -Time.deltaTime * 50f));
            backgroundBlur.color = new Color(0, 0, 0, -10 * p + 1);

            if (p >= 0.9f)
            {

                backgroundBlur.color = new Color(0, 0, 0, (p - 0.9f) * 10);
                if (p > 0.98f)
                {
                    backgroundBlur.color = new Color(0, 0, 0, 1);
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
        
    }
}
