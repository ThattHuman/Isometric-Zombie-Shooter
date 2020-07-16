using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary> Manage game activities </summary>
public class Scenario : MonoBehaviour, IAgentObserver, IPlayerObserver
{
    [SerializeField] private GameObject playerGO = null;                // need to set up observer
    [SerializeField] private GameObject enemySpawnerGO = null;          // handle spawn proccesses

    [SerializeField] private int waveCount = 3;                         //
    [SerializeField] private int enemiesPerWave = 20;                   //
    [SerializeField] private int enemiesWaveIncrement = 0;              //
    [SerializeField] private int enemiesAtTime = 5;                     // Stage Settings
    [SerializeField] private int enemiesAtTimeIncrement = 0;            //
    [SerializeField] private float spawnCooldown = 2;                   //
    [SerializeField] private float spawnCooldownIncrement = 0;          //

    [SerializeField] private GameObject UIEnemyCount = null;            //
    [SerializeField] private GameObject UIAnnouncement = null;          //  UI Object's links
    [SerializeField] private GameObject UIPauseMenu = null;             //

    [SerializeField] private bool useDiffculiyPresetAtStart = false;    // if true - use preset, if false - use Inspector in Unity Editor
    [SerializeField] private Difficulty difficultyContainer = null;     // link to containte with difficulty index

    private AgentSpawner enemySpawner;

    private int currentWave = 0;            //
    private int enemiesLeftInWave = 0;      // Counters
    private int enemiesToSpawn = 0;         //
    private int currentEnemiesAtTime = 0;   //

    private float announceTime = 2f;

    private bool isReadyToSpawn = false;    // prevents spawn while in cooldown
    private bool isWave = false;            // prevents from end wave while next is dont start
    private bool isGameOver = false;        // prevents 

    private Text enemyCountText = null;     // UI elements
    private Text announcementText = null;   //

    #region Properties
        public int EnemiesAtTime
        {
            get
            {
                return enemiesAtTime;
            }
        }
        public float SpawnCooldown
        {
            get
            {
                return spawnCooldown;
            }
        }
    #endregion

    private void Awake()
    {
        enemySpawner = enemySpawnerGO.GetComponent<AgentSpawner>();
        enemySpawner.SetUpScenarioObserver(this);

        playerGO?.GetComponent<Player>().SetScenarioObserver(this);     
        
        enemyCountText = UIEnemyCount.GetComponent<Text>();
        announcementText = UIAnnouncement.GetComponent<Text>();

        announcementText.text = "";
    }

    private void Start()
    {
        // set up Stage Settings via Difficulty Container if activated
        if(useDiffculiyPresetAtStart && difficultyContainer != null)
        {
            PrepareWaveViaDifficultyPreset(difficultyContainer.DifficultyIndex);
        }

        // set up counters
        enemiesLeftInWave = enemiesPerWave;
        enemiesToSpawn = enemiesPerWave;

        StartCoroutine(NextWave());
    }

    // checking for spawn, end of wave and pause toggle; updates UI
    private void Update()
    {
        if(isReadyToSpawn && currentEnemiesAtTime < enemiesAtTime && enemiesToSpawn > 0)    // If not in cooldown, enemies less than may be at a time 
        {                                                                                   // and there are some enemies to spawn in wave
            enemySpawner.SpawnAgent();
            isReadyToSpawn = false;                 // prevents spawn while in cooldown
            enemiesToSpawn--;
            StartCoroutine(WaitSpawnCooldown());
        }

        if(isWave && enemiesLeftInWave <= 0)        // prevents from end wave while next is dont start; also check left enemies
        {
            EndWave();
        }

        UIUpdate();

        if(Input.GetButtonDown("Cancel") && !isGameOver)    // by default it's "Esc", can't be pressed when Game Over (defeat)
        {
            TogglePause();
        }
    }

    /// <summary> Performs the necessary actions when agent is removed </summary>
    public void AgentRemoved()
    {
        currentEnemiesAtTime--;
        enemiesLeftInWave--;
    }

