using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnHuman : MonoBehaviour
{
    [SerializeField] private Transform spawnpoint;
    [SerializeField] private GameObject[] human;
    [SerializeField] private ListReactions listReactions;
    private List<GameObject> spawnedHuman = new List<GameObject>();
    private bool canSpawn = true;
    private int len;
    private float timerSpawnHuman = 0;
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
                GameObject h = Instantiate(human[Random.Range(0,len)], spawnpoint);
                spawnedHuman.Add(h);
            }
        }
    }

    public void DeleteHuman(GameObject hum)
    {
        spawnedHuman.Remove(hum);
        Destroy(hum);
    }
}
