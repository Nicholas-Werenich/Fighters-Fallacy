using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

 public class CheckpointDetection : MonoBehaviour 
{
    LevelTransition levelTransition;
    private void Start()
    {
        levelTransition = FindFirstObjectByType<LevelTransition>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            StartCoroutine(levelTransition.NextLevel());
    }
}
