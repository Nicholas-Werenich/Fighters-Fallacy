using NUnit;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;


[System.Serializable]
public class LevelColors
{
    public Color[] colors = new Color[4];
}

public class LevelTransition : MonoBehaviour
{
    [Header("End Level")]
    public float timeToChangeLevel;

    [Header("Start Level")]
    public float timeToStartLevel;

    [Header("Colour Control")]
    public Material cloudMat;

    public List<LevelColors> levelColors = new List<LevelColors>();


    SkyLoader skyLoader;
    PlayerMovement playerMovement;
    Animator animator;
    private void Awake()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        PlayerPrefs.SetInt("Current Level", 1);
        skyLoader = FindFirstObjectByType<SkyLoader>();
        playerMovement = FindFirstObjectByType<PlayerMovement>();
        animator = GetComponent<Animator>();
    }

    private void StartLevel()
    {
        PlayerPrefs.SetInt("Current Level", PlayerPrefs.GetInt("Current Level") + 1);
    }

    public IEnumerator NextLevel()
    {

        //Move sky background
        StartCoroutine(skyLoader.SceneChange(PlayerPrefs.GetInt("Current Level"), timeToChangeLevel));

        yield return new WaitForSeconds(timeToChangeLevel/2);
        StartCoroutine(TransitionColor(PlayerPrefs.GetInt("Current Level") -1, timeToChangeLevel/2));

        animator.SetTrigger("Next Level");

        //Player can't move
        playerMovement.enabled = false;


        
      

        //SceneManager.LoadScene($"Level {PlayerPrefs.GetInt("Current Level") + 1}", LoadSceneMode.Additive);

        //StartLevel();
    }
    
    //Smoothly change the 4 colors in clouds to the 4 colors in the next level
    private IEnumerator TransitionColor(int currentLevel, float duration)
    {
        string getColorLevel(int i) => $"To Color {i + 1}";
        Color[] startColors = levelColors[currentLevel].colors;
        Color[] targetColors = levelColors[currentLevel + 1].colors;

        float t = 0f;
        Debug.Log("Start Clouds Changed");
        while (t < 1f)
        {
            t += Time.deltaTime / duration;

            for (int i = 0; i < 4; i++)
            {
                ColorChangeStep(getColorLevel(i), startColors[i], targetColors[i],t);
            }
           
            //Make the transition happen over multiple frames
            yield return null;
        }
        Debug.Log("Cloud colors changed");
        //Snap colors at end
        for(int i = 0; i < 4; i++)
        {
            cloudMat.SetColor(getColorLevel(i), targetColors[i]);
        }
    }

    private void ColorChangeStep(string colorName, Color startColor, Color endColor, float t)
    {
        Debug.Log($"{colorName} - {startColor} : {endColor}");
        cloudMat.SetColor(colorName, Color.Lerp(startColor, endColor, t));
    }
}
