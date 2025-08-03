using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Points")]
    [SerializeField] public Transform leftEdge;
    [SerializeField] public Transform rightEdge;

    [Header("Movement parameters")]
    public float speed;
    [SerializeField] public float usedSpeed;
    private bool movingLeft;

    [Header("Idle Behaviour")]
    [SerializeField] private float idleDuration;
    private float idleTimer;

    [Header("Enemy Animator")]
    private Animator anim;

    Rigidbody2D rb;
    private Transform canvas;

    private void Awake()
    {
        canvas = transform.GetChild(0);
        speed = usedSpeed;
        rb = GetComponent<Rigidbody2D>();   
        anim = GetComponent<Animator>();
    }
    private void OnDisable()
    {
        anim.SetBool("IsMoving", false);
    }

    private void Update()
    {
        if (movingLeft)
        {
            
            transform.rotation = new Quaternion(0, 0, 0, 0);
            canvas.transform.rotation = new Quaternion(0, 180, 0, 0);

            if (transform.position.x >= leftEdge.position.x)
                MoveInDirection(-1);
            else
                DirectionChange();
        }
        else
        {
           
            transform.rotation = new Quaternion(0, 180, 0, 0);
            canvas.transform.rotation = new Quaternion(0, 0, 0, 0);

            if (transform.position.x <= rightEdge.position.x)
                MoveInDirection(1);
            else
                DirectionChange();
        }
    }

    private void DirectionChange()
    {
        anim.SetBool("IsMoving", false);
        idleTimer += Time.deltaTime;

        if (idleTimer > idleDuration)
            movingLeft = !movingLeft;
    }

    private void MoveInDirection(int _direction)
    {
        idleTimer = 0;
        anim.SetBool("IsMoving", true);


        //Move in that direction
        transform.position = new Vector3(transform.position.x + Time.deltaTime * _direction * usedSpeed,
            transform.position.y, transform.position.z);
    }
}