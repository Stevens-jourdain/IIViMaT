﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;
using UnityEngine.Video;

using Valve.VR;

public class Menu : MonoBehaviour {                
    public GameObject ItemMenu_prefabs;
    public GameObject[] itemsObj;
    private ItemMenu[] itemsList;
    public int indexItem = 0, nbItems;

    public Camera cam;
    public Main main;

    // Gestion du trigger HTC VIVE
    public TriggerManager triggerManager;

    private bool isObject = false;

    private float lastTime;

    Del handler;

	// Use this for initialization
	void Start () {
		string[] TitleMenu = new string[2];
		TitleMenu[0] = "Nouveau";
		TitleMenu[1] = "Charger";

		Menu.Del handler = main.GestionMenu;

		main.menu.AddItems(TitleMenu, handler);
    }

    public delegate void Del(string item);

    public void AddItems(string[] items, Del handler)
    {
        isObject = false;

        this.handler = handler;
        nbItems = items.Length;
        indexItem = 0;
        itemsList = new ItemMenu[nbItems];
        itemsObj = new GameObject[nbItems];

        for (int i = 0; i < nbItems; ++i)
        {
            itemsObj[i] = Instantiate(ItemMenu_prefabs, transform);
            itemsObj[i].transform.parent = this.transform;
            itemsObj[i].transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
            itemsObj[i].transform.position = new Vector3(0 + this.transform.parent.position.x, 1.0f - (i * 0.07f), 0.7f + this.transform.parent.position.z);
            itemsObj[i].SetActive(true);           

            itemsList[i] = itemsObj[i].GetComponent<ItemMenu>();
            itemsList[i].SetValue(items[i]);
        }

        if(nbItems > 0)
            itemsList[0].Select();
    }

    public void AddItemsObj(GameObject[] items, Del handler)
    {
        isObject = true;

        this.handler = handler;
        nbItems = items.Length;
        indexItem = 0;
        itemsObj = items;

        if(nbItems > 0)
            itemsObj[indexItem].GetComponent<ToggleShowAnimation>().LancerAnimation();
    }

    void ViderMenu()
    {
        if (!isObject)
        {
            for (int i = 0; i < nbItems; ++i)
            {
                Destroy(itemsObj[i]);
            }
        }
        else
        {
            // Stop animation de l'objet
            itemsObj[indexItem].GetComponent<ToggleShowAnimation>().StopperAnimation();
        }

        itemsList = null;
        itemsObj = null;
        nbItems = 0;
    }


    // Update is called once per frame
    void Update () {
        if (main.actionEnCours == true)
            return;

        // Selection dans le menu
        if (nbItems > 0)
        {
            if (Input.GetKeyUp(KeyCode.Escape))
                ViderMenu();

            if (Input.GetKeyUp(KeyCode.UpArrow))
            {                
                if (indexItem - 1 >= 0)
                {
                    if (isObject)
                    {
                        // Stop animation de l'objet
                        itemsObj[indexItem].GetComponent<ToggleShowAnimation>().StopperAnimation();

                        // Objet suivant
                        indexItem = (indexItem - 1);

                        // On regarde l'objet
                        cam.transform.LookAt(itemsObj[indexItem].transform);

                        // Lancer l'animation de l'objet
                        itemsObj[indexItem].GetComponent<ToggleShowAnimation>().LancerAnimation();
                    }
                    else
                    {
                        if (indexItem > 5)
                        {
                            this.transform.position -= new Vector3(0, 0.075f, 0);
                        }

                        itemsList[indexItem].Unselect();
                        indexItem = (indexItem - 1);

                        if (indexItem < 0)
                            indexItem = nbItems - 1;

                        itemsList[indexItem].Select();
                    }
                }
                
            }
            else if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                if (indexItem + 1 < nbItems)
                {
                    if (isObject)
                    {
                        // Stop animation de l'objet
                        itemsObj[indexItem].GetComponent<ToggleShowAnimation>().StopperAnimation();

                        // Objet suivant
                        indexItem = (indexItem + 1);

                        // On regarde l'objet
                        cam.transform.LookAt(itemsObj[indexItem].transform);

                        // Lancer l'animation de l'objet
                        itemsObj[indexItem].GetComponent<ToggleShowAnimation>().LancerAnimation();
                    }
                    else
                    {
                        itemsList[indexItem].Unselect();
                        indexItem = (indexItem + 1);
                        itemsList[indexItem].Select();

                        if (indexItem > 5)
                        {
                            this.transform.position += new Vector3(0, 0.075f, 0);
                        }
                    }
                }                                
            }
            else if (Input.GetKeyUp(KeyCode.KeypadEnter))
            {
                string value = "";
                if (isObject)
                    value = itemsObj[indexItem].name;
                else
                    value = itemsList[indexItem].GetValue();
                
                ViderMenu();
                handler(value);                
            }

            // Sans VR on va pas plus loin dans cette fonction
            if ((main.leftDevice == null) || (main.rightDevice == null))
            {
                return;
            }

            // Selection menu en VR
            float y = 0;
            if (main.rightDevice.GetPress(SteamVR_Controller.ButtonMask.Touchpad))            
                y = main.rightDevice.GetAxis(EVRButtonId.k_EButton_SteamVR_Touchpad).y;
            else if(main.leftDevice.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
                y = main.leftDevice.GetAxis(EVRButtonId.k_EButton_SteamVR_Touchpad).y;

            float currentTime = Time.time;

            if (y > 0.2 && indexItem - 1 >= 0 && currentTime - lastTime > 0.5f)
            {
                lastTime = currentTime;
                if (isObject)
                {
                    // Stop animation de l'objet
                    itemsObj[indexItem].GetComponent<ToggleShowAnimation>().StopperAnimation();

                    // Objet suivant
                    indexItem = (indexItem - 1);

                    // On regarde l'objet
                    cam.transform.LookAt(itemsObj[indexItem].transform);

                    // Lancer l'animation de l'objet
                    itemsObj[indexItem].GetComponent<ToggleShowAnimation>().LancerAnimation();
                }
                else
                {
                    itemsList[indexItem].Unselect();
                    indexItem = (indexItem - 1);
                    itemsList[indexItem].Select();
                }
            }
            else if (y < -0.2 && indexItem + 1 < nbItems && currentTime - lastTime > 0.5f)
            {
                lastTime = currentTime;
                if (isObject)
                {
                    // Stop animation de l'objet
                    itemsObj[indexItem].GetComponent<ToggleShowAnimation>().StopperAnimation();

                    // Objet suivant
                    indexItem = (indexItem + 1);

                    // On regarde l'objet
                    cam.transform.LookAt(itemsObj[indexItem].transform);

                    // Lancer l'animation de l'objet
                    itemsObj[indexItem].GetComponent<ToggleShowAnimation>().LancerAnimation();
                }
                else
                {
                    itemsList[indexItem].Unselect();
                    indexItem = (indexItem + 1);
                    itemsList[indexItem].Select();
                }
            }                

            if (triggerManager.OnTriggerUp(main.rightDevice) || triggerManager.OnTriggerUp(main.leftDevice))
            {
                string value = "";
                if (isObject)
                    value = itemsObj[indexItem].name;
                else
                    value = itemsList[indexItem].GetValue();

                ViderMenu();
                handler(value);
            }

            if (main.rightDevice.GetPress(Valve.VR.EVRButtonId.k_EButton_ApplicationMenu) || main.leftDevice.GetPress(Valve.VR.EVRButtonId.k_EButton_ApplicationMenu))
                ViderMenu();
        }

        
    }
}
