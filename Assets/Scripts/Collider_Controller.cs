using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Collider_Controller : MonoBehaviour
{
    // Collider Controller
    // Listens for player enter/exit events and calls
    // corr. functions in Dungeon_Controller and Area_Controller.

    public enum Collider_Type
    {
        Room,
        Area
    }

    [Header("Collider Controller Misc.")]
    public int roomIndex;
    public int dungeonIndex;
    public int areaIndex;
    public Collider_Type colliderType;

    private readonly bool _debug = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if(colliderType == Collider_Type.Room)
            {
                if(_debug) Debug.Log($"Player Entered Room {roomIndex + 1}");
                Dungeon_Controller.instance.EnterRoom(dungeonIndex, roomIndex);
            }
            else if(colliderType == Collider_Type.Area)
            {
                if(_debug) Debug.Log($"Player Entered Area {areaIndex + 1}");
                Area_Controller.instance.EnterArea(areaIndex);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if (colliderType == Collider_Type.Room)
            {
                if(_debug) Debug.Log($"Player Exited Room {roomIndex + 1}");
                Dungeon_Controller.instance.ExitRoom(dungeonIndex, roomIndex);
            }
            else if (colliderType == Collider_Type.Area)
            {
                if(!Player_Controller.instance.canInput) return;
                if(_debug) Debug.Log($"Player Exited Area {areaIndex + 1}");
                Area_Controller.instance.ExitArea(areaIndex);
            }
        }
    }
}
