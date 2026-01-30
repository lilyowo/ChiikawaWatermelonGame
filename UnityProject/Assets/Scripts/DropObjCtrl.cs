using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropObjCtrl : MonoBehaviour
{
    public int rank;  // 物件的等級 (從1到10)
    public GameObject nextRankObj;  // Rank+1 的物件 prefab
    public AudioClip mergeSound;  // 撥放合併音效
    AudioSource audioSource;
    CircleCollider2D circleCollider;
    SpriteRenderer spriteRenderer;
    public bool isTriggered = false; // 判斷是否與 EndLine 碰撞
    public float triggerTimer = 0.0f; // 計時器
    private ScoreManager scoreManager;  // 引用 ScoreManager

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        circleCollider = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        scoreManager = GameObject.Find("EndLine").GetComponent<ScoreManager>();
    }

    void Update(){
        // 如果物件碰到了 EndLine 並持續超過 3 秒，觸發 GameOver
        if (isTriggered)
        {
            triggerTimer += Time.deltaTime;
            if (triggerTimer >= 3.0f){
                if (scoreManager != null){
                    scoreManager.GameOver();  // 調用 ScoreManager 的 GameOver 方法
                }
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        DropObjCtrl otherCircleCtrl = collision.gameObject.GetComponent<DropObjCtrl>();

        if (otherCircleCtrl != null && otherCircleCtrl.rank == this.rank){
            if(rank==10){
                
            }else{
                if (mergeSound != null){
                    audioSource.PlayOneShot(mergeSound, 1.0f);
                }
                Merge(otherCircleCtrl);
                if (rank==9 && scoreManager != null){
                    scoreManager.GameWin();  
                }
            }
            
            
            
        }
    }
    // 檢測是否與 EndLine 觸發
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EndLine")){
            isTriggered = true;
        }
    }

    void OnTriggerExit2D(Collider2D other){
        if (other.CompareTag("EndLine")){
            isTriggered = false;
            triggerTimer = 0.0f;  // 重置計時器
        }
    }

    void Merge(DropObjCtrl otherCircleCtrl)
    {
        // 1. 消除碰撞的二者中 y 位置較高者，留下 y 位置較低者
        if (this.transform.position.y < otherCircleCtrl.transform.position.y){// this is lower
            Destroy(otherCircleCtrl.gameObject);
            // 2. 在留下的那個的相同位置生成一個 nextRankObj
            if (nextRankObj != null)
            {
                Instantiate(nextRankObj, this.transform.position, Quaternion.identity);
                Debug.Log("Instantiate nextRankObj");
            }

            // 移除物件的碰撞體
            circleCollider.enabled = false;

            // 將圖片透明度設置為 0（完全透明）
            Color color = spriteRenderer.color;
            color.a = 0f;  // alpha 值設為 0
            spriteRenderer.color = color;
            StartCoroutine(DestroyThis());
            if (scoreManager != null){
                scoreManager.AddScore(rank*5);
            }
        }

    }

    private IEnumerator DestroyThis()
    {
        // 等待音效播放的持續時間
        yield return new WaitForSeconds(audioSource.clip.length);

        // 3. 確認生成完以後，消除原本留下的那個
        Destroy(this.gameObject);
    }
}
