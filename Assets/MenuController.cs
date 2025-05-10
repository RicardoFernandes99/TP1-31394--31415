using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainPanel;

    private bool isPaused = false;

    public void OnStartGame()
    {

        SceneManager.LoadScene("Main", LoadSceneMode.Single);  
    }

    public void OnLeaderboard()
    {
        SceneManager.LoadScene("Leaderboard", LoadSceneMode.Single);
    }
    public void OnQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        Debug.Log("Quit Game");
    }
}