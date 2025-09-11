using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyControl : MonoBehaviour
{

    [Header("--States--")]
    public State CurrentState;
    public State previousState;
    [Space(30)]

    [Header("--Waypoints--")]
    public Transform[] waypoints;
    public int currentWaypoint;
    [Space(30)]

    [Header("--Layer Masks--")]
    public LayerMask playerLayer;
    public LayerMask obstacleLayer;
    [Space(30)]

    [Header("--Components/Game Objects--")]
    public Rigidbody2D rb;
    public Animator anim;
    public SpriteRenderer spriteRenderer;
    public GameObject player;
    public GameObject bullet;
    public NavMeshAgent agent;
    [Space(30)]

    [Header("--General Data--")]
    public float speed;
    public float waitTimer;
    public float visionRadius;  
    public float visionAngle;
    public bool playerDetected;
    public float alertTime;
    public float fireRate;
    public float fireTimer;
    public RaycastHit2D obstacleCheck;
    private Vector2 playerLastPosition;   
    public int life;
    private Vector3 lastPosition;
    public string moveDirection;
    public Text stateText;
    [SerializeField] private string initialDirection;
    Dictionary<string, Vector3> _rotation = new Dictionary<string, Vector3>();
    Dictionary<string, int> _movementSide = new Dictionary<string, int>();
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
        CurrentState = State.Walking;
        speed /= 10;
        life = 2;
        lastPosition = transform.position;
        moveDirection = initialDirection;
        //dictionary to get side the character is looking and return a transform side
        _rotation.Add("up", transform.up);
        _rotation.Add("down", -transform.up);
        _rotation.Add("right", transform.right);
        _rotation.Add("left", -transform.right);
        //dictionary to get side the character is looking and return a interger value, to use on Animator
        _movementSide.Add("up", 0);
        _movementSide.Add("down", 1);
        _movementSide.Add("right", 2);
        _movementSide.Add("left", 2);
        
    }

    // Update is called once per frame
    void Update()
    {
        
        stateText.text = CurrentState.ToString();
        transform.rotation = Quaternion.Euler(0, 0, 0);
        fireTimer += Time.deltaTime;
        
        //flipping the sprite depending if its going to right or left
        spriteRenderer.flipX = moveDirection.Equals("right") ? true : false;

        //the 
        if (transform.position != lastPosition ) moveDirection = getDirection((transform.position - lastPosition).normalized);
        
        lastPosition = transform.position;

        //detecting the player on a circle area
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, visionRadius, playerLayer);
        //getting the direction to the player
        Vector2 playerDirection = (player.transform.position - transform.position).normalized;
        //raycast between the enemy and the player, to if have any obstacles between them
        obstacleCheck = Physics2D.Raycast(transform.position, playerDirection, Vector2.Distance(transform.position, player.transform.position), obstacleLayer);
        //if the players get into the circle area
        if (playerCollider != null)
        {
            
            float angle = Vector2.Angle(_rotation[moveDirection], playerDirection);
            //detecting the player on a cone area
            if (angle < visionAngle / 2)
            {
                //detecting the player is not behind a obstacle or wall, if not, will go to alert state
                if (obstacleCheck.collider == null)
                {
                    if (playerDetected) return;
                    alertTime = 0;
                    playerDetected = true;
                    ChangeState(State.Alert);
                    Debug.Log("Detectei o player");
                }
                else if (playerDetected) playerDetected = false;
            }
            else playerDetected = false;
        }
        else playerDetected = false;

        if (life <= 0) ChangeState(State.Dead);
        
    }
    private void FixedUpdate()
    {

        switch (CurrentState)
        {
            //in idle state, the character will remain stopped for a few seconds, before start walking again
            case State.Idle:
                agent.ResetPath();
                anim.SetBool("moving", false);
                rb.linearVelocity = Vector2.zero;

                waitTimer += Time.deltaTime;

                if (waitTimer >= 2)
                {

                    ChangeState(State.Walking);
                    waitTimer = 0;
                }
                break;
            //in walking state, the character will patrol a area determined by waypoints
            case State.Walking:
                //checking if it's not going to out of bonds of waypoints array
                if (currentWaypoint <= waypoints.Length - 1)
                {
                    //checking if the character arrived in the waypoint
                    if (Vector2.Distance(transform.position, waypoints[currentWaypoint].position) > .1f)
                    {

                        agent.SetDestination(waypoints[currentWaypoint].position);
                        anim.SetInteger("moveIndex", _movementSide[moveDirection]);
                        anim.SetBool("moving", true);
                        Debug.Log(moveDirection);
                    }
                    else
                    {
                        currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
                        ChangeState(State.Idle);
                    }
                }

                break;
            case State.Alert:
                //in state alert, the character will look to the player, and if he stay on his vision for one second, the character will start shooting
                if (previousState.Equals(State.Chasing)) CurrentState = State.Firing;
                //reseting the destination of NavMesh
                agent.ResetPath();
                //animator data
                anim.SetBool("moving", false);
                anim.SetInteger("moveIndex", _movementSide[moveDirection]);
                //remains stopped
                rb.linearVelocity = Vector2.zero;
                
                alertTime += Time.deltaTime;
                //if the player remains in the detect zone,the enemy will start to fire, if not, he will keep doing what he was before
                if (alertTime >= 1f && playerDetected)
                {
                    ChangeState(State.Firing);
                }
                if (!playerDetected) {
                    ChangeState(previousState);
                }


                break;
            case State.Firing:
                //animator data
                anim.SetBool("moving", false);
                anim.SetInteger("moveIndex", _movementSide[moveDirection]);
                //gettng the rotation based on where the player is
                Vector2 rot = player.transform.position - transform.position;
                float RotZ = Mathf.Atan2(rot.y, rot.x) * Mathf.Rad2Deg;
                //firing
                if (fireTimer >= fireRate)
                {
                    GameObject projectile = Instantiate(bullet, transform.position, Quaternion.Euler(0, 0, RotZ - 90));
                    projectile.GetComponent<Bullet>().origin = "enemy";
                    fireTimer = 0;

                }
                //if the player leave the detect area, the enemy wil chase him
                if (!playerDetected)
                {
                    ChangeState(State.Chasing);
                }

                break;
            case State.Chasing:               
                //animator data
                anim.SetBool("moving", true);
                anim.SetInteger("moveIndex", _movementSide[moveDirection]);

                //the enemy will chase the player if him keep a certain distance, if he escapes, the enemy will return to patrol
                if (Vector2.Distance(transform.position, player.transform.position) < 8)
                {   
                    agent.SetDestination(player.transform.position);
                } else ChangeState(State.Idle);
                //if the player get behind a obstacle, will go to his last position to search for him
                if (obstacleCheck.collider == null) playerLastPosition = player.transform.position;                         
                else
                {
                    ChangeState(State.Checking);
                }    
                break;

            case State.Checking:
                //animator data
                anim.SetBool("moving", true);
                anim.SetInteger("moveIndex", _movementSide[moveDirection]);
                //if the enemy see the player in checking state, he will fire, if he not find anything, will return to patrol
                agent.SetDestination(playerLastPosition);
                if (playerDetected) ChangeState(State.Firing);
                if (Vector2.Distance(transform.position, agent.destination) < .5f && Vector2.Distance(transform.position, agent.destination) != 0)
                {
                    agent.ResetPath();
                    ChangeState(State.Idle);

                }

                break;
            case State.Dead:
                Destroy(gameObject);
                break;

        }
        //the exclamation mark will only appears during alert state
        GameObject exclamationMark = transform.GetChild(0).gameObject;
        exclamationMark.SetActive(CurrentState.Equals(State.Alert));
    }

    public enum State
    {
        Idle,
        Walking,
        Firing,
        Alert,
        Chasing,
        Checking,
        Dead,

    }

    private void OnDrawGizmosSelected()
    {
        //drawing the vision area
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, visionRadius);

        Vector3 right = Quaternion.Euler(0, 0, visionAngle / 2) * _rotation[moveDirection];
        Vector3 left = Quaternion.Euler(0, 0, -visionAngle / 2) * _rotation[moveDirection];

        Gizmos.color = Color.red;
        Gizmos.DrawLine (transform.position, transform.position + right * visionRadius);
        Gizmos.DrawLine(transform.position, transform.position + left * visionRadius);
    }

    public string getDirection(Vector2 velocity)
    {
        //getting the direction the character is looking
        if (Mathf.Abs(velocity.x) > Mathf.Abs(velocity.y))
        {
            return velocity.x > 0 ? "right" : "left";
        }
        else return velocity.y > 0 ? "up" : "down";

    }

    public void ChangeState(State nextState)
    {
        previousState = CurrentState;
        CurrentState = nextState;
        
    }
    
    public void TakeDamage(int damage)
    {
        life -= damage;
    }
}
