using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlaidType //枚举出格子的类型：可走、不可走
{
    walk,
    stop
}

public class AStarPlaid //A星寻路的格子管理脚本 （unity唐老师教程）
{
    //格子坐标
    public int x;
    public int y;

    //寻路消耗公式：f = g + h
    public float f; //消耗值
    public float g; //离起点的距离
    public float h; //离终点的距离
    public AStarPlaid father; //父对象
    public PlaidType type; //格子类型

    public AStarPlaid(int x,int y,PlaidType type)
    {
        this.x = x;
        this.y = y;
        this.type = type;
    }

}
