using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Timers;
using Valve.VR;

public class PlayCurve : MonoBehaviour {

    // Attributes
    private int currentIndexPoint = -1;
    private int currentIndexCurve = -1;

    // List of curve
    public List<Curve> curves = new List<Curve>();

    // Camera
    public GameObject cam = null;

    // Vitesse 
    public double vitesse = 100;

    // Changement de courbe
    private bool changed = false;

    // Action reaction
    public ActionReaction actionReaction;

    // Timer
    Timer t = null;

    // Main script
    public Main main;

    /* -------------------------------------------------------------- */
    /* -------------------- Play Curve Method ----------------------- */
    public void IncrementCamera()
    {        
        ++currentIndexPoint;

        if(currentIndexPoint == curves[currentIndexCurve].GetNbPoints())
        {
            t.Stop();
            t.Close();
            t = null;
            ChangeCurve(currentIndexCurve+1);
        }
        changed = true;
    }

    public void ChangeCurve(int index)
    {
        if (curves.Count > index)
        {
            currentIndexCurve = index;
            currentIndexPoint = 0;

            changed = true;
        }
        else        
            ChangeCurve(0);        
    }

    public void Play()
    {
        if (currentIndexPoint == 0)
        {
            t = new Timer();
            t.Elapsed += new ElapsedEventHandler(OnTimer);
        }

        t.Interval = vitesse; // 1000/24;

        if (currentIndexPoint == 0)        
            t.Start();        
    }

    public void Pause()
    {
        if(t != null)        
            t.Interval = double.MaxValue;        
    }

    public void OnTimer(object source, ElapsedEventArgs e)
    {
        if (t != null)
        {
            IncrementCamera();
        }
    }

    // Use this for initialization
    void Start ()
    {
        
	}
	
	// Update is called once per frame
	void Update () {

        if(curves.Count == 0)
        {
            GameObject[] curvesObjects = GameObject.FindGameObjectsWithTag("Curve");
            
            foreach (GameObject curve in curvesObjects)
            {
                Curve c = curve.GetComponent<Curve>();
                curves.Add(c);
            }
            currentIndexPoint = 0;
            currentIndexCurve = 0;
        }

        if(changed)
        {
            cam.transform.position = curves[currentIndexCurve%3].GetPoints()[currentIndexPoint];
            changed = false;

            // Application d'une reaction ?
            actionReaction.ProcessActionReaction(currentIndexPoint);
        }

	    if(main.leftDevice != null && main.rightDevice != null)
        {
            if (currentIndexPoint != -1)
            {
                if (main.leftDevice.GetPress(Valve.VR.EVRButtonId.k_EButton_ApplicationMenu))
                {
                    Play();
                }

                if (main.rightDevice.GetPress(Valve.VR.EVRButtonId.k_EButton_ApplicationMenu))
                {
                    Pause();
                }
            }

            /*if (leftDevice.GetPress(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger) || rightDevice.GetPress(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger))
            {
                Debug.Log("Appuie sur le trigger");

                // Foreach all curve
                GameObject[] curvesObjects = GameObject.FindGameObjectsWithTag("Curve");

                Debug.Log(curvesObjects);

                foreach(GameObject curve in curvesObjects)
                {
                    Curve c = curve.GetComponent<Curve>();
                    Debug.Log(c);
                    if (c.IsPadLeftContact || c.IsPadRightContact)
                    {
                        if (!curves.Contains(c))
                        {
                            curves.Add(c);

                            Debug.Log("Ajout d'une trajectoire à la liste de lecture");

                            if (currentIndexPoint == -1)
                            {
                                currentIndexPoint = 0;
                                currentIndexCurve = 0;
                            }
                        }
                    }
                }
            }*/
        }    
	}
}
