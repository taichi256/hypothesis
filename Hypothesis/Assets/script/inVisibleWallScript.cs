using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inVisibleWall : MonoBehaviour
{
    private bool inCam;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnBecameVisible()
    {
        inCam = true;
    }

    private void OnBecameInvisible()
    {
        inCam = false;
    }

}
