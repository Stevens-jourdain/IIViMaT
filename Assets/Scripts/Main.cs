using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Main : MonoBehaviour {

    public Config config = null;

	// Use this for initialization
	void Awake() {
        // Ouverture du fichier JSON
        string dataAsJson = File.ReadAllText(Application.dataPath + "/config.json");
        
        // Parse du fichier .json
        config = JsonUtility.FromJson<Config>(dataAsJson);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
