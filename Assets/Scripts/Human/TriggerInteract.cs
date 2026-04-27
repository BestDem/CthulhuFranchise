using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerInteract : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Human")
        {
            collision.TryGetComponent(out Human human);
            human.interact = true;
            human.emotion[0].SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Human")
        {
            collision.TryGetComponent(out Human human);
            human.interact = false;
            human.emotion[0].SetActive(false);
        }
    }
}
