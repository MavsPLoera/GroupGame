using System.Collections;
using UnityEngine;

public class Warp_Controller : MonoBehaviour
{
    public Transform warpDestination;
    public GameObject dungeonManager;
    public GameObject playerCamera;
    public GameObject player;
    public GameObject crossFade;
    public float transitionTime = 1f;
    public Animator crossfadeAnimator;
    public AudioClip enterOverworldSound;
    public AudioClip enterDungeonSound;
    private Camera_Controller cameraController;
    private Dungeon_Controller dungeonController;

    void Start()
    {
        dungeonController = dungeonManager.GetComponent<Dungeon_Controller>();
        cameraController = playerCamera.GetComponent<Camera_Controller>();
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
        crossFade.SetActive(true);
        crossfadeAnimator.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        crossFade.SetActive(false);
        player.gameObject.transform.position = warpDestination.position;
        playerCamera.transform.position = new Vector3(warpDestination.position.x, warpDestination.position.y, -100f);

        if (gameObject.CompareTag("DungeonWarp"))
        {
            dungeonController.inDungeon = true;
            cameraController.inDungeon = true;
        }
        else if (gameObject.CompareTag("OverworldWarp"))
        {
            dungeonController.inDungeon = false;
            cameraController.inDungeon = false;
        }
   
        Player_Controller.instance.canInput = true;
    }
}
