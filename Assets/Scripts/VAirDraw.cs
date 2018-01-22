using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class VAirDraw : MonoBehaviour {

    /* ----------------------------------------------------------- */
    public GameObject curvePrefabs;
    public Bezier bezier;
    public Main main;
    public Menu menu;

    private float scale = 1f;

    private int TimeToMoveCurve = 0;

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
    public void ImportFromVAirDraw(string file)
    {
        // Open the file
        StreamReader reader = new StreamReader(main.config.path_to_import + "/VAirDraw/" + file);

        // Initialize map of <int, Curve>
        Dictionary<int, Curve> curves = new Dictionary<int, Curve>();

        // Current line of file
        string currentline = null;

        // Version of VAirDraw Capture
        currentline = reader.ReadLine();
        int version = currentline == "#VRCAPA 1" ? 1 : 3;

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
            float r, g, b;

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

        // Variable of current point
        Vector3 point = new Vector3(), normal = new Vector3();
        int indexColor;

        // Read all point
        while (currentline != null)
        {
            // Get data
            string[] data = currentline.Split(' ');

            if (version == 1)
            {
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
            }
            else
            {
                // Get color
                indexColor = int.Parse(data[6]);

                // Get position
                point.x = float.Parse(data[7]);
                point.y = float.Parse(data[8]);
                point.z = float.Parse(data[9]);

                // Get normal
                normal.x = float.Parse(data[10]);
                normal.y = float.Parse(data[11]);
                normal.z = float.Parse(data[12]);
            }

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

        main.ShowMessage("Mise en place des courbes dans l'espace.");
        TimeToMoveCurve = 1;
    }

    void Update()
    {   
        if(TimeToMoveCurve == 2)
        {           
            Vector3 position = new Vector3();

            if(Input.GetKeyUp(KeyCode.KeypadPlus))
            {
                scale *= 1.5f;
            }

            if (Input.GetKeyUp(KeyCode.KeypadMinus))
            {
                scale *= 0.75f;
            }

            if (Input.GetKeyUp(KeyCode.UpArrow))
                position.z++;
            if (Input.GetKeyUp(KeyCode.DownArrow))
                position.z--;
            if (Input.GetKeyUp(KeyCode.RightArrow))
                position.x++;
            if (Input.GetKeyUp(KeyCode.LeftArrow))
                position.x--;

            if (Input.GetKeyUp(KeyCode.KeypadEnter))
                TimeToMoveCurve = 0;

            // HTC Vive Controller
            if (main.rightDevice != null && main.leftDevice != null)
            {
                // Définition echelle et position
                scale = main.rightDevice.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x + 1 * 5;
                position = main.right.transform.position;

                // Si on valide
                if (main.rightDevice.GetPress(Valve.VR.EVRButtonId.k_EButton_ApplicationMenu))
                {
                    TimeToMoveCurve = 0;
                }
            }    

            // Application pour chaque courbe
            GameObject[] curvesObjects = GameObject.FindGameObjectsWithTag("Curve");
            
            foreach (GameObject curve in curvesObjects)
            {
                curve.transform.localScale = new Vector3(scale, scale, scale);
                curve.transform.position += position;
            }
            GameObject[] BezierObjects = GameObject.FindGameObjectsWithTag("Bezier");

            foreach (GameObject bezier in BezierObjects)
            {
                bezier.transform.localScale = new Vector3(scale, scale, scale);
                bezier.transform.position += position;
            }
        }

        // Si on retire le message box
        if (TimeToMoveCurve == 1)
        {
            if (Input.GetKeyUp(KeyCode.KeypadEnter))
                TimeToMoveCurve = 2;

            if (main.rightDevice != null && main.leftDevice != null && main.rightDevice.GetPress(Valve.VR.EVRButtonId.k_EButton_ApplicationMenu))
                TimeToMoveCurve = 2;
        }
    }


    void Awake()
    {
        // List all curves
        /*ListFilesFromDir lfd = new ListFilesFromDir(main.config.path_to_import + "/VAirDraw/");
        string[] allFilesCurves = lfd.files;
        
        Menu.Del handler = ImportFromVAirDraw;

        // Show list to content's creator
        menu.AddItems(allFilesCurves, handler);*/
    }
   
}
