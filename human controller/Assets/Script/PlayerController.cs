using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rbody;

    bool goJump = false;
    bool onGround = false;
    bool goAirJump = false;
    public LayerMask groundLayer;
    public float jump = 9.0f;
    public float airjump = 12.0f;

    public float speed = 3.0f;
    bool goRight = false;
    bool goLeft = false;

    public float dush= 9.0f;
    public int dushLength = 6;
    bool goDush = false;
    bool stopDush = false;
    float dushPositionX = 0;
    float dushPositionY = 0;
    int dushDirection = 0;

    bool lastDirection = true;

    //仮設置の変数Direction
    float Direction = 0.0f;

    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        //仮設置→方向キーで左右移動、ジャンプをスペースキー
        //ダブルジャンプはNキー、ダッシュはBキー
        Direction = Input.GetAxisRaw("Horizontal");
        if (!stopDush)
        {
            if (Direction > 0) GoRight();
            if (Direction < 0) GoLeft();
            if (Input.GetButtonDown("Jump"))
            {
                Jump();
            }
            if (Input.GetButtonDown("AirJump"))
            {
                AirJump();
            }
            if (Input.GetButtonDown("Dush"))
            {
                Dush();
            }
        }
        //ここまで仮設置
    }

    void FixedUpdate()
    {
        onGround = Physics2D.Linecast(transform.position, transform.position - (transform.up * 0.1f), groundLayer);
        if (goRight) {
            rbody.velocity = new Vector2(speed, rbody.velocity.y);
            goRight = false;
            lastDirection = true;
        }
        if (goLeft) {
            rbody.velocity = new Vector2(-1 * speed, rbody.velocity.y);
            goLeft = false;
            lastDirection = false;
        }
        if (onGround && goJump||goAirJump && !onGround)
        {
            Vector2 jumpPw=new Vector2(0, 0); ;
            if (goJump)jumpPw = new Vector2(0, jump);
            if (goAirJump)jumpPw = new Vector2(0, airjump);

            rbody.AddForce(jumpPw, ForceMode2D.Impulse);
            goJump = false;
            goAirJump = false;
        }
        if (goDush)
        {   
            dushDirection = 0;
            if (lastDirection == true)
            {
                dushDirection = 1;
            }
            else
            {
                dushDirection = -1;
            }
            Vector2 dushPw = new Vector2(dushDirection * dush,0);
            rbody.velocity = new Vector2(0,0);
            rbody.AddForce(dushPw, ForceMode2D.Impulse);
            rbody.bodyType = RigidbodyType2D.Kinematic;
            goDush = false;
            stopDush = true;
            dushPositionX = transform.position.x;
            dushPositionY = transform.position.y;
        }
        if(stopDush && new Vector2((int)transform.position.x,(int)transform.position.y)== new Vector2((int)dushPositionX,(int)dushPositionY) + new Vector2(dushDirection*dushLength, 0))
        {;
            rbody.bodyType = RigidbodyType2D.Dynamic;
            rbody.velocity = new Vector2(0,0);
            stopDush = false;
        }
    }

//別クラスからの呼び出し用
//ジャンプ→Jump、右移動GoRight、左移動GoLeft、ジャンプJump、ダブルジャンプAirJump
//掴むGrab、走るDush
    public void Jump()
    {
        goJump = true;
    }
    public void AirJump()
    {
        goAirJump = true;
    }
    public void GoRight()
    {
        goRight = true;
    }
    public void GoLeft()
    {
        goLeft = true;
    }
    public void Dush()
    {
        goDush = true;
    }
}
