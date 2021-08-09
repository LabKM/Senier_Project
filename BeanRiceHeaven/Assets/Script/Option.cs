using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Option : MonoBehaviour { 

    public GameObject pausePanel;
    public GameObject resultPanel;
    public BeanController player;

    void Start()
    {
        isPause = false;
    }

    public void OnClickExit()
    {
        #if UNITY_EDITOR 
            UnityEditor.EditorApplication.isPlaying=false; 
        #else 
            Application.Quit(); 
        #endif
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GamePause();
        }
    }

    public bool isPause {
        private set; get;
    }
    public void GamePause()
    {
        isPause = !isPause;
        if (isPause)
        {
            Time.timeScale = 0;
            player.isInputable = false;
            MouseLocker.ShowMouse();
            //pausePanel.SetActive(true);
        }
        else
        {
            Continue();
        }
    }
    public void Continue()
    {
        isPause = true;
        Time.timeScale = 1;
        player.isInputable = true;
        MouseLocker.HideMouse();
        //resultPanel.SetActive(false);
        //pausePanel.SetActive(false);
    }
    public void Retry()
    {
        player.transform.position = Vector3.zero;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }
        Continue();
    }
    public void GameOver()
    {
        resultPanel.SetActive(true);
        Time.timeScale = 0;
        player.isInputable = false;
        resultPanel.transform.Find("Result").GetComponent<TextMeshProUGUI>().text = "You Can't Clear Game.....";
    }
    public void GameClear()
    {
        resultPanel.SetActive(true);
        Time.timeScale = 0;
        player.isInputable = false;
        resultPanel.transform.Find("Result").GetComponent<TextMeshProUGUI>().text = "You Successly Escape!!";
    }
}
