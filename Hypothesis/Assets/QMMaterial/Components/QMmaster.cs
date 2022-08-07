using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QMmaster : MonoBehaviour
{
    public GameObject obj;
    bool QMshow = false;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("タブキーが押されたことを認識しました。");
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
            }
            else
            {
                GetComponent<Canvas>().enabled = false;
                QMshow = false;
            }
        }
        
    }
}
