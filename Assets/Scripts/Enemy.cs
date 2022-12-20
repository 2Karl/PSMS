using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float health;
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        Move();
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
        health-=damage;
        if (health <= 0){
            Destroy(gameObject);
        }
    }

    

    

    
}
