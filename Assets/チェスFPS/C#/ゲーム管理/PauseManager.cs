using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject UI_pauseMenu;
    private bool isPaused = false;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)){
        Debug.Log("Esc pressed");
            if(isPaused) Resume();
            else         Pause();
        
        }
    }

    public void Resume()
    {
        UI_pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Pause()
    {
        Debug.Log("Pause実行中");
        UI_pauseMenu.SetActive(true);  // ← 表示してボタン有効化
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // エディタ用
    #endif
    }

}
