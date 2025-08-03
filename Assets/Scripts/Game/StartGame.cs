using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void BeginGame()
    {
        SceneManager.LoadScene("Level 1");
    }
}
