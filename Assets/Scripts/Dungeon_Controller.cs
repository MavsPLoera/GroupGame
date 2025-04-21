using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class Dungeon_Controller: MonoBehaviour
{
    // Dungeon Controller
    // Manages enemies, lighting, camera movement, etc.
    // for each dungeon.

    // (?) might merge this with Area_Controller.

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
    }

    [Header("Dungeon Controller Misc.")]
    public List<Dungeon> dungeons;
    public DungeonRoom currentRoom;
    public bool inDungeon = true;
    public bool isTransitioning = false;

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
        if(currentRoom != null && !currentRoom.isCleared && !isTransitioning)
        {
            // Continually check enemies. If all dead, set isCleared and open doors.
            List<GameObject> currentEnemies = currentRoom.enemies.Where(enemy => enemy.activeSelf != false).ToList();
            if(currentEnemies.Count == 0)
            {
                if(_debug) Debug.Log($"Room Cleared (Update)");
                currentRoom.isCleared = true;
                currentRoom.doors.ForEach(door => door.SetActive(false));
            }
        }
    }

    public void EnterRoom(int dungeonIndex, int roomIndex)
    {
        if (dungeonIndex < 0 || dungeonIndex >= dungeons.Count) return;
        Dungeon currentDungeon = dungeons[dungeonIndex];
        if (roomIndex < 0 || roomIndex >= currentDungeon.rooms.Count) return;

        // Get current room, set vars., and turn on lights.
        currentRoom = currentDungeon.rooms[roomIndex];
        currentRoom.lights.ForEach(light => light.SetActive(true));

        // Enable enemies and freeze their positions.
        if (!currentRoom.isCleared)
        {
            // (?) is there a better solution for this?
            currentRoom.enemies.ForEach(enemy =>
            {
                enemy.SetActive(true);
                enemy.GetComponent<Enemy_Controller>().isInAnimation = true;
            });
        }

        // Update camera position.
        Vector3 newCameraPosition = currentRoom.roomCollider.GetComponent<BoxCollider2D>().bounds.center;
        newCameraPosition.z = -100;
        // Let camera reposition before continuing.
        StartCoroutine(Camera_Controller.instance.UpdatePosition(newCameraPosition, () =>
        {
            // Once the camera has repositioned, check for isCleared.
            if(!currentRoom.isCleared)
            {
                if(_debug) Debug.Log($"Room Not Cleared (EnterRoom)");
                // Close doors and unfreeze all enemies in current room.
                currentRoom.doors.ForEach(door => door.SetActive(true));
                // currentRoom.enemies.ForEach(enemy => enemy.SetActive(true));
                currentRoom.enemies.ForEach(enemy => {
                    enemy.GetComponent<Enemy_Controller>().isInAnimation = false;
                });
            }
            else
            {
                if(_debug) Debug.Log($"Room Cleared (EnterRoom)");
                currentRoom.doors.ForEach(door => door.SetActive(false));
            }
        }));
    }

    public void ExitRoom(int dungeonIndex, int roomIndex)
    {
        if(dungeonIndex < 0 || dungeonIndex >= dungeons.Count) return;
        Dungeon currentDungeon = dungeons[dungeonIndex];
        if(roomIndex < 0 || roomIndex >= currentDungeon.rooms.Count) return;

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
    }
}
