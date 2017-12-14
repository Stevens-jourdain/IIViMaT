using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bezier : MonoBehaviour {

    /* ----------------------------------------------------------- */
    /* -------------------- Attributes --------------------------- */    
    public GameObject bezierCurvePrefabs;
    private int nbPoints = 100000;

    /* -------------------------------------------------------------- */
    /* -------------------- Curve Methods --------------------------- */
    public void DefineBezier(Curve controlsPoint)
    {
        // Get controls points
        List<Vector3> points = controlsPoint.GetPoints();
        // How many ?
        int nbControlsPoints = points.Count;

        // Cast to double only one time
        float d_nbPoint = (float)nbPoints;

        // Instanciate Bezier
        Curve c = Instantiate(bezierCurvePrefabs).GetComponent<Curve>();
        c.SetColor(controlsPoint.GetColor(), 0.9f);

        // Current point
        Vector3 P = new Vector3(), T1 = new Vector3(), T0 = new Vector3();

        // Fo each point
        for(int i = 0; i < nbPoints; ++i)
        {
            // Calculate t
            float t = (float)i / d_nbPoint;

            // Calculate index
            int i_0 = (int) (t * nbControlsPoints);
            int i_1 = i_0 + 1;

            // check index 
            if (i_1 == nbControlsPoints)
            {
                i_1 = i_0;
                T1 = points[i_1] - points[i_0 - 1];
            }
            else
            {
                if(i_1 + 1 == nbControlsPoints)
                    T1 = points[i_1] - points[i_0];
                else
                    T1 = points[i_1 + 1] - points[i_1];
            }

            if (i_0 == 0)
            {
                T0 = points[i_1] - points[i_0];
            }
            else
            {
                T0 = points[i_0] - points[i_0 - 1];
            }

            P = points[i_0] * Mathf.Pow(1f - t, 3f) + 3 * T0 * t * Mathf.Pow(1f-t, 2f) + 3 * T1 * (t * t) * (1f-t) + points[i_1] * Mathf.Pow(t, 3f);

            // Add this point to curve
            c.AddPoint(P);
        }

        // Draw Curve
        c.FinalizeImport();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
