using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;


/// <summary>
/// n 个敌人, 持续时长
/// </summary>
[System.Serializable]
public class Wave
{
    public List<GameObject> ships = new List<GameObject>();
    public float duration = 5f;
    public float spawnInterval = 0.5f;
}

/// <summary>
/// 波次, 槽位, FIFO, 呼吸环, 自转
/// </summary>
public class InGameManager : MonoBehaviour
{
    public static InGameManager Instance { get; private set; }

    public string nextScene;
    
    public static float RingOffsetDeg { get; private set; } = 0f;
    public static float RingRadius { get; private set; } = 3f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); return;
        }
        Instance = this;
        slotOccupied = new bool[stackCapacity];
        RingRadius = ringRadiusBase;

        BuildStage();
    }
    
    [Header("Ring Settings")]
    public float ringRadiusBase = 3f;
    public float ringRadiusAmplitude = 0.5f;
    public float ringBreathFrequency = 0.2f;
    public float ringRotateSpeedDeg = 30f;
    
    [Header("Slot Capacity")]
    [Min(1)] public int stackCapacity = 20;
    private readonly Queue<GameObject> fifo = new();
    private bool[] slotOccupied;
    private readonly Dictionary<GameObject, int> shipSlotMap = new();
    
    [Header("Wave Setting")]
    public List<Wave> currentWaves = new();
    private int waveIndex = 0;
    private float waveTimer = 0f;

    [Header("Wave Building")]
    public GameObject oakPrefab;
    public GameObject evergreenPrefab;

    public List<List<Wave>> stages;
    private int stageIndex = 0;
    
    [Header("Leaving Time Interval")]
    public float waitTime = 4f;
    private float waitTimer = 0f;

    [Header("Wave Indicators")]
    public GameObject[] waves;
    
    float breathPhase = 0f; // Phase

    bool waveFinished;

    int wavesPassed = 0;

    bool nightFinished = false;

    [Header("Boss Level Stuff")]
    public bool onBossLevel = false;
    public GameObject boss;

    void Update()
    {
        if (nightFinished) return;

        if (!waveFinished)
        {
            breathPhase += 2f * Mathf.PI * ringBreathFrequency * Time.deltaTime;
            RingRadius = ringRadiusBase + Mathf.Sin(breathPhase) * ringRadiusAmplitude;
            RingOffsetDeg = (RingOffsetDeg + ringRotateSpeedDeg * Time.deltaTime) % 360f;

            HandleAutoDequeue();
            HandleWaveTimer();
            TryLaunchNextWave();
        }

        // once all waves are released & DEFEATED
        if (waveIndex > 2 && shipSlotMap.Count == 0 && wavesPassed > 2)
        {
            HandleHourFinish();
        }
    }

    void BuildStage()
    {
        stages = new List<List<Wave>>();

        for (int i = 0; i < 3; i++)
        {
            stages.Add(new List<Wave>());
            for (int j = 0; j < 3; j++)
            {
                stages[i].Add(new Wave());
                for (int k = 0; k < 7; k++) stages[i][j].ships.Add(k % 2 == 0 ? oakPrefab : evergreenPrefab);
                stages[i][j].duration = Random.Range(4, 7);
                stages[i][j].spawnInterval = Random.Range(0.1f, 0.3f);
            }
        }

        stageIndex = 0;
        currentWaves = stages[stageIndex];
        Debug.Log(stageIndex + " " + waves.Length);
        if (!onBossLevel) waves[stageIndex].SetActive(true);
    }

    void HandleHourFinish()
    {
        GameManager.Instance.SetState(GameManager.GameState.FinishedHour);

        // move onto next hour
        stageIndex++;
        if (stageIndex < stages.Count)
        {
            //Debug.Log("hour finished " + stageIndex);

            ResetWave();

            GameManager.Instance.endedWave = true;
            GameManager.Instance.SetState(GameManager.GameState.Playing);

            if(!onBossLevel) waves[stageIndex].SetActive(true);
        }
        // finish night
        else
        {
            HandleNightFinish();
        }
    }

    void HandleNightFinish()
    {

        if (onBossLevel)
        {
            if (boss != null)
            {
                GameManager.Instance.SetState(GameManager.GameState.GameOver);
            }
        }

        //GameManager.Instance.endedHour = true;

        // move onto NEXT SCENE
        StartCoroutine(EndScene());
        
    }

    void ResetWave()
    {
        currentWaves = stages[stageIndex];
        
        waveIndex = 0;
        waveTimer = 0;
        wavesPassed = 0;
        waveFinished = false;

        slotOccupied = new bool[stackCapacity];
    }

    void ResetStage()
    {
        stageIndex = 0;
        nightFinished = false;
        ResetWave();
        BuildStage();
    }

    System.Collections.IEnumerator EndScene()
    {
        nightFinished = true;
        GameManager.Instance.SetState(GameManager.GameState.FinishedNight);

        GameObject.Find("Background").GetComponent<Animator>().SetBool("gameEnd", true);

        yield return new WaitForSeconds(10f);

        nextScene = SceneManager.GetActiveScene().name;
        int num = nextScene[0] - '0';
        // loops
        num = (num % 3) + 1;
        nextScene = num + nextScene.Substring(1);

        onBossLevel = (num == 3);

        SceneManager.LoadScene(nextScene);

        GameManager.Instance.endedWave = true;
        GameManager.Instance.endedHour = true;
        GameManager.Instance.SetState(GameManager.GameState.Playing);
        ResetStage();
    }

    // FIFO
    void HandleAutoDequeue()
    {
        if (fifo.Count == 0) { waitTimer = 0f; return; }

        waitTimer += Time.deltaTime;
        if (waitTimer >= waitTime)
        {
            GameObject ship = fifo.Dequeue();
            if (ship && ship.TryGetComponent(out EnemyController ec)) ec.TriggerExit();
            waitTimer = 0f;
        }
    }
    
    void HandleWaveTimer()
    {
        if (waveIndex == 0 || waveIndex > currentWaves.Count) return;
        waveTimer = Mathf.Min(waveTimer + Time.deltaTime, currentWaves[waveIndex - 1].duration);
    }
    
    void TryLaunchNextWave()
    {
        if (waveIndex >= currentWaves.Count)
        {
            waveFinished = true;
            return;
        }

        Wave next = currentWaves[waveIndex];
    
        bool prevFinished = (waveIndex == 0) || (waveTimer >= currentWaves[waveIndex - 1].duration);
        bool capacityOK = FreeSlotCount() >= next.ships.Count;

        if (prevFinished && capacityOK)
        {
            StartCoroutine(SpawnWave(next));
            waveIndex++; waveTimer = 0f;
        }
    }

    System.Collections.IEnumerator SpawnWave(Wave w)
    {
        foreach (GameObject prefab in w.ships)
        {
            if (prefab == null) continue;

            GameObject go = Instantiate(prefab, transform.position * 10, Quaternion.identity);
            int slot = ReserveRandomSlot(go);
            if (slot < 0)
            {
                Destroy(go);
                continue;
            }

            if (go.TryGetComponent(out EnemyController ec))
                ec.SetSlot(slot);
            yield return new WaitForSeconds(w.spawnInterval);
        }
        wavesPassed++;
    }
    
    int ReserveRandomSlot(GameObject ship)
    {
        List<int> free = new();
        for (int i = 0; i < slotOccupied.Length; i++)
            if (!slotOccupied[i]) 
                free.Add(i);

        if (free.Count == 0) 
            return -1;

        int pick = free[Random.Range(0, free.Count)];
        slotOccupied[pick] = true;
        shipSlotMap[ship] = pick;
        return pick;
    }

    public void ReleaseSlot(GameObject ship)
    {
        if (shipSlotMap.TryGetValue(ship, out int idx))
        {
            slotOccupied[idx] = false; 
            shipSlotMap.Remove(ship);
        }
    }

    int FreeSlotCount()
    {
        int c = 0; 
        foreach (bool occ in slotOccupied) 
            if (!occ) 
                c++; 
        return c;
    }
    
    public void RegisterShip(GameObject ship) => fifo.Enqueue(ship);
}