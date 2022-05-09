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

    bool goDush = false;
    int dushDirection = 0;

    //fixeUpdateRecorderはdush時のフレームを記録するためのint型の変数です
    int fixedUpdateRecorder =0;
    public int dushDecelerateTiming = 60;
    public int dushEndTiming = 80;
    public float startDushSpeed = 9.0f;
    float dushSpeed=0.0f;
    public float dushAcceleration = 0.25f;
    float gravityScaleRecord=0;

    bool lastDirection = true;
    //仮設置の変数Direction
    float Direction = 0.0f;

    bool grab = false;

    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        //仮設置→方向キーで左右移動、ジャンプをスペースキー
        //ダブルジャンプはNキー、ダッシュはBキー
        Direction = Input.GetAxisRaw("Horizontal");
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
        if (onGround && goJump || goAirJump && !onGround)
        {
            if(goAirJump && !onGround)
            {
                rbody.velocity=new Vector2(0, 0);
            }
            Vector2 jumpPw = new Vector2(0, 0); ;
            if (goJump) jumpPw = new Vector2(0, jump);
            if (goAirJump) jumpPw = new Vector2(0, airjump);
            notOnGroundSpeed = 0.0f;

            rbody.AddForce(jumpPw, ForceMode2D.Impulse);
            goJump = false;
            goAirJump = false;
        }

        //ダッシュの処理
        if (goDush)
        {
            dushDirection = 0;
            gravityScaleRecord = rbody.gravityScale;
            rbody.gravityScale = 0;
            if (lastDirection == true)
            {
                dushDirection = 1;
            }
            else
            {
                dushDirection = -1;
            }
            rbody.velocity = new Vector2(0,0);
            goDush = false;
            fixedUpdateRecorder = 1;
            dushSpeed = startDushSpeed;
        }
        if (fixedUpdateRecorder != 0)
        {
            Debug.Log(rbody.velocity);
            fixedUpdateRecorder++;
            if (fixedUpdateRecorder <= 1 + dushDecelerateTiming)
            {
                rbody.velocity = new Vector2(dushDirection * startDushSpeed, 0);
            }
            else if (fixedUpdateRecorder < 1 + dushEndTiming)
            {
                dushSpeed -= dushAcceleration;
                rbody.velocity = new Vector2(dushDirection * dushSpeed, rbody.velocity.y);
                rbody.gravityScale = gravityScaleRecord;
            }
            if (fixedUpdateRecorder == 1 + dushEndTiming)
            {
                dushEnd();
            }
        }
    }
    void dushEnd()
    {
        fixedUpdateRecorder = 0;
        rbody.velocity = new Vector2(0, 0);
        notOnGroundSpeed = 0.0f;
    }
       
        void OnCollisionEnter2D(Collision2D collision)
        {
        if (collision.gameObject.CompareTag("grabable block")&& fixedUpdateRecorder == 0)
        {
            rbody.bodyType = RigidbodyType2D.Kinematic;
            rbody.velocity = new Vector2(0, 0);
            grab = true;
        }
        if (transform.parent == null && collision.gameObject.CompareTag("move block")&& fixedUpdateRecorder == 0)
        {
            Vector2 hitPos = collision.contacts[0].point;
            if (hitPos.y <= rbody.transform.position.y)
            {
                var emptyObject = new GameObject();
                emptyObject.transform.parent = collision.gameObject.transform;
                transform.parent = emptyObject.transform;
            }
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        if (transform.parent != null && collision.gameObject.CompareTag("move block"))
        {
            Vector2 hitPos = collision.contacts[0].point;
            if (hitPos.y <= rbody.transform.position.y)
            {
                transform.parent = null;
            }
        }
    }

        //別クラスからの呼び出し用
        //ジャンプ→Jump、右移動GoRight、左移動GoLeft、ジャンプJump、ダブルジャンプAirJump
        //掴むGrab、走るDush
        public void Jump()
    {
        if (check() == false) return;
        goJump = true;
    }
    public void AirJump()
    {
        if (check() == false) return;
        goAirJump = true;
    }
    public void GoRight()
    {
        if (check() == false) return;
        goRight = true;
    }
    public void GoLeft()
    {
        if (check() == false) return;
        goLeft = true;
    }
    public void Dush()
    {
        if (check() == false) return;
        goDush = true;
    }

    bool check()
    { 
            if (grab == true)
        {
            grab = false;
            rbody.bodyType = RigidbodyType2D.Dynamic;
        }
        if(fixedUpdateRecorder > 1 + dushDecelerateTiming)
        {
            dushEnd();
            return true;
        }else if (fixedUpdateRecorder == 0)
        {
            return true;
        }
        return false;
    }
}
