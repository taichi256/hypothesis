using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mainCameraScript : MonoBehaviour
{
    public bool onInvisibleWall;
    private Rigidbody2D cameraRb;
    // Start is called before the first frame update
    void Start()
    {
        cameraRb = GetComponent<Rigidbody2D>();
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "invisibleWall")
        {
            onInvisibleWall = true;
        }
    }
}
