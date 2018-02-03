using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionReaction : MonoBehaviour
{

    // Systeme d'action-réaction pour la courbe
    public Dictionary<int, Dictionary<int, Dictionary<int, List<GameObject>>>> action_reactions_courbes;
    public Dictionary<string, int> actions_courbes, reactions_courbes;

    // Systeme d'action-réaction pour les sphères
    public Dictionary<int, Dictionary<int, Dictionary<int, List<GameObject>>>> action_reactions_spheres360;
    public Dictionary<string, int> actions_spheres360, reactions_spheres360;

    /**
     * @brief Ajoute une réaction à une action à partir d'un point
     * 
     * @param[in]   pointCourbe         le point où se définit l'action
     * @param[in]   action              l'action à définir
     * @param[in]   reaction            la réaction en conséquence
     */
    public void AddActionReactionsCourbe(int pointCourbe, int action, int reaction, List<GameObject> obj_reaction_list)
    {
        if (action_reactions_courbes[pointCourbe] == null)
            action_reactions_courbes[pointCourbe] = new Dictionary<int, Dictionary<int, List<GameObject>>>();

        if (action_reactions_courbes[pointCourbe][action] == null)
            action_reactions_courbes[pointCourbe][action] = new Dictionary<int, List<GameObject>>();

        action_reactions_courbes[pointCourbe][action].Add(reaction, obj_reaction_list);
    }

    public void AddActionReactionsSphere360(int indiceSphere, int action, int reaction, List<GameObject> obj_reaction_list)
    {
        if (action_reactions_spheres360[indiceSphere] == null)
            action_reactions_spheres360[indiceSphere] = new Dictionary<int, Dictionary<int, List<GameObject>>>();

        if (action_reactions_spheres360[indiceSphere][action] == null)
            action_reactions_spheres360[indiceSphere][action] = new Dictionary<int, List<GameObject>>();

        action_reactions_spheres360[indiceSphere][action].Add(reaction, obj_reaction_list);
    }

    // Constructeur
    public ActionReaction()
    {
        /// ---- Les courbes
        // Instanciate
        action_reactions_courbes = new Dictionary<int, Dictionary<int, Dictionary<int, List<GameObject>>>>();
        actions_courbes = new Dictionary<string, int>();
        reactions_courbes = new Dictionary<string, int>();

        // Fill list of actions
        int i = 0;
        actions_courbes.Add("position", i);
        actions_courbes.Add("button", ++i);
        actions_courbes.Add("menu", ++i);

        // Fill list of reaction
        i = 0;
        reactions_courbes.Add("show", i);
        reactions_courbes.Add("hide", ++i);
        reactions_courbes.Add("play_sequence", ++i);
        reactions_courbes.Add("play_curve", ++i);
        reactions_courbes.Add("pause_curve", ++i);
        reactions_courbes.Add("play_360", ++i);
        reactions_courbes.Add("pause_360", ++i);
        reactions_courbes.Add("play_sound", ++i);

        /// ---- Les spheres 360
        // Instanciate
        action_reactions_spheres360 = new Dictionary<int, Dictionary<int, Dictionary<int, List<GameObject>>>>();
        actions_spheres360 = new Dictionary<string, int>();
        reactions_spheres360 = new Dictionary<string, int>();

        // Fill list of actions
        i = 0;
        actions_spheres360.Add("enter", i);
        actions_spheres360.Add("exit", ++i);
        actions_spheres360.Add("end_of_video", ++i);

        // Fill list of reaction
        i = 0;
        reactions_spheres360.Add("activate", i);
        reactions_spheres360.Add("desactivate", ++i);
        reactions_spheres360.Add("play_curve", ++i);
        reactions_spheres360.Add("tp", ++i);
    }


    public void ProcessActionReaction(int position)
    {
        if (action_reactions_courbes.Count > 0)
        {
            if (action_reactions_courbes.ContainsKey(position))
            {
                // en fonction de la position
                if (action_reactions_courbes[position].ContainsKey(actions_courbes["position"]))
                {
                    // Parcourt des réactions possibles
                    foreach (KeyValuePair<int, List<GameObject>> reaction in action_reactions_courbes[position][actions_courbes["position"]])
                    {
                        // Afficher les objets
                        if (reaction.Key == reactions_courbes["show"])
                        {
                            foreach (GameObject obj in reaction.Value)
                                obj.SetActive(true);
                        }

                        // Cacher des objets
                        if (reaction.Key == reactions_courbes["hide"])
                        {
                            foreach (GameObject obj in reaction.Value)
                                obj.SetActive(false);
                        }

                        // Jouer une sequence
                        if (reaction.Key == reactions_courbes["play_sequence"])
                        {
                            foreach (GameObject obj in reaction.Value)
                            {
                                // GetComponent<Sequence>().Play()
                            }
                        }

                        // Mettre en lecture la trajectoire
                        if (reaction.Key == reactions_courbes["play_curve"])
                        {
                            foreach (GameObject obj in reaction.Value)
                            {

                            }
                        }

                        // Mettre en pause la trajectoire
                        if (reaction.Key == reactions_courbes["pause_curve"])
                        {
                            foreach (GameObject obj in reaction.Value)
                            {

                            }
                        }

                        // Mettre en lecture la video 360
                        if (reaction.Key == reactions_courbes["play_360"])
                        {
                            foreach (GameObject obj in reaction.Value)
                            {
                                // GetComponent<360>().Play()
                            }
                        }

                        // Mettre en pause la video 360
                        if (reaction.Key == reactions_courbes["pause_360"])
                        {
                            foreach (GameObject obj in reaction.Value)
                            {
                                // GetComponent<360>().Pause()
                            }
                        }

                        // Mettre en lecture la source sonore
                        if (reaction.Key == reactions_courbes["play_sound"])
                        {
                            foreach (GameObject obj in reaction.Value)
                            {
                                // GetComponent<Sound>().Play()
                            }
                        }
                    }
                }
            }
        }
    }
}
