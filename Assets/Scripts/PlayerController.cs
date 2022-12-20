using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Animator playerAnim;
    [SerializeField] private float speed = 10.0f;
    private Vector3 direction = new Vector3(0.0f,0.0f,0.0f);
    private Rigidbody playerRb;
    private float xBound = 13;
    private float zBound = 7;
    public GameObject bulletPrefab;
    private bool canFire = true;
    private float fireRate = 0.5f;
    private bool gameOver = false;
    
    private Vector3 bulletOffset = new Vector3 (0.1f, 1.58f, 1f);
    // Start is called before the first frame update
    void Start()
    {
        //playerAnim = GetComponent<SimplePeople_Man_Business_White>();
        Debug.Log("Paul Smith must Survive!");
        playerRb = GetComponent<Rigidbody>();
        playerAnim.SetInteger("WeaponType_int", 1);

        
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        TurnToFace();
        ConstrainPlayerPosition();
        if (Input.GetMouseButton(0) && canFire){
            Debug.Log(fireRate);
            SpawnBullet();
        }
    }


    IEnumerator BulletCooldown()
    {
        yield return new WaitForSeconds(fireRate);
        canFire = true;

    }

    void SpawnBullet()
    {
        Instantiate(bulletPrefab, GenerateBulletPos(), transform.rotation);
        canFire = false;
        StartCoroutine(BulletCooldown());
    }
    //point in the direction of the mouse
    void TurnToFace()
    {
        // Translate mouse coordinates to a point in the world
        Vector3 mousePos = Input.mousePosition;
        Vector3 point = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 15));
        Vector3 turnDirection = point - transform.position;
        turnDirection.y = 0;
        turnDirection.Normalize();
        transform.forward = turnDirection;
    }
    // Moves the player
    void MovePlayer()
    {        
        direction.x = Input.GetAxis("Horizontal");
        direction.z = Input.GetAxis("Vertical");
        // Prevent the size of the direction vector for exceeding 1
        direction = Vector3.ClampMagnitude(direction, 1.0f);
        playerAnim.SetBool("Static_b", true);
        playerAnim.SetFloat("Speed_f", direction.magnitude);
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    // Prevents the player from going out of bounds
    void ConstrainPlayerPosition()
    {
        if(transform.position.z < -zBound) {
            transform.position = new Vector3(transform.position.x, transform.position.y, -zBound);
        }
        else if (transform.position.z > zBound) {
            transform.position = new Vector3(transform.position.x, transform.position.y, zBound);
        }
        if (transform.position.x > xBound) {
            transform.position = new Vector3(xBound, transform.position.y, transform.position.z);
        }
        else if (transform.position.x < -xBound) {
            transform.position = new Vector3(-xBound, transform.position.y, transform.position.z);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Powerup")){
            canFire = true;
            fireRate /= 3;
            StartCoroutine(PowerUpCooldown());
            Destroy(other.gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy")) {
            gameOver = true;
            Debug.Log("Paul Smith did not Survive");
        }
    }

    IEnumerator PowerUpCooldown()
    {
        yield return new WaitForSeconds(5);
        fireRate *= 3;
    }

    Vector3 GenerateBulletPos()
    {
        Vector3 bulletPos = transform.position + transform.forward * bulletOffset.z;
        bulletPos.y = bulletOffset.y;
        bulletPos += transform.right * bulletOffset.x;
        return bulletPos;
        
    }
}
