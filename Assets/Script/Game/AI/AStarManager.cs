using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class AStarManager
{
    // 实现单例模式
    private static AStarManager instance;
    public static AStarManager Instance
    {
        get
        {
            if (instance == null)
                instance = new AStarManager();
            return instance;
        }
    }

    private int gridRadius; // 检测半径（可调整）
    private int localGridSize;   // 网格大小（以AI为中心的检测区域）
    public AStarPlaid[,] localGrid; // 局部网格（不再是整个地图）
    private List<AStarPlaid> openList;
    private List<AStarPlaid> closeList;

    private AStarManager()
    {
        openList = new List<AStarPlaid>();
        closeList = new List<AStarPlaid>();
    }

    // 初始化网格地图（AI创建时调用）
    public void InitLocalGrid(int radius)
    {
        gridRadius = radius;
        localGridSize = gridRadius * 2 + 1;
        localGrid = new AStarPlaid[localGridSize, localGridSize]; //存储了一个边为localGridSize的正方形数组，用作AI的检测网格地图
    }

    // 随AI移动更新网格地图
    public void UpdateLocalGrid(Vector3 aiWorldPos, float gridSize = 1f)
    {
        // 计算AI所在的网格坐标,Mathf.RoundToInt:四舍五入取整
        int aiGridX = Mathf.RoundToInt(aiWorldPos.x / gridSize);
        int aiGridY = Mathf.RoundToInt(aiWorldPos.z / gridSize);
        
        // 计算局部网格的起始世界坐标
        int startWorldX = aiGridX - gridRadius;
        int startWorldY = aiGridY - gridRadius;

        // 生成以AI为中心的局部网格
        for (int x = 0; x < localGridSize; x++)
        {
            for (int y = 0; y < localGridSize; y++)
            {
                // 计算当前格子的世界网格坐标
                int worldGridX = startWorldX + x;
                int worldGridY = startWorldY + y;
                
                // 检查这个位置是否可行走（这里简单判断，你可以扩展）
                bool walkable = true;
                
                // 创建格子时记录局部坐标和世界坐标
                if (localGrid[x, y] == null)
                {
                    localGrid[x, y] = new AStarPlaid(x, y, PlaidType.walk);
                }
                else
                {
                    localGrid[x, y].x = x;
                    localGrid[x, y].y = y;
                    localGrid[x, y].type = walkable ? PlaidType.walk : PlaidType.stop;
                }
            }
        }

        MarkAllWalls(aiWorldPos, gridSize);
    }

    public AStarPlaid WorldToLocalGrid(int worldGridX, int worldGridY, Vector3 aiWorldPos, float gridSize = 1f)
    {
        // 计算AI所在的网格坐标
        int aiGridX = Mathf.RoundToInt(aiWorldPos.x / gridSize);
        int aiGridY = Mathf.RoundToInt(aiWorldPos.z / gridSize);
        
        // 计算相对于AI的偏移
        int offsetX = worldGridX - aiGridX;
        int offsetY = worldGridY - aiGridY;
        
        // 转换为局部网格索引
        int localX = offsetX + gridRadius;
        int localY = offsetY + gridRadius;
        
        // 检查是否在局部网格范围内
        if (localX >= 0 && localX < localGridSize && localY >= 0 && localY < localGridSize)
        {
            return localGrid[localX, localY];
        }
        
        return null; // 不在局部网格内
    }

    public bool IsPlayerInRange(Vector3 aiWorldPos, Vector3 playerWorldPos, float gridSize = 1f)
    {
        // 计算网格距离
        int aiGridX = Mathf.RoundToInt(aiWorldPos.x / gridSize);
        int aiGridY = Mathf.RoundToInt(aiWorldPos.z / gridSize);
        
        int playerGridX = Mathf.RoundToInt(playerWorldPos.x / gridSize);
        int playerGridY = Mathf.RoundToInt(playerWorldPos.z / gridSize);
        
        // 计算曼哈顿距离
        int distanceX = Mathf.Abs(playerGridX - aiGridX);
        int distanceY = Mathf.Abs(playerGridY - aiGridY);
        
        // 返回玩家是否在检测区域内的布尔值
        return distanceX <= gridRadius && distanceY <= gridRadius;
    }

    public List<AStarPlaid> FindPath(Vector3 aiWorldPos, Vector3 playerWorldPos, float gridSize = 1f)
    {
        UpdateLocalGrid(aiWorldPos, gridSize); // 更新网格地图

        if (!IsPlayerInRange(aiWorldPos, playerWorldPos, gridSize)) // 检查玩家是否在范围内
            return null;
        
        // 将AI和玩家的世界坐标转换为网格坐标
        int aiGridX = Mathf.RoundToInt(aiWorldPos.x / gridSize);
        int aiGridY = Mathf.RoundToInt(aiWorldPos.z / gridSize);
        
        int playerGridX = Mathf.RoundToInt(playerWorldPos.x / gridSize);
        int playerGridY = Mathf.RoundToInt(playerWorldPos.z / gridSize);
        
        // 转换成网格地图中的起点（AI）和终点（玩家）
        AStarPlaid start = WorldToLocalGrid(aiGridX, aiGridY, aiWorldPos, gridSize);
        AStarPlaid end = WorldToLocalGrid(playerGridX, playerGridY, aiWorldPos, gridSize);
        
        if (start == null || end == null || end.type == PlaidType.stop)
            return null;
        
        // 找到路径
        return FindPathInLocalGrid(start, end);
    }

    private List<AStarPlaid> FindPathInLocalGrid(AStarPlaid start, AStarPlaid end)
    {
        // 清空开启、关闭列表
        openList.Clear();
        closeList.Clear();

        // 初始化起点
        start.father = null;
        start.f = 0;
        start.g = 0;
        start.h = 0;
        closeList.Add(start);

        while (true)
        {
            // 将周围相邻的八个格子进行寻路公式计算
            FindNearToOpenListLocal(start.x - 1, start.y - 1, 1.4f, start, end);
            FindNearToOpenListLocal(start.x, start.y - 1, 1f, start, end);
            FindNearToOpenListLocal(start.x + 1, start.y - 1, 1.4f, start, end);
            FindNearToOpenListLocal(start.x - 1, start.y, 1f, start, end);
            FindNearToOpenListLocal(start.x + 1, start.y, 1f, start, end);
            FindNearToOpenListLocal(start.x - 1, start.y + 1, 1.4f, start, end);
            FindNearToOpenListLocal(start.x, start.y + 1, 1f, start, end);
            FindNearToOpenListLocal(start.x + 1, start.y + 1, 1.4f, start, end);

            if (openList.Count == 0) return null; //说明没有格子能加入开启列表，即周围全是不可通行的路

            // 按自己定义的规则（SortOpenList）排序
            openList.Sort(SortOpenList);

            // 将f最小的加入关闭列表
            closeList.Add(openList[0]);
            start = openList[0];
            openList.RemoveAt(0);

            if (start == end)
            {
                Debug.Log("找到玩家啦！");
                List<AStarPlaid> path = new List<AStarPlaid>();
                path.Add(end);
                while (end.father != null)
                {
                    path.Add(end.father);
                    end = end.father;
                }
                path.Reverse();
                return path;
            }
        }
    }
    private void FindNearToOpenListLocal(int x, int y, float g, AStarPlaid father, AStarPlaid end)
    {
        // 判断是否在局部网格范围内
        if (x < 0 || x >= localGridSize || y < 0 || y >= localGridSize)
            return;
            
        AStarPlaid node = localGrid[x, y];

        //判断要检查格子是否是空的（边界）？、不可走的区域（手动设置：场景中的障碍物）、已经存进过开启列表、关闭列表
        if (node == null || node.type == PlaidType.stop || closeList.Contains(node) || openList.Contains(node))
            return;

        // 计算寻路消耗
        node.father = father;
        node.g = father.g + g;
        node.h = Mathf.Abs(end.x - node.x) + Mathf.Abs(end.y - node.y);
        node.f = node.g + node.h;

        openList.Add(node);
    }

    private int SortOpenList(AStarPlaid a, AStarPlaid b) //排序，返回1则排后面，即a.f>b.f时，返回1，所以a排后面，让f值小的b排前面，方便列表快速取出
    {
        if (a.f > b.f)
            return 1;
        else if (a.f < b.f)
            return -1;
        else
            return 0;
    }

    //检测墙体并将墙体所占的格子设为不可通行
    //存在bug：
    // 只计算墙体占地面积会导致AI绕过墙体拐角时回穿模；
    // (可设为玩法创意或修复)将墙体格子面积多加1格，玩家进入这1格内会被AI检测属于不可通行目的地，不会继续寻路移动
    public void MarkWallAreaAsStop(Vector3 wallWorldPos, Vector3 wallScale, Vector3 aiWorldPos, float gridSize = 1f)
    {
        //计算墙体实际尺寸和占据网格数量
        float wallRealWidth = wallScale.x;
        float wallRealDepth = wallScale.z;
        int gridCountX = Mathf.CeilToInt(wallRealWidth / gridSize);
        int gridCountZ = Mathf.CeilToInt(wallRealDepth / gridSize);

        // 将网格面积多加1（左右/前后各扩1个，即X、Y各总加2）
        int expandGridCountX = gridCountX + 2;
        int expandGridCountZ = gridCountZ + 2;

        //墙体中心点世界网格坐标
        int wallCenterGridX = Mathf.RoundToInt(wallWorldPos.x / gridSize);
        int wallCenterGridZ = Mathf.RoundToInt(wallWorldPos.z / gridSize);

        int startGridX = wallCenterGridX - Mathf.FloorToInt(gridCountX / 2f) - 1;
        int startGridZ = wallCenterGridZ - Mathf.FloorToInt(gridCountZ / 2f) - 1;

        //遍历所有网格，标记为不可通行
        for (int x = 0; x < expandGridCountX; x++)
        {
            for (int z = 0; z < expandGridCountZ; z++)
            {
                int currentWallGridX = startGridX + x;
                int currentWallGridZ = startGridZ + z;

                // 转换为AI局部网格并标记为不可通行
                AStarPlaid localNode = WorldToLocalGrid(currentWallGridX, currentWallGridZ, aiWorldPos, gridSize);
                if (localNode != null)
                {
                    localNode.type = PlaidType.stop;
                }
            }
        }
    }
    
    //在更新AI移动更新网格时调用
    private void MarkAllWalls(Vector3 aiWorldPos, float gridSize)
    {
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        foreach (GameObject wall in walls)
        {
            Vector3 wallWorldPos = wall.transform.position;
            Vector3 wallScale = wall.transform.lossyScale;

            // 调用批量标记方法，标记墙体所有占据网格
            MarkWallAreaAsStop(wallWorldPos, wallScale, aiWorldPos, gridSize);
        }
    }
}