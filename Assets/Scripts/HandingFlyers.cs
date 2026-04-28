using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandingFlyers : MonoBehaviour
{
    [SerializeField] private int radiusMegafon = 2;
    [SerializeField] private LayerMask layer;
    private bool isMegafon = false;
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 mouseScreenPos = Input.mousePosition;
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);
            if (hit.collider != null)
            {
                if(hit.transform.GetComponent<Human>() && FlyersController.singltoneFlyers.CurrentFlyer() != "")
                {
                    hit.transform.TryGetComponent<Human>(out Human human);
                    human.emotion[0].SetActive(false);
                    human.SetReaction(FlyersController.singltoneFlyers.CurrentFlyer());
                    FlyersController.singltoneFlyers.DeletFlyer();
                    if(isMegafon)
                    {
                        Megafon(human, mouseWorldPos);
                    }
                }
            }
        }
    }

    private void Megafon(Human human, Vector3 mouseWorldPos)
    {
        Collider2D hitM = Physics2D.OverlapCircle(mouseWorldPos, radiusMegafon, layer);
        if(hitM != null)
        {
            hitM.transform.TryGetComponent<Human>(out Human humanM);
            if(human.name == humanM.name)
            {
                humanM.emotion[0].SetActive(false);
                humanM.SetReaction(FlyersController.singltoneFlyers.CurrentFlyer());
                FlyersController.singltoneFlyers.DeletFlyer();
            }
        }
    }
}
