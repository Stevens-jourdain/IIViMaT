using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEditor;
using Valve.VR;

public class Video360 : MonoBehaviour
{

    public bool isSequentiel = true;
    public GameObject cam;

    public GameObject prefab_player;
    public List<GameObject> videos = null;

    private int indexVideoCourante = 0;

    public Main main;

    // Pour déplacer la vidéo
    public MoveObject mo = null;

    // Index pour activer/desactiver des videos
    private int indexActive = 0;

    // Use this for initialization
    public void Start()
    {
        videos = new List<GameObject>();

        mo.main = main;
    }
    public void Reset()
    {
        indexVideoCourante = 0;
        this.cam.transform.position = videos[0].transform.position;

        // Mettre toutes les vidéos en stop
        foreach (GameObject v in videos)
        {
            v.GetComponent<VideoPlayer>().Stop();
            v.GetComponent<AudioSource>().Stop();
        }

        PlayVideo();
    }

    // Update is called once per frame
    void Update()
    {
        if(mo != null && mo.isVideo)
            mo.MoveUpdate();

        // S'il n'y a pas de vidéo, ou s'il y a un menu, ou s'il y a une main mise sur l'interaction, pas d'interaction ici
        if (videos.Count == 0 || main.menu.nbItems > 0 || main.actionEnCours)
            return;

        // Ouverture de video au clavier
        if (Input.GetKeyUp(KeyCode.O))        
            PauseVideo();        

        // Lecture des videos
        if (Input.GetKeyUp(KeyCode.P))        
            PlayVideo();              

        if (Input.GetKeyUp(KeyCode.N))
        {
            if (indexVideoCourante >= videos.Count)
                indexVideoCourante = 0;

            NextVideo(videos[indexVideoCourante].GetComponent<VideoPlayer>());

            int nb = videos.Count;
            for (int i = 0; i < nb; ++i)
            {
                if (i != indexVideoCourante)
                    videos[i].SetActive(false);
            }

            videos[indexVideoCourante].SetActive(true);
            videos[indexVideoCourante].GetComponent<VideoPlayer>().Play();
        }

        // Sans VR on va pas plus loin dans cette fonction
        if ((main.leftDevice == null) || (main.rightDevice == null))        
            return;        

        // Changement de video en VR
        if (main.rightDevice.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
        {
            float x = main.rightDevice.GetAxis(EVRButtonId.k_EButton_SteamVR_Touchpad).x;

            if (x > 0.2f)
                ++indexVideoCourante;
            else if (x < -0.2f)
                --indexVideoCourante;
            else
                return;

            if (indexVideoCourante >= videos.Count)
                indexVideoCourante = 0;

            NextVideo(videos[indexVideoCourante].GetComponent<VideoPlayer>());

            int nb = videos.Count;
            for (int i = 0; i < nb; ++i)
            {
                if (i != indexVideoCourante)
                    videos[i].SetActive(false);
            }

            videos[indexVideoCourante].SetActive(true);
            videos[indexVideoCourante].GetComponent<VideoPlayer>().Play();
        }
    }

    public void NextVideo(VideoPlayer source)
    {
        ++indexVideoCourante;

        // Fin du film
        if (indexVideoCourante >= videos.Count)
            indexVideoCourante = 0;
        else
        {
            // Teleportation vers la sphère
            cam.transform.position = videos[indexVideoCourante].transform.position;

            // Lecture de la vidéo
            PlayVideo();
        }
    }

    public void AddVideo(string path)
    {
        GameObject video = AddToScene(path);
        DeplacerVideo(video);
    }

    public GameObject AddToScene(string path)
    {
        GameObject video = Instantiate<GameObject>(prefab_player);
        video.name += ("_" + videos.Count);
        video.SetActive(true);

        var videoPlayer = video.GetComponent<VideoPlayer>();
        videoPlayer.playOnAwake = true;
        videoPlayer.url = main.config.path_to_import + "/Video360/" + path;
        videoPlayer.isLooping = true;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;

        var audioPlayer = video.GetComponent<AudioSource>();
        audioPlayer.playOnAwake = false;

        videoPlayer.EnableAudioTrack(0, true);
        videoPlayer.SetTargetAudioSource(0, audioPlayer);

        // Add to list
        videos.Add(video);

        if (isSequentiel)
        {
            // A la fin de la lecture on passe à la suivante
            VideoPlayer.EventHandler hand = NextVideo;
            videoPlayer.loopPointReached += hand;
        }

        return video;
    }

    public void DeplacerVideo(GameObject video)
    {
        MoveObject.Handler hand = setPlay;
        mo.fn = hand;

        GameObject[] arr = new GameObject[1];
        arr[0] = video;

        mo.isVideo = true;

        mo.SetObjects(arr);
        main.ShowMessage("Mise en place de la vidéo dans l'espace.");
        mo.StartMove();
    }

    public void setPlay(GameObject videoObj)
    {
        VideoPlayer player = videoObj.GetComponent<VideoPlayer>();
        player.playOnAwake = false;
        player.isLooping = !isSequentiel;
        player.Pause();
    }

    public void LancerMenuActivationVideo(int indexActive)
    {
        this.indexActive = indexActive;

        // Activer ou non la vidéo par defaut ?
        string[] ActivateVideoMenu = { "Activer la vidéo", "Désactiver la vidéo" };
        Menu.Del handler = ActiveVideo;

        main.menu.AddItems(ActivateVideoMenu, handler);
    }
    

    public void ActiveVideo(string value)
    {
        MeshRenderer renderer = videos[indexActive].GetComponent<MeshRenderer>();

        if (value == "Désactiver la vidéo")
            renderer.enabled = false;
        else
            renderer.enabled = true;
    }

    public void AddVideoAt(string path, Vector3 pos, Vector3 rot, Vector3 scale, bool isActive)
    {
        this.AddToScene(path);
        videos[videos.Count - 1].transform.localScale = scale;

        videos[videos.Count - 1].transform.Rotate(Vector3.right, rot.x);
        videos[videos.Count - 1].transform.Rotate(Vector3.up, rot.y);
        videos[videos.Count - 1].transform.Rotate(Vector3.forward, rot.y);

        videos[videos.Count - 1].transform.position = pos;

        MeshRenderer renderer = videos[videos.Count - 1].GetComponent<MeshRenderer>();
        renderer.enabled = isActive;

        PauseVideo(videos.Count - 1);
    }

    public void PlayVideo()
    {
        if (isSequentiel)
        {
            videos[indexVideoCourante].GetComponent<VideoPlayer>().Play();
            videos[indexVideoCourante].GetComponent<AudioSource>().Play();
        }
        else
        {
            foreach (GameObject v in videos)
            {
                v.GetComponent<VideoPlayer>().Play();
                v.GetComponent<AudioSource>().Play();
            }
        }
    }

    public void PlayVideo(int index)
    {
        videos[index].GetComponent<VideoPlayer>().Play();
        videos[index].GetComponent<AudioSource>().Play();
    }

    public void PauseVideo()
    {
        if (isSequentiel)
        {
            videos[indexVideoCourante].GetComponent<VideoPlayer>().Pause();
            videos[indexVideoCourante].GetComponent<AudioSource>().Pause();
        }
        else
        {
            foreach (GameObject v in videos)
            {
                v.GetComponent<VideoPlayer>().Pause();
                v.GetComponent<AudioSource>().Pause();
            }
        }
    }

    public void PauseVideo(int index)
    {        
       videos[index].GetComponent<VideoPlayer>().Pause();
       videos[index].GetComponent<AudioSource>().Pause();       
    }
}
