using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Main : MonoBehaviour {

    // Configuration générale
    public Config config = null;

    // Canvas pour le message popup
    public GameObject Messagebox_Canvas;

    // Pad HTC Vive
    public SteamVR_TrackedObject left = null, right = null;
    public SteamVR_Controller.Device leftDevice = null, rightDevice = null;

    void FixedUpdate()
    {
        // Initialisation des pads HTC Vive
        if (((int)left.index != -1 && (int)right.index != -1) && (leftDevice == null || rightDevice == null))
        {
            leftDevice = SteamVR_Controller.Input((int)left.index);
            rightDevice = SteamVR_Controller.Input((int)right.index);

            Debug.Log("Initialisation des pads");
        }
    }
    
    void Awake() {
        // Ouverture du fichier JSON
        string dataAsJson = File.ReadAllText(Application.dataPath + "/config.json");
        
        // Parse du fichier .json
        config = JsonUtility.FromJson<Config>(dataAsJson);
    }
	
	void Update () {
        // Retire le message de l'écran
        if(Input.GetKeyUp(KeyCode.KeypadEnter))        
            Messagebox_Canvas.SetActive(false);        

        // Retire le message de l'écran - Version VR
        if (rightDevice == null || leftDevice == null)
            return;
        if(rightDevice.GetPress(Valve.VR.EVRButtonId.k_EButton_ApplicationMenu) || leftDevice.GetPress(Valve.VR.EVRButtonId.k_EButton_ApplicationMenu))        
            Messagebox_Canvas.SetActive(false);                
    }

    /*
    * @brief Affiche une zone de texte affichant le message
    * 
    * @param[in]    msg         Le message à afficher
    */
    public void ShowMessage(string msg)
    {
        Messagebox_Canvas.GetComponentInChildren<Text>().text = msg;
        Messagebox_Canvas.SetActive(true);
    }

    /*
     * @brief Sauvegarder un projet IIViMaT
     */
    public void Save()
    {

    }

    /*
     * @brief Charger un projet  IIViMaT
     */
    public void Load()
    {

    }
}
