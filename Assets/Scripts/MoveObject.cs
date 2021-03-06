﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{

    public delegate void Handler(GameObject obj);

    public Handler fn = null;
    public Main main;
    public GameObject[] listOfObject;

    int TimeToMove = 0;
    float scale = 1.0f;

    public bool isVideo = false;

    // Gestion du trigger HTC VIVE
    public TriggerManager triggerManager;

    public void SetObjects(GameObject[] listOfObject)
    {
        this.listOfObject = listOfObject;
    }

    public void StartMove()
    {
        TimeToMove = 1;
        main.actionEnCours = true;
    }

    // Update is called once per frame
    public void MoveUpdate()
    {
        if (TimeToMove == 2)
        {
            Vector3 position = new Vector3(), angleRot = new Vector3();      
            
            // scale
            if (Input.GetKeyUp(KeyCode.KeypadPlus))            
                scale += 0.5f;            

            if (Input.GetKeyUp(KeyCode.KeypadMinus))            
                scale -= 0.5f;

            // Position et rotation (avec ctrl)
            if (Input.GetKey(KeyCode.UpArrow))
            {
                if (Input.GetKey(KeyCode.RightControl))
                    angleRot.z += 0.01f;
                else
                    position.z += 0.1f;
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                if (Input.GetKey(KeyCode.RightControl))
                    angleRot.z -= 0.01f;
                else
                    position.z -= 0.1f;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                if (Input.GetKey(KeyCode.RightControl))
                    angleRot.x += 0.01f;
                else
                    position.x += 0.1f;
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                if (Input.GetKey(KeyCode.RightControl))
                    angleRot.x -= 0.01f;
                else
                    position.x -= 0.1f;
            }
            if (Input.GetKey(KeyCode.PageUp))
            {
                if (Input.GetKey(KeyCode.RightControl))
                    angleRot.y += 0.01f;
                else
                    position.y += 0.1f;
            }
            if (Input.GetKey(KeyCode.PageDown))
            {
                if (Input.GetKey(KeyCode.RightControl))
                    angleRot.y -= 0.01f;
                else
                    position.y -= 0.1f;
            }

            if (Input.GetKeyUp(KeyCode.KeypadEnter))
            {
                if (fn != null)
                {
                    foreach (GameObject obj in listOfObject)
                        fn(obj);
                }

                TimeToMove = 0;
                main.actionEnCours = false;                
            }

            // HTC Vive Controller
            if (main.rightDevice != null && main.leftDevice != null)
            {
                // Définition echelle et position en hauteur
                float rightX = main.rightDevice.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;
                float rightY = main.rightDevice.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).y;
                scale += rightX > 0.5f ? (0.5f * (rightX+1)) : (rightX < -0.5f ? (-0.5f * (-rightX + 1)) : 0);
                position.y += rightY > 0.5f ? (0.01f * (rightY + 1)) : (rightY < -0.5f ? (-0.01f * (-rightY + 1)): 0);

                // Position par rapport au sol
                float leftX = main.leftDevice.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;
                float leftY = main.leftDevice.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).y;
                position.x = leftX > 0.5f ? (0.01f * (leftX + 1)): (leftX < -0.5f ? (-0.01f * (-leftX + 1)) : 0);
                position.z = leftY > 0.5f ? (0.01f * (leftY + 1)): (leftY < -0.5f ? (-0.01f * (-leftY + 1)) : 0);

                // Rotation de l'objet
                Vector3 anglePad = main.right.transform.rotation.eulerAngles;

                if (anglePad.x > 0.5f)
                    angleRot.x += (0.1f * (anglePad.x + 1));
                if (anglePad.x < -0.5f)
                    angleRot.x -= (0.1f * (-anglePad.x + 1));

                if (anglePad.y > 0.5f)
                    angleRot.y += (0.1f * (anglePad.y + 1));
                if (anglePad.y > 0.5f)
                    angleRot.y += (0.1f * (-anglePad.y + 1));

                if (anglePad.z > 0.5f)
                    angleRot.z += (0.1f * (anglePad.z + 1));
                if (anglePad.z > 0.5f)
                    angleRot.z += (0.1f * (-anglePad.z + 1));

                // Si on valide
                if (triggerManager.OnTriggerUp(main.rightDevice) || triggerManager.OnTriggerUp(main.leftDevice))
                {
                    if (fn != null)
                    {
                        foreach (GameObject obj in listOfObject)
                            fn(obj);
                    }

                    TimeToMove = 0;
                    main.actionEnCours = false;                    
                }
            }

            // Application pour chaque objet
            foreach (GameObject obj in listOfObject)
            {
                obj.transform.localScale = new Vector3(scale, scale, scale);
                obj.transform.Translate(position);                
                obj.transform.Rotate(angleRot * 360);                
            }
        }

        // Si on retire le message box
        if (TimeToMove == 1)
        {
            if (Input.GetKeyUp(KeyCode.KeypadEnter))
                TimeToMove = 2;

            if (main.rightDevice != null && main.leftDevice != null && (triggerManager.OnTriggerUp(main.rightDevice) || triggerManager.OnTriggerUp(main.leftDevice)))
                TimeToMove = 2;
        }
    }
}
