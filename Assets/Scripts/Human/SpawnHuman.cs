using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnHuman : MonoBehaviour
{
    [SerializeField] private Transform spawnpoint;
    [SerializeField] private GameObject[] human;
    [SerializeField] private int speedSpawn;
    private List<GameObject> spawnedHuman = new List<GameObject>();
    private bool canSpawn = true;
    private int len;
    private float timer = 0;
    private void Start()
    {
        len = human.Length;
        timer = speedSpawn;
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
    }

    private void Update()
    {
        if(canSpawn)
        {
            timer -= Time.deltaTime;
            if(timer < 0)
            {
                timer = speedSpawn;
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
