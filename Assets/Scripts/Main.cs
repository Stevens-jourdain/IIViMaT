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
    public GameObject curvePrefabs;

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
        IIViMaTFile file = JsonUtility.FromJson<IIViMaTFile>(dataAsJson);

        // Application
        video360.isSequentiel = file.isSequentiel;

        int nbVideos = file.videos.Length;
        for (int i = 0; i < nbVideos; ++i)
        {
            // Ajout de la vidéo à la scène
            video360.AddVideoAt(file.videos[i].path, file.videos[i].position, file.videos[i].rotation, file.videos[i].scale);
        }

        // Chargement des courbes
        for(int i = 0; i < file.courbes.Length; ++i)
        {
            GameObject obj = Instantiate(curvePrefabs);
            Curve curve = obj.GetComponent<Curve>();
            curve.SetColor(new Color(file.courbes[i].color.x, file.courbes[i].color.y, file.courbes[i].color.z));

            for(int j = 0; j < file.courbes[i].points.Length; ++i)
            {
                Vector3 point = new Vector3(file.courbes[i].points[j].x, file.courbes[i].points[j].y, file.courbes[i].points[j].z);
                Vector3 normal = new Vector3(file.courbes[i].normals[j].x, file.courbes[i].normals[j].y, file.courbes[i].normals[j].z);
                curve.AddPointAndNormal(point, normal);
            }
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
        file.videos = new MediaJSON[nbVideos];

        // Enregistrement des vidéos
        for (int i = 0; i < nbVideos; ++i)
        {
            GameObject video = videosObjects[i];
            MediaJSON vid_json = new MediaJSON();

            // Position
            Vecteur3_IIViMaT pos = new Vecteur3_IIViMaT();
            pos.x = video.transform.position.x;
            pos.y = video.transform.position.y;
            pos.z = video.transform.position.z;
            vid_json.position = pos;

            // Rotation
            Vecteur3_IIViMaT rot = new Vecteur3_IIViMaT();
            rot.x = video.transform.rotation.x;
            rot.y = video.transform.rotation.y;
            rot.z = video.transform.rotation.z;
            vid_json.rotation = rot;

            // Scale
            Vecteur3_IIViMaT scale = new Vecteur3_IIViMaT();
            scale.x = video.transform.localScale.x;
            scale.y = video.transform.localScale.y;
            scale.z = video.transform.localScale.z;
            vid_json.scale = scale;

            vid_json.path = video.GetComponent<VideoPlayer>().url;

            file.videos[i] = vid_json;
        }

        // Enregistrement des courbes
        // Liste des vidéos 360
        GameObject[] curvesObjects = GameObject.FindGameObjectsWithTag("Curve");

        // Nombre de vidéo
        int nbCurves = curvesObjects.Length;

        Debug.Log("Il y a " + nbCurves + " courbe dans la scène.");

        // Remplissage des données
        file.courbes = new CourbeJSON[nbCurves];

        for (int i = 0; i < nbCurves; ++i)
        {
            GameObject courbe = curvesObjects[i];
            file.courbes[i] = new CourbeJSON();

            // Enregistrement points et normals
            Vector3[] points = courbe.GetComponent<Curve>().GetPoints().ToArray();
            Vector3[] normals = courbe.GetComponent<Curve>().GetNormals().ToArray();
            file.courbes[i].points = new Vecteur3_IIViMaT[points.Length];
            file.courbes[i].normals = new Vecteur3_IIViMaT[points.Length];

            for (int j = 0; j < points.Length; ++j)
            {
                file.courbes[i].points[j] = new Vecteur3_IIViMaT();
                file.courbes[i].points[j].x = points[i].x;
                file.courbes[i].points[j].y = points[i].y;
                file.courbes[i].points[j].z = points[i].z;

                file.courbes[i].normals[j] = new Vecteur3_IIViMaT();
                file.courbes[i].normals[j].x = normals[i].x;
                file.courbes[i].normals[j].y = normals[i].y;
                file.courbes[i].normals[j].z = normals[i].z;
            }

            // Enregistrement couleur
            Color c = courbe.GetComponent<Curve>().GetColor();
            file.courbes[i].color = new Vecteur3_IIViMaT();
            file.courbes[i].color.x = c.r;
            file.courbes[i].color.y = c.g;
            file.courbes[i].color.z = c.b;
        }

        // Enregistrement des modèles

        // Enregistrement des actions-réactions

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
			string[] ParSeqMenu = { "Sequentiel" , "Parallele" };
			Menu.Del handler = ParSeq;

			menu.AddItems(ParSeqMenu, handler);
		} 
		else {
//			Load ();
			ListFilesFromDir lfd = new ListFilesFromDir(config.path_to_import + "/Projects/");
			string[] allFilesCurves = lfd.files;

			Menu.Del handler = ChargerFichier;

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
        string path = EditorUtility.OpenFilePanel("Import a virtual scene", "", "IIViMaT");
        if(path != null && path.Length > 0)
            this.ChargerFichier(path);
    }
}
