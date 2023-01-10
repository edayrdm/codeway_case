using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class SaveData
{
    public PlayerData MyPlayerData { get; set; }

    public List<MonsterData> MyMonsterData { get; set; }

    public List<TowerData> MyTowerData { get; set; }

    public SaveData()
    {
        MyMonsterData = new List<MonsterData>();

        MyTowerData = new List<TowerData>();
    }
}

[Serializable]
public class PlayerData
{
    public int MyWave { get; set; }

    public int MyLives { get; set; }

    public int MyScores { get; set; }

    public int MyCurrency { get; set; }

    public PlayerData(int wave, int lives, int scores, int currency)
    {
        this.MyWave = wave;
        this.MyLives = lives;
        this.MyScores = scores;
        this.MyCurrency = currency;
    }
}


[Serializable]
public class MonsterData
{

    public string MyType { get; set; }

    public float MyHealth { get; set; }

    public float MyMaxHealth { get; set; }

    public float MyX { get; set; }

    public float MyY { get; set; }

    public int MyGridX { get; set; }

    public int MyGridY { get; set; }

    public  List<Node> MyPath { get; set; }

    public  MonsterData( string type, float health, float maxHealth, Vector2 position,int gridX, int gridY)
    {

        this.MyType = type;
        this.MyMaxHealth = maxHealth;
        this.MyHealth = health;
        this.MyX = position.x;
        this.MyY = position.y;
        this.MyGridX = gridX;
        this.MyGridY = gridY;
    }
}


[Serializable]
public class TowerData
{

    public string MyType { get; set; }

    public float MyX { get; set; }

    public float MyY { get; set; }

    public int MyParentGridX { get; set; }

    public int MyParentGridY { get; set; }

    public TowerData(string type, Vector2 position, Point parentGrid)
    {
        this.MyType = type;
        this.MyX = position.x;
        this.MyY = position.y;
        this.MyParentGridX = parentGrid.X;
        this.MyParentGridY = parentGrid.Y;
    }
}