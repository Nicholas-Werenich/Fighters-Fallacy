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
    public float transitionAnimationDelay;

    [Header("Colour Control")]
    public Material cloudMat;
    public float cloudColorDelayFraction;
    public float cloudColorSpeedFraction;

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

        //Player can't move
        playerMovement.enabled = false;

        yield return new WaitForSeconds(transitionAnimationDelay);

        //Move sky background
        StartCoroutine(skyLoader.SceneChange(PlayerPrefs.GetInt("Current Level"), timeToChangeLevel));

        animator.SetTrigger("isEntering");

        yield return new WaitForSeconds(timeToChangeLevel/cloudColorDelayFraction);
        StartCoroutine(TransitionColor(PlayerPrefs.GetInt("Current Level") -1, timeToChangeLevel/cloudColorSpeedFraction));

        SceneManager.LoadSceneAsync($"Level {PlayerPrefs.GetInt("Current Level") + 1}", LoadSceneMode.Additive);

        yield return new WaitForSeconds(timeToChangeLevel - timeToChangeLevel/cloudColorDelayFraction + 0.5f);
        animator.SetTrigger("isExiting");

        playerMovement.enabled = true;





        StartLevel();
    }
    
    //Smoothly change the 4 colors in clouds to the 4 colors in the next level

    private IEnumerator TransitionColor(int currentLevel, float duration)
    {
        string getColorLevel(int i) => $"_Replace{i + 1}";
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

    //Resets the cloud colors out of play mode
    private void OnDisable()
    {
        string getColorLevel(int i) => $"_Replace{i + 1}";
        Color[] startColors = levelColors[0].colors;

        for (int i = 0; i < 4; i++)
        {
            cloudMat.SetColor(getColorLevel(i), startColors[i]);
        }
    }
}
