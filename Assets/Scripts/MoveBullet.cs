using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBullet : MonoBehaviour
{
    private float speed = 40;
    private float xRange = 40;
    private float zRange = 20;
    [SerializeField] private float damage = 1.0f;

    void Update()
    {
        MoveForward();
    }

    void MoveForward()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        if(Mathf.Abs(transform.position.x) > xRange || Mathf.Abs(transform.position.z) > zRange){
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // should probably have some particle effects for blood splatter in here.
        if(other.CompareTag("Enemy")){
            Destroy(gameObject);
            other.GetComponent<Enemy>().TakeDamage(damage);
        }
    }
    
}
