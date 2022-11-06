using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunMan : Enemy
{
    public int gunManHP = 1;
    GameObject bullet;
    public float bulletSpeed = 4.0f;
    float lastTime = 0.0f;
    public float gunCoolTime = 2.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        HP = gunManHP;
        bullet= (GameObject)Resources.Load("Bullet");
        base.Start();
    }
    void Move()
    {
        if (timeElapsed - lastTime >= gunCoolTime)
        {
            GameObject instance = (GameObject)Instantiate(bullet, new Vector3(transform.position.x,transform.position.y,0.0f), Quaternion.identity);
            lastTime = timeElapsed;
        }
    }
    // Update is called once per frame
    void Update()
    {
        base.Update();
        Move();
    }
}
