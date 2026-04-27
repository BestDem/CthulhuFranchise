using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteHumanTrigger : MonoBehaviour
{
    [SerializeField] private SpawnHuman spawnHuman;
    [SerializeField] private CounterHuman counterHuman;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Human")
        {
            spawnHuman.DeleteHuman(other.gameObject);
            other.TryGetComponent(out Human human);
            int i = 0;
            if(human.reactionState == 1)
            {
                i = Random.Range(0, 2);
                counterHuman.AddHuman(1, human.name);
            }
            else if(human.reactionState == 2)
            {
                counterHuman.AddHuman(human.reactionState, human.name);
            }
        }
    }
}
