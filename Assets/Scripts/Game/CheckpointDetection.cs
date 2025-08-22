using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;


public class CheckpointDetection : MonoBehaviour
{
    private LevelTransition levelTransition;
    private void Start()
    {
        levelTransition = FindFirstObjectByType<LevelTransition>();
    }

    //Stop player movement when entering trigger
    private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                levelTransition.ExitLevel();
            }
        }

    //Stop any movement when leaving trigger 
    private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                Debug.Log("Leaving triggerbox");
            StartCoroutine(levelTransition.EnterLevel());
            }
    }
}
