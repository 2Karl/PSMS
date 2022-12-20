using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBullet : MonoBehaviour
{
    private float speed = 40;
    private float xRange = 40;
    private float zRange = 20;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        if(Mathf.Abs(transform.position.x) > xRange || Mathf.Abs(transform.position.z) > zRange){
            Destroy(gameObject);
        }
    }

    
}
