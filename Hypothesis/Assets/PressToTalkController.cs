using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressToTalkController : MonoBehaviour
{
    private GameObject talk;
    // Start is called before the first frame update
    void Start()
    {
        talk = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            talk.transform.position = new Vector3(this.transform.position.x + 5.4f, this.transform.position.y + 0.25f , this.transform.position.z);
        }
    }
}
