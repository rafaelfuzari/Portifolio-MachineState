using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    public State CurrentState;
    public State previousState;
    public Transform[] waypoints;
    public int currentWaypoint;
    public float speed;
    public float waitTimer;
    public string rotation;
    public float visionRadius;
    public LayerMask playerLayer;
    public LayerMask obstacleLayer;
    public float visionAngle;
    public Rigidbody2D rb;
    public Animator anim;
    public SpriteRenderer spriteRenderer;
    public GameObject player;
    public bool playerDetected;
    public float alertTime;
    Dictionary<string, Vector3> _rotation = new Dictionary<string, Vector3>();
    Dictionary<string, int> _movementSide = new Dictionary<string, int>();
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
        CurrentState = State.Walking;
        speed /= 10;
        
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
        rotation = getDirection(rb.linearVelocity);
    }

    // Update is called once per frame
    void Update()
    {

        //flipping the sprite depending if its going to right or left
        spriteRenderer.flipX = rotation.Equals("right") ? true : false;


        //detecting the player on a circle area
        Collider2D player = Physics2D.OverlapCircle(transform.position, visionRadius, playerLayer);
        if (player != null)
        {
            Vector2 playerDirection = (player.transform.position - transform.position).normalized;

            float angle = Vector2.Angle(_rotation[rotation], playerDirection);
            //detecting the player on a cone area
            if (angle < visionAngle / 2)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, playerDirection, visionRadius, obstacleLayer);

                //detecting the player is not behind a obstacle or wall, if not, will go to alert state
                if (hit.collider == null)
                {
                    if (playerDetected) return;
                    ChangeState(State.Alert);
                    alertTime = 0;
                    playerDetected = true;
                    
                }else if (playerDetected) playerDetected = false;
                

                    
            }else playerDetected = false;

        }
        else playerDetected = false;


    }
    private void FixedUpdate()
    {
        switch (CurrentState)
        {
           //in idle state, the character will remain stopped for a few seconds, before start walking again
            case State.Idle:
                anim.SetBool("moving", false);
                rb.linearVelocity = Vector2.zero;
               
                waitTimer += Time.deltaTime;
                
                if (waitTimer >= 2)
                {
                    currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
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
                      
                        
                        Vector2 direction = (waypoints[currentWaypoint].position - transform.position).normalized;
                        rb.linearVelocity = direction * speed;
                        rotation = getDirection(rb.linearVelocity);
                        anim.SetInteger("moveIndex", _movementSide[rotation]);
                        anim.SetBool("moving", true);
                        


                    }
                    else
                    {
                        ChangeState(State.Idle);
                        
                    }
                }
                
                break;
            case State.Alert:
               //in state alert, the character will look to the player, and if he stay on his vision for one second, the character will start shooting
                GameObject exclamationMark = transform.GetChild(0).gameObject;
                anim.SetBool("moving", false);
                rb.linearVelocity = Vector2.zero;
                exclamationMark.SetActive(true);    
                rotation = getDirection(player.transform.position - transform.position);
                anim.SetInteger("moveIndex", _movementSide[rotation]);
                alertTime += Time.deltaTime;
                
                if (alertTime >= 1f)
                {
                    exclamationMark.SetActive(false);
                    ChangeState(State.Firing);
                }
                if (!playerDetected) {
                    ChangeState(previousState);
                    exclamationMark.SetActive(false);
                }


                break;
            case State.Firing:
                break;
            case State.Chasing:
                break;
            case State.Returning:
                break;
            case State.Dead:
                break;

        }
    }

    public enum State
    {
        Idle,
        Walking,
        Firing,
        Alert,
        Chasing,
        Returning,
        Dead,

    }

    private void OnDrawGizmosSelected()
    {
        //drawing the vision area
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, visionRadius);

        Vector3 right = Quaternion.Euler(0, 0, visionAngle / 2) * _rotation[rotation];
        Vector3 left = Quaternion.Euler(0, 0, -visionAngle / 2) * _rotation[rotation];

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
}
