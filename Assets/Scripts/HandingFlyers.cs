using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandingFlyers : MonoBehaviour
{
    public static HandingFlyers singltoneFlyer {get; private set;}
    [SerializeField] private Text suspicionText;
    [SerializeField] private int radiusMegafon = 2;
    [SerializeField] private LayerMask layer;
    [SerializeField] private ListReactions listReactions;
    private bool isMegafon = false;
    public bool isPolice = false;
    private float currentSuspicion = 0;
    private void Start()
    {
        singltoneFlyer = this;
    }
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
                    if(isPolice)
                        CheakPolice(human);
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
                if(isPolice)
                    CheakPolice(humanM);
                humanM.emotion[0].SetActive(false);
                humanM.SetReaction(FlyersController.singltoneFlyers.CurrentFlyer());
                FlyersController.singltoneFlyers.DeletFlyer();
            }
        }
    }

    private void CheakPolice(Human human)
    {
        if(human.name == FlyersController.singltoneFlyers.CurrentFlyer())
        {
            string d = human.name;
            switch(d)
            {
                case "worker":
                    currentSuspicion += listReactions.Suspicion[0];
                    suspicionText.text = currentSuspicion.ToString();
                    break;
                case "student":
                    currentSuspicion += listReactions.Suspicion[1];
                    suspicionText.text = currentSuspicion.ToString();
                    break;
                case "retiree":
                    currentSuspicion += listReactions.Suspicion[2];
                    suspicionText.text = currentSuspicion.ToString();
                    break;
                case "blogger":
                    currentSuspicion += listReactions.Suspicion[3];
                    suspicionText.text = currentSuspicion.ToString();
                    break;
                case "esoteric":
                    currentSuspicion += listReactions.Suspicion[4];
                    suspicionText.text = currentSuspicion.ToString();
                    break;
            }
        }
        else
        {
            currentSuspicion += listReactions.Suspicion[5];
            suspicionText.text = currentSuspicion.ToString();
        }
    }
}
