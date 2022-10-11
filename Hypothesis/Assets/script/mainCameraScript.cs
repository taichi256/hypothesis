using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mainCameraScript : MonoBehaviour
{
    public bool onInvisibleWall;
    private Rigidbody2D maincameraRb;
    private bool canJudge = true;
    private GameObject player;
    public bool onUpWall;
    public bool outOfUpWall;
    public bool upCamera = false;
    public bool finishedMove;
    PlayerController playerScript;
    public float finishedPos;

    // Start is called before the first frame update
    void Start()
    {
        maincameraRb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("robo");
        playerScript = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LateUpdate()
    {
        if (playerScript.upCamera)
        {
            maincameraRb.velocity = new Vector2(speedFunctionX() * 3, speedFunctionY() * 3);
        }
        else
        {
            maincameraRb.velocity = new Vector2(speedFunctionX() * 3, 0);
        }
        if (PlayerRelativePositionY() <= 1 && PlayerRelativePositionY() >= -0.3)
        {
            finishedMove = true;
        }
    }

    private float PlayerRelativePositionX()
    {
        
        float relativePos = player.transform.position.x - this.transform.position.x;

        return relativePos;
    }

    private float PlayerRelativePositionY()
    {
        float relativePos = player.transform.position.y - this.transform.position.y + 5;

        return relativePos;
    }

    float speedFunctionX()
    {
        float symbol = Mathf.Abs(PlayerRelativePositionX()) / PlayerRelativePositionX();
        float speed = Mathf.Pow(PlayerRelativePositionX(), 2f);
        speed = speed * symbol;

        if(Mathf.Abs(speed) > 0.1f)
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
        float symbol = Mathf.Abs(PlayerRelativePositionY()) / PlayerRelativePositionY();
        float speed = Mathf.Pow(Mathf.Abs(PlayerRelativePositionY()), 2f);
        speed = speed * symbol;

        if (Mathf.Abs(speed) > 0.1f)
        {
            return speed;
        }
        else
        {
            return 0f;
        }
    }

    


}
