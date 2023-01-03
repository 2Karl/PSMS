using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Animator playerAnim;
    [SerializeField] private float speed = 10.0f;
    [SerializeField] private GameObject bulletPrefab;
    private Vector3 direction = new Vector3(0.0f,0.0f,0.0f);
    private float xBound = 13;
    private float zBound = 7;
    private bool canFire = true;
    private float fireRate = 0.5f;
    private Vector3 bulletOffset = new Vector3 (0.1f, 1.58f, 1f); // kinda hacky; do not like
    private AudioSource playerAudio;
    [SerializeField] private AudioClip gunshot;
    [SerializeField] private AudioClip[] deathSounds;


    void Start()
    {
        Debug.Log("Paul Smith must Survive!");
        // Set up animation flags
        playerAnim.SetInteger("WeaponType_int", 1);
        playerAnim.SetBool("Static_b", true);
        playerAudio = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (MainManager.Instance.gameState == MainManager.GameState.playing) {
            MovePlayer();
            TurnToFace();
            ConstrainPlayerPosition();
            HandleInputs();
        }
    }
    void MovePlayer()
    {        
        // Hopefully I'll be able to use the direction vector to set the animations
        direction.x = Input.GetAxis("Horizontal");
        direction.z = Input.GetAxis("Vertical");
        // Prevent the size of the direction vector for exceeding 1
        direction = Vector3.ClampMagnitude(direction, 1.0f);
        // Set the speed of the animation - consider farming this out to dedicated method
        playerAnim.SetFloat("Speed_f", direction.magnitude);
        // We move relative to world space - otherwise you get tank controls.
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    void HandleInputs()
    {
        if (Input.GetMouseButton(0) && canFire){
            Shoot();
        }
    }
    
    void Shoot()
    {
        
        // Consider object pooling for bullets
        playerAudio.PlayOneShot(gunshot);
        Instantiate(bulletPrefab, GenerateBulletPos(), transform.rotation);
        canFire = false;
        StartCoroutine(BulletCooldown());
        
    }

    Vector3 GenerateBulletPos()
    {
        Vector3 bulletPos = transform.position + transform.forward * bulletOffset.z;
        bulletPos += transform.right * bulletOffset.x;
        // Adjust the y offset based on how fast we're moving to adjust for animation changing the gun position
        // This feels hacky - rework this when I know more about animations!
        bulletPos.y = bulletOffset.y - (float)(0.3 * direction.magnitude);
        return bulletPos;
    }

    IEnumerator BulletCooldown()
    {
        yield return new WaitForSeconds(fireRate);
        canFire = true;
    }
    
    //point in the direction of the mouse
    void TurnToFace()
    {
        // Translate mouse coordinates to a point in the world
        Vector3 mousePos = Input.mousePosition;
        Vector3 point = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 15));
        // get faving based on position and mouse location
        Vector3 turnDirection = point - transform.position;
        turnDirection.y = 0;
        turnDirection.Normalize();
        transform.forward = turnDirection;
    }
    // Moves the player
    

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
        // So far the only power up increases fire rate. This will need to change later.
        if(other.CompareTag("Powerup")){
            Time.timeScale = 0.5f;
            fireRate /= 3;
            StartCoroutine(PowerUpCooldown());
            Destroy(other.gameObject);
            playerAudio.pitch = 0.5f;
            
        }
    }

    IEnumerator PowerUpCooldown()
    {
        // Power up lasts for 5 seconds - this will change depending on powerup
        yield return new WaitForSeconds(5);
        fireRate *= 3;
        Time.timeScale = 1f;
        playerAudio.pitch = 1f;
        
    }

    void OnCollisionEnter(Collision collision)
    {
        // This needs to do more
        if (collision.gameObject.CompareTag("Enemy") && MainManager.Instance.gameState != MainManager.GameState.death) {
            KillSelf();
        }
    }

    void KillSelf()
    {
        playerAnim.SetInteger("WeaponType_int", 0);
        playerAnim.SetInteger("DeathType_int", Random.Range(1,3));
        playerAudio.clip = deathSounds[Random.Range(0,deathSounds.Length)];
        playerAudio.Play();
        playerAnim.SetBool("Death_b", true);
        MainManager.Instance.GameOver();
    }
}
