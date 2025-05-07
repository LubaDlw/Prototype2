using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerUI : MonoBehaviour
{
    public GameObject panel1; // Start panel
    public GameObject panel2; // How to play panel
    public GameObject panel3; // Final panel

    public GameObject LevelPanel;
    public bool gameOver = false;

    void Start()
    {
         Time.timeScale = 1;
         gameOver = false;
    // ShowPanel1(); // Show the first panel on game start
    panel1.SetActive(false);
        panel2.SetActive(false);
        panel3.SetActive(false);
    }

    public void level()
    {
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        LevelPanel.SetActive(false);
    }

    public void ShowPanel1()
    {
        panel1.SetActive(true);
        panel2.SetActive(false);
        panel3.SetActive(false);
    }

    public void ShowPanel2()
    {
        panel1.SetActive(false);
        panel2.SetActive(true);
        panel3.SetActive(false);
    }

    public void ShowPanel3()
    {
        panel1.SetActive(false);
        panel2.SetActive(false);
        panel3.SetActive(true);
    }

    public void LoadNextScene()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("No more scenes in the build index.");
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quit game called.");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop in editor
#endif
    }

    private void Update()
    {
        

    }

    public void RestartToMainMenu()
    {
        
            SceneManager.LoadScene(0);
        Time.timeScale = 1;

    }
}
