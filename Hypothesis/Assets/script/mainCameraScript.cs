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
    public bool talking = false;
    public bool talkOnce = false;
    public bool eventOnce = false;
    [SerializeField]
	private Message messageScript;
 
	//　表示させるメッセージ
	private string message = "かかかかかかかかかかかかかかかかかかかかかかかかかかかかかかかかかかかかかかかかかかかかかかかかかかかかかかかかかかかかかかかかかかかかかか"
	                         + "ききききききききききききききききききききききききききききききききききききききききききききききききききききききき\n"
	                         + "くくくくく\n"
	                         + "けけけけけけけけけけけけ";
    

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
        if(Input.GetKeyDown(KeyCode.Return) && playerScript.inTalkBox && !talkOnce && !talking)
        {
            StartCoroutine("StopTalking");
        }

        if(playerScript.inEventBox && !eventOnce && !talking)
        {
            StartCoroutine("MoveTalking");
        }
    }

    void FixedUpdate()
    {
        if (playerScript.upCamera && !talking)
        {
            maincameraRb.velocity = new Vector2(speedFunctionX() * 2, speedFunctionY() * 2);
        }
        if (!playerScript.upCamera && !talking)
        {
            maincameraRb.velocity = new Vector2(speedFunctionX() * 2, 0);
        }
        if (talking)
        {
            maincameraRb.velocity = new Vector2(0, 0);
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

    private IEnumerator StopTalking()
    {
        talkOnce = true;
        talking = true;
        playerScript.talk.SetActive(false);
        playerScript.enabled = false;

        for(int i=0; i<20; i++)
        {
            transform.Translate(0, -0.1f, 0);
            yield return new WaitForSeconds(0.01f);
        }

        messageScript.SetMessagePanel (message);

        while(!messageScript.isEndMessage)
	    {
		    yield return new WaitForSeconds(0.2f);
	    }

        talking = false;
        talkOnce = false;

        for(int i=0; i<20; i++)
        {
            transform.Translate(0, 0.1f, 0);
            yield return new WaitForSeconds(0.01f);
        }

        playerScript.talk.SetActive(true);
        playerScript.enabled = true;
    }

    private IEnumerator MoveTalking()
    {
        eventOnce = true;
        talking = true;
        playerScript.talk.SetActive(false);

        for(int i=0; i<20; i++)
        {
            transform.Translate(0, -0.1f, 0);
            yield return new WaitForSeconds(0.01f);
        }

        messageScript.SetMessagePanel (message);

        while(!messageScript.isEndMessage)
	    {
		    yield return new WaitForSeconds(0.2f);
	    }

        talking = false;

        for(int i=0; i<20; i++)
        {
            transform.Translate(0, 0.1f, 0);
            yield return new WaitForSeconds(0.01f);
        }

        playerScript.talk.SetActive(true);
    }

}
