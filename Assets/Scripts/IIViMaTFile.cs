[System.Serializable]
public class Vecteur3_IIViMaT
{
    public float x, y, z;
}

[System.Serializable]
public class VideoJSON
{
    public string path;
    public Vecteur3_IIViMaT position;
}

[System.Serializable]
public class IIViMaTFile  {
    public bool isSequentiel;
    public VideoJSON[] videos;
}
