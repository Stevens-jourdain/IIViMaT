using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.IO;
using UnityEditor;

public class Main : MonoBehaviour {

    // Configuration générale
    public Config config = null;

    // Canvas pour le message popup
    public GameObject Messagebox_Canvas;

    // Vidéo360 script pour l'import de vidéo
    public Video360 video360;

    // Pad HTC Vive
    public SteamVR_TrackedObject left = null, right = null;
    public SteamVR_Controller.Device leftDevice = null, rightDevice = null;

	public Menu menu;

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

    void ChargerFichier(string filepath)
    {
        // Ouverture en JSON
        string dataAsJson = File.ReadAllText(filepath);
        IIViMaTFile file =  JsonUtility.FromJson<IIViMaTFile>(dataAsJson);

        // Application
        video360.isSequentiel = file.isSequentiel;

        int nbVideos = file.videos.Length;
        for(int i = 0;  i < nbVideos; ++i)
        {
            // Ajout de la vidéo à la scène
            video360.AddVideoAt(file.videos[i].path, file.videos[i].position);
        }
    }

    void SauvegarderFichier()
    {
        // Liste des vidéos 360
        GameObject[] videosObjects = GameObject.FindGameObjectsWithTag("Video360");

        // Nombre de vidéo
        int nbVideos = videosObjects.Length;

        Debug.Log("Il y a " + nbVideos + " vidéos dans la scène.");

        // Initialisation de notre objet IIViMaTFile
        IIViMaTFile file = new IIViMaTFile();

        // Remplissage des données
        file.isSequentiel = video360.isSequentiel;
        file.videos = new VideoJSON[nbVideos];

        for(int i = 0; i < nbVideos; ++i)
        {
            GameObject video = videosObjects[i];
            VideoJSON vid_json = new VideoJSON();

            Vecteur3_IIViMaT pos = new Vecteur3_IIViMaT();
            pos.x = video.transform.position.x;
            pos.y = video.transform.position.y;
            pos.z = video.transform.position.z;
            vid_json.position = pos;
            vid_json.path = video.GetComponent<VideoPlayer>().url;

            file.videos[i] = vid_json;
            Debug.Log("Dans la boucle : " + i + ", position : " + pos.x + "; " + pos.y + " ; " + pos.z);
        }

        var path = EditorUtility.SaveFilePanel("Enregistrer votre projet", "", "", "IIViMaT");
        StreamWriter stream = File.CreateText(path);

        string json = JsonUtility.ToJson(file);
        stream.WriteLine(json);
        stream.Close();
    }

	public void ParSeq (string str) {
		if (str == "Parallele") {
			video360.isSequentiel = false;
		} 
		else {
			video360.isSequentiel = true;
		}
	}

	public void GestionMenu (string str) {
		if (str == "Nouveau") {
			//Gestion du menu ParSeq
			string[] ParSeqMenu = new string[2];
			ParSeqMenu [0] = "Sequentiel";
			ParSeqMenu [1] = "Parallele";

			Menu.Del handler = ParSeq;

			menu.AddItems (ParSeqMenu, handler);
		} 
		else {
			//Load ();
			ListFilesFromDir lfd = new ListFilesFromDir(config.path_to_import + "/Video360/");
			string[] allFilesCurves = lfd.files;

			Menu.Del handler = video360.AddVideo;

			// Show list to content's creator
			menu.AddItems(allFilesCurves, handler);
		}
	}
    
    void Awake() {
        // Ouverture du fichier JSON
        string dataAsJson = File.ReadAllText(Application.dataPath + "/config.json");
        
        // Parse du fichier .json
        config = JsonUtility.FromJson<Config>(dataAsJson);

		// Gestion du menu
//		string[] TitleMenu = new string[2];
//		TitleMenu[0] = "Nouveau";
//		TitleMenu[1] = "Charger";
//
//		Menu.Del handler = GestionMenu;
//
//		menu.AddItems(TitleMenu, handler);
    }
	
	void Update () {
        // Retire le message de l'écran
        if(Input.GetKeyUp(KeyCode.KeypadEnter))        
            Messagebox_Canvas.SetActive(false);

        // Sauvegarde
        if (Input.GetKeyUp(KeyCode.S))
            this.SauvegarderFichier();

        // Chargement
        if (Input.GetKeyUp(KeyCode.C))
            this.Load();

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
     * @brief Charger un projet  IIViMaT
     */
    public void Load()
    {
        var path = EditorUtility.OpenFilePanel("Import a virtual scene", "", "IIViMaT");
        this.ChargerFichier(path);
    }
}
