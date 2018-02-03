using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere360 : MonoBehaviour {

    public ActionReaction actionReaction;
    public GameObject cam;
    public Video360 video360;
    public PlayCurve playcurves;

    int video_index = -1;
    public void Start()
    {
        video_index = int.Parse(gameObject.name.Replace("Video360_player(Clone)_", ""));
    }


    public void OnTriggerEnter(Collider other)
    {
        if(video_index != -1)
        {
            if (actionReaction.action_reactions_spheres360.Count > 0)
            {
                if (actionReaction.action_reactions_spheres360.ContainsKey(video_index))
                {
                    // en fonction de la position
                    if (actionReaction.action_reactions_spheres360[video_index].ContainsKey(actionReaction.actions_spheres360["enter"]))
                    {
                        // Parcourt des réactions possibles
                        foreach (KeyValuePair<int, List<GameObject>> reaction in actionReaction.action_reactions_spheres360[video_index][actionReaction.actions_spheres360["enter"]])
                        {
                            if(reaction.Key == actionReaction.reactions_spheres360["activate"])
                            {
                                foreach (GameObject obj in reaction.Value)
                                {
                                    obj.SetActive(true);

                                    if(obj.name.Contains("Video360_player(Clone)_"))
                                    {
                                        int index = int.Parse(obj.name.Replace("Video360_player(Clone)_", ""));
                                        video360.PlayVideo(index);
                                    }
                                }
                            }

                            if (reaction.Key == actionReaction.reactions_spheres360["desactivate"])
                            {
                                foreach (GameObject obj in reaction.Value)
                                    obj.SetActive(false);
                            }

                            if (reaction.Key == actionReaction.reactions_spheres360["play_curve"])
                            {
                                foreach (GameObject obj in reaction.Value)
                                {
                                    int index = int.Parse(obj.name.Replace("Curve(Clone)_", ""));
                                    playcurves.Play(index);
                                }
                            }

                            if (reaction.Key == actionReaction.reactions_spheres360["tp"])
                            {
                                foreach (GameObject obj in reaction.Value)
                                    cam.transform.position = obj.transform.position;
                            }
                        }
                    }
                }
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {

    }
}
