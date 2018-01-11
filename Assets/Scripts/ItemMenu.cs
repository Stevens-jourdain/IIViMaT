using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemMenu : MonoBehaviour {
    
    public void Select()
    {
        GetComponent<Image>().color = Color.blue;
    }

    public void Unselect()
    {
        GetComponent<Image>().color = Color.white;
    }

    public string GetValue()
    {
        return GetComponentInChildren<Text>().text;
    }

    public void SetValue(string v)
    {
        GetComponentInChildren<Text>().text = v;
    }
    
}
