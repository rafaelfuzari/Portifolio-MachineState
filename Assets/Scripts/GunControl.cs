using UnityEngine;
using UnityEngine.InputSystem;

public class GunControl : MonoBehaviour
{
    public Vector3 mousePos;
    public float rotZ;
    public Vector3 rot;
    public InputActionReference fire;
    public GameObject bullet;
    public GameObject gunBarrel;
    public Animator anim;
    public Transform player;
    
    void Start()
    {
        player = transform.parent.gameObject.GetComponent<Transform>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //Rotating the weapon on the mouse direction
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        rot = mousePos - transform.position;
        rotZ = Mathf.Atan2(rot.y, rot.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotZ);
        
        //flipping the weapon and the character depending on the angle
        player.gameObject.GetComponent<SpriteRenderer>().flipX = rotZ > 90 || rotZ < -90 ? true : false;
        gameObject.GetComponent <SpriteRenderer>().flipY = rotZ > 90 || rotZ < -90 ? true : false;
    }

    public void OnEnable()
    {
        fire.action.started += Fire;
    }

    public void OnDisable()
    {
        fire.action.started -= Fire;
    }
    public void Fire(InputAction.CallbackContext obj)
    {
        //instantianting the bullet
        var projectile = Instantiate(bullet,gunBarrel.transform.position, Quaternion.Euler(0, 0, rotZ - 90));
       
        projectile.GetComponent<Bullet>().dir = rot;
        anim.SetTrigger("Shot");
        
    }
}
