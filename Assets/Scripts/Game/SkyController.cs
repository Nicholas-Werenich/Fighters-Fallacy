using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class SkyController : MonoBehaviour
{
    [Header("Scene Transition")]
    private float yShiftPerLevel;

    [Header("Sprite Controls")]
    public Sprite childSprite;
    public float childSize;
    public int sortingOrder;

    [Header("Speed Controls")]
    public bool floatingLeft;
    public float parallaxSpeedX;
    public float parallaxSpeedY;
    public float passiveSpeedMultiplier;

    private Transform cameraTransform;
    private Vector2 startPosition;
    private Transform[] backgroundPieces = new Transform[3];

    //Middle of size 3 array
    private int currentMiddlePiece = 1;

    

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        startPosition = transform.position;

        CreateChildren();
    }

    private void CreateChildren()
    {
        //Create 3 for front back and middle
        for(int i = -1; i < 2; i++) 
        {
            GameObject child = new GameObject($"Child {i + 2}");

            //Configure SpriteRenderer
            SpriteRenderer renderer = child.AddComponent<SpriteRenderer>();
            renderer.sprite = childSprite;
            renderer.sortingOrder = sortingOrder;

            //Configure transform
            child.transform.localPosition = new Vector3(transform.position.x + i * childSize, transform.position.y, transform.position.z);
            child.transform.parent = transform;
            backgroundPieces[i + 1] = child.transform;
        }
    }

    private void Update()
    {
        UpdateMovement();
            
        CheckPosition();

        PassiveMovement();
    }

    private void PassiveMovement()
    {
        foreach(Transform transform in backgroundPieces)
        {
            transform.localPosition = !floatingLeft ? transform.localPosition + new Vector3(parallaxSpeedX * passiveSpeedMultiplier, transform.localPosition.y, 0) : transform.localPosition - new Vector3(parallaxSpeedX * passiveSpeedMultiplier, transform.localPosition.y, 0);
        }
    }

    private void UpdateMovement()
    {
        //Need to set to camera positon locally but still have normal positon be affectd by SkyLoader
        transform.position = new Vector2(startPosition.x - cameraTransform.position.x * parallaxSpeedX, cameraTransform.position.y + transform.parent.position.y);
    }

    private void CheckPosition()
    {
        if (backgroundPieces[currentMiddlePiece].position.x > cameraTransform.position.x + childSize / 2f)
        {
            Transform nextPiece = backgroundPieces[IndexWrap(backgroundPieces, 1)];
            nextPiece.position = new Vector2(nextPiece.position.x - (childSize * backgroundPieces.Length), nextPiece.position.y);
            currentMiddlePiece = IndexWrap(backgroundPieces, -1);
        }

        else if (backgroundPieces[currentMiddlePiece].position.x < cameraTransform.position.x - childSize / 2f)
        {
            Transform nextPiece = backgroundPieces[IndexWrap(backgroundPieces, -1)];
            nextPiece.position = new Vector2(nextPiece.position.x + (childSize * backgroundPieces.Length), nextPiece.position.y);
            currentMiddlePiece = IndexWrap(backgroundPieces, 1);
        }
    }

    private int IndexWrap<T>(T[] array, int direction)
    {
        return (currentMiddlePiece + direction + array.Length) % array.Length;
    }
}
