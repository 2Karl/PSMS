using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float health;
    [SerializeField] ParticleSystem bloodSplatter;
    [SerializeField] ParticleSystem bloodExplosion;
    [SerializeField] Animator enemyAnim;
    private GameObject player;
    private bool canMove = true;
    [SerializeField] private float damageDelaySeconds;
    private bool firstTimeTrigger = true;
    // Start is called before the first frame update
    private AudioSource enemySource;
    [SerializeField] private AudioClip yelp;
    [SerializeField] private AudioClip bark;
    
    void Start()
    {
        player = GameObject.Find("Player");
        enemySource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(MainManager.Instance.gameState == MainManager.GameState.death && firstTimeTrigger) {
            firstTimeTrigger = false;
            StartCoroutine(WaitToBark());
        }
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
        enemySource.PlayOneShot(yelp);
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
        MainManager.Instance.RemoveEnemy();
        Destroy(gameObject);
    }

    IEnumerator WaitToBark()
    {
        float seconds = Random.Range(0f, 2f);
        yield return new WaitForSeconds(seconds);
        Debug.Log(seconds);
        Bark();
    }

    void Bark()
    {
        enemySource.clip = bark;
        enemySource.loop = true;
        enemySource.Play();
        enemyAnim.SetBool("Bark_b", true);
        enemyAnim.SetFloat("Speed_f", 0f);
        canMove = false;
    }

    

    

    
}
