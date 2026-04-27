using UnityEngine.SceneManagement;
using UnityEngine;

public class LoadSceneComponent : MonoBehaviour
{
    public void LoadScene(int i)
    {
        SceneManager.LoadScene(i);
    }
}
