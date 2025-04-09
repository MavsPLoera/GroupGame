using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Room_Collider_Controller : MonoBehaviour
{
    // Room Collider Controller
    // Listens for player enter/exit events
    // and calls corr. functions in Dungeon_Controller.

    [Header("Room Collider Controller Misc.")]
    public bool lerpCamera;
    public int roomIndex;
    public int dungeonIndex;

    private bool _debug = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if(_debug) Debug.Log($"Player Entered Room {roomIndex + 1}");
            Dungeon_Controller.instance.EnterRoom(dungeonIndex, roomIndex, lerpCamera);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if(_debug) Debug.Log($"Player Exited Room {roomIndex + 1}");
            Dungeon_Controller.instance.ExitRoom(dungeonIndex, roomIndex);
        }
    }
}
