using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Human : MonoBehaviour
{
    public string[] needReaction;
    protected abstract string[] GetReactions();
    public GameObject background;
    public GameObject[] emotion;
    public string name;
    public int reactionState;  //2 - придет, 1 - не придет
    public bool interact = false;
    protected virtual void Awake()
    {
        needReaction = GetReactions();
    }
    public void SetReaction(string reaction)
    {
        if(interact)
        {
            interact = false;
            if(reaction == needReaction[0])
            {
                gameObject.layer = 2;
                reactionState = 2;
            }
            //else if(reaction == needReaction[1])
            //{
            //    reactionState = 1;
            //}
            else
            {
                reactionState = 1;
            }
            Debug.Log(needReaction[0] + reaction);
            emotion[reactionState].SetActive(true);
        }
    }

    public void OnMouseEnter()
    {
        background.SetActive(true);
    }
    public void OnMouseExit()
    {
        background.SetActive(false);
    }
}
