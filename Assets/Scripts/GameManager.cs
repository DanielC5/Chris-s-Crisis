using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    public enum GameState { None, Start, Loading, Playing, Died, Respawning, FinishedHour, FinishedNight, GameOver, };
    [SerializeField] private GameState currentState = GameState.None;

    [SerializeField] private GameObject startInputHandler;
    private InputActionMap startActionMap;

    [SerializeField] private GameObject Player;
    public bool playerIsImmune;

    [Header("Score")]
    [SerializeField] private int score;
    [SerializeField] private int hiscore;

    [Header("Score UI")]
    [SerializeField] private TextMeshProUGUI currentScoreText;
    [SerializeField] private TextMeshProUGUI hiScoreText;

    [Header("Player Health")]
    [SerializeField] private int playerLives;
    [SerializeField] private int startPlayerLives;
    [SerializeField] private int maxPlayerLives;

    [Header("Player Health UI")]
    [SerializeField] private GameObject playerLivesUI;
    [SerializeField] private GameObject playerLifeIcon;
    [SerializeField] private List<GameObject> playerLifeIcons;

    [Header("Game State UI")]
    [SerializeField] private TextMeshProUGUI centerBlurbText;
    public bool endedWave = true;
    public bool endedHour = true;

    [Header("In Game Info")]
    public int wavesLeft;
    public int currentHour;

    // [Header("Sound")]
    // [SerializeField] public SoundManager soundManager;

    void Awake()
    {
        if (Instance)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        
        StartSetup();
    }

    #region Utilities

    void StartSetup()
    {
        startActionMap = startInputHandler.GetComponent<PlayerInput>().actions.FindActionMap("Start");

        hiscore = PlayerPrefs.GetInt("HiScore", hiscore);

        PlayerLives = startPlayerLives;
        SetupLivesUI();

        // setup game state
        SetState(GameState.Start);
    }

    public void SetPlayer(GameObject player)
    {
        Player = player;
    }

    #endregion

    #region UI

    public void addScore(int pointsGiven)
    {
        score += pointsGiven;
        UpdateScoreUI();
    }

    public void UpdateScoreUI()
    {
        // update scores
        string currentScoreTitle = $"SCORE\n{score.ToString().PadLeft(5, '0')}";
        currentScoreText.text = currentScoreTitle;

        if (score > hiscore)
        {
            hiscore = score;
            PlayerPrefs.SetInt("HiScore", hiscore);
            PlayerPrefs.Save();
        }

        string hiScoreTitle = $"HI-SCORE\n{hiscore.ToString().PadLeft(5, '0')}";
        hiScoreText.text = hiScoreTitle;
    }

    public void SetupLivesUI()
    {
        playerLifeIcons = new List<GameObject> { playerLifeIcon };

        Vector3 newIconPos = playerLifeIcon.transform.position;
        for (int i = 1; i < maxPlayerLives; i++)
        {
            GameObject newIcon = Instantiate(playerLifeIcon);
            newIcon.transform.SetParent(playerLivesUI.transform);

            newIconPos.x += 0.75f;
            newIcon.transform.position = newIconPos;

            playerLifeIcons.Add(newIcon);
        }
    }

    public void UpdateLivesUI()
    {
        for (int i = 0; i < maxPlayerLives; i++)
        {
            playerLifeIcons[i].SetActive(i < PlayerLives);
        }
    }

    public void SetInactiveLivesUI()
    {
        for (int i=0; i<maxPlayerLives; i++)
        {
            playerLifeIcons[i].SetActive(false);
        }
    }

    #endregion

    #region Managing State

    public GameState CurrentState
    {
        get => currentState;
        private set
        {
            Debug.Log(currentState + " " + value);
            if (currentState == value) return;
            currentState = value;

            switch (currentState)
            {
                case GameState.Start:
                    ProcessStart();
                    break;

                case GameState.Loading:
                    ProcessLoading();
                    break;

                case GameState.Playing:
                    ProcessPlaying();
                    StartCoroutine(HourNightPopup());
                    break;

                case GameState.Died:
                    StartCoroutine(ProcessDeath());
                    break;

                case GameState.Respawning:
                    StartCoroutine(ProcessRespawning());
                    break;

                case GameState.FinishedHour:
                    ProcessFinishedHour();
                    break;

                case GameState.FinishedNight:
                    StartCoroutine(ProcessFinishedNight());
                    break;

                case GameState.GameOver:
                    StartCoroutine(ProcessGameOver());
                    break;
            }
        }
    }

    public void SetState(GameState state)
    {
        CurrentState = state;
    }

    #region State Processes

    public IEnumerator HourNightPopup()
    {
        
        centerBlurbText.fontSize = 45;

        if (endedHour)
        {
            centerBlurbText.text = "Hour " + currentHour;
            centerBlurbText.gameObject.SetActive(true);
            yield return new WaitForSeconds(3f);
            centerBlurbText.gameObject.SetActive(false);

            yield return new WaitForSeconds(0.5f);
        }

        endedHour = false;

        if (endedWave)
        {
            centerBlurbText.text = wavesLeft.ToString();
            if (SceneManager.GetActiveScene().name == "3_game")
            {
                centerBlurbText.text += "0 Minutes Remaining";
            }
            else
            {
                if (wavesLeft == 1) centerBlurbText.text += " Wave Remaining";
                else centerBlurbText.text += " Waves Remaining";
            }
            centerBlurbText.gameObject.SetActive(true);
            yield return new WaitForSeconds(3f);
            centerBlurbText.gameObject.SetActive(false);
        }

        endedWave = false;
    }

    public void ProcessStart()
    {
        // enable player input for entering game
        startInputHandler.SetActive(true);
        startInputHandler.GetComponent<StartInputHandler>().SetupStartScene();
        startActionMap.Enable();

        // enable relevant UI
        centerBlurbText.fontSize = 30;
        centerBlurbText.text = "";

        // reset score
        score = 0;
        UpdateScoreUI();

        // reset player lives
        PlayerLives = startPlayerLives;
        UseLife();
        SetInactiveLivesUI();

        // reset stage + level
        wavesLeft = 3;
        currentHour = 1;
        endedWave = true;
        endedHour = true;
    }

    public void ProcessLoading()
    {
        // disable player input for entering game
        startActionMap.Disable();

        centerBlurbText.text = "loading...\nUse Arrow Keys to Move and Click to Attack";
    }

    public void ProcessPlaying()
    {
        playerIsImmune = false;

        // disable start, loading displays
        startInputHandler.SetActive(false);
        centerBlurbText.gameObject.SetActive(false);

        // show player lives
        UpdateLivesUI();
    }

    public IEnumerator ProcessDeath()
    {
        // --> pause all movement

        // disable components
        Player.GetComponent<PlayerController>().enabled = false;
        Player.GetComponent<PlayerInput>().enabled = false;

        // --> add animation + sound
        Player.GetComponentInChildren<AnimationController>().TriggerDiedAnimation();
        SoundManager.Instance.DeathSFX();

        yield return new WaitForSeconds(1f);

        Player.SetActive(false);

        yield return new WaitForSeconds(1f);

        SetState((PlayerLives == 0) ? GameState.GameOver : GameState.Respawning);
    }

    public IEnumerator ProcessRespawning()
    {
        // add UI
        centerBlurbText.fontSize = 45;
        centerBlurbText.text = "Ready";
        centerBlurbText.gameObject.SetActive(true);

        // sound
        SoundManager.Instance.RespawnSFX();

        UseLife();

        Player.GetComponent<PolarTransform>().angleDeg = -90;
        Player.GetComponent<PlayerController>().rotateSpeed = 0; //stop speed
        
        // reenable components
        Player.GetComponent<PlayerController>().enabled = true;
        Player.GetComponent<PlayerInput>().enabled = true;

        Player.SetActive(true);
        // animation
        Player.GetComponentInChildren<AnimationController>().TriggerRespawnAnimation();

        Player.GetComponent<PlayerController>().rotateSpeed = 180;  //reset speed

        // immune time
        playerIsImmune = true;
        yield return new WaitForSeconds(3f);
        playerIsImmune = false;

        Debug.Log("no immunity anymore");

        centerBlurbText.gameObject.SetActive(false);

        SetState(GameState.Playing);
    }

    public void ProcessFinishedHour()
    {
        wavesLeft--;

        Debug.Log(wavesLeft);

        //GameObject.Find("Background").GetComponent<BackgroundController>().gameEnd();

        // --> add UI
        // centerBlurbText.fontSize = 45;
        // centerBlurbText.text = "Wave Finished";
        // centerBlurbText.gameObject.SetActive(true);

        //StartCoroutine(PopoutHours());

        // play animation
    }

    public IEnumerator ProcessFinishedNight()
    {
        GameObject.Find("Background").GetComponent<BackgroundController>().gameEnd();

        centerBlurbText.fontSize = 45;
        centerBlurbText.text = "END OF HOUR " + currentHour;
        centerBlurbText.gameObject.SetActive(true);

        yield return new WaitForSeconds(3f);

        centerBlurbText.gameObject.SetActive(false);
        
        wavesLeft = 3;
        currentHour++;

        //StartCoroutine(PopoutNight());

        // play animation??
    }

    public IEnumerator ProcessGameOver()
    {
        SceneManager.LoadScene("EndScene");
        // --> add UI
        centerBlurbText.fontSize = 45;
        centerBlurbText.text = "Game Over";
        centerBlurbText.gameObject.SetActive(true);

        // --> add sound
        SoundManager.Instance.NotPlayingSnapshot();
        SoundManager.Instance.GameOverSFX();

        yield return new WaitForSeconds(7.2f);

        SetState(GameState.Start);
        SceneManager.LoadScene("0_title");
    }
    
    #endregion

    #endregion

    #region Player Lives

    public int PlayerLives
    {
        get => playerLives;
        private set
        {
            playerLives = Mathf.Clamp(value, 0, maxPlayerLives);
        }
    }

    public void GainLife()
    {
        PlayerLives++;
        UpdateLivesUI();
    }

    public void UseLife()
    {
        PlayerLives--;
        UpdateLivesUI();
    }

    #endregion
}
