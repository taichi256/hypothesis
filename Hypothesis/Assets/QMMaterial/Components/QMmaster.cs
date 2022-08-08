using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QMmaster : MonoBehaviour
{
    public GameObject obj;
    bool QMshow = false;
    public PlayerController robocontroller;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Canvas>().enabled = false;
    }

    

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetButtonDown("OpenQM"))
        {
            if (QMshow == false)
            {
                GetComponent<Canvas>().enabled = true;
                QMshow = true;
                Debug.Log("Quick Menu:On");
                robocontroller.enabled = false;
                Time.timeScale = 0.05f;
            }
            else
            {
                GetComponent<Canvas>().enabled = false;
                QMshow = false;
                Debug.Log("Quick Menu:Off");
                robocontroller.enabled = true;
                Time.timeScale = 1f;
            }
        }
        
    }



}
