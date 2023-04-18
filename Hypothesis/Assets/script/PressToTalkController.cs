using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressToTalkController : MonoBehaviour
{
    [SerializeField]
    [TextArea(1, 20)]
    private string message;
    private GameObject talk;
    public GameObject mainCamera;
    mainCameraScript mainCameraScript;
    // Start is called before the first frame update
    void Start()
    {
        talk = transform.GetChild(0).gameObject;
        mainCameraScript = mainCamera.GetComponent<mainCameraScript>();
        talk.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            talk.SetActive(true);
            mainCameraScript.message = message;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            talk.SetActive(false);
        }

    }
}
