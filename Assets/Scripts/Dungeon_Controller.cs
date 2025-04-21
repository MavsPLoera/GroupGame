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
        public List<GameObject> lights;
        public GameObject roomCollider;
        public List<GameObject> enemies;
        public List<GameObject> doors;
        public bool isCleared = false;
        public Vector2[] enemyStartPositions;
    }

    [Header("Dungeon Controller Misc.")]
    public List<Dungeon> dungeons;
    public int currentDungeonIndex;
    public int currentRoomIndex;
    public DungeonRoom currentRoom;
    public bool inDungeon = true;
    public bool isTransitioning = false;

    public static Dungeon_Controller instance;
    private readonly bool _debug = true;

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
        if(currentRoom != null && !currentRoom.isCleared && !isTransitioning)
        {
            // Continually check enemies. If all dead, set isCleared and open doors.
            List<GameObject> currentEnemies = currentRoom.enemies.Where(enemy => enemy.activeSelf != false).ToList();
            if(currentEnemies.Count == 0)
            {
                if(_debug) Debug.Log($"{currentRoomIndex + 1} Cleared (Update)");
                currentRoom.isCleared = true;
                currentRoom.doors.ForEach(door => door.SetActive(false));
            }
        }
    }

    public void EnterRoom(int dungeonIndex, int roomIndex)
    {
        // Get current room and turn on light.
        currentRoom = dungeons[dungeonIndex].rooms[roomIndex];
        currentRoom.lights.ForEach(light => light.SetActive(true));
        currentDungeonIndex = dungeonIndex;
        currentRoomIndex = roomIndex;
        if(currentRoom.enemyStartPositions == null || currentRoom.enemyStartPositions.Length == 0)
        {
            currentRoom.enemyStartPositions = new Vector2[currentRoom.enemies.Count];
        }

        // Change camera position.
        Vector3 newCameraPosition = currentRoom.roomCollider.GetComponent<BoxCollider2D>().bounds.center;
        newCameraPosition.z = -100;
        StartCoroutine(Camera_Controller.instance.UpdatePosition(newCameraPosition, () =>
        {
            if(!currentRoom.isCleared)
            {
                if(_debug) Debug.Log($"{currentRoomIndex + 1} Not Cleared (Enter Room)");
                // Close doors and set isActive for all enemies in current room.
                currentRoom.doors.ForEach(door => door.SetActive(true));
                currentRoom.enemies.ForEach(enemy => enemy.SetActive(true));
            }
            else
            {
                if(_debug) Debug.Log($"{currentRoomIndex + 1} Cleared (Enter Room)");
                currentRoom.doors.ForEach(door => door.SetActive(false));
            }
        }));
    }

    public void ExitRoom(int dungeonIndex, int roomIndex)
    {
        // Get current room and turn off light.
        DungeonRoom currentRoom = dungeons[dungeonIndex].rooms[roomIndex];
        currentRoom.lights.ForEach(light => light.SetActive(false));
    }

    public void ResetRoom()
    {
        // Reset current room state on death.
        currentRoom.enemies.ForEach(enemy => enemy.SetActive(false));
        currentRoom.doors.ForEach(door => door.SetActive(false));
        currentRoom.isCleared = false;
        currentRoom = null;
        currentDungeonIndex = -1;
        currentRoomIndex = -1;
    }
}
