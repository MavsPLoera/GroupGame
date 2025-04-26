using System.Collections;
using UnityEngine;

public class Music_Controller : MonoBehaviour
{
    public AudioSource musicAudioSource;
    public AudioClip ruinedTownMusic;
    public AudioClip tavernInteriorFloorMusic;
    public AudioClip winterForestMusic;
    public AudioClip dungeon1Music;
    public float transitionTime = .5f;
    public static Music_Controller instance;

    void Start()
    {
        if (instance == null)
            instance = this;

        musicAudioSource.clip = ruinedTownMusic;
        musicAudioSource.Play();
    }

    //Used for swapping songs when warping
    public void warpChangeMusic(string areaMusic)
    {
        AudioClip temp = null;

        if (areaMusic.ToLower().Equals("dungeon1"))
        {
            temp = dungeon1Music;
        }
        else if(areaMusic.ToLower().Equals("ruinedtown"))
        {
            temp = ruinedTownMusic;
        }
        else if(areaMusic.ToLower().Equals("winterforest"))
        {
            temp = winterForestMusic;
        }
        else if(areaMusic.ToLower().Equals("tavern"))
        {
            temp = tavernInteriorFloorMusic;
        }

        StartCoroutine(interludeMusic(temp));
    }

    public IEnumerator interludeMusic(AudioClip song)
    {
        float temp = musicAudioSource.volume;
        float time = 0f;

        while (musicAudioSource.volume > 0f)
        {
            musicAudioSource.volume = Mathf.Lerp(temp, 0f, time);
            time += Time.deltaTime / transitionTime;
            yield return null;
        }

        time = 0f;
        musicAudioSource.clip = song;
        musicAudioSource.Play();

        while (musicAudioSource.volume < temp)
        {
            musicAudioSource.volume = Mathf.Lerp(0, temp, time);
            time += Time.deltaTime / transitionTime;
            yield return null;
        }
    }


    /*
     * For future use when pause menu is implemented. Call these methods.
     */

    public void pauseMusic()
    {
        musicAudioSource.Pause();
    }

    public void resumeMusic()
    {
        musicAudioSource.Play();
    }

}
