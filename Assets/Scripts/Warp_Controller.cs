using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class Warp_Controller : MonoBehaviour
{
    public Transform warpDestination;
    public GameObject dungeonManager;
    public GameObject playerCamera;
    public GameObject player;
    public GameObject crossFadeIn;
    public GameObject crossFadeOut;
    public float transitionTime = 1f;
    public AudioClip warpSound;
    public AudioSource audioSource;
    public string direction = "down";
    public string destinationMusic;

    void Start()
    {
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        //Only want the player to teleport so the collider will only warp if the player collides with the warp.
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(warp());
        }
    }

    private IEnumerator warp()
    {
        /*
         * Prevent the player from inputing, this will allow the walk animator to player to give a sense of "walking" to the next area
         * 
         * set the Crossfade game object as active and trigger Start animation to fade to black
         * 
         * wait for trasition time and then warp player to the warp destination
         * 
         * last if statement is to check to see if the gameobject tag is warping player to overworld or dungeon
         * 
         * if the player is going to a dungeon we need to seet the bools to true for proper camera control change.
         */

        Player_Controller.instance.canInput = false;
        Player_Controller.instance.playerAnimator.Play("Player_Walk", 0);
        crossFadeIn.SetActive(true);
        audioSource.PlayOneShot(warpSound);

        yield return new WaitForSeconds(transitionTime);

        Area_Controller.instance.currentLocationText.text = "";
        Music_Controller.instance.warpChangeMusic(destinationMusic);
        Player_Controller.instance.changeFacingDirectionWarp(direction);
        Player_Controller.instance.playerAnimator.Play("Player_Idle", 0);

        //Cross fade in complete, no cross fade out to new destination.
        crossFadeIn.SetActive(false);
        crossFadeOut.SetActive(true);
        player.gameObject.transform.position = warpDestination.position;
        playerCamera.transform.position = new Vector3(warpDestination.position.x, warpDestination.position.y, -100f);

        if (gameObject.CompareTag("DungeonWarp"))
        {
            //Insert Audio Clip here
            Dungeon_Controller.instance.inDungeon = true;
            Camera_Controller.instance.inDungeon = true;
        }
        else if (gameObject.CompareTag("OverworldWarp"))
        {
            //Insert Audio Clip here
            Dungeon_Controller.instance.inDungeon = false;
            Camera_Controller.instance.inDungeon = false;
        }
   
        yield return new WaitForSeconds(transitionTime);

        crossFadeOut.SetActive(false);
        Player_Controller.instance.canInput = true;
    }
}
