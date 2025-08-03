using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyLoader : MonoBehaviour
{
    //List of prefab backgrounds
    public List<GameObject> backgrounds = new List<GameObject>();

    [Header("Backgrounds")]
    private GameObject currentBackground;
    private GameObject nextBackground;

    [Header("Metrics")]
    public float backgroundHeight;
    public float transitionSpeed;

    private Vector2 startPosition;

    private Vector2 velocity = Vector3.zero;
    private Vector2 velocity2 = Vector3.zero;

    private Transform cam;
    private void Awake()
    {
        startPosition = transform.position;
        cam = Camera.main.transform;

        currentBackground = Instantiate(backgrounds[0], transform);
    }

    //Next scene is getting pulled down
    public IEnumerator SceneChange(int nextLevel, float duration)
    {
        
        nextBackground = Instantiate(backgrounds[nextLevel], transform);
        nextBackground.transform.position = new Vector2(cam.position.x, cam.position.y - backgroundHeight);

        Vector2 targetPosition = new Vector2(startPosition.x, startPosition.y + backgroundHeight);

        while (Vector2.Distance(currentBackground.transform.position, targetPosition) > 0.01f || velocity.magnitude > 0.01f)
        {
            Debug.Log("Duration: " + duration);

            currentBackground.transform.position = Vector2.SmoothDamp(currentBackground.transform.position, targetPosition, ref velocity, duration);
            nextBackground.transform.position = Vector2.SmoothDamp(nextBackground.transform.position, startPosition, ref velocity2, duration);
            yield return null;
        }
        Debug.Log("Transition Done");
        currentBackground.transform.position = targetPosition;
        nextBackground.transform.position = startPosition;
    }
}
