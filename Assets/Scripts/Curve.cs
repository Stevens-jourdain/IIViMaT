using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curve : MonoBehaviour {

    /* ----------------------------------------------------------- */
    /* -------------------- Attributes --------------------------- */
    private List<Vector3> positions  = new List<Vector3>();
    private List<Vector3> normals    = new List<Vector3>();
    private bool redraw = false;
    private LineRenderer lineRenderer = null;
    private Color color = Color.red;

    public bool IsPadLeftContact = false, IsPadRightContact = false;

    /* -------------------------------------------------------------- */
    /* -------------------- Curve Methods --------------------------- */

    /**
     * @brief getter position
     * 
     * @return list of position
     */
    public List<Vector3> GetPoints()
    {
        return positions;
    }

    public int GetNbPoints()
    {
        return positions.Count;
    }

    /**
     * @brief add a point to curve
     * 
     * @param[in]   p       the point to add
     */
    public void AddPointAndNormal(Vector3 p, Vector3 n)
    {
        positions.Add(p);
        normals.Add(n);
    }

    /**
     * @brief add a point to curve
     * 
     * @param[in]   p       the point to add
     */
    public void AddPoint(Vector3 p)
    {
        positions.Add(p);
    }

    /**
     * @brief add a normal to curve
     * 
     * @param[in]   n       the normal to add
     */
    public void AddNormal(Vector3 n)
    {
        normals.Add(n);
    }

    /**
     * @brief set color of curve
     * 
     * @param[in]   c       the color
     */
    public void SetColor(Color c)
    {
        color = c;
    }

    public void SetColor(Color c, float a)
    {
        c.a = a;
        color = c;
    }

    public Color GetColor()
    {
        return color;
    }

    /**
     * @brief  tell to script "ok draw now"
     */
    public void FinalizeImport()
    {
        redraw = true;
    }

    /**
     * @brief Modify a point of curve
     * 
     * @param[in]   index           the relative position in memory from the first point of curve
     * @param[in]   newPosition     the new position
     */ 
    public void ModifyPoint(int index, Vector3 newPosition)
    {
        // Set the position
        positions[index] = newPosition;

        // Redraw courbe
        redraw = true;
    }

    /**
     * @brief Modify a normal (orientation) of curve
     * 
     * @param[in]   index         the relative position in memory from the first point of curve
     * @param[in]   newNormal     the new normal (orientation)
     */
    public void ModifyNormal(int index, Vector3 newNormal)
    {
        // Set the position
        normals[index] = newNormal;

        // Redraw courbe
        redraw = true;
    }

    /* ----------------------------------------------------------- */
    /* -------------------- Unity Func --------------------------- */
    // Use this for initialization
    void Start () {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
    }
	
	// Update is called once per frame
	void Update () {
        if (redraw && lineRenderer != null)
        {            
            // Update number of point
            lineRenderer.positionCount = positions.Count;
            // Set all of points
            lineRenderer.SetPositions(positions.ToArray());
            // Set color
            lineRenderer.material.color = color;

            // No redraw after that
            redraw = false;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("il y a un contact ");
        Debug.Log(other.gameObject.name);
        if(other.CompareTag("LeftPad"))
        {
            IsPadLeftContact = true;
            Debug.Log("Contact avec le pad gauche : " + color);
        }

        if (other.CompareTag("RightPad"))
        {
            IsPadRightContact = true;
            Debug.Log("Contact avec le pad droit : " + color);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        Debug.Log("il y a une perte de contact ");
        Debug.Log(other.gameObject.name);

        if (other.CompareTag("LeftPad"))
        {
            IsPadLeftContact = false;
            Debug.Log("Perte du contact avec le pad gauche : " + color);
        }

        if (other.CompareTag("RightPad"))
        {
            IsPadRightContact = false;

            Debug.Log("Perte du contact avec le pad droit : " + color);
        }
    }
}
