using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    public void RestartButton()
    {
        SceneManager.LoadScene(PlayerPrefs.GetString("Current Level"));
    }

    public void MenuButton()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
