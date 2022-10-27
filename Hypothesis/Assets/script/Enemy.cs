using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //継承元のクラスです敵に付けるscriptは全てこのクラスを継承してください
    //このクラスにあるメソッドを定義する場合super()で元のメソッドを必ず呼び出してください。
    //HPをStartメソッドで設定してください。
    public int HP;
    public float coolTime = 3.0f;
    public int damage = 1;
    float lastAttackedTime;

    // Start is called before the first frame update
    protected void Start()
    {
        lastAttackedTime = 0f;
    }

    // Update is called once per frame
    protected void Update()
    {
        Move();
    }
    protected void Move()
    {

    }
    protected void OnCollisionStay2D(Collision2D collision)
    {
        GameObject touchedObject = collision.gameObject;
        if (touchedObject.CompareTag("Attack"))
        {
            float thisTime = Time.time;
            if (thisTime >= lastAttackedTime + coolTime)
            {
                HP -= damage;
                lastAttackedTime = thisTime;
            }
        }
    }
}
