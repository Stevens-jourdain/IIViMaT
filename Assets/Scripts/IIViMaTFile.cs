[System.Serializable]
public class Vecteur3_IIViMaT
{
    public float x, y, z;
}

[System.Serializable]
public class MediaJSON
{
    public string path;
    public bool isActive;
    public Vecteur3_IIViMaT position, rotation, scale;
}

[System.Serializable]
public class CourbeJSON
{
    public Vecteur3_IIViMaT[] points;
    public Vecteur3_IIViMaT[] normals;
    public Vecteur3_IIViMaT color;
}

[System.Serializable]
public class IIViMaTFile
{
    public bool isSequentiel;
    public MediaJSON[] videos;
    public CourbeJSON[] courbes;
    public MediaJSON[] modeles;
}
