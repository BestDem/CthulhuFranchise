using UnityEngine;

public class StreetExitTrigger : MonoBehaviour
{
    [SerializeField] private SpawnHuman spawnHuman;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Human human = other.GetComponent<Human>();
        if (human == null)
            human = other.GetComponentInParent<Human>();

        if (human != null)
        {
            DeleteObject(human.gameObject);
            return;
        }

        PoliceController policeController = other.GetComponent<PoliceController>();
        if (policeController == null)
            policeController = other.GetComponentInParent<PoliceController>();

        PolicePatrol policePatrol = other.GetComponent<PolicePatrol>();
        if (policePatrol == null)
            policePatrol = other.GetComponentInParent<PolicePatrol>();

        if (policeController != null || policePatrol != null)
        {
            GameObject policeRoot = policeController != null ? policeController.gameObject : policePatrol.gameObject;
            DeleteObject(policeRoot);
        }
    }

    private void DeleteObject(GameObject target)
    {
        if (target == null)
            return;

        if (spawnHuman != null)
            spawnHuman.DeleteHuman(target);
        else
            Destroy(target);
    }
}
