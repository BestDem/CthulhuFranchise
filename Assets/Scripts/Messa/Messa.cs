using UnityEngine;
using TMPro;


public class Messa : MonoBehaviour
{
    public int Money;
    public Improvement[] Improvements;

    public int[] auditory = new int[5];
    public int[] adepts = new int[5];

    public TextMeshProUGUI AuditoryCountLabel;
    public TextMeshProUGUI AdeptsCountLabel;   

    public TextMeshProUGUI AuditoryWorkersLabel;
    public TextMeshProUGUI AuditoryStudentsLabel;
    public TextMeshProUGUI AuditoryRetireesLabel;
    public TextMeshProUGUI AuditoryBloggersLabel;
    public TextMeshProUGUI AuditoryEsotericsLabel;

    public TextMeshProUGUI AdeptWorkersLabel;
    public TextMeshProUGUI AdeptStudentsLabel;
    public TextMeshProUGUI AdeptRetireesLabel;
    public TextMeshProUGUI AdeptBloggersLabel;
    public TextMeshProUGUI AdeptEsotericsLabel;

    private int TotalCount(int[] array)
    {
        int count = 0;
        foreach (int i in array) count += i;
        return count;
    }
    public void SpellPropoved(int peopleClass)
    {

        for (int i = 0; i < auditory.Length; i++)
        {
            if (i == peopleClass)
            {
                adepts[i] += auditory[i];
            }
            auditory[i] = 0;
        }
    }
    void Update()
    {
        AuditoryWorkersLabel.SetText($"Рабочие: {auditory[0]}");
        AuditoryStudentsLabel.SetText($"Студенты: {auditory[1]}");
        AuditoryRetireesLabel.SetText($"Пенсионеры: {auditory[2]}");
        AuditoryBloggersLabel.SetText($"Блогеры: {auditory[3]}");
        AuditoryEsotericsLabel.SetText($"Эзотерики: {auditory[4]}");

        AdeptWorkersLabel.SetText($"Рабочие: {adepts[0]}");
        AdeptStudentsLabel.SetText($"Студенты: {adepts[1]}");
        AdeptRetireesLabel.SetText($"Пенсионеры: {adepts[2]}");
        AdeptBloggersLabel.SetText($"Блогеры: {adepts[3]}");
        AdeptEsotericsLabel.SetText($"Эзотерики: {adepts[4]}");

        AuditoryCountLabel.SetText($"Аудитория: {TotalCount(auditory)}");
        AdeptsCountLabel.SetText($"Адепты: {TotalCount(adepts)}");
    }   
}
