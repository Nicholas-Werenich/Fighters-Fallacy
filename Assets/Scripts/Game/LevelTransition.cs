using NUnit;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


[System.Serializable]
public class LevelColors
{
    public Color[] colors = new Color[4];
}

public class LevelTransition : MonoBehaviour
{

    [Header("End Level")]
    [SerializeField]
    private float timeToChangeLevel;
    [SerializeField]
    private float minimumSkySmoothSpeed;

    [Header("UI Control")]
    [SerializeField]
    private float UITransitionDuration;

    [Header("Colour Control")]

    [SerializeField]
    private Material cloudMat;

    [SerializeField]
    private float cloudColorDelayFraction;

    [SerializeField]
    private float cloudColorSpeedFraction;

    [SerializeField]
    private List<LevelColors> levelColors = new List<LevelColors>();



    [Header("References")]
    private SkyLoader skyLoader;
    private PlayerMovement playerMovement;
    private Animator animator;
    private GameObject player;
    private CanvasGroup UICanvasGroup;
    private Inventory inventory;
    private void Awake()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        PlayerPrefs.SetInt("Current Level", 1);
        skyLoader = FindFirstObjectByType<SkyLoader>();
        playerMovement = FindFirstObjectByType<PlayerMovement>();
        animator = GetComponent<Animator>();

        player = GameObject.FindGameObjectWithTag("Player");
        UICanvasGroup = player.GetComponentInChildren<CanvasGroup>();
        inventory = player.GetComponent<Inventory>();
    }

    public void ExitLevel()
    {
        playerMovement.enabled = false;
    }

    public IEnumerator EnterLevel()
    {
        //Stop player movement
        playerMovement.rb.linearVelocity = Vector2.zero;
        playerMovement.rb.gravityScale = 0f;

        //Move sky background
        StartCoroutine(skyLoader.SceneChange(PlayerPrefs.GetInt("Current Level"), timeToChangeLevel));

        //Animate clouds to cover up scene
        animator.SetTrigger("isEntering");

        //Change cloud colours to next level theme
        yield return new WaitForSeconds(timeToChangeLevel/cloudColorDelayFraction);
        StartCoroutine(TransitionColor(PlayerPrefs.GetInt("Current Level") -1, timeToChangeLevel/cloudColorSpeedFraction));

        StartCoroutine(PlayerUIExitTransition(UITransitionDuration));

        //Load the next level
        //SceneManager.LoadSceneAsync($"Level {PlayerPrefs.GetInt("Current Level") + 1}", LoadSceneMode.Additive);

        //Unload last level
        //SceneManager.UnloadSceneAsync($"Level {PlayerPrefs.GetInt("Current Level")}");

        //Animate clouds to slowly disapear
        yield return new WaitForSeconds(timeToChangeLevel - timeToChangeLevel / cloudColorDelayFraction + 0.5f);
        animator.SetTrigger("isExiting");
        StartCoroutine(PlayerUIEnterTransition(UITransitionDuration));
        skyLoader.EnableAddedCloudMovement(minimumSkySmoothSpeed);

        playerMovement.enabled = true;


        //Progress to next level count
        PlayerPrefs.SetInt("Current Level", PlayerPrefs.GetInt("Current Level") + 1);
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

    //Change colour by value t 
    private void ColorChangeStep(string colorName, Color startColor, Color endColor, float t)
    {
        cloudMat.SetColor(colorName, Color.Lerp(startColor, endColor, t));
    }

    private IEnumerator PlayerUIExitTransition(float duration)
    {
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            UICanvasGroup.alpha = Mathf.Lerp(1, 0, t);
            yield return null;
        }
    }

    private IEnumerator PlayerUIEnterTransition(float duration)
    {
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            UICanvasGroup.alpha = Mathf.Lerp(0, 1, t);
            yield return null;
        }
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
