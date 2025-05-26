using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ゲーム画面へ : MonoBehaviour
{
    public void ClickStartButton()
    {
        SceneManager.LoadScene("Scene_main");
    }
}
