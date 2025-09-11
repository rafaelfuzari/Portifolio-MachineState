using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float force;
    public Rigidbody2D rb;
    public Vector3 dir;
    public string origin;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //The bullet always go foward
        rb.linearVelocity = transform.up * force;

    }

    // Update is called once per frame
    void Update()
    {
       
        
    }
    private void FixedUpdate()
    {
       //raycast to check what it hits
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, 0.5f);
        if (hit.collider != null)
        {
            //checking if it hit a wall
            if (hit.collider.gameObject.CompareTag("Wall"))
            {
                Debug.Log("acertei uma parede");
                Destroy(this.gameObject);
            }
            if (hit.collider.gameObject.CompareTag("Player") && origin.Equals("enemy")){
                
                hit.collider.GetComponent<PlayerControl>().TakeDamage(1);
                Destroy(this.gameObject);
            }
            if (hit.collider.gameObject.CompareTag("Enemy") && origin.Equals("player"))
            {
                hit.collider.GetComponent<EnemyControl>().TakeDamage(1);
                Destroy(this.gameObject);
            }

        }
    }

    private void OnDrawGizmos()
    {
        Vector2 debugDir = transform.up;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, debugDir * 0.5f);
    }
}
