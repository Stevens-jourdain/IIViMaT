using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerManager : MonoBehaviour
{

    private float time;
    private bool isDown = false;

    public bool OnTriggerUp(SteamVR_Controller.Device pad)
    {
        if (!isDown && pad.GetPress(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger))
        {
            isDown = true;
            time = Time.time;
        }

        float currentTime = Time.time;
        if (isDown && currentTime - time > 0.5f)
        {
            bool v = pad.GetPress(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger);
            
            if (v)
            {
                isDown = false;
                return true;
            }
        }

        return false;
    }
}
