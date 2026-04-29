using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterHuman : MonoBehaviour
{
    private int countRecruited = 0;
    private Dictionary<string, int> countRecruit = new Dictionary<string, int>();

    public void AddHuman(int add, string name)
    {
        if(add != 0)
        {
            int value = 0;
            countRecruited += add;
            countRecruit.TryGetValue(name, out value);
            if(value > 0)
                countRecruit[name] = value + 1;
            else
                countRecruit.Add(name, 1);
            Debug.Log("Количество: " + countRecruit[name] + " людей группы: " +  name);
            //countRecruit.Add(name, 1);
        }
    }
}
