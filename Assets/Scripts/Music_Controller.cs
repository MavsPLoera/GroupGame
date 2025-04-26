using UnityEngine;

public class Music_Controller : MonoBehaviour
{
    public AudioSource musicAudioSource;
    public AudioClip ruinedTownMusic;
    public AudioClip tavernInteriorFloorMusic;
    public AudioClip winterForestMusic;
    public AudioClip dungeon1Music;

    public static Music_Controller instance;

    void Start()
    {
        if (instance == null)
            instance = this;
    }

    public void playMusic()
    {

    }
}
