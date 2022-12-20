using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public int health;
    private Rigidbody enemyRb;
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        enemyRb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        transform.forward = GetDirection();
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void TakeDamage()
    {
        health--;
        if (health == 0){
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Bullet")){
            Destroy(other.gameObject);
            TakeDamage();
            Debug.Log("Collision");
        }
    }

    Vector3 GetDirection()
    {
        Vector3 playerPos = player.transform.position;
        playerPos.y = 0;
        Vector3 pos = transform.position;
        pos.y = 0;
        return playerPos - pos;
    }

    
}
