using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEditor;

public class Video360 : MonoBehaviour
{

    public bool isSequentiel = true;
    public GameObject cam;

    public GameObject prefab_player;
    private List<GameObject> videos;

    private int indexVideoCourante = 0;

    public Main main;
    
    // Use this for initialization
    void Start()
    {
        videos = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        // Ouverture de video au clavier
        if (Input.GetKeyUp(KeyCode.O))
        {
            PauseVideo();
            //string path = EditorUtility.OpenFilePanel("Import a virtual scene", "", "mp4");
            //AddVideo(path);
        }

        // Lecture des videos
        if (Input.GetKeyUp(KeyCode.P))
        {
            PlayVideo();
        }        

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
        {
            return;
        }

        // Ouverture models en vr
        if (main.leftDevice.GetPress(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger))
        {
            Debug.Log("Trigger gauche");

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

            /*string path = EditorUtility.OpenFilePanel("Import a virtual scene", "", "obj");
            if (path.Length != 0)
            {
                var fileContent = File.ReadAllBytes(path);
            }*/
        }

        // Ouverture 360 en vr
        if (main.rightDevice.GetPress(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger))
        {
            /* Debug.Log("Trigger droite");
             string path = EditorUtility.OpenFilePanel("Import a virtual scene", "", "mp4");
             AddVideo(path);*/
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
        GameObject video = Instantiate<GameObject>(prefab_player);

        var videoPlayer = video.GetComponent<VideoPlayer>();
        videoPlayer.playOnAwake = false;
        videoPlayer.url = main.config.path_to_import + "/Video360/" + path;
        videoPlayer.isLooping = !isSequentiel;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;

        var audioPlayer = video.GetComponent<AudioSource>();
        //audioPlayer.playOnAwake = false;

        videoPlayer.EnableAudioTrack(0, true);
        videoPlayer.SetTargetAudioSource(0, audioPlayer);

        // Add to list
        videos.Add(video);

        if (isSequentiel)
        {
            if (videos.Count == 1)
            {
                videoPlayer.Play();
                audioPlayer.Play();
                video.transform.position = cam.transform.position;
            }

            // A la fin de la lecture on passe à la suivante
            VideoPlayer.EventHandler hand = NextVideo;
            videoPlayer.loopPointReached += hand;
        }
    }

    public void AddVideoAt(string path, Vecteur3_IIViMaT pos, Vecteur3_IIViMaT rot, Vecteur3_IIViMaT scale)
    {
        this.AddVideo(path);
        videos[videos.Count - 1].transform.localScale = new Vector3(scale.x, scale.y, scale.z);

        videos[videos.Count - 1].transform.Rotate(Vector3.right, rot.x);
        videos[videos.Count - 1].transform.Rotate(Vector3.up, rot.y);
        videos[videos.Count - 1].transform.Rotate(Vector3.forward, rot.y);

        videos[videos.Count - 1].transform.position = new Vector3(pos.x, pos.y, pos.z);
    }

    public void PlayVideo()
    {
        if (isSequentiel)
        {
            videos[indexVideoCourante].GetComponent<VideoPlayer>().Play();
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

    public void PauseVideo()
    {
        if (isSequentiel)
        {
            videos[indexVideoCourante].GetComponent<VideoPlayer>().Pause();
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
}
