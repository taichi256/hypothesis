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

    public int AJLimitLimit = 1;
    public int DashLimitLimit = 2;
    public int GrappleLimitLimit = 3;

    public void ClickAJ()
    {
        int AC = AJLimit + DashLimit + GrappleLimit;
        if (AbilityLimit>AC && AJLimitLimit>AJLimit)
        {
            AJLimit++;
        }
    }
    public void ClickD()
    {
        int AC = AJLimit + DashLimit + GrappleLimit;
        if (AbilityLimit > AC && DashLimitLimit>DashLimit)
        {
            DashLimit++;
        }
    }

    public void ClickG()
    {
        int AC = AJLimit + DashLimit + GrappleLimit;
        if (AbilityLimit > AC && GrappleLimitLimit>GrappleLimit)
        {
            GrappleLimit++;
        }
    }

    public void Reset()
    {
        AJLimit = 0;
        DashLimit = 0;
        GrappleLimit = 0;
    }
}
