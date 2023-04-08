using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityCount : MonoBehaviour
{
    [SerializeField] public int AbilityLimit = 3;
    //set limit of all of Ability

    [SerializeField] public int AJLimit = 1;
    [SerializeField] public int DashLimit = 1;
    [SerializeField] public int GrappleLimit = 0;
    //set limit of Ability

    [SerializeField] public int AJLimitLimit = 1;
    [SerializeField] public int DashLimitLimit = 2;
    [SerializeField] public int GrappleLimitLimit = 3;
	//set the maximum count of setable ability (not used)

    public void ClickA()
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
