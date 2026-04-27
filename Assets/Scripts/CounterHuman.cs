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
            if(countRecruit.TryGetValue(name, out value))
            {
                countRecruit.Add(name, value + 1);
            }
            countRecruit.Add(name, 1);
        }
    }
}
