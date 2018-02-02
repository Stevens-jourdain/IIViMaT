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

    public Main main;

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
            itemsObj[i].transform.position = new Vector3(0, 0.21f - (i * 0.07f), -0.5f);
            itemsObj[i].SetActive(true);           

            itemsList[i] = itemsObj[i].GetComponent<ItemMenu>();
            itemsList[i].SetValue(items[i]);
        }

        if(nbItems > 0)
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
        // Selection dans le menu
        if (nbItems > 0)
        {
            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                if (indexItem - 1 >= 0)
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
            else if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                if (indexItem + 1 < nbItems)
                {
                    itemsList[indexItem].Unselect();
                    indexItem = (indexItem + 1) % nbItems;
                    itemsList[indexItem].Select();

                    if (indexItem > 5)
                    {
                        this.transform.position += new Vector3(0, 0.075f, 0);
                    }
                }                                
            }
            else if (Input.GetKeyUp(KeyCode.KeypadEnter))
            {
                string value = itemsList[indexItem].GetValue();
                ViderMenu();
                handler(value);                
            }
        }

        // Sans VR on va pas plus loin dans cette fonction
        if ((main.leftDevice == null) || (main.rightDevice == null))
        {
            return;
        }

        // Selection menu en VR
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
        if(nbItems > 0 && main.rightDevice.GetPress(Valve.VR.EVRButtonId.k_EButton_A))
        {
            string value = itemsList[indexItem].GetValue();
            ViderMenu();
            handler(value);
        }
    }
}
