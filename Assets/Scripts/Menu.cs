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
    
    public GameObject ItemMenu_prefabs;
    public GameObject[] itemsObj;
    private ItemMenu[] itemsList;
    public int indexItem = 0, nbItems;

    public Main main;

    Del handler;

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

    public delegate void Del(string item);

    public void AddItems(string[] items, Del handler)
    {
        this.handler = handler;
        nbItems = items.Length;
        indexItem = 0;
        itemsList = new ItemMenu[nbItems];
        itemsObj = new GameObject[nbItems];

        for (int i = 0; i < nbItems; ++i)
        {
            itemsObj[i] = Instantiate(ItemMenu_prefabs);
            itemsObj[i].transform.parent = this.transform;
            itemsObj[i].SetActive(true);
            itemsList[i] = itemsObj[i].GetComponent<ItemMenu>();
            itemsList[i].SetValue(items[i]);

            Vector3 pos = itemsObj[i].GetComponent<RectTransform>().position;
            pos.y = Screen.height - ( i * (50 + 10)) - 45;
            itemsObj[i].GetComponent<RectTransform>().position = pos;
        }

        itemsList[0].Select();
    }

    void ViderMenu()
    {
        for(int i = 0; i < nbItems; ++i)
        {
            Destroy(itemsObj[i]);
        }

        itemsList = null;
        itemsObj = null;
        nbItems = 0;
    }


    // Update is called once per frame
    void Update () {
        if (nbItems > 0)
        {
            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                itemsList[indexItem].Unselect();
                indexItem = (indexItem - 1);

                if (indexItem < 0)
                    indexItem = nbItems - 1;

                itemsList[indexItem].Select();
            }
            else if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                itemsList[indexItem].Unselect();
                indexItem = (indexItem + 1) % nbItems;
                itemsList[indexItem].Select();
            }
            else if (Input.GetKeyUp(KeyCode.KeypadEnter))
            {
                handler(itemsList[indexItem].GetValue());
                ViderMenu();
            }
        }

        if ((main.leftDevice == null) || (main.rightDevice == null)) {
            return;
        }

        if (main.leftDevice.GetPress(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger)) {
            Debug.Log("Trigger gauche");
            string path = EditorUtility.OpenFilePanel("Import a virtual scene", "", "obj");
            if (path.Length != 0)
            {
                var fileContent = File.ReadAllBytes(path);
            }
        }

        if (main.rightDevice.GetPress(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger))
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

        if (nbItems > 0 && main.rightDevice.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
        {
            float y = main.rightDevice.GetAxis(EVRButtonId.k_EButton_SteamVR_Touchpad).y;
            if (y > 0.2)
            {
                itemsList[indexItem].Unselect();
                indexItem = (indexItem + 1) % nbItems;
                itemsList[indexItem].Select();
            }
            else if(y < -0.2)
            {
                itemsList[indexItem].Unselect();
                indexItem = (indexItem - 1) % nbItems;
                itemsList[indexItem].Select();
            }            
        }

        if(nbItems > 0 && main.rightDevice.GetPress(Valve.VR.EVRButtonId.k_EButton_ApplicationMenu))
        {
            handler(itemsList[indexItem].GetValue());
            ViderMenu();
        }
    }
}
