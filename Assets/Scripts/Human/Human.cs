using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Human : MonoBehaviour
{
    public string name;
    public int reactionState;  //2 - придет, 1 - сомнение, 0 - не придет
    public bool interact = true;
    public void SetReaction(string reaction)
    {
        if(interact)
        {
            interact = false;
            if(reaction == needReaction[0])
            {
                reactionState = 2;
            }
            else if(reaction == needReaction[1])
            {
                reactionState = 1;
            }
            else
            {
                reactionState = 0;
            }
        }
    }
    public string[] needReaction;
}
