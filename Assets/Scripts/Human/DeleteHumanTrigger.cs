using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteHumanTrigger : MonoBehaviour
{
    [SerializeField] private SpawnHuman spawnHuman;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Human")
        {
            Debug.Log("Коснулся триггера");
            spawnHuman.DeleteHuman(other.gameObject);
        }
    }
}
