using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anchor : MonoBehaviour
{
    [SerializeField]
    float anchorSpeed = 5f;

    [SerializeField]
    float destroyTime;


    [SerializeField]
    GameObject player;

    float time;
    bool grapping=false;


    Vector2 mouseWorld;
    Vector2 transform2;
    Vector2 targetPos;



    void Start()
    {
        grapping = false;

        this.transform.position = player.transform.position;
        transform2 = this.transform.position;
        mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetPos = mouseWorld - transform2;

        GetComponent<Rigidbody2D>().velocity = targetPos*anchorSpeed;
    }

    private void Update()
    {

        if (Input.GetButtonDown("Jump"))
        {
            this.gameObject.SetActive(false);
            grapping = false;
        }   
        else if (time > destroyTime && !grapping)
        {
            

        }
    }

    private void FixedUpdate()
    {
        time++;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Grabbable")
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            grapping = true;
        }
    }
}
