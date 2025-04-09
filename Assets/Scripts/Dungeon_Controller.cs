using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class Dungeon_Controller: MonoBehaviour
{
    // Dungeon Controller
    // Manages enemies, lighting, camera movement, etc.
    // for each dungeon.

    [System.Serializable]
    public class Dungeon
    {
        public List<DungeonRoom> rooms;
        public bool isCleared = false;
    }

    [System.Serializable]
    public class DungeonRoom
    {
        public GameObject light;
        public GameObject roomCollider;
        public List<GameObject> enemies;
        public List<GameObject> teleports;
        public List<GameObject> doors;
        public bool isCleared = false;
    }

    [Header("Dungeon Controller Misc.")]
    public List<Dungeon> dungeons;
    public int currentDungeonIndex;
    public int currentRoomIndex;
    public DungeonRoom currentRoom;
    public bool inDungeon = true;
    
    public static Dungeon_Controller instance;

    private readonly bool _debug = false;

    private void Awake()
    {
        if(!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if(currentRoom != null && !currentRoom.isCleared)
        {
            // Continually check enemies. If all dead, set isCleared and open doors.
            List<GameObject> currentEnemies = currentRoom.enemies.Where(enemy => enemy != null).ToList();
            if(currentEnemies.Count == 0)
            {
                if(_debug) Debug.Log($"Cleared");
                currentRoom.isCleared = true;
                currentRoom.doors.ForEach(door => door.SetActive(false));
            }
        }
    }

    public void EnterRoom(int dungeonIndex, int roomIndex, bool lerpCamera)
    {
        // Get current room and turn on light.
        currentRoom = dungeons[dungeonIndex].rooms[roomIndex];
        currentRoom.light.SetActive(true);

        // Change camera position.
        Vector3 newCameraPosition = currentRoom.roomCollider.GetComponent<BoxCollider2D>().bounds.center;
        newCameraPosition.z = -100;
        if(lerpCamera)
        {
            // TODO
            Camera_Controller.instance.UpdatePosition(newCameraPosition);
        }
        else
        {
            // TODO
            Camera_Controller.instance.UpdatePosition(newCameraPosition);
        }

        if(!currentRoom.isCleared)
        {
            if(_debug) Debug.Log($"Not Cleared");
            // Close doors and set isActive for all enemies in current room.
            currentRoom.doors.ForEach(door => door.SetActive(true));
            currentRoom.enemies.ForEach(enemy => enemy.SetActive(true));
        }
        else
        {
            if(_debug) Debug.Log($"Cleared");
            currentRoom.doors.ForEach(door => door.SetActive(false));
        }
    }

    public void ExitRoom(int dungeonIndex, int roomIndex)
    {
        // Get current room and turn off light.
        DungeonRoom currentRoom = dungeons[dungeonIndex].rooms[roomIndex];
        currentRoom.light.SetActive(false);
    }
}
