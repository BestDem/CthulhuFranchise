using UnityEngine;
using TMPro;



public class CthulhuSpeech : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI SpeechLabel;
    [SerializeField] private string BadSpeech;
    [SerializeField] private string GoodSpeech;
    [SerializeField] private string ExcellentSpeech;

    public void SetSpeech(MessaResults result)
    {
        if (result == MessaResults.Bad) SpeechLabel.SetText(BadSpeech);
        if (result == MessaResults.Good) SpeechLabel.SetText(GoodSpeech);
        if (result == MessaResults.Excellent) SpeechLabel.SetText(ExcellentSpeech);
    }
}
