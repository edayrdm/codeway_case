using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;


public class SaveManager : Singleton<SaveManager>
{
    [SerializeField]
    private Monster[] monsters;

    [SerializeField]
    private GameObject[] towers;

    private ObjectPool pool;
    // Use this for initialization
    void Awake()
    {
        pool = FindObjectOfType<ObjectPool>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Save();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Load();
        }
    }

    public void SaveGame()
    {
        Save();
    }

    public void LoadGame()
    {
        Load();
    }

    private void Save()
    {
        try
        {
            BinaryFormatter bf = new BinaryFormatter();

            FileStream file = File.Open(Application.persistentDataPath + "/" + "SaveTest.dat", FileMode.Create);

            SaveData data = new SaveData();
           
            SaveTowers(data);
            SaveMonsters(data);
            SavePlayer(data);

            bf.Serialize(file, data);

            file.Close();

        }
        catch (System.Exception)
        {
            //This is for handling errors
            throw;
        }
    }

    private void SavePlayer(SaveData data)
    {
        data.MyPlayerData = new PlayerData(GameManager.Instance.Wave,
            GameManager.Instance.Lives, GameManager.Instance.Scores,
            GameManager.Instance.Currency);
    }

    private void SaveMonsters(SaveData data)
    {
        monsters = GameObject.FindObjectsOfType<Monster>();
        for (int i = 0; i < monsters.Length; i++)
        {
            data.MyMonsterData.Add(new MonsterData(
                monsters[i].name,
                monsters[i].MyHealth, monsters[i].MyMaxHealth,
                monsters[i].transform.position,
                monsters[i].GridPosition.X, monsters[i].GridPosition.Y));      
        }
    }

    private void SaveTowers(SaveData data)
    {
        towers = GameObject.FindGameObjectsWithTag("Tower");
        for (int i = 0; i < towers.Length; i++)
        {
            data.MyTowerData.Add(new TowerData(
                towers[i].name,
                towers[i].transform.position,
                towers[i].transform.parent.gameObject.GetComponent<TileScript>().GridPosition
                ));
        }
    }



    private void Load()
    {
        try
        {
            BinaryFormatter bf = new BinaryFormatter();

            FileStream file = File.Open(Application.persistentDataPath + "/" + "SaveTest.dat", FileMode.Open);

            SaveData data = (SaveData)bf.Deserialize(file);

            file.Close();

            
            LoadTowers(data);
            LoadMonsters(data);
            LoadPlayer(data);

        }
        catch (System.Exception)
        {
            //This is for handling errors
            throw;
        }
    }

    private void LoadPlayer(SaveData data)
    {
        GameManager.Instance.Wave = data.MyPlayerData.MyWave;
        GameManager.Instance.Lives = data.MyPlayerData.MyLives;
        GameManager.Instance.Scores = data.MyPlayerData.MyScores;
        GameManager.Instance.Currency = data.MyPlayerData.MyCurrency;

        if(GameManager.Instance.ActiveMonsters.Count > 0)
        {
            Debug.Log("active monster");
            GameManager.Instance.SetWaveButton(false);
        }
        else
        {
            Debug.Log("no active monster");
        }
    }

    private void LoadMonsters(SaveData data)
    {
        
        foreach (MonsterData monster in data.MyMonsterData)
        {
            Monster tmp = pool.GetObject(monster.MyType).GetComponent<Monster>();

            tmp.SetMyMaxHealth(monster.MyMaxHealth);
            tmp.SetMyHealth(monster.MyHealth);
           
            tmp.transform.position = new Vector2(monster.MyX, monster.MyY);
            tmp.IsActive = true;

            Point redSpawn = new Point(11, 6);
            Point start = new Point(monster.MyGridX, monster.MyGridY);
            Stack<Node> fullPath = AStar.GetPath(start, redSpawn);
            tmp.SetPath(fullPath);

            //tmp.Initialize(monster.MyHealth, monster.MyMaxHealth);
            //Instantiate(tmp, tmp.transform.position, Quaternion.identity);

            GameManager.Instance.ActiveMonsters.Add(tmp);

        }

    }

    private void LoadTowers(SaveData data)
    {

        foreach (TowerData tower in data.MyTowerData)
        {
            GameObject tmp = pool.GetObject(tower.MyType);

            tmp.transform.position = new Vector2(tower.MyX, tower.MyY);
            tmp.GetComponent<SpriteRenderer>().sortingOrder = tower.MyParentGridY;

            TileScript trns = LevelManager.Instance.Tiles[new Point(tower.MyParentGridX, tower.MyParentGridY)].GetComponent<TileScript>();
            trns.IsEmpty = false;
            trns.Walkable = false;

            tmp.transform.SetParent(trns.transform);

        }

    }
}
