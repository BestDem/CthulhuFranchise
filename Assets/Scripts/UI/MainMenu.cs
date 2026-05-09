using UnityEngine;
public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        MusicPlayer.Instance.PlayDefaultMusic();
    }
    private void OnEnable()
    {
        MusicPlayer.Instance.PlayDefaultMusic();
    }
}
