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
    public float genericTransitionTime = 1f;
    public float inDungeonTransitionTime = 1f;
    public AudioClip warpSound;
    public AudioSource audioSource;

    public enum destinationFacingDirection
    {
        Up, Down, Left, Right
    }

    public enum destinationMusic
    {
        DungeonSewers, DungeonCave, DungeonCrypt, Tavern, WinterForest, RuinedTown, Cemetary
    }

    public enum warpTypes
    {
        ToDungeon, ToOverworld, InDungeon
    }

    public destinationMusic music;
    public destinationFacingDirection direction;
    public warpTypes warpType;

    void Start()
    {

    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        //Only want the player to teleport so the collider will only warp if the player collides with the warp.
        if (collision.gameObject.CompareTag("Player"))
        {
            if (warpType != warpTypes.InDungeon)
            {
                StartCoroutine(genericWarp());
            }
            else
            {
                // Separate functions in case we
                // want to differentiate these further.
                StartCoroutine(inDungeonWarp());
            }
        }
    }

    private IEnumerator genericWarp()
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

        // Prevent area colliders from trigger their OnExit() events
        // during warp. Solves issue with currentLocation not displaying.
        Player_Controller.instance.canInput = false;
        Player_Controller.instance.isTransitioning = true;
        Player_Controller.instance.playerAnimator.Play("Player_Walk", 0);
        crossFadeIn.SetActive(true);
        audioSource.PlayOneShot(warpSound);

        yield return new WaitForSeconds(genericTransitionTime);

        Music_Controller.instance.warpChangeMusic(music);
        Player_Controller.instance.changeFacingDirectionWarp(direction);
        Player_Controller.instance.playerAnimator.Play("Player_Idle", 0);

        //Cross fade in complete, no cross fade out to new destination.
        crossFadeIn.SetActive(false);
        crossFadeOut.SetActive(true);
        player.gameObject.transform.position = warpDestination.position;
        playerCamera.transform.position = new Vector3(warpDestination.position.x, warpDestination.position.y, -100f);

        if (warpType == warpTypes.ToDungeon)
        {
            Dungeon_Controller.instance.inDungeon = true;
            Camera_Controller.instance.inDungeon = true;
        }
        else if (warpType == warpTypes.ToOverworld)
        {
            Dungeon_Controller.instance.inDungeon = false;
            Camera_Controller.instance.inDungeon = false;
        }

        yield return new WaitForSeconds(genericTransitionTime);

        crossFadeOut.SetActive(false);
        Player_Controller.instance.isTransitioning = false;
        Player_Controller.instance.canInput = true;
    }

    private IEnumerator inDungeonWarp()
    {
        Player_Controller.instance.canInput = false;
        // Player_Controller.instance.playerAnimator.Play("Player_Walk", 0);
        crossFadeIn.SetActive(true);

        yield return new WaitForSeconds(inDungeonTransitionTime);

        // Cross fade in complete, no cross fade out to new destination.
        crossFadeIn.SetActive(false);
        crossFadeOut.SetActive(true);
        player.gameObject.transform.position = warpDestination.position;
        playerCamera.transform.position = new Vector3(warpDestination.position.x, warpDestination.position.y, -100f);

        yield return new WaitForSeconds(inDungeonTransitionTime);

        crossFadeOut.SetActive(false);
        Player_Controller.instance.canInput = true;
    }
}
