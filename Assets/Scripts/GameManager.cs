using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    /// <summary>
    /// a property for the towerBtn
    /// </summary>
    public TowerBtn ClickedBtn { get; set; }

    /// <summary>
    /// A reference to the currency text
    /// </summary>
    private int currency;

    private int wave = 0;

    public int Wave
    {
        get
        {
            return wave;
        }
        set
        {
            this.wave = value;

            waveTxt.text = string.Format("Wave: <color=lime>{0}</color>", value.ToString());
        }
    }

    private int lives;

    private int scores;

    private bool gameOver = false;

    private int health = 100;

    [SerializeField]
    private Text livesTxt;

    [SerializeField]
    private Text scoresTxt;

    [SerializeField]
    private Text waveTxt;

    [SerializeField]
    private Text currencyTxt;

    [SerializeField]
    private GameObject waveBtn;

    [SerializeField]
    private GameObject gameOverMenu;

    [SerializeField]
    private GameObject pauseMenu;

    [SerializeField]
    private GameObject upgradePanel;

    [SerializeField]
    private Text sellText;

    /// <summary>
    /// The current selected tower
    /// </summary>
    private Tower selectedTower;

    private List<Monster> activeMonsters = new List<Monster>();

    public List<Monster> ActiveMonsters
    {
        get
        {
            return activeMonsters;
        }
    }

    /// <summary>
    /// A property for the object pool
    /// </summary>
    public ObjectPool Pool { get; set; }

    public bool WaveActive
    {
        get {
            return activeMonsters.Count > 0;
        }
    }

    public void SetWaveButton(bool setting)
    {
        waveBtn.SetActive(setting);
    }

    /// <summary>
    /// Property for accessing the currency
    /// </summary>
    public int Currency
    {
        get
        {
            return currency;
        }

        set
        {
            this.currency = value;
            this.currencyTxt.text = value.ToString() + " <color=lime>$</color>";
        }
    }

    public int Lives
    {
        get
        {
            return lives;
        }
        set
        {
            this.lives = value;

            if (lives <= 0)
            {
                this.lives = 0;
                GameOver();
            }

            livesTxt.text = lives.ToString();
        }
    }

    public int Scores
    {
        get
        {
            return scores;
        }
        set
        {
            this.scores = value;

            scoresTxt.text = "Score: " + scores.ToString();
        }
    }

    private void Awake()
    {
        Pool = GetComponent<ObjectPool>();
    }

    // Use this for initialization
    void Start ()
    {
        Lives = 10;
        Currency = 50;
        Scores = 0;
	}
	
	// Update is called once per frame
	void Update () {

        HandleEscape();
	}

    /// <summary>
    /// Pick a tower then a buy button is pressed
    /// </summary>
    /// <param name="towerBtn">The clicked button</param>
    public void PickTower(TowerBtn towerBtn)
    {
        if (Currency >= towerBtn.Price && !WaveActive)
        {
            //Stores the clicked button
            this.ClickedBtn = towerBtn;
        }

 
    }

    /// <summary>
    /// Buys a tower
    /// </summary>
    public void BuyTower()
    {
        if (Currency >= ClickedBtn.Price)
        {
            Currency -= ClickedBtn.Price;

            GameManager.Instance.ClickedBtn = null;
        }
        
    }

    /// <summary>
    /// Selects a tower by clicking it
    /// </summary>
    /// <param name="tower">The clicked tower</param>
    public void SelectTower(Tower tower)
    {
        if (selectedTower != null)//If we have selected a tower
        {
            //Selects the tower
            selectedTower.Select();
        }

        //Sets the selected tower
        selectedTower = tower;

        //Selects the tower
        selectedTower.Select();        
    }

    /// <summary>
    /// Deselect the tower
    /// </summary>
    public void DeselectTower()
    {
        //If we have a selected tower
        if (selectedTower != null)
        {
            //Calls select to deselect it
            selectedTower.Select();
        }

        //Remove the reference to the tower
        selectedTower = null;

  
    }

    /// <summary>
    /// Handles escape presses
    /// </summary>
    private void HandleEscape()
    {
        if (Input.GetKeyDown(KeyCode.Escape))//if we press escape
        {
            GameManager.Instance.ClickedBtn = null;

            //ShowPauseMenu();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            ShowPauseMenu();
        }
    }

    public void StartWave()
    {
        wave++;

        waveTxt.text = string.Format("Wave: <color=lime>{0}</color>", wave);

        StartCoroutine(SpawnWave());

        waveBtn.SetActive(false);
    }

    /// <summary>
    /// Spawns a wave of monsters
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnWave()
    {
        //Generates the path
        LevelManager.Instance.GeneratePath();

        for (int i = 0; i < wave; i++)
        {
            int monterIndex = Random.Range(0, 4);

            string type = string.Empty;

            switch (monterIndex)
            {
                case 0:
                    type = "archer2";
                    break;
                case 1:
                    type = "knight";
                    break;
                case 2:
                    type = "mage_dark";
                    break;
                case 3:
                    type = "zombie";
                    break;
            }

            //Requests the monster form the pool
            Monster monster = Pool.GetObject(type).GetComponent<Monster>();

            monster.Spawn(health);

            if (wave % 3 == 0)
            {
                health += 5;
            }

            //Adds the monster to the activemonster list
            activeMonsters.Add(monster);

            yield return new WaitForSeconds(2.5f);
        }

    }

    /// <summary>
    /// Removes a monster from the game
    /// </summary>
    /// <param name="monster">Monster to remove</param>
    public void RemoveMonster(Monster monster)
    {
        //Removes the monster from the active list
        activeMonsters.Remove(monster);

        //IF we don't have more active monsters and the game isn't over, then we need to show the wave button
        if (!WaveActive && !gameOver)
        {
            //Shows the wave button
            waveBtn.SetActive(true);
        }
    }

    public void GameOver()
    {
        if (!gameOver)
        {
            gameOver = true;
            gameOverMenu.SetActive(true);
        }
    }

    public void Restart()
    {
        Time.timeScale = 1;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SaveAndQuitGame()
    {
        SaveManager.Instance.SaveGame();
        Application.Quit();
    }

    public void ShowPauseMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf); 

        if ( !pauseMenu.activeSelf)
        {
            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0;
        }
    }

    public void SellTower()
    {
        if (selectedTower != null)
        {
            Currency += selectedTower.Price / 2;

            selectedTower.GetComponentInParent<TileScript>().IsEmpty = true;

            Destroy(selectedTower.transform.parent.gameObject);

            DeselectTower();
        }
    }
}
