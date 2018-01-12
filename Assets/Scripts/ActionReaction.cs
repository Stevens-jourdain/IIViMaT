using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionReaction : MonoBehaviour {

    public Dictionary<int, Dictionary<int, Dictionary<int, List<GameObject>>>> action_reactions;

    public Dictionary<string, int> actions, reactions;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /**
     * @brief Ajoute une réaction à une action à partir d'un point
     * 
     * @param[in]   pointCourbe         le point où se définit l'action
     * @param[in]   action              l'action à définir
     * @param[in]   reaction            la réaction en conséquence
     */ 
    public void AddActionReactions(int pointCourbe, int action, int reaction, List<GameObject> obj_list)
    {
        if (action_reactions[pointCourbe] == null)
            action_reactions[pointCourbe] = new Dictionary<int, Dictionary<int, List<GameObject>>>();

        if (action_reactions[pointCourbe][action] == null)
            action_reactions[pointCourbe][action] = new Dictionary<int, List<GameObject>>();

        action_reactions[pointCourbe][action].Add(reaction, obj_list);
    }

    // Constructeur
    public ActionReaction()
    {
        // Instanciate
        action_reactions = new Dictionary<int, Dictionary<int, Dictionary<int, List<GameObject>>>>();
        actions = new Dictionary<string, int>();
        reactions = new Dictionary<string, int>();

        // Fill list of actions
        int i = 0;
        actions.Add("position", i);
        actions.Add("button", ++i);
        actions.Add("menu", ++i);

        // Fill list of reaction
        i = 0;
        reactions.Add("show", i);
        reactions.Add("hide", ++i);
        reactions.Add("play_sequence", ++i);
        reactions.Add("play_curve", ++i);
        reactions.Add("pause_curve", ++i);
        reactions.Add("play_360", ++i);
        reactions.Add("pause_360", ++i);
        reactions.Add("play_sound", ++i);
    }

    public void ProcessActionReaction(int position)
    {
        if(action_reactions[position] != null)
        {
            // en fonction de la position
            if(action_reactions[position][actions["position"]] != null)
            {
                // Parcourt des réactions possibles
                foreach(KeyValuePair<int, List<GameObject>> reaction in action_reactions[position][actions["position"]])
                {
                    // Afficher les objets
                    if(reaction.Key == reactions["show"])
                    {
                        foreach (GameObject obj in reaction.Value)
                            obj.SetActive(true);
                    }

                    // Cacher des objets
                    if (reaction.Key == reactions["hide"])
                    {
                        foreach (GameObject obj in reaction.Value)
                            obj.SetActive(false);
                    }

                    // Jouer une sequence
                    if (reaction.Key == reactions["play_sequence"])
                    {
                        foreach (GameObject obj in reaction.Value)
                        {
                            // GetComponent<Sequence>().Play()
                        }
                    }
                    
                    // Mettre en lecture la trajectoire
                    if (reaction.Key == reactions["play_curve"])
                    {
                        foreach (GameObject obj in reaction.Value)
                        {

                        }
                    }

                    // Mettre en pause la trajectoire
                    if (reaction.Key == reactions["pause_curve"])
                    {
                        foreach (GameObject obj in reaction.Value)
                        {

                        }
                    }

                    // Mettre en lecture la video 360
                    if (reaction.Key == reactions["play_360"])
                    {
                        foreach (GameObject obj in reaction.Value)
                        {
                            // GetComponent<360>().Play()
                        }
                    }

                    // Mettre en pause la video 360
                    if (reaction.Key == reactions["pause_360"])
                    {
                        foreach (GameObject obj in reaction.Value)
                        {
                            // GetComponent<360>().Pause()
                        }
                    }

                    // Mettre en lecture la source sonore
                    if (reaction.Key == reactions["play_sound"])
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
