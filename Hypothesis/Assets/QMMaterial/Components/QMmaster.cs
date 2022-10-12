using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QMmaster : MonoBehaviour
{
    public GameObject obj;
    bool QMshow = false;
    public GameObject Camera;
    public PlayerController robocontroller;
    public GameObject APanel;
    public GameObject MPanel;
    public GameObject TPanel;
    public GameObject EDPanel;

    public GameObject AIco;
    public GameObject MIco;
    public GameObject TIco;
    public GameObject EDIco;

    

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Canvas>().enabled = false;
        APanel.SetActive(false);
        MPanel.SetActive(false);
        TPanel.SetActive(false);
        EDPanel.SetActive(false);
    }

    

    // Update is called once per frame
    void Update()
    {
        //QMÇÃON,OffÇä«óù
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


        //QMÇÃÉAÉCÉRÉìïœâªÇÃä«óù
        if (APanel.activeSelf==true)
        {
            Debug.Log("AbillityPanel is enabled");

            Transform Cam = Camera.transform;
            Transform AIT = AIco.transform;
            Transform MIT = MIco.transform;
            Transform TIT = TIco.transform;
            Transform EDIT = EDIco.transform;

            Vector2 IcosY = Cam.position;
            IcosY.y += 6;
            Vector2 Wider = new (2f, 0f);


            AIT.position = IcosY + Wider*2;
            MIT.position = IcosY- Wider*2;
            TIT.position = IcosY+Wider;
            EDIT.position = IcosY-Wider;

            AIco.transform.localScale = Vector2.one*0.5f;
            MIco.transform.localScale = Vector2.one * 0.5f;
            TIco.transform.localScale = Vector2.one * 0.5f;
            EDIco.transform.localScale = Vector2.one * 0.5f;
        }

    }

    private void OnEnable()
    {
        
    }





}
