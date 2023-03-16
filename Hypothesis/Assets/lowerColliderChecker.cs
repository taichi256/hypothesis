using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lowerColliderChecker : MonoBehaviour
{
    public bool onGround = false;
    public bool OnGround()
    {
        if (E || S)
        {
            onGround = true;
            Debug.Log(onGround);
        }
        else if (X)
        {
            onGround = false;
            Debug.Log(onGround);
        }

        E = false;
        S = false;
        X = false;
        return onGround;
    }

    bool E, S, X = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            E = true;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            S = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        
            X = true;

    }

    
}
