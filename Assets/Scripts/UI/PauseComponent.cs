using UnityEngine;

public class PauseComponent : MonoBehaviour
{
    [SerializeField] private GameObject MainMenu;
    [SerializeField] private GameObject PauseMenu;
    [SerializeField] private GameObject OptionsMenu;

    public static PauseComponent Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PauseMenu?.SetActive(false);
    }

    public void Pause()
    {
        if (MainMenu.activeSelf || PauseMenu == null) return;
        OptionsMenu?.SetActive(false);

        if (PauseMenu.activeSelf && Time.timeScale == 0f)
        {
            PauseMenu.SetActive(false);
            Time.timeScale = 1f;
            Cursor.visible = false;
        }
        else if (Time.timeScale == 1f)
        {
            Cursor.visible = true;
            PauseMenu.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Pause();
    }
}