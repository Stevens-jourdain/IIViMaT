using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class VAirDraw : MonoBehaviour {

    /* ----------------------------------------------------------- */
    public GameObject curvePrefabs;
    public Bezier bezier;

    private void ReadWithoutComment(ref StreamReader reader, ref string line)
    {
        do
        {
            line = reader.ReadLine();
        } while (line != null && line[0] == '#');
    }

    /**
     * @brief import curve from VAirDraw
     **/
    public void ImportFromVAirDraw()
    {
        // Open the file
        string path = EditorUtility.OpenFilePanel("Importer une trajectoire", Application.dataPath + "/To import/VAirDraw/", "txt");

        // If it's selected
        if (path.Length != 0)
        {
            // Open the file
            StreamReader reader = new StreamReader(path);

            // Initialize map of <int, Curve>
            Dictionary<int, Curve> curves = new Dictionary<int, Curve>();

            // Current line of file
            string currentline = null;
            // Remove comments
            ReadWithoutComment(ref reader, ref currentline);

            // Get number of colors
            int nbColors = int.Parse(currentline);

            // Initialize list of color
            Color[] colors = new Color[nbColors];

            // Read colors
            for(int i = 0; i < nbColors; ++i)
            {
                ReadWithoutComment(ref reader, ref currentline);

                // Get data
                string[] data = currentline.Split(' ');
                float r, g, b, a;

                // Extract rgb data
                r = (float)int.Parse(data[0]) / 255f;
                g = (float)int.Parse(data[1]) / 255f;
                b = (float)int.Parse(data[2]) / 255f;

                // Set the color
                colors[i] = new Color(r, g, b, 0.1f);
            }

            // Skip timestamp and first point
            ReadWithoutComment(ref reader, ref currentline);
            ReadWithoutComment(ref reader, ref currentline);
            ReadWithoutComment(ref reader, ref currentline);

            // Variable of current point
            Vector3 point = new Vector3(), normal = new Vector3();
            int indexColor;

            // Read all point
            while (currentline != null)
            {
                // Get data
                string[] data = currentline.Split(' ');

                // Get color
                indexColor = int.Parse(data[3]);

                // Get position
                point.x = float.Parse(data[4]);
                point.y = float.Parse(data[5]);
                point.z = float.Parse(data[6]);

                // Get normal
                normal.x = float.Parse(data[7]);
                normal.y = float.Parse(data[8]);
                normal.z = float.Parse(data[9]);

                // If it's new curve
                if (!curves.ContainsKey(indexColor))
                {
                    // Instantiate prefabs
                    GameObject obj = Instantiate(curvePrefabs);

                    // Initialize
                    curves[indexColor] = obj.GetComponent<Curve>();
                    // Set color
                    curves[indexColor].SetColor(colors[indexColor]);
                }

                // Add Point and normal
                curves[indexColor].AddPointAndNormal(point, normal);

                // Read next line
                ReadWithoutComment(ref reader, ref currentline);
            }

            foreach(var keyvalue in curves)
            {
                // Draw curve
                keyvalue.Value.FinalizeImport();

                // Draw Bezier
                bezier.DefineBezier(keyvalue.Value);
            }            
                

            // Close reader
            reader.Close();
        }
    }

    /* ----------------------------------------------------------- */
    /* -------------------- Unity Func --------------------------- */
    // Use this for initialization
    void Start () {
        ImportFromVAirDraw();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
