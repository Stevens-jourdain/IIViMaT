using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemMenu : MonoBehaviour {
    
    public void Select()
    {
        GetComponent<Image>().color = new Color(0.0f, 158.0f / 255.0f, 1.0f, 0.8f); 
    }

    public void Unselect()
    {
        GetComponent<Image>().color = new Color(0.9f, 0.9f, 0.9f, 0.8f);
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
