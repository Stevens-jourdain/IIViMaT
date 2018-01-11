using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;
using UnityEngine.Video;

using Valve.VR;

public class Menu : MonoBehaviour {

	public Button ButtonVR;
	public Button Button360;

    public GameObject playerVideo;
    public VideoPlayer videoPlayer;

	public SteamVR_TrackedObject left = null, right = null;
	private SteamVR_Controller.Device leftDevice = null, rightDevice = null;

	// Use this for initialization
	void Start () {
        /*
        Mesh mesh = playerVideo.GetComponent<Mesh>();
        Vector3[] normals = mesh.normals;
        for (int i = 0; i < normals.Length; i++)
            normals[i] = -normals[i];
        mesh.normals = normals;*/

       /* for (int m = 0; m < mesh.subMeshCount; m++)
        {
            int[] triangles = mesh.GetTriangles(m);
            for (int i = 0; i < triangles.Length; i += 3)
            {
                int temp = triangles[i + 0];
                triangles[i + 0] = triangles[i + 1];
                triangles[i + 1] = temp;
            }
            mesh.SetTriangles(triangles, m);
        }*/

        //Button btnVR = ButtonVR.GetComponent<Button> ();
        //btnVR.onClick.AddListener (TaskOnClick);
        //Button btn360 = Button360.GetComponent<Button> ();
        //btn360.onClick.AddListener (TaskOnClick2);

    }


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


    // Update is called once per frame
    void Update () {
        if ((leftDevice == null) || (rightDevice == null)) {
            return;
        }

        if (leftDevice.GetPress(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger)) {
            Debug.Log("Trigger gauche");
            string path = EditorUtility.OpenFilePanel("Import a virtual scene", "", "obj");
            if (path.Length != 0)
            {
                var fileContent = File.ReadAllBytes(path);
            }
        }

        if (rightDevice.GetPress(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger))
        {
            Debug.Log("Trigger droite");
            string path = EditorUtility.OpenFilePanel("Import a virtual scene", "", "mp4");
            if (path.Length != 0)
            {
                //var fileContent = File.ReadAllBytes(path);
                var videoPlayer = playerVideo.AddComponent<UnityEngine.Video.VideoPlayer>();
                videoPlayer.playOnAwake = false;
                videoPlayer.url = path;
                videoPlayer.isLooping = true;
                videoPlayer.Play();

            }


        }
    }
}
