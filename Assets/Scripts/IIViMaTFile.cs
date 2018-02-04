using System.Collections;
using System.Collections.Generic;

using SimpleJSON;

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
public class ActionReactionJSON
{
    public Dictionary<int, Dictionary<int, Dictionary<int, List<string>>>> action_reactions_spheres360;
    public Dictionary<int, Dictionary<int, Dictionary<int, List<string>>>> action_reactions_courbes;
}

[System.Serializable]
public class IIViMaTFile
{
    public bool isSequentiel;
    public MediaJSON[] videos;
    public CourbeJSON[] courbes;
    public MediaJSON[] modeles;
    public ActionReactionJSON actionReaction;

    public JSONNode ToJSON()
    {
        JSONNode json = new JSONObject();
        json["isSequentiel"] = isSequentiel;

        /// Vidéos
        json["videos"] = new JSONArray();
        if (videos != null)
        {
            foreach (MediaJSON media in videos)
            {
                JSONNode video = new JSONObject();
                video["path"] = media.path;
                video["isActive"] = media.isActive;

                video["position"] = new JSONObject();
                video["position"]["x"] = media.position.x;
                video["position"]["y"] = media.position.y;
                video["position"]["z"] = media.position.z;

                video["rotation"] = new JSONObject();
                video["rotation"]["x"] = media.rotation.x;
                video["rotation"]["y"] = media.rotation.y;
                video["rotation"]["z"] = media.rotation.z;

                video["scale"] = new JSONObject();
                video["scale"]["x"] = media.scale.x;
                video["scale"]["y"] = media.scale.y;
                video["scale"]["z"] = media.scale.z;

                json["videos"].Add(video);
            }
        }

        /// Courbes 
        json["courbes"] = new JSONArray();
        if (courbes != null)
        {
            foreach (CourbeJSON courbe in courbes)
            {
                JSONNode c = new JSONObject();

                c["color"] = new JSONObject();
                c["color"]["x"] = courbe.color.x;
                c["color"]["y"] = courbe.color.y;
                c["color"]["z"] = courbe.color.z;

                JSONArray points = new JSONArray();
                foreach (Vecteur3_IIViMaT point in courbe.points)
                {
                    JSONObject p = new JSONObject();
                    p["x"] = point.x;
                    p["y"] = point.y;
                    p["z"] = point.z;
                    points.Add(p);
                }
                c["points"] = points;

                JSONArray normals = new JSONArray();
                foreach (Vecteur3_IIViMaT normal in courbe.normals)
                {
                    JSONObject n = new JSONObject();
                    n["x"] = normal.x;
                    n["y"] = normal.y;
                    n["z"] = normal.z;
                    normals.Add(n);
                }
                c["normals"] = normals;

                json["courbes"].Add(c);
            }
        }

        /// Modeles
        json["modeles"] = new JSONArray();
        if (modeles != null)
        {
            foreach (MediaJSON media in modeles)
            {
                JSONNode modele = new JSONObject();
                modele["path"] = media.path;
                modele["isActive"] = media.isActive;

                modele["position"] = new JSONObject();
                modele["position"]["x"] = media.position.x;
                modele["position"]["y"] = media.position.y;
                modele["position"]["z"] = media.position.z;

                modele["rotation"] = new JSONObject();
                modele["rotation"]["x"] = media.rotation.x;
                modele["rotation"]["y"] = media.rotation.y;
                modele["rotation"]["z"] = media.rotation.z;

                modele["scale"] = new JSONObject();
                modele["scale"]["x"] = media.scale.x;
                modele["scale"]["y"] = media.scale.y;
                modele["scale"]["z"] = media.scale.z;

                json["modeles"].Add(modele);
            }
        }

        /// Action-reactions
        json["actionReaction"] = new JSONObject();
        json["actionReaction"]["action_reactions_spheres360"] = new JSONObject();
        if (actionReaction != null && actionReaction.action_reactions_spheres360 != null)
        {
            foreach (KeyValuePair<int, Dictionary<int, Dictionary<int, List<string>>>> source in actionReaction.action_reactions_spheres360)
            {
                json["actionReaction"]["action_reactions_spheres360"][source.Key.ToString()] = new JSONObject();
                foreach (KeyValuePair<int, Dictionary<int, List<string>>> action in source.Value)
                {
                    json["actionReaction"]["action_reactions_spheres360"][source.Key.ToString()][action.Key.ToString()] = new JSONObject();
                    foreach (KeyValuePair<int, List<string>> reaction in action.Value)
                    {
                        json["actionReaction"]["action_reactions_spheres360"][source.Key.ToString()][action.Key.ToString()][reaction.Key.ToString()] = new JSONArray();

                        foreach (string obj_name in reaction.Value)
                            json["actionReaction"]["action_reactions_spheres360"][source.Key.ToString()][action.Key.ToString()][reaction.Key.ToString()].Add(obj_name);
                    }
                }
            }
        }

        json["actionReaction"]["action_reactions_courbes"] = new JSONObject();
        if (actionReaction != null && actionReaction.action_reactions_courbes != null)
        {
            foreach (KeyValuePair<int, Dictionary<int, Dictionary<int, List<string>>>> source in actionReaction.action_reactions_courbes)
            {
                json["actionReaction"]["action_reactions_courbes"][source.Key.ToString()] = new JSONObject();
                foreach (KeyValuePair<int, Dictionary<int, List<string>>> action in source.Value)
                {
                    json["actionReaction"]["action_reactions_courbes"][source.Key.ToString()][action.Key.ToString()] = new JSONObject();
                    foreach (KeyValuePair<int, List<string>> reaction in action.Value)
                    {
                        json["actionReaction"]["action_reactions_courbes"][source.Key.ToString()][action.Key.ToString()][reaction.Key.ToString()] = new JSONArray();

                        foreach (string obj_name in reaction.Value)
                            json["actionReaction"]["action_reactions_courbes"][source.Key.ToString()][action.Key.ToString()][reaction.Key.ToString()].Add(obj_name);
                    }
                }
            }
        }

        return json;
    }
}
