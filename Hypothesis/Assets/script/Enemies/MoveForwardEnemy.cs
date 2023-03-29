using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForwardEnemy : Enemy
{
    Rigidbody2D rbody;
    public float moveDirection = -1;
    public float speed = 2.0f;
    public bool isDestroy = false;
    public float destroyTime=50000f;
    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        HP = 100;
        base.Start();
    }

    void Move()
    {
        rbody.velocity= new Vector2(moveDirection * speed,rbody.velocity.y);
        destroyTime--;
        if(destroyTime < 1)
        {
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        base.Update();
        Move();
    }
}
