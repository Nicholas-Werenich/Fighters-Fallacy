using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseForAnimation : MonoBehaviour
{

    public float interactionRange = 2f;
    public int nextLevel;

    public GameObject canvas;
    private GameObject levelLoader;
    private Transform player;
    private Animator openDoor;

    private bool isPaused = false;



    void Start()
    {
        openDoor = GetComponent<Animator>();
        levelLoader = GameObject.Find("Scene Transition");
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isPaused && PlayerIsNearby())
        {
            StartCoroutine(PlayAnimationAndPause());
        }
    }

    private IEnumerator PlayAnimationAndPause()
    {
        isPaused = true;
        Time.timeScale = 0;

        openDoor.SetTrigger("ExitLevel");

        float animationLength = openDoor.GetCurrentAnimatorStateInfo(0).length;

        Debug.Log(animationLength);
       
        float elapsedTime = 0f;
        bool triggerExit = false;

        while (elapsedTime < animationLength + 1.5f)
        {
            elapsedTime += Time.unscaledDeltaTime;
            
            if(elapsedTime > animationLength && !triggerExit)
            {
                levelLoader.transform.position = transform.position + Vector3.up * 2;
                levelLoader.GetComponentInChildren<Animator>().SetTrigger("SceneExit");
                triggerExit = true;
            }

            yield return null;
        }


        Time.timeScale = 1; 
        isPaused = false;

        SceneManager.LoadScene($"Level {nextLevel}");
    }

    bool PlayerIsNearby()
    {

        if(Vector3.Distance(player.position, transform.position) < interactionRange)
        {
            canvas.SetActive(true);
            return true;
        }
        return false;
    }
}
