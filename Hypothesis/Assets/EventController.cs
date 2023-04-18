using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventController : MonoBehaviour
{
    [SerializeField]
    [TextArea(1, 20)]
    private string message;
    public GameObject mainCamera;
    mainCameraScript mainCameraScript;
    // Start is called before the first frame update
    void Start()
    {
        mainCameraScript = mainCamera.GetComponent<mainCameraScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            mainCameraScript.message = message;
        }
    }
}
