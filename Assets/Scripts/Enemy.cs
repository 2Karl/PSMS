using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float health;
    [SerializeField] ParticleSystem bloodSplatter;
    [SerializeField] ParticleSystem bloodExplosion;
    private GameObject player;
    private bool canMove = true;
    [SerializeField] private float damageDelaySeconds;
    // Start is called before the first frame update
    
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if( canMove) {
            Move();
        }
        // Chance for unique behaviour - howl/bark?
    }

    void Move()
    {
        transform.forward = GetDirection();
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    Vector3 GetDirection()
    {
        Vector3 playerPos = player.transform.position;
        Vector3 pos = transform.position;
        // flatten the y values - There might be a better way of doing this.
        playerPos.y = 0;
        pos.y = 0;
        return playerPos - pos;
    }
    public void TakeDamage(float damage=1.0f)
    {
        canMove = false;
        StartCoroutine(PauseOnDamage(damageDelaySeconds));
        bloodSplatter.transform.forward = GetDirection();
        bloodSplatter.Play();
        health-=damage;
        if (health <= 0){
            KillSelf();
        }
    }

    IEnumerator PauseOnDamage(float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        canMove = true;
    }

    void KillSelf()
    {
        Instantiate(bloodExplosion, transform.position, bloodExplosion.transform.rotation);
        Destroy(gameObject);
    }

    

    

    
}
