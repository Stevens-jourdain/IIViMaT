using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bezier : MonoBehaviour {

    /* ----------------------------------------------------------- */
    /* -------------------- Attributes --------------------------- */    
    public GameObject bezierCurvePrefabs;
    private int nbPoints = 10000;

    /* -------------------------------------------------------------- */
    /* -------------------- Curve Methods --------------------------- */

    /**
     * @brief Define the i-th tangeante
     * 
     * @param[in]   i           the index
     * @param[in]   points      the controls points
     * 
     * @return the tangent
     */
    private Vector3 Tangent(int i, ref List<Vector3> points)
     {
        if (i + 1 == points.Count)
            return points[i] - points[i - 1];
        else if (i - 1 < 0)
            return points[i + 1] - points[i];
        else
            return points[i + 1] - points[i - 1];        
     }

    /**
     * @brief Define the Bezier Cubique
     * 
     * @param[in]   controlsPoints      the controls' points
     */ 
    public void DefineBezier(Curve controlsPoint)
    {
        // Get controls points
        List<Vector3> points = controlsPoint.GetPoints();
        // How many ?
        int nbControlsPoints = points.Count;
        
        // Cast to double only one time
        float d_nbPoint = nbPoints;

        // Instanciate Bezier
        GameObject curveObj = Instantiate(bezierCurvePrefabs);
        curveObj.tag = "Bezier";
        Curve c = curveObj.GetComponent<Curve>();        
        c.SetColor(controlsPoint.GetColor(), 0.9f);

        // Current point
        Vector3 P0 = new Vector3(), P1 = new Vector3(), T1 = new Vector3(), T0 = new Vector3();

        // Fo each point
        for(int i = 0; i < nbPoints; ++i)
        {
            // Calculate t
            float t = (float)i / d_nbPoint;

            // Calculate index
            int i_0 = (int) (t * (nbControlsPoints-1));
            int i_1 = i_0 + 50;

            // check index 
            if (i_1 >= nbControlsPoints)
            {
                // Add this point to curve
                c.AddPoint(points[i_0]);
            }
            else
            {
                // Get points
                P0 = points[i_0];
                P1 = points[i_1];

                // Get tangent
                T0 = Tangent(i_0, ref points);
                T1 = Tangent(i_1, ref points);

                // Ajust paramater
                t = t * (nbControlsPoints - 1) - i_0;

                // Add the Bezier point
                //c.AddPoint(points[i_0] * Mathf.Pow(1f - t, 3f) + 3 * T0 * t * Mathf.Pow(1f - t, 2f) + 3 * T1 * (t * t) * (1f - t) + points[i_1] * Mathf.Pow(t, 3f));
                c.AddPoint(Mathf.Pow(t, 3.0f) * (2 * P0 - 2 * P1 + T0 + T1) + Mathf.Pow(t, 2.0f) * (-3 * P0 + 3 * P1 - 2 * T0 - T1) + t * T0 + P0); 
            }            
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
