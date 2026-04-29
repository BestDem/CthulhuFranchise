using UnityEngine;

// Старый триггер оставлен для совместимости.
// Лучше использовать MessaDoorTrigger у двери и StreetExitTrigger в конце улицы.
public class DeleteHumanTrigger : MonoBehaviour
{
    [SerializeField] private SpawnHuman spawnHuman;
    [SerializeField] private CounterHuman counterHuman;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Human human = other.GetComponent<Human>();
        if (human == null)
            human = other.GetComponentInParent<Human>();

        if (human == null)
            return;

        if (human.reactionState == 2 && counterHuman != null)
            counterHuman.AddHuman(1, human.name);

        if (spawnHuman != null)
            spawnHuman.DeleteHuman(human.gameObject);
        else
            Destroy(human.gameObject);
    }
}
