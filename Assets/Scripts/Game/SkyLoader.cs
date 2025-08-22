using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyLoader : MonoBehaviour
{
    [Header("Backgrounds")]
    //List of prefab backgrounds
    public List<GameObject> backgrounds = new List<GameObject>();
    private GameObject currentBackground;
    private GameObject nextBackground;

    [Header("Transition Assets")]
    public GameObject levelInBetween;

    [Header("Metrics")]
    public float levelInBetweenOffset;
    float backgroundHeight;

    private Vector2 startPosition;

    private Vector2 velocity = Vector3.zero;
    private Vector2 velocity2 = Vector3.zero;
    private Vector2 velocity3 = Vector3.zero;

    private Transform cam;
    private void Awake()
    {
        startPosition = transform.position;
        cam = Camera.main.transform;

        currentBackground = Instantiate(backgrounds[0], transform);

        //Get the height of the background asset
        backgroundHeight = backgrounds[0].GetComponentInChildren<SkyController>().childSprite.bounds.size.y - 0.1f;    
    }

    //Next scene is getting pulled down
    public IEnumerator SceneChange(int nextLevel, float duration)
    {
        //Initialize next background and its target position
        nextBackground = Instantiate(backgrounds[nextLevel], transform);
        nextBackground.transform.position = new Vector2(currentBackground.transform.position.x, currentBackground.transform.position.y - backgroundHeight);
        Vector2 targetPosition = new Vector2(startPosition.x, startPosition.y + backgroundHeight);

        //Initialize level in between position and its target position
        levelInBetween.transform.position = new Vector2(currentBackground.transform.position.x, currentBackground.transform.position.y - backgroundHeight / 2f + levelInBetweenOffset);
        Vector2 inBetweenTargetPosition = new Vector2(currentBackground.transform.position.x, currentBackground.transform.position.y + backgroundHeight / 2f + levelInBetweenOffset + 1f);

        while (Vector2.Distance(currentBackground.transform.position, targetPosition) > 0.01f || velocity.magnitude > 0.01f)
        {

            //Smoothly move backgrounds up to next slot
            currentBackground.transform.position = Vector2.SmoothDamp(currentBackground.transform.position, targetPosition, ref velocity, duration);
            nextBackground.transform.position = Vector2.SmoothDamp(nextBackground.transform.position, startPosition, ref velocity2, duration);
            levelInBetween.transform.position = Vector2.SmoothDamp(levelInBetween.transform.position, inBetweenTargetPosition, ref velocity3, duration);
            yield return null;
        }

        Debug.Log("Sky transition complete");

        //Snap positions at end
        currentBackground.transform.position = targetPosition;
        nextBackground.transform.position = startPosition;

        //Prepare for next transition
        Destroy(currentBackground);
        currentBackground = nextBackground;
    }
}
