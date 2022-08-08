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

    //アビリティ回数上限の設定
    public int AirJumpLimit = 0;
    public int DashLimit = 0;
    public int GrappleLimit = 0;

    //アビリティ発生状況ののカウンタ
    public int AirJumpCount = 0;
    public int DashCount = 0;
    public int GrappleCount = 0;

    //fixeUpdateRecorderはdush時のフレームを記録するためのint型の変数です
    int fixedUpdateRecorder =0;
    public int dushDecelerateTiming = 50;
    public int dushOnGravityTiming = 60;
    public int dushEndTiming = 80;
    public float startDushSpeed = 4.0f;
    float dushSpeed=0.0f;
    public float dushAcceleration = 0.25f;
    float gravityScaleRecord=0;

    public int dushCooltime = 5;
    int lastDushRecord = 0;

    bool lastDirection = true;
    //仮設置の変数Direction
    float Direction = 0.0f;

    bool grab = false;
    public int grabCooltime=5;
    int lastGrabRecord=0;

    private GameObject mainCamera;
    private Rigidbody2D mainCameraRb;
    private bool rotateCameraForward;
    public bool onInvisibleWall;
    public bool rightCamera;
    private bool upCamera;
    public bool fall;
    mainCameraScript mainCameraScript;

    float firstPosX;
    float firstPosY;
    float firstCamPosX;
    float firstCamPosY;

    Vector3 vec;

    public Transform follow;
    private int _currentTarget = 0;
    public bool onUpWall;
    public bool outOfUpWall;

    int AttackTimeRecord = 0;
    public int AttackTimeMax = 100;
    bool goAttack = false;

    public Animator anim;

    public Dictionary<Movement,int> mov = new Dictionary<Movement,int>()
    {
        {Movement.AirJump,1},
        {Movement.Dush,1},
        {Movement.Grab,1}
    };

    public Dictionary<Movement,int> movRest= new Dictionary<Movement, int>()
    {
        {Movement.AirJump,0},
        {Movement.Dush,0},
        {Movement.Grab,0}
    };

    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        mainCamera = GameObject.Find("Main Camera");
        mainCameraRb = mainCamera.GetComponent<Rigidbody2D>();
        mainCameraScript = mainCamera.GetComponent<mainCameraScript>();
        firstPosX = this.transform.position.x;
        firstPosY = this.transform.position.y;
        firstCamPosX = mainCamera.transform.position.x;
        firstCamPosY = mainCamera.transform.position.y;
    }

    void Update()
    {
        //方向キーで左右移動、ジャンプをスペースキー
        //ダブルジャンプはNキー、ダッシュはBキー
        Direction = Input.GetAxisRaw("Horizontal");
        Vector3 scale = transform.localScale;
        if (Direction > 0) { GoRight(); anim.SetTrigger("Walk"); if (scale.x < 0) { scale.x = -scale.x; } }
        if (Direction < 0){GoLeft(); anim.SetTrigger("Walk"); if (scale.x > 0) { scale.x = -scale.x; } }
        transform.localScale = scale;
        if (Direction == 0)anim.SetTrigger("OffWalk");
        
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
        if (Input.GetButtonDown("AirJump") && (AirJumpCount > 0))
        {
            AirJump();
        }
        if (Input.GetButtonDown("Dush") && (DashCount > 0))
        {
            Dush();
        }
        if(Input.GetButtonDown("Attack"))
        {
            Attack();
        }
    }

    void FixedUpdate()
    {
        onGround = Physics2D.Linecast(transform.position - (transform.up * 0.1f)+transform.right*1/2, transform.position - (transform.up * 0.1f) - transform.right * 1/2, groundLayer);
        if (onGround)
        {
            anim.SetTrigger("OffJump");
            movRest[Movement.Grab] = mov[Movement.Grab];
            movRest[Movement.Dush] = mov[Movement.Dush];
            movRest[Movement.AirJump] = mov[Movement.AirJump];
            notOnGroundSpeed = 0.0f;
            if (goRight)
            {
                rbody.velocity = new Vector2(speed, rbody.velocity.y);
                goRight = false;
                lastDirection = true;
            }else if (goLeft)
            {
                rbody.velocity = new Vector2(-1 * speed, rbody.velocity.y);
                goLeft = false;
                lastDirection = false;
            }
        }
        else
        {
            
                notOnGroundSpeed = rbody.velocity.x;
            
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
        if ((onGround && goJump) || (goAirJump && !onGround))
        {
            if (goAirJump && !onGround)
            {
                rbody.velocity=new Vector2(rbody.velocity.x, 0);
            }
            else
            {
                anim.ResetTrigger("OffJump");
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
            fixedUpdateRecorder++;
            if (fixedUpdateRecorder <= 1 + dushDecelerateTiming)
            {
                rbody.velocity = new Vector2(dushDirection * startDushSpeed, 0);
            }else if (fixedUpdateRecorder<1+dushOnGravityTiming)
            {
                if (dushDecelerateTiming + 2 == fixedUpdateRecorder) lastDushRecord =1;
                dushSpeed -= dushAcceleration;
                rbody.velocity = new Vector2(dushDirection * dushSpeed, 0);
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
        //グラップルのクールタイムの処理
        if (lastGrabRecord != 0)
        {
            lastGrabRecord++;
            if (2 + grabCooltime == lastGrabRecord)
            {
                lastGrabRecord = 0;
            }
        }
        //ダッシュのクールタイムの処理
        if (lastDushRecord!= 0)
        {
            if (onGround != true) { lastDushRecord = 0; }
            else if (lastDushRecord == 1 + dushCooltime) { lastDushRecord = 0; }
            else { lastDushRecord++; }
        }

        //設置時のアビリティカウンタのリセット
        if (onGround == true)
        {
            AirJumpCount = AirJumpLimit;
            DashCount = DashLimit;
            GrappleCount = GrappleLimit;
            Debug.Log("AbilityCounter:Reset");
        }


        //カメラの処理かな
        /*Vector3 direction = new Vector3(Mathf.Sin(CameraRelativeRotation()), Mathf.Cos(CameraRelativeRotation()), 0);
        vec = direction * CameraSpeed() * Time.deltaTime;*/
        /*if (CameraRelativePosition().x >= -1 && CameraRelativePosition().x <= 1)
        {
            rightCamera = true;
            mainCameraScript.onInvisibleWall = false;
        }
        if (mainCameraScript.onInvisibleWall &&)
        {
            rightCamera = false;
        }*/
        if (outOfUpWall && onGround)
        {
            upCamera = false;
        }
        if(onUpWall)
        {
            upCamera = true;
        }
    }

    void LateUpdate()
    {
        if (upCamera)
        {
            mainCameraRb.velocity = new Vector2(speedFunctionX() * 3, speedFunctionY() * 3);
        }
        else
        {
            mainCameraRb.velocity = new Vector2(speedFunctionX() * 3, speedFunctionY() * 3);
        }
        /*if (rightCamera && !upCamera)
        {
            mainCameraRb.velocity = new Vector2(rbody.velocity.x, 0);
        }
        if (upCamera && !rightCamera)
        {
            mainCameraRb.velocity = new Vector2(0, rbody.velocity.y);
        }
        if (rightCamera && upCamera)
        {
            mainCameraRb.velocity = new Vector2(rbody.velocity.x, rbody.velocity.y);
        }*/
    }

    void dushEnd()
    {
        fixedUpdateRecorder = 0;
        notOnGroundSpeed = rbody.velocity.x;
    }
       
        void OnCollisionEnter2D(Collision2D collision)
        {
        if (collision.gameObject.CompareTag("grabable block") && fixedUpdateRecorder == 0 && movRest[Movement.Grab] != 0)
        {
            if (lastGrabRecord == 0)
            {
                mov[Movement.Grab] -= 1;
                lastGrabRecord++;
                rbody.bodyType = RigidbodyType2D.Kinematic;
                rbody.velocity = new Vector2(0, 0);
                grab = true;
            }
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
        if(collision.gameObject.CompareTag("GameOver"))
        {
            transform.position = new Vector2(firstPosX,firstPosY);
            mainCamera.transform.position = new Vector3(firstCamPosX, firstCamPosY, -10);
        }
        /*if(collision.gameObject.CompareTag("SkyGround"))
        {
            mainCameraCinema.Follow = this.transform;
        }*/
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("upWall"))
        {
            onUpWall = true;
            outOfUpWall = false;
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("upWall"))
        {
            outOfUpWall = true;
            onUpWall = false;
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


    void OkCamera()
    {
        rightCamera = true;
    }

    private float CameraRelativePositionX()
    {
        
        float relativePos = transform.position.x - mainCamera.transform.position.x;

        return relativePos;
    }

    private float CameraRelativePositionY()
    {
        float relativePos = transform.position.y - mainCamera.transform.position.y + 5;

        return relativePos;
    }

        //別クラスからの呼び出し用
        //ジャンプ→Jump、右移動GoRight、左移動GoLeft、ジャンプJump、ダブルジャンプAirJump
        //掴むGrab、走るDush
        public void Jump()
    {
        if (check(Movement.NoMovement) == false) return;
        anim.SetTrigger("Jump");
        goJump = true;
    }
    public void AirJump()
    {
        if (check(Movement.AirJump) == false) return;
        goAirJump = true;
    }
    public void GoRight()
    {
        if (check(Movement.NoMovement) == false) return;
        goRight = true;
    }
    public void GoLeft()
    {
        if (check(Movement.NoMovement) == false) return;
        goLeft = true;
    }
    public void Dush()
    {
        if (check(Movement.Dush) == false) return;
        if (lastDushRecord != 0) return;
        goDush = true;
    }
    public void Attack()
    {
        if (check(Movement.Dush) == false) return;
        goAttack = true;
        anim.SetTrigger("Attack");
        AttackTimeRecord = 1;
    }
    bool check(Movement thisMov)
    {
        if (thisMov != Movement.NoMovement && movRest[thisMov]==0) { return false; }
        else if(thisMov!=Movement.NoMovement){ movRest[thisMov] -= 1; }
        if (grab == true)
        {
            grab = false;
            rbody.bodyType = RigidbodyType2D.Dynamic;
        }
        if(fixedUpdateRecorder > 1 + dushOnGravityTiming)
        {
            dushEnd();
            return true;
        }else if (fixedUpdateRecorder == 0)
        {
            return true;
        }
        return false;
    }
    public enum Movement
    {
        Grab = 1,
        AirJump, Dush, NoMovement
    }

    float CameraRelativeRotation()
    {
        float rad = Mathf.Atan2(CameraRelativePositionX(), CameraRelativePositionY());
        float degree = rad * Mathf.Rad2Deg;

        return degree;
    }

    float speedFunctionX()
    {
        float symbol = Mathf.Abs(CameraRelativePositionX()) / CameraRelativePositionX();
        float speed = Mathf.Pow(CameraRelativePositionX(), 2f);
        speed = speed * symbol;

        if(Mathf.Abs(speed) > 0.00000001f)
        {
            return speed;
        }
        else
        {
            return 0f;
        }      
    }

    float speedFunctionY()
    {
        float symbol = Mathf.Abs(CameraRelativePositionY()) / CameraRelativePositionY();
        float speed = Mathf.Pow(Mathf.Abs(CameraRelativePositionY()), 2f);
        speed = speed * symbol;

        if (Mathf.Abs(speed) > 0.00000001f)
        {
            return speed;
        }
        else
        {
            return 0f;
        }
    }
}
