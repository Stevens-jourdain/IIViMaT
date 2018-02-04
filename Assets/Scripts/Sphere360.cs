using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere360 : MonoBehaviour {    
    public GameObject cam;
    public Video360 video360;
    public PlayCurve playcurves;

    private string currentAction, currentReaction;

    public int video_index = -1;
    public void Awake()
    {
        video_index = int.Parse(gameObject.name.Replace("Video360_player(Clone)_", ""));
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("enter : " + other.tag);

        if (video360.main.isPlayMode && other.tag == "MainCamera")
        {
            // S'il s'agit de la tete, on applique le système d'action réaction
            if (video_index != -1)
            {
                if (video360.main.actionReaction.action_reactions_spheres360.Count > 0)
                {
                    if (video360.main.actionReaction.action_reactions_spheres360.ContainsKey(video_index))
                    {
                        // en fonction de la position
                        if (video360.main.actionReaction.action_reactions_spheres360[video_index].ContainsKey(video360.main.actionReaction.actions_spheres360["enter"]))
                        {
                            // Parcourt des réactions possibles
                            foreach (KeyValuePair<int, List<GameObject>> reaction in video360.main.actionReaction.action_reactions_spheres360[video_index][video360.main.actionReaction.actions_spheres360["enter"]])
                            {
                                if (reaction.Key == video360.main.actionReaction.reactions_spheres360["activate"])
                                {
                                    foreach (GameObject obj in reaction.Value)
                                    {
                                        obj.SetActive(true);

                                        if (obj.name.Contains("Video360_player(Clone)_"))
                                        {
                                            int index = int.Parse(obj.name.Replace("Video360_player(Clone)_", ""));
                                            video360.PlayVideo(index);
                                        }
                                    }
                                }

                                if (reaction.Key == video360.main.actionReaction.reactions_spheres360["desactivate"])
                                {
                                    foreach (GameObject obj in reaction.Value)
                                        obj.SetActive(false);
                                }

                                if (reaction.Key == video360.main.actionReaction.reactions_spheres360["play_curve"])
                                {
                                    foreach (GameObject obj in reaction.Value)
                                    {
                                        int index = int.Parse(obj.name.Replace("Curve(Clone)_", ""));
                                        playcurves.Play(index);
                                    }
                                }

                                if (reaction.Key == video360.main.actionReaction.reactions_spheres360["tp"])
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
    }

    public void OnTriggerStay(Collider other)
    {
        if (!video360.main.isPlayMode && (other.tag == "RightPad" || other.tag == "LeftPad"))
        {
            // S'il s'agit du pad avec le trigger enfoncer
            if (video360.main.rightDevice != null && video360.main.leftDevice != null)
            {
                if (video360.main.rightDevice.GetPress(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger) || video360.main.leftDevice.GetPress(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger))
                {
                    ShowMenuActionReaction();
                }
            }

            if (Input.GetKeyUp(KeyCode.T))
            {
                ShowMenuVideo();
            }
        }
    }

    public void ShowMenuVideo()
    {
        string[] actionMenu = { "Lancer la vidéo", "Déplacer la vidéo", "Activer/Désactiver", "Action-Réaction"};
        Menu.Del handler = GestionMenuVideo;

        video360.main.menu.AddItems(actionMenu, handler);
    }

    public void GestionMenuVideo(string value)
    {
        if(value == "Lancer la vidéo")
        {
            video360.PlayVideo(video_index);
        }
        else if(value == "Déplacer la vidéo")
        {
            video360.DeplacerVideo(this.gameObject);
        }
        else if(value == "Activer/Désactiver")
        {
            video360.LancerMenuActivationVideo(video_index);
        }
        else if(value == "Action-Réaction")
        {
            ShowMenuActionReaction();
        }
    }

    public void ShowMenuActionReaction()
    {
        // On affiche le menu pour enregistrer une action réaction
        string[] actionMenu = { "Entrer dans la sphere", "Sortir de la sphere", "Fin de video" };
        Menu.Del handler = showReaction;

        video360.main.menu.AddItems(actionMenu, handler);
    }

    public void showReaction(string action)
    {
        if(action == "Entrer dans la sphere")
        {
            currentAction = "enter";
        }
        else if(action == "Sortir dans la sphere")
        {
            currentAction = "exit";
        }
        else
        {
            currentAction = "end_of_video";
        }

        // Affichage du menu de reaction
        // On affiche le menu pour enregistrer une action réaction
        string[] actionMenu = { "Activer un objet", "Desactiver un objet", "Changer de vidéo", "Lancer une courbe" };
        Menu.Del handler = addReaction;

        video360.main.menu.AddItems(actionMenu, handler);
    }

    public void addReaction(string reaction)
    {
        GameObject[] listeObjets = null;

        if (reaction == "Activer un objet")
        {
            currentReaction = "activate";
            listeObjets = GameObject.FindGameObjectsWithTag("Video360");
        }
        else if (reaction == "Desactiver un objet")
        {
            currentReaction = "desactivate";
            listeObjets = GameObject.FindGameObjectsWithTag("Video360");
        }
        else if (reaction == "Changer de vidéo")
        {
            currentReaction = "tp";
            listeObjets = GameObject.FindGameObjectsWithTag("Video360");            
        }
        else
        {
            currentReaction = "play_curve";
            listeObjets = GameObject.FindGameObjectsWithTag("Curve");
        }

        // Lister les objets
        Menu.Del handler = choisirObjet;
        video360.main.menu.AddItemsObj(listeObjets, handler);
    }

    public void choisirObjet(string objname)
    {
        // Enregistrer l'action-reaction
        GameObject obj = GameObject.Find(objname);

        video360.main.actionReaction.AddActionReactionsSphere360(video_index, video360.main.actionReaction.actions_spheres360[currentAction], video360.main.actionReaction.reactions_spheres360[currentReaction], obj);
    }

    public void OnTriggerExit(Collider other)
    {
        Debug.Log("Exit : " + other.tag);
        if (video360.main.isPlayMode && other.tag == "MainCamera")
        {
            // S'il s'agit de la tete, on applique le système d'action réaction
            if (video_index != -1)
            {
                if (video360.main.actionReaction.action_reactions_spheres360.Count > 0)
                {
                    Debug.Log("Il y a une interaction ");

                    if (video360.main.actionReaction.action_reactions_spheres360.ContainsKey(video_index))
                    {
                        Debug.Log("avec cette vidéo");
                        // en fonction de la position
                        if (video360.main.actionReaction.action_reactions_spheres360[video_index].ContainsKey(video360.main.actionReaction.actions_spheres360["exit"]))
                        {

                            Debug.Log("quand on en sort");

                            // Parcourt des réactions possibles
                            foreach (KeyValuePair<int, List<GameObject>> reaction in video360.main.actionReaction.action_reactions_spheres360[video_index][video360.main.actionReaction.actions_spheres360["exit"]])
                            {
                                if (reaction.Key == video360.main.actionReaction.reactions_spheres360["activate"])
                                {
                                    foreach (GameObject obj in reaction.Value)
                                    {
                                        obj.GetComponent<Renderer>().enabled = true;

                                        if (obj.name.Contains("Video360_player(Clone)_"))
                                        {
                                            int index = int.Parse(obj.name.Replace("Video360_player(Clone)_", ""));
                                            video360.PlayVideo(index);
                                        }
                                    }
                                }

                                if (reaction.Key == video360.main.actionReaction.reactions_spheres360["desactivate"])
                                {
                                    foreach (GameObject obj in reaction.Value)
                                        obj.GetComponent<Renderer>().enabled = false;
                                }

                                if (reaction.Key == video360.main.actionReaction.reactions_spheres360["play_curve"])
                                {
                                    foreach (GameObject obj in reaction.Value)
                                    {
                                        int index = int.Parse(obj.name.Replace("Curve(Clone)_", ""));
                                        playcurves.Play(index);
                                    }
                                }

                                if (reaction.Key == video360.main.actionReaction.reactions_spheres360["tp"])
                                {
                                    Debug.Log("Il s'agit d'une lecture ");
                                    foreach (GameObject obj in reaction.Value)
                                    {
                                        Debug.Log("TP à l'emplacement : " + obj.transform.position);
                                        cam.transform.position = obj.transform.position;
                                        int index_autre_video = int.Parse(obj.name.Replace("Video360_player(Clone)_", ""));
                                        video360.PlayVideo(index_autre_video);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
