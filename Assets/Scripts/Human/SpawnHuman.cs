using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnHuman : MonoBehaviour
{
    [SerializeField] private Transform spawnpoint;
    [SerializeField] private GameObject[] human; //0-worker 1-student 2-blogger 3-esoteric 4-retiree 5-police
    [SerializeField] private ListReactions listReactions;
    private Dictionary<string, int> nameToIndex = new Dictionary<string, int>()
    {
        {"worker", 0}, {"student", 1}, {"blogger", 2}, {"esoteric", 3}, {"retiree", 4}
    };
    private Dictionary<string, float> weights = new Dictionary<string, float>()
    {
        {"worker", 25}, {"student", 25}, {"retiree", 25}, {"blogger", 25}, {"esoteric", 25}
    };
    private List<GameObject> spawnedHuman = new List<GameObject>();
    private Dictionary<string, float> weight = new Dictionary<string, float>();
    private bool canSpawn = true;
    private int len;
    private float timerSpawnHuman = 0;
    private float timerPolice = 7;
    private float frequencySpawn = 0;
    private int currentDay = 0;
    private void Start()
    {
        len = human.Length;
        frequencySpawn = listReactions.LenDaySec[currentDay] / listReactions.CountHuman[currentDay];
        timerSpawnHuman = frequencySpawn;
    }

    private void OnEnable()
    {
        DayController.dayEnd += EndDay;
    }
    private void OnDisable()
    {
        DayController.dayEnd -= EndDay;
    }
    private void EndDay(bool endD)
    {
        canSpawn = !canSpawn;
        if(canSpawn)
        {
            currentDay+=1;
            frequencySpawn = listReactions.LenDaySec[currentDay] / listReactions.CountHuman[currentDay];
            timerSpawnHuman = frequencySpawn;
        }
    }

    private void Update()
    {
        if(canSpawn)
        {
            timerSpawnHuman -= Time.deltaTime;
            if(timerSpawnHuman < 0)
            {
                timerSpawnHuman = frequencySpawn;
                GameObject prefab = GetRandomHumanByWeight();
                GameObject h = Instantiate(prefab, spawnpoint);
                spawnedHuman.Add(h);
            }
            if(currentDay > 0)
            {
                timerPolice -= Time.deltaTime;
                if(timerPolice < 0)
                {
                    timerPolice = 7;
                    GameObject h = Instantiate(human[5], spawnpoint);
                    spawnedHuman.Add(h);
                }
            }
        }
    }

    public void SetWeight(string type, float newWeight)
    {
        if (weights.ContainsKey(type))
        {
            weights[type] = newWeight;
        }
    }
    private GameObject GetRandomHumanByWeight()
    {
        float totalWeight = 0f;

        foreach (var w in weights.Values)
            totalWeight += w;

        float randomPoint = Random.Range(0f, totalWeight);

        float current = 0f;

        foreach (var pair in weights)
        {
            current += pair.Value;

            if (randomPoint <= current)
            {
                int index = nameToIndex[pair.Key];
                return human[index];
            }
        }

        return human[0];
    }

    public void DeleteHuman(GameObject hum)
    {
        spawnedHuman.Remove(hum);
        Destroy(hum);
    }
}
