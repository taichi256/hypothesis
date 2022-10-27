using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1 : Enemy
{
    public int enemy1HP = 10;
    // Start is called before the first frame update
    void Start()
    {
        HP = 10;
        base.Start();
    }
    void Move()
    {
        base.Move();
    }
    // Update is called once per frame
    void Update()
    {
        base.Update();
    }
}
