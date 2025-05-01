using UnityEngine;

public class Button_Controller : MonoBehaviour
{
    public AudioClip audioClip;
    public AudioSource audioSource;

    public void PlayAudio()
    {
        audioSource.PlayOneShot(audioClip);
    }
}
