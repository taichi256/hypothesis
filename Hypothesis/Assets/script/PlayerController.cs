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
    public float cameraSpeed;
    private bool rotateCameraForward;
    public bool onInvisibleWall;
    public bool rightCamera;
    private bool upCamera;
    mainCameraScript mainCameraScript;

    float firstPosX;
    float firstPosY;
    float firstCamPosX;
    float firstCamPosY;

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
        mainCamera = GameObject.Find("CM vcam1");
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

    void FixedUpdate()
    {
        onGround = Physics2D.Linecast(transform.position - (transform.up * 0.1f)+transform.right*1/2, transform.position - (transform.up * 0.1f) - transform.right * 1/2, groundLayer);
        if (onGround)
        {
           movRest[Movement.Grab] = mov[Movement.Grab];
            movRest[Movement.Dush] = mov[Movement.Dush];
            movRest[Movement.AirJump] = mov[Movement.AirJump];
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
        if (onGround && goJump || goAirJump && !onGround)
        {
            if(goAirJump && !onGround)
            {
                rbody.velocity=new Vector2(rbody.velocity.x, 0);
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
        //カメラの処理かな
        if (CameraRelativePosition().x >= -1)
        {
            rightCamera = true;
        }
        if (CameraRelativePosition().x <= -2)
        {
            rightCamera = false;
        }
        if (CameraRelativePosition().y >= 1)
        {
            upCamera = true;
        }
        if (CameraRelativePosition().y <= 0.5)
        {
            upCamera = false;
        }
        
    }

    void LateUpdate()
    {
        if (rightCamera && !upCamera)
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
        }
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
            Debug.Log("adada");
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

    private Vector2 CameraRelativePosition()
    {
        
        var relativePos = transform.position - mainCamera.transform.position;

        return relativePos;
    }

    private IEnumerator DelayCoroutine()
    {
        yield return null;
        rightCamera = true;
    }

        //別クラスからの呼び出し用
        //ジャンプ→Jump、右移動GoRight、左移動GoLeft、ジャンプJump、ダブルジャンプAirJump
        //掴むGrab、走るDush
        public void Jump()
    {
        if (check(Movement.NoMovement) == false) return;
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
}
