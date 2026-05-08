using UnityEngine;

public class MenuScript : MonoBehaviour
{
    void OnEnable()
    {
        MusicPlayer.Instance.PlayDefaultMusic();
    }
}
