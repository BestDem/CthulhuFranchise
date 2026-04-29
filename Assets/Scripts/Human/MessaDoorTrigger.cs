using System.Collections.Generic;
using UnityEngine;

// Точка входа в церковь.
// Теперь может работать двумя способами:
// 1) как обычный trigger у двери;
// 2) напрямую из Human, когда X персонажа дошёл до X двери.
public class MessaDoorTrigger : MonoBehaviour
{
    public static MessaDoorTrigger Current { get; private set; }

    [SerializeField] private SpawnHuman spawnHuman;
    [SerializeField] private CounterHuman counterHuman;

    private readonly HashSet<Human> acceptedHumans = new HashSet<Human>();

    private void Awake()
    {
        Current = this;
    }

    private void OnDestroy()
    {
        if (Current == this)
            Current = null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Human human = other.GetComponent<Human>();
        if (human == null)
            human = other.GetComponentInParent<Human>();

        AcceptHuman(human);
    }

    public void AcceptHuman(Human human)
    {
        if (human == null)
            return;

        if (acceptedHumans.Contains(human))
            return;

        if (human.reactionState != 2 || !human.IsGoingToMessa)
            return;

        acceptedHumans.Add(human);

        if (counterHuman != null)
            counterHuman.AddHuman(1, human.name);

        DeleteHuman(human.gameObject);
    }

    private void DeleteHuman(GameObject humanObject)
    {
        if (spawnHuman != null)
            spawnHuman.DeleteHuman(humanObject);
        else
            Destroy(humanObject);
    }
}
