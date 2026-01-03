using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{   
    public float gridSize = 1f; // 网格大小
    public float speed = 2f;
    public int detectionRadius = 10; // 检测半径（格子数）
    
    private GameObject player;
    private List<AStarPlaid> currentPath;
    private int currentTargetIndex;
    private float updateTimer = 0f;
    private float updateInterval = 0.3f; // 更新寻路的间隔

    void Awake()
    {   
        player = GameObject.FindGameObjectWithTag("Player");
        
        AStarManager.Instance.InitLocalGrid(detectionRadius);
    }

    void Update()
    {   
        updateTimer += Time.deltaTime;       //0.3s开始一次寻路（防止每帧都寻路，优化性能）
        if (updateTimer >= updateInterval)
        {
            updateTimer = 0f;        
            MoveByAStar();
        }
        
        // 根据寻路算法得出的路径开始向玩家移动
        if (currentPath != null && currentTargetIndex < currentPath.Count)
        {
            MoveAlongPath();
        }
    }

    private void MoveByAStar()
    {
        Vector3 aiWorldPos = transform.position;
        Vector3 playerWorldPos = player.transform.position;
        
        currentPath = AStarManager.Instance.FindPath(aiWorldPos, playerWorldPos, gridSize);
        currentTargetIndex = 0;
     
    }

    private void MoveAlongPath()
    {
        if (currentPath == null || currentPath.Count == 0)
            return;     
        
        AStarPlaid currentTargetNode = currentPath[currentTargetIndex];
        Vector3 targetWorldPos = GridToWorld(currentTargetNode, transform.position);

        // 向目标点移动
        transform.position = Vector3.MoveTowards(
            transform.position, 
            targetWorldPos, 
            speed * Time.deltaTime
        );

        // 判断是否到下一个路径点
        float distanceToTarget = Vector3.Distance(transform.position, targetWorldPos);
        if (distanceToTarget <= gridSize / 2)
        {
            currentTargetIndex++;
        }

    }

    private Vector3 GridToWorld(AStarPlaid gridNode, Vector3 aiWorldPos)
    {
        // 计算AI的网格坐标
        int aiGridX = Mathf.RoundToInt(aiWorldPos.x / gridSize);
        int aiGridY = Mathf.RoundToInt(aiWorldPos.z / gridSize);
        
        // 计算局部坐标对应的世界网格坐标
        int worldGridX = aiGridX + (gridNode.x - detectionRadius);
        int worldGridY = aiGridY + (gridNode.y - detectionRadius);
        
        // 转换为世界坐标
        return new Vector3(
            worldGridX * gridSize,
            transform.position.y,
            worldGridY * gridSize
        );
    }
    
    // 调试绘制
    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        
        // 绘制检测范围
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawWireSphere(transform.position, detectionRadius * gridSize);
        
        // 绘制路径
        if (currentPath != null)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < currentPath.Count - 1; i++)
            {
                Vector3 pos1 = GridToWorld(currentPath[i], transform.position);
                Vector3 pos2 = GridToWorld(currentPath[i + 1], transform.position);
                Gizmos.DrawLine(pos1, pos2);
            }
        }
    }
}