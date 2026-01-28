using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // for text mesh pro (normal text use UnityEngine.UI)
using UnityEngine.SceneManagement; // 切換場景用

public class ScoreManager : MonoBehaviour
{
    public int score = 0;
    public GameObject ReplayButton;
    public GameObject FailMSG;
    public GameObject WinMsG;
    public GameObject WinImg;
    public GameObject DeathGraph;
    public GameObject Player;
    public TMP_Text scoreText;
    public AudioClip winSound;
    public AudioClip BGMSound;
    public AudioClip loseSound;
    private AudioSource audioSource;

    private bool isGameEnded = false; // 新增此變數來檢查遊戲狀態

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        PlayNewMusic(BGMSound);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 加分方法，由 DropObjCtrl 呼叫
    public void AddScore(int points)
    {
        score += points;
        UpdateScoreText();
    }

    // 更新分數顯示
    private void UpdateScoreText()
    {
        scoreText.text = "Score: " + score; // 更新 Text 的內容
    }

    // 遊戲結束方法
    public void GameOver()
    {
        if (isGameEnded) return; // 如果遊戲已結束，則不執行後續代碼
        isGameEnded = true; // 設置遊戲結束狀態

        FailMSG.SetActive(true);
        DeathGraph.SetActive(true);
        PlayNewMusic(loseSound);
        Time.timeScale = 0;
        ReplayButton.SetActive(true);
        Player.GetComponent<PlayerCtrl>().setGameOver();
        Debug.Log("Game Over! Final Score: " + score);
    }

    // 遊戲勝利方法
    public void GameWin()
    {
        if (isGameEnded) return; // 如果遊戲已結束，則不執行後續代碼
        isGameEnded = true; // 設置遊戲結束狀態

        WinMsG.SetActive(true);
        WinImg.SetActive(true);
        PlayNewMusic(winSound);
        Time.timeScale = 0;
        ReplayButton.SetActive(true);
        Player.GetComponent<PlayerCtrl>().setGameOver();
        Debug.Log("Game Over Win! Final Score: " + score);
    }

    // 重玩方法
    public void Replay()
    {
        SceneManager.LoadScene("SampleScene");
        PlayNewMusic(BGMSound);
        Time.timeScale = 1f;
        Player.GetComponent<PlayerCtrl>().Reset();
        score = 0;
        isGameEnded = false; // 重置遊戲結束狀態
        Debug.Log("Restart!!");
    }

    public void PlayNewMusic(AudioClip clip)
    {
        // 如果 AudioSource 正在播放，停止它
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        // 設置新的音樂剪輯
        audioSource.clip = clip;

        // 設置為循環播放
        audioSource.loop = true;

        // 播放音樂
        audioSource.Play();
    }
}
