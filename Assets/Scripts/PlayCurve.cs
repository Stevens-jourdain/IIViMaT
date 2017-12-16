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

    private bool changed = false;

    // Timer
    Timer t = null;

    // Pad
    public SteamVR_TrackedObject left = null, right = null;
    private SteamVR_Controller.Device leftDevice = null, rightDevice = null;

    /* -------------------------------------------------------------- */
    /* -------------------- Play Curve Method ----------------------- */
    public void IncrementCamera()
    {
        ++currentIndexPoint;

        if(currentIndexPoint == curves[currentIndexCurve].GetNbPoints())
        {
            t = null;
            currentIndexPoint = 0;
            currentIndexCurve++;
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
    }

    public void Play()
    {
        t = new Timer();
        t.Elapsed += new ElapsedEventHandler(OnTimer);
        t.Interval = 100; // 1000/24;
        t.Start();
    }

    public void Pause()
    {
        if(t != null)
            t.Stop();
    }

    public void OnTimer(object source, ElapsedEventArgs e)
    {
        if(t != null)
            IncrementCamera();
    }

    void FixedUpdate()
    {
        // Get Device
        if(((int) left.index != -1 && (int) right.index != -1) && (leftDevice == null || rightDevice == null))
        {
            leftDevice = SteamVR_Controller.Input((int)left.index);
            rightDevice = SteamVR_Controller.Input((int)right.index);

            Debug.Log("Initialisation des pads");
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
            GameObject[] curvesObjects = GameObject.FindGameObjectsWithTag("Bezier");

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
            cam.transform.position = curves[currentIndexCurve].GetPoints()[currentIndexPoint];
            changed = false;
        }

	    if(leftDevice != null && rightDevice != null)
        {
            if (currentIndexPoint != -1)
            {
                if (leftDevice.GetPress(Valve.VR.EVRButtonId.k_EButton_ApplicationMenu))
                {
                    Play();
                }

                if (rightDevice.GetPress(Valve.VR.EVRButtonId.k_EButton_ApplicationMenu))
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
