using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Music_Controller : MonoBehaviour
{
    public AudioSource musicAudioSource;
    public AudioClip ruinedTownMusic;
    public AudioClip tavernInteriorFloorMusic;
    public AudioClip winterForestMusic;
    public AudioClip dungeonSewersMusic;
    public AudioClip dungeonCaveMusic;
    public AudioClip dungeonCryptMusic;
    public AudioClip cemetaryMusic;
    public AudioClip gameOverMusic;
    public AudioClip gameWinMusic;
    public Dictionary<AudioClip, float> clipTimes = new();
    public float transitionTime = .5f;
    private float volume;
    public static Music_Controller instance;

    void Start()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        clipTimes[ruinedTownMusic] = 0f;
        clipTimes[tavernInteriorFloorMusic] = 0f;
        clipTimes[winterForestMusic] = 0f;
        clipTimes[dungeonSewersMusic] = 0f;
        clipTimes[dungeonCaveMusic] = 0f;
        clipTimes[dungeonCryptMusic] = 0f;
        clipTimes[cemetaryMusic] = 0f;
        clipTimes[gameOverMusic] = 0f;
        clipTimes[gameWinMusic] = 0f;

        musicAudioSource.clip = ruinedTownMusic;
        volume = musicAudioSource.volume;
        musicAudioSource.Play();
    }

    //Used for swapping songs when warping
    public void warpChangeMusic(Warp_Controller.destinationMusic areaMusic)
    {
        AudioClip temp = null;

        switch(areaMusic)
        {
            case Warp_Controller.destinationMusic.DungeonSewers:
                temp = dungeonSewersMusic;
                break;
            case Warp_Controller.destinationMusic.DungeonCave:
                temp = dungeonCaveMusic;
                break;
            case Warp_Controller.destinationMusic.DungeonCrypt:
                temp = dungeonCryptMusic;
                break;
            case Warp_Controller.destinationMusic.RuinedTown:
                temp = ruinedTownMusic;
                break;
            case Warp_Controller.destinationMusic.WinterForest:
                temp = winterForestMusic;
                break;
            case Warp_Controller.destinationMusic.Tavern:
                temp = tavernInteriorFloorMusic;
                break;
            case Warp_Controller.destinationMusic.Cemetary:
                temp = cemetaryMusic;
                break;
        }

        if(musicAudioSource.clip != temp)
            StartCoroutine(interludeMusic(temp));
    }

    public IEnumerator interludeMusic(AudioClip song)
    {
        float time = 0f;

        //Lerp music to not be heard any more
        while (musicAudioSource.volume > 0f)
        {
            musicAudioSource.volume = Mathf.Lerp(volume, 0f, time);
            time += Time.deltaTime / transitionTime;
            yield return null;
        }
        time = 0f;

        /*
         * Get time of the current song
         * Store in clipTimes
         * Swap it
         * play it from where the old song left off
         */
        // audioTime = musicAudioSource.time;
        clipTimes[musicAudioSource.clip] = musicAudioSource.time;
        musicAudioSource.Pause();
        musicAudioSource.clip = song;
        float audioTime = clipTimes[musicAudioSource.clip];
        musicAudioSource.time = Mathf.Min(audioTime, song.length - .01f);
        musicAudioSource.Play();

        //Lerp music volume back up
        while (musicAudioSource.volume < volume)
        {
            musicAudioSource.volume = Mathf.Lerp(0, volume, time);
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
        musicAudioSource.UnPause();
    }

}
