using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    private Vector2 moveDirection;
    public float moveSpeed;
    public Animator anim;
    public Rigidbody2D rb;
    public InputActionReference move;
    public bool moving;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //changin' the idle animation to moving animation
        moving = (rb.linearVelocityX < 1 && rb.linearVelocityY < 1) && (rb.linearVelocityX > -1 && rb.linearVelocityY > -1) ? false : true;
        anim.SetBool("Moving", moving);
       

    }

    private void FixedUpdate()
    {
        WASDMove();
    }

    public void WASDMove()
    {
        //moving the character
        moveDirection = move.action.ReadValue<Vector2>();
        rb.linearVelocity = moveDirection.normalized * moveSpeed;
    }

}
