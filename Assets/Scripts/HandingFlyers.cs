using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandingFlyers : MonoBehaviour
{
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 mouseScreenPos = Input.mousePosition;
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                if(hit.transform.GetComponent<Human>())
                {
                    hit.transform.TryGetComponent<Human>(out Human human);
                    human.emotion[0].SetActive(false);
                    human.SetReaction(FlyersController.singltoneFlyers.CurrentFlyer());
                    FlyersController.singltoneFlyers.DeletFlyer();
                }
            }
        }
    }
}
