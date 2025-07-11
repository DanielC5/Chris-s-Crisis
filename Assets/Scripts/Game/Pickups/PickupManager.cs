using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PickupWave
{
    public List<GameObject> pickups = new();
    public float duration = 5f;
    public float spawnInterval = 1f;
}

/// <summary>
/// 按波次随机生成Pickups
/// </summary>
public class PickupManager : MonoBehaviour
{
    [Header("Pickup Waves")]
    public List<PickupWave> pickupWaves = new();

    private int waveIndex = 0;
    private float waveTimer = 0f;

    void Update()
    {
        if (waveIndex >= pickupWaves.Count) return;
        PickupWave cur = pickupWaves[waveIndex];
        waveTimer += Time.deltaTime;
        if (waveTimer >= cur.duration)
        {
            waveTimer = 0f;
            waveIndex++;
            return;
        }
    }
    
    public void SpawnPickupWave(int index)
    {
        if (index < 0 || index >= pickupWaves.Count) return;
        StartCoroutine(SpawnRoutine(pickupWaves[index]));
    }

    System.Collections.IEnumerator SpawnRoutine(PickupWave wave)
    {
        foreach (GameObject prefab in wave.pickups)
        {
            Instantiate(prefab, Vector3.up* 10, Quaternion.identity);
            yield return new WaitForSeconds(wave.spawnInterval);
        }
    }
}