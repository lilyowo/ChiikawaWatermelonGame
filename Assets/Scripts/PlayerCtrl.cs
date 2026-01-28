using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    public float leftBound = -3f;
    public float rightBound = 3f;
    public List<GameObject> ballCandidates;
    public Transform spawnPoint;
    private Vector3 initialPosition;
    private Camera mainCamera;
    private bool canGenerate = true;
    private bool gameOver = false;
    
    // 新增：存儲預生成的物件
    private GameObject currentBall;
    private GameObject dropBall;
    private Rigidbody2D ballRigidbody;

    void Start()
    {
        mainCamera = Camera.main;
        initialPosition = transform.position;
        // 開始時就生成第一個球
        PreGenerateBall();
    }

    void Update()
    {
        MoveObjectWithMouse();
        
        // 修改：更新預生成球的位置
        if (currentBall != null)
        {
            currentBall.transform.position = GetAdjustedSpawnPosition();
        }

        if (Input.GetMouseButtonDown(0) && canGenerate && !gameOver)
        {
            Vector3 mousePos = GetMouseWorldPosition();
            // if (mousePos.x >= leftBound && mousePos.x <= rightBound){
                StartCoroutine(DropBallWithDelay());
            
        }

        
    }
    // 新增：獲取調整後的生成位置
    private Vector3 GetAdjustedSpawnPosition()
    {
        Vector3 mousePos = GetMouseWorldPosition();
        Vector3 spawnPos = spawnPoint.position;
        
        if (mousePos.x >= 0)
        {
            // 滑鼠在右側，球生成在左邊
            spawnPos.x -= 1f;
        }
        else
        {
            // 滑鼠在左側，球生成在右邊
            spawnPos.x += 1f;
        }
        
        return spawnPos;
    }

    // 新增：預先生成球的方法
    void PreGenerateBall()
    {
        if (ballCandidates.Count > 0)
        {
            int randomIndex = Random.Range(0, ballCandidates.Count);
            GameObject selectedBall = ballCandidates[randomIndex];
            
            // 使用調整後的生成位置
            Vector3 spawnPos = GetAdjustedSpawnPosition();
            currentBall = Instantiate(selectedBall, spawnPos, Quaternion.identity);
            
            ballRigidbody = currentBall.GetComponent<Rigidbody2D>();
            if (ballRigidbody != null)
            {
                ballRigidbody.gravityScale = 0;
                ballRigidbody.isKinematic = true;
            }
        }
    }

    // 修改：改為處理球的釋放
    private IEnumerator DropBallWithDelay()
    {
        canGenerate = false;
        dropBall = currentBall;
        currentBall = null;
        // 啟用當前球的重力
        if (ballRigidbody != null)
        {
            ballRigidbody.gravityScale = 1;
            ballRigidbody.isKinematic = false;
        }
        
        
        
        yield return new WaitForSeconds(0.5f);
        // 生成下一個球
        PreGenerateBall();
        canGenerate = true;
    }

    // 其他方法保持不變
    void MoveObjectWithMouse()
    {
        Vector3 mousePos = GetMouseWorldPosition();
        float clampedX = Mathf.Clamp(mousePos.x, leftBound, rightBound);
        transform.position = new Vector3(clampedX, initialPosition.y, initialPosition.z);
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(mainCamera.transform.position.z);
        return mainCamera.ScreenToWorldPoint(mousePos);
    }

    public void setGameOver()
    {
        gameOver = true;
    }

    public void Reset()
    {
        canGenerate = true;
        gameOver = false;
        
        // 銷毀當前預生成的球（如果存在）
        if (currentBall != null)
        {
            Destroy(currentBall);
        }
        
        DropObjCtrl[] dropObjects = FindObjectsOfType<DropObjCtrl>();
        foreach (DropObjCtrl dropObj in dropObjects)
        {
            dropObj.triggerTimer = 0.0f;
            dropObj.isTriggered = false;
        }
        
        // 重新生成新的球
        PreGenerateBall();
    }
}