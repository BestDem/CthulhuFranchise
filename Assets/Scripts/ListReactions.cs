using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "HumanReact", menuName ="human")]
public class ListReactions : ScriptableObject
{
    [SerializeField] private string[] needWorker;
    [SerializeField] private string[] needStudent;
    public string[] NeedWorker => needWorker;
    public string[] NeedStudent => needStudent;
}
