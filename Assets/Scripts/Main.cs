using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Main : MonoBehaviour {

    public Config config = null;
    public GameObject Messagebox_Canvas;

    // Pad
    public SteamVR_TrackedObject left = null, right = null;
    public SteamVR_Controller.Device leftDevice = null, rightDevice = null;

    void FixedUpdate()
    {
        // Get Device
        if (((int)left.index != -1 && (int)right.index != -1) && (leftDevice == null || rightDevice == null))
        {
            leftDevice = SteamVR_Controller.Input((int)left.index);
            rightDevice = SteamVR_Controller.Input((int)right.index);

            Debug.Log("Initialisation des pads");
        }
    }

    // Use this for initialization
    void Awake() {
        // Ouverture du fichier JSON
        string dataAsJson = File.ReadAllText(Application.dataPath + "/config.json");
        
        // Parse du fichier .json
        config = JsonUtility.FromJson<Config>(dataAsJson);
    }
	
	// Update is called once per frame
	void Update () {

        if(Input.GetKeyUp(KeyCode.KeypadEnter))
        {
            Messagebox_Canvas.SetActive(false);
        }

        if (rightDevice == null || leftDevice == null)
            return;

        if(rightDevice.GetPress(Valve.VR.EVRButtonId.k_EButton_ApplicationMenu) || leftDevice.GetPress(Valve.VR.EVRButtonId.k_EButton_ApplicationMenu))
        {
            Messagebox_Canvas.SetActive(false);
        }        
    }

    public void ShowMessage(string msg)
    {
        Messagebox_Canvas.GetComponentInChildren<Text>().text = msg;
        Messagebox_Canvas.SetActive(true);
    }
}
