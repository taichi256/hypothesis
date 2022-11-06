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

    //未使用
    /*//アビリティ発生状況ののカウンタ
    public int AirJumpCount = 0;
    public int DashCount = 0;
    public int GrappleCount = 0;*/

    //fixeUpdateRecorderはdush時のフレームを記録するためのint型の変数です
    public int fixedUpdateRecorder =0;
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
    public GameObject talk;
    private GameObject another;
    private bool rotateCameraForward;
    public bool fall;
    mainCameraScript mainCameraScript;

    float firstPosX;
    float firstPosY;
    float firstCamPosX;
    float firstCamPosY;
    public bool upCamera=false;
    private bool onUpWall=false;

    Vector3 vec;

    public Transform follow;
    private int _currentTarget = 0;

    int attackTimeRecord = 0;
    public int attackTimeMax = 100;
    public int attackDamageTiming = 50;
    bool goAttack = false;

    public string playerTag = "Player";
    public string attackTag = "Attack";
    public Animator anim;

    public bool inTalkBox = false;

    AbilityCount abilityCount;

    //未使用
    /*public Dictionary<Movement,int> mov = new Dictionary<Movement,int>()
    {
        {Movement.AirJump,1},
        {Movement.Dash,1},
        {Movement.Grab,1}
    };*/

    public Dictionary<Movement,int> movRest= new Dictionary<Movement, int>()
    {
        {Movement.AirJump,0},
        {Movement.Dash,0},
        {Movement.Grab,0}
    };

    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        mainCamera = GameObject.Find("Main Camera");
        mainCameraRb = mainCamera.GetComponent<Rigidbody2D>();
        mainCameraScript = mainCamera.GetComponent<mainCameraScript>();
        talk = GameObject.Find("Talk");
        this.transform.position = new Vector2(PlayerPrefs.GetFloat("PLAYERPOSX", -25), PlayerPrefs.GetFloat("PLAYERPOSY", -2));
        mainCamera.transform.position = new Vector3(PlayerPrefs.GetFloat("CAMPOSX", -22), PlayerPrefs.GetFloat("CAMPOSY", 3), PlayerPrefs.GetFloat("CAMPOSZ", -10));
        firstPosX = -25;
        firstPosY = -2;
        firstCamPosX = -22;
        firstCamPosY = 3;
        upCamera=false;

        GameObject QM = GameObject.Find("Main Camera/QM");
        GameObject QMPanel = QM.GetComponent<Transform>().transform.GetChild(0).gameObject;
        GameObject abilityPanel= QMPanel.GetComponent<Transform>().transform.GetChild(0).gameObject;
        GameObject abilityManager=abilityPanel.GetComponent<Transform>().transform.GetChild(0).gameObject;
        abilityCount = (AbilityCount)(abilityManager.GetComponent<AbilityCount>());
        abilityCount.AJLimit = PlayerPrefs.GetInt("AIRJUMP", 0);
        abilityCount.DashLimit = PlayerPrefs.GetInt("DASH", 0);
        abilityCount.GrappleLimit = PlayerPrefs.GetInt("GRAPPLE", 0);
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
        if (Input.GetButtonDown("AirJump"))
        {
            AirJump();
        }
        if (Input.GetButtonDown("Dush"))
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
            movRest[Movement.Grab] = abilityCount.GrappleLimit;
            movRest[Movement.Dash] = abilityCount.DashLimit;
            movRest[Movement.AirJump] = abilityCount.AJLimit;
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
        //動作~limit系の変数は使用していません。dictionaryのmovRestを使用する為ここはコメントアウトします。
        /*if (onGround == true)
        {
            AirJumpCount = AirJumpLimit;
            DashCount = DashLimit;
            GrappleCount = GrappleLimit;
            //Debug.Log("AbilityCounter:Reset");
        }*/
        if (attackTimeRecord != 0)
        {
            attackTimeRecord++;
            if (attackTimeRecord == attackDamageTiming+1)
            {
                gameObject.tag = attackTag;
            }else if (attackTimeRecord == attackDamageTiming + 2)
            {
                gameObject.tag = playerTag;
            }else if (attackTimeMax+1 == attackTimeRecord)
            {
                attackTimeRecord = 0;
                rbody.gravityScale = gravityScaleRecord;
                gameObject.tag = playerTag;
            }
        }
    }

    void LateUpdate()
    {
        if (!onUpWall && onGround && mainCameraScript.finishedMove)
        {
            upCamera = false;
        }
        if (onUpWall)
        {
            upCamera = true;
            mainCameraScript.finishedMove = false;
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
                movRest[Movement.Grab] -= 1;
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
        if (check(Movement.Dash) == false) return;
        if (lastDushRecord != 0) return;
        goDush = true;
    }
    public void Attack()
    {
        if (check(Movement.NoMovement) == false) return;
        goAttack = true;
        anim.SetTrigger("Attack");
        gravityScaleRecord = rbody.gravityScale;
        rbody.gravityScale = 0;
        attackTimeRecord = 1;
        rbody.velocity = new Vector2(0,0);
    }
    bool check(Movement thisMov)
    {
        if (attackTimeRecord != 0)return false;
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
        AirJump, Dash, NoMovement
    }

    float CameraRelativeRotation()
    {
        float rad = Mathf.Atan2(CameraRelativePositionX(), CameraRelativePositionY());
        float degree = rad * Mathf.Rad2Deg;

        return degree;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("upWall"))
        {
            onUpWall = true;
        }
        if(other.gameObject.CompareTag("Talk"))
        {
            inTalkBox = true;
            another = other.transform.parent.gameObject;
            talk.transform.position = new Vector3(another.transform.position.x + 5.4f, another.transform.position.y + 0.25f , another.transform.position.z);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("upWall"))
        {
            onUpWall = false;
        }
        if(other.gameObject.CompareTag("Talk"))
        {
            inTalkBox = false;
            another = other.transform.parent.gameObject;
            talk.transform.position = new Vector3(1000, 1000, another.transform.position.z);
        }
    }
}
