using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anchor : MonoBehaviour
{
    [SerializeField]
    float anchorSpeed = 5f;

    [SerializeField]
    float destroyTime;

    GameObject player;
    Rigidbody2D rb;

    float time;
    bool grapping=false;


    Vector3 mouseWorld;

    Vector2 targetPos;



    void OnEnable()
    {
        player=GameObject.Find("robo");
        this.transform.position = player.transform.position+new Vector3(0,2,0);
        GetComponent<Rigidbody2D>().bodyType=RigidbodyType2D.Dynamic;
        time = 0;
       
        mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetPos = Vector3.Scale(mouseWorld - transform.position,new Vector3(1,1,0)).normalized;

        GetComponent<Rigidbody2D>().velocity = targetPos*anchorSpeed;
    }

    private void Update()
    {
        time += 1 * Time.deltaTime;
        
        if (Input.GetButtonDown("Jump"))
        {
            this.gameObject.SetActive(false);
            grapping = false;
        }   
        else if (time > destroyTime && !grapping)
        {
            this.gameObject.SetActive(false);
        }
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Grabbable")
        {
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            grapping = true;
        }
    }
}