    /// <summary> Performs the necessary actions when agent is activated </summary>
    public void AgentActivated()
    {
        currentEnemiesAtTime++;
    }

    /// <summary> Performs the necessary actions when player is died </summary>
    public void PlayerDied()
    {
        GameOver(false);
    }

    /// <summary> Handle spawn cooldown </summary>
    public IEnumerator WaitSpawnCooldown()
    {
        yield return new WaitForSeconds(spawnCooldown);
        isReadyToSpawn = true;
    }

    /// <summary> Set up Stage Settings with index from difficulty container </summary>
    private void PrepareWaveViaDifficultyPreset(int difficulty)
    {
        switch(difficulty)
        {
            case 0: // Easy
            {
                enemiesPerWave = 10;
                enemiesWaveIncrement = 2;
                enemiesAtTime = 4;
                enemiesAtTimeIncrement = 1;
                spawnCooldown = 1f;
                spawnCooldownIncrement = -0.1f;
            } break;
            case 1: // Medium
            {
                enemiesPerWave = 18;
                enemiesWaveIncrement = 4;
                enemiesAtTime = 5;
                enemiesAtTimeIncrement = 1;
                spawnCooldown = 1f;
                spawnCooldownIncrement = -0.2f;
            } break;
            case 2: // Hard
            {
                enemiesPerWave = 20;
                enemiesWaveIncrement = 10;
                enemiesAtTime = 8;
                enemiesAtTimeIncrement = 2;
                spawnCooldown = 0.5f;
                spawnCooldownIncrement = 0f;
            } break;
            default:
            {} break;
        }
    }

    /// <summary> Prepare settings to the next wave </summary>
    public void EndWave()
    {
        isWave = false;
        isReadyToSpawn = false;
        if(currentWave + 1 > waveCount) // stop the game 
        {
            GameOver(true);             // Victory!
            return;
        }

        // increase difficulty with increments
        enemiesPerWave += enemiesWaveIncrement;
        enemiesAtTime += enemiesAtTimeIncrement;
        spawnCooldown += spawnCooldownIncrement;
        StartCoroutine(NextWave());
    }

    /// <summary> Make break before next wave </summary>
    private IEnumerator NextWave()
    {
        currentWave++;
        announcementText.text = "Wave " + currentWave;

        yield return new WaitForSeconds(announceTime);

        announcementText.text = "";
        enemiesLeftInWave = enemiesPerWave;
        enemiesToSpawn = enemiesPerWave;
        isReadyToSpawn = true;
        isWave = true;
    }

    /// <summary> Endgame actions </summary>
    private void GameOver(bool isWin)
    {
        TogglePause();
        if(isWin)
        {
            announcementText.text = "Victory!\nPress \"Esc\"";
        }
        else
        {
            announcementText.text = "Defeat!";
            isGameOver = true;
        }
    }

    /// <summary> Update UI elements that linked to Scenario </summary>
    private void UIUpdate()
    {
        enemyCountText.text = "Enemies left: " + enemiesLeftInWave.ToString();
    }

    /// <summary> Toggle pause menu </summary>
    private void TogglePause()
    {
        if(!isGameOver)
        {
            if(Time.timeScale == 1)
            {
                Time.timeScale = 0;
                UIPauseMenu.SetActive(true);
            }
            else
            {
                Time.timeScale = 1;
                UIPauseMenu.SetActive(false);
            }
        }
    }

    /// <summary> Restart game </summary>
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1; // set normal time scale
    }
    
    /// <summary> Exit from builded game </summary>
    public void ExitGame()
    {
        Application.Quit();
    }
}

/// <summary> Can handle feedback from agent </summary>
public interface IAgentObserver
{
    void AgentRemoved();
    void AgentActivated();
}

/// <summary> Can handle feedback from player </summary>
public interface IPlayerObserver
{
    void PlayerDied();
}
