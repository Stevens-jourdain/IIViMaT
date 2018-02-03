using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{

    public delegate void Handler(GameObject obj);

    public Handler fn = null;
    public Main main;
    public GameObject[] listOfObject;

    int TimeToMove = 0;
    float scale = 1f;

    public void SetObjects(GameObject[] listOfObject)
    {
        this.listOfObject = listOfObject;
    }

    public void StartMove()
    {
        TimeToMove = 1;
    }

    // Update is called once per frame
    public void MoveUpdate()
    {
        if (TimeToMove == 2)
        {
            Vector3 position = new Vector3(), angleRot = new Vector3();
            Quaternion rotation = new Quaternion();
            
            
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
                TimeToMove = 0;

            // HTC Vive Controller
            if (main.rightDevice != null && main.leftDevice != null)
            {
                // Définition echelle et position
                scale = main.rightDevice.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x + 1 * 5;
                position = main.right.transform.position;
                rotation = main.right.transform.rotation;

                // Si on valide
                if (main.rightDevice.GetPress(Valve.VR.EVRButtonId.k_EButton_ApplicationMenu))
                {
                    TimeToMove = 0;
                }
            }

            // Application pour chaque objet
            foreach (GameObject obj in listOfObject)
            {
                obj.transform.localScale = new Vector3(scale, scale, scale);

                if (main.rightDevice != null && main.leftDevice != null)
                {
                    obj.transform.position = position;
                    obj.transform.rotation = rotation;
                }
                else
                {
                    obj.transform.Translate(position);
                    obj.transform.Rotate(angleRot * 360);
                }

                if(fn != null)
                    fn(obj);
            }
        }

        // Si on retire le message box
        if (TimeToMove == 1)
        {
            if (Input.GetKeyUp(KeyCode.KeypadEnter))
                TimeToMove = 2;

            if (main.rightDevice != null && main.leftDevice != null && main.rightDevice.GetPress(Valve.VR.EVRButtonId.k_EButton_ApplicationMenu))
                TimeToMove = 2;
        }
    }
}
