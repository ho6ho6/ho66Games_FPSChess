using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject UI_pauseMenu;
    public GameObject IsUI_show;
    private bool isPaused = false;
    public string Scene_main;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
        Debug.Log("Esc pressed");
            if(isPaused) Resume();
            else         Pause();
        }

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            IsUI_show.SetActive(!IsUI_show.activeSelf);
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

        if(SceneManager.GetActiveScene().name != Scene_main)
        {
            SceneManager.LoadScene(Scene_main);
        } 
        else 
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
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
