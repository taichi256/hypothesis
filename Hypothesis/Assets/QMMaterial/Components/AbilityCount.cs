using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityCount : MonoBehaviour
{
    public int AbilityLimit = 3;
    //set limit of all of Ability

    public int AJLimit = 0;
    public int DashLimit = 0;
    public int GrappleLimit = 0;
    //set limit of Ability

    public void ClickAJ()
    {
        int AC = AJLimit + DashLimit + GrappleLimit;
        if (AbilityLimit>AC)
        {
            AJLimit++;
        }
    }
    public void ClickD()
    {
        int AC = AJLimit + DashLimit + GrappleLimit;
        if (AbilityLimit > AC)
        {
            DashLimit++;
        }
    }

    public void ClickG()
    {
        int AC = AJLimit + DashLimit + GrappleLimit;
        if (AbilityLimit > AC)
        {
            GrappleLimit++;
        }
    }
}
