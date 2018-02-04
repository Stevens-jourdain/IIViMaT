using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.IO;
using UnityEditor;

using SimpleJSON;

public class Main : MonoBehaviour {

    // Configuration générale
    public Config config = null;

    // Canvas pour le message popup
    public GameObject Messagebox_Canvas;

    // Vidéo360 script pour l'import de vidéo
    public Video360 video360;

    // VAirDraw
    public VAirDraw vairdraw;
    public GameObject curvePrefabs;

    // Pad HTC Vive
    public SteamVR_TrackedObject left = null, right = null;
    public SteamVR_Controller.Device leftDevice = null, rightDevice = null;

    // Action réaction
    public ActionReaction actionReaction;

    // Menu
	public Menu menu;

    // Action en cours, permet d'éviter le conflit de bouton avec les interactions
    public bool actionEnCours = false;

    // Mode play ou edit ?
    public bool isPlayMode = false;

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
        string dataAsJson = File.ReadAllText(config.path_to_import + "/Projects/" + filepath);
        var file = JSON.Parse(dataAsJson);

        // Application
        video360.isSequentiel = file["isSequentiel"].AsBool;

        int nbVideos = file["videos"].Count;
        for (int i = 0; i < nbVideos; ++i)
        {
            // Ajout de la vidéo à la scène
            video360.AddVideoAt(file["videos"][i]["path"].Value, file["videos"][i]["position"].ReadVector3(), file["videos"][i]["rotation"].ReadVector3(), file["videos"][i]["scale"].ReadVector3(), file["videos"][i]["isActive"].AsBool);
        }

        // Chargement des courbes
        for (int i = 0; i < file["courbes"].Count; ++i)
        {
            GameObject obj = Instantiate(curvePrefabs);
            Curve curve = obj.GetComponent<Curve>();
            Vector3 rgb = file["courbes"][i]["color"].ReadVector3();
            curve.SetColor(new Color(rgb.x, rgb.y, rgb.z));

            for (int j = 0; j < file["courbes"][i]["points"].Count; ++i)
            {
                Vector3 point = file["courbes"][i]["points"][j].ReadVector3();
                Vector3 normal = file["courbes"][i]["normals"][j].ReadVector3();
                curve.AddPointAndNormal(point, normal);
            }
        }

        // Chargement des actions-réactions
        if (file["actionReaction"]["action_reactions_courbes"].IsObject)
        {
            foreach (JSONNode source in file["actionReaction"]["action_reactions_courbes"].AsObject.Keys)
            {
                foreach (JSONNode action in file["actionReaction"]["action_reactions_courbes"][source.Value].Keys)
                {
                    foreach (JSONNode reaction in file["actionReaction"]["action_reactions_courbes"][source.Value][action.Value].Keys)
                    {
                        foreach (JSONNode name_obj in file["actionReaction"]["action_reactions_courbes"][source.Value][action.Value][reaction.Value].AsArray)
                        {
                            if (!actionReaction.action_reactions_courbes.ContainsKey(source.AsInt))
                                actionReaction.action_reactions_courbes.Add(source.AsInt, new Dictionary<int, Dictionary<int, List<GameObject>>>());

                            if (!actionReaction.action_reactions_courbes[source.AsInt].ContainsKey(action.AsInt))
                                actionReaction.action_reactions_courbes[source.AsInt].Add(action.AsInt, new Dictionary<int, List<GameObject>>());

                            if (!actionReaction.action_reactions_courbes[source.AsInt][action.AsInt].ContainsKey(reaction.AsInt))
                                actionReaction.action_reactions_courbes[source.AsInt][action.AsInt].Add(reaction.AsInt, new List<GameObject>());

                            actionReaction.action_reactions_courbes[source.AsInt][action.AsInt][reaction.AsInt].Add(GameObject.Find(name_obj.Value));
                        }
                    }
                }
            }
        }

