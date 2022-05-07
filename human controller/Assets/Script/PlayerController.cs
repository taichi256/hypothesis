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
    public float notOnGroundAcceleration = 0.2f;
    public float notOnGroundSpeed = 0.0f;

    public float dush= 9.0f;
    public int dushLength = 6;
    bool goDush = false;
    int dushDirection = 0;

    //fixeUpdateRecorderはdush時のフレームを記録するためのint型の変数です
    int fixedUpdateRecorder =0;
    public int dushDecelerateTiming = 60;
    public int dushEndTiming = 80;
    public float startDushSpeed = 9.0f;
    float dushSpeed=0.0f;
    public float dushAcceleration = 0.25f;

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
        if (fixedUpdateRecorder==0)
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
        if (onGround)
        {
            notOnGroundSpeed = 0.0f;
            if (goRight)
            {
                rbody.velocity = new Vector2(speed, rbody.velocity.y);
                goRight = false;
                lastDirection = true;
            }
            if (goLeft)
            {
                rbody.velocity = new Vector2(-1 * speed, rbody.velocity.y);
                goLeft = false;
                lastDirection = false;
            }
        }
        else
        {
            if (notOnGroundSpeed == 0.0f)
            {
                notOnGroundSpeed = rbody.velocity.x;
            }
            if (goRight)
            {
                notOnGroundSpeed += notOnGroundAcceleration;
                rbody.velocity = new Vector2(notOnGroundSpeed, rbody.velocity.y);
                goRight = false;
                lastDirection = true;
            }
            if (goLeft)
            {
                notOnGroundSpeed -= notOnGroundAcceleration;
                rbody.velocity = new Vector2(notOnGroundSpeed, rbody.velocity.y);
                goLeft = false;
                lastDirection = false;
            }
        }


        //ジャンプの処理
        if (onGround && goJump||goAirJump && !onGround)
        {
            Vector2 jumpPw=new Vector2(0, 0); ;
            if (goJump)jumpPw = new Vector2(0, jump);
            if (goAirJump)jumpPw = new Vector2(0, airjump);

            rbody.AddForce(jumpPw, ForceMode2D.Impulse);
            goJump = false;
            goAirJump = false;
        }

        //ダッシュの処理
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
            rbody.velocity = new Vector2(0,0);
            rbody.bodyType = RigidbodyType2D.Kinematic;
            goDush = false;
            fixedUpdateRecorder = 1;
        }
        if (fixedUpdateRecorder != 0)
        {
            fixedUpdateRecorder++;
            if (fixedUpdateRecorder <= 1 + dushDecelerateTiming)
            {
                rbody.velocity = new Vector2(-1*dushDirection * startDushSpeed, 0);
            }
            if (fixedUpdateRecorder < 1 + dushEndTiming)
            {
                rbody.bodyType = RigidbodyType2D.Dynamic;
                dushSpeed -= dushAcceleration;
                rbody.velocity = new Vector2(-1*dushDirection * dushSpeed, 0);
            }
            if (fixedUpdateRecorder == 1 + dushEndTiming)
            {
                fixedUpdateRecorder = 0;
                rbody.velocity = new Vector2(rbody.velocity.x, 0);
                notOnGroundSpeed = 0.0f;
            }
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("grabable block"))
            {
                rbody.bodyType = RigidbodyType2D.Kinematic;
                rbody.velocity = new Vector2(0,0);
            }
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