        if (file["actionReaction"]["action_reactions_spheres360"].IsObject)
        {
            foreach (JSONNode source in file["actionReaction"]["action_reactions_spheres360"].AsObject.Keys)
            {
                foreach (JSONNode action in file["actionReaction"]["action_reactions_spheres360"][source.Value].Keys)
                {
                    foreach (JSONNode reaction in file["actionReaction"]["action_reactions_spheres360"][source.Value][action.Value].Keys)
                    {
                        foreach (JSONNode name_obj in file["actionReaction"]["action_reactions_spheres360"][source.Value][action.Value][reaction.Value].AsArray)
                        {
                            if (!actionReaction.action_reactions_spheres360.ContainsKey(source.AsInt))
                                actionReaction.action_reactions_spheres360.Add(source.AsInt, new Dictionary<int, Dictionary<int, List<GameObject>>>());

                            if (!actionReaction.action_reactions_spheres360[source.AsInt].ContainsKey(action.AsInt))
                                actionReaction.action_reactions_spheres360[source.AsInt].Add(action.AsInt, new Dictionary<int, List<GameObject>>());

                            if (!actionReaction.action_reactions_spheres360[source.AsInt][action.AsInt].ContainsKey(reaction.AsInt))
                                actionReaction.action_reactions_spheres360[source.AsInt][action.AsInt].Add(reaction.AsInt, new List<GameObject>());

                            actionReaction.action_reactions_spheres360[source.AsInt][action.AsInt][reaction.AsInt].Add(GameObject.Find(name_obj.Value));
                        }
                    }
                }
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

            string url = video.GetComponent<VideoPlayer>().url;
            url = url.Substring(url.LastIndexOf('/') + 1);
            vid_json.path = url;

            // Vidéo active par defaut ?
            vid_json.isActive = video.activeInHierarchy;

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
        file.actionReaction = new ActionReactionJSON();

        if (actionReaction.action_reactions_courbes.Count > 0)
        {
            file.actionReaction.action_reactions_courbes = new Dictionary<int, Dictionary<int, Dictionary<int, List<string>>>>();

            foreach (KeyValuePair<int, Dictionary<int, Dictionary<int, List<GameObject>>>> source in actionReaction.action_reactions_courbes)
            {
                foreach (KeyValuePair<int, Dictionary<int, List<GameObject>>> action in source.Value)
                {
                    foreach (KeyValuePair<int, List<GameObject>> reaction in action.Value)
                    {
                        foreach (GameObject obj in reaction.Value)
                        {
                            if (!file.actionReaction.action_reactions_courbes.ContainsKey(source.Key))
                                file.actionReaction.action_reactions_courbes.Add(source.Key, new Dictionary<int, Dictionary<int, List<string>>>());

                            if (!file.actionReaction.action_reactions_courbes[source.Key].ContainsKey(action.Key))
                                file.actionReaction.action_reactions_courbes[source.Key].Add(action.Key, new Dictionary<int, List<string>>());

                            if (!file.actionReaction.action_reactions_courbes[source.Key][action.Key].ContainsKey(reaction.Key))
                                file.actionReaction.action_reactions_courbes[source.Key][action.Key].Add(reaction.Key, new List<string>());

                            file.actionReaction.action_reactions_courbes[source.Key][action.Key][reaction.Key].Add(obj.name);
                        }
                    }
                }
            }
        }

        if (actionReaction.action_reactions_spheres360.Count > 0)
        {
            file.actionReaction.action_reactions_spheres360 = new Dictionary<int, Dictionary<int, Dictionary<int, List<string>>>>();

            foreach (KeyValuePair<int, Dictionary<int, Dictionary<int, List<GameObject>>>> source in actionReaction.action_reactions_spheres360)
            {
                foreach (KeyValuePair<int, Dictionary<int, List<GameObject>>> action in source.Value)
                {
                    foreach (KeyValuePair<int, List<GameObject>> reaction in action.Value)
                    {
                        foreach (GameObject obj in reaction.Value)
                        {
                            if (!file.actionReaction.action_reactions_spheres360.ContainsKey(source.Key))
                                file.actionReaction.action_reactions_spheres360.Add(source.Key, new Dictionary<int, Dictionary<int, List<string>>>());

                            if (!file.actionReaction.action_reactions_spheres360[source.Key].ContainsKey(action.Key))
                                file.actionReaction.action_reactions_spheres360[source.Key].Add(action.Key, new Dictionary<int, List<string>>());

                            if (!file.actionReaction.action_reactions_spheres360[source.Key][action.Key].ContainsKey(reaction.Key))
                                file.actionReaction.action_reactions_spheres360[source.Key][action.Key].Add(reaction.Key, new List<string>());

                            file.actionReaction.action_reactions_spheres360[source.Key][action.Key][reaction.Key].Add(obj.name);
                        }
                    }
                }
            }
        }
        else
        {
            Debug.Log("Pas d'action dans les spheres...");
        }

        var path = EditorUtility.SaveFilePanel("Enregistrer votre projet", "", "", "IIViMaT");
        StreamWriter stream = File.CreateText(path);
                
        string json = file.ToJSON().ToString();
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

        if ((Input.GetKeyUp(KeyCode.M)))
            MenuPad();

            // Retire le message de l'écran - Version VR
        if (rightDevice == null || leftDevice == null)
            return;

        if (rightDevice.GetPress(Valve.VR.EVRButtonId.k_EButton_ApplicationMenu) || leftDevice.GetPress(Valve.VR.EVRButtonId.k_EButton_ApplicationMenu))        
            MenuPad();

        if (rightDevice.GetPress(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger) || leftDevice.GetPress(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger))
        {
            if(Messagebox_Canvas.activeInHierarchy)
                Messagebox_Canvas.SetActive(false);
        }
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

	public void LoadMenu(string str) {
		if (str == "Charger courbe") {
			ListFilesFromDir lfd = new ListFilesFromDir(config.path_to_import + "/VAirDraw/");
			string[] allFilesCurves = lfd.files;

			Menu.Del handler = vairdraw.ImportFromVAirDraw;

			// Show list to content's creator
			menu.AddItems(allFilesCurves, handler);
		} 
		else if(str == "Charger video")
        {
			ListFilesFromDir lfd = new ListFilesFromDir(config.path_to_import + "/Video360/");
			string[] allFilesCurves = lfd.files;

			Menu.Del handler = video360.AddVideo;

			// Show list to content's creator
			menu.AddItems(allFilesCurves, handler);
		}
        else
        {
            isPlayMode = !isPlayMode;

            if(isPlayMode)
            {
                // Reset de tous les index, lancement de la lecture

            }
            else
            {

            }
        }
	}

	public void MenuPad(){		
		string[] LoadingMenu = { "Charger courbe", "Charger video", isPlayMode ? "Passer en mode edit" : "Passer en mode play" };
		Menu.Del handler = LoadMenu;

		menu.AddItems(LoadingMenu, handler);		
	}
}
