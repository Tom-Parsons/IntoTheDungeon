using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RoomGeneration : MonoBehaviour
{

    public static RoomGeneration instance;

    public float roomHeight = 50; // The Z axis
    public float roomWidth = 50; // The X Axis
    [SerializeField] private GameObject centreRoom;
    [SerializeField] private List<GameObject> rooms;
    [SerializeField] private List<GameObject> walls;
    private int newRoomChance = 50;

    private List<Room> generatedRooms;
    private List<GameObject> generatedRoomsGameobjects;
    private List<GameObject> generatedWalls;

    private List<GameObject> enemySpawnpoints;

    public bool generated;

    private int yOffset;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        generatedRooms = new List<Room>();
        generatedWalls = new List<GameObject>();
        enemySpawnpoints = new List<GameObject>();
        generatedRoomsGameobjects = new List<GameObject>();
        yOffset = 0;
        GenerateLevel();
    }

    public void GenerateLevel()
    {
        if(generatedRooms.Count > 0)
        {
            foreach(GameObject room in generatedRoomsGameobjects)
            {
                Destroy(room);
            }
            foreach(GameObject wall in generatedWalls)
            {
                Destroy(wall);
            }
            generatedRooms.Clear();
            generatedWalls.Clear();
            generatedRoomsGameobjects.Clear();
            enemySpawnpoints.Clear();
            EnemySpawnController.instance.spawners.Clear();
            newRoomChance = 50;
            yOffset -= 10;
        }
        GameObject mainCentreRoom = centreRoom;
        mainCentreRoom.transform.position = new Vector3(PlayerControl.instance.gameObject.transform.position.x, yOffset, PlayerControl.instance.gameObject.transform.position.z);
        Room mainRoom = new Room(mainCentreRoom.transform.position, mainCentreRoom);
        generatedRooms.Add(mainRoom);
        generatedRoomsGameobjects.Add(Instantiate(mainCentreRoom, mainCentreRoom.transform.position, Quaternion.identity));

        GenerateRoom(mainRoom, 1);

        GenerateWalls();

        Invoke("GenerateSpawnpoints", 0.1f);

        GenerateNavMesh();

        generated = true;
    }

    private void GenerateNavMesh(){
        foreach(GameObject room in generatedRoomsGameobjects)
        {
            foreach(Transform child in room.transform)
            {
                if(child.gameObject.name == "Floor")
                {
                    child.gameObject.AddComponent<NavMeshSourceTag>();
                }
            }
        }
    }

    private void GenerateWalls()
    {
        //Loop through the rooms and check if there are rooms next to it. If not, then create a wall (if there isn't already a wall there)
        foreach(Room r in generatedRooms)
        {
            bool hasRoomUp = false;
            bool hasRoomDown = false;
            bool hasRoomLeft = false;
            bool hasRoomRight = false;
            foreach (Room r2 in generatedRooms)
            {
                if((r2.getCentre() - new Vector3(r.getCentre().x, r.getCentre().y, r.getCentre().z + roomHeight)).magnitude < 10)
                {
                    hasRoomUp = true;
                }
                if ((r2.getCentre() - new Vector3(r.getCentre().x, r.getCentre().y, r.getCentre().z - roomHeight)).magnitude < 10)
                {
                    hasRoomDown = true;
                }
                if ((r2.getCentre() - new Vector3(r.getCentre().x - roomWidth, r.getCentre().y, r.getCentre().z)).magnitude < 10)
                {
                    hasRoomLeft = true;
                }
                if ((r2.getCentre() - new Vector3(r.getCentre().x + roomWidth, r.getCentre().y, r.getCentre().z)).magnitude < 10)
                {
                    hasRoomRight = true;
                }
            }
            if (!hasRoomUp)
            {
                bool wallAlreadyExistsUp = false;
                foreach (GameObject wall in generatedWalls)
                {
                    if ((wall.transform.position - new Vector3(r.getCentre().x, r.getCentre().y, r.getCentre().z + roomHeight / 2)).magnitude < 5)
                        wallAlreadyExistsUp = true;
                }
                if (!wallAlreadyExistsUp)
                {
                    GameObject randomWall = walls[Random.Range(0, walls.Count)];
                    generatedWalls.Add(Instantiate(randomWall, new Vector3(r.getCentre().x, r.getCentre().y + randomWall.transform.localScale.y / 2, r.getCentre().z + roomHeight / 2), Quaternion.Euler(0, 0, 0)));
                }
            }
            if (!hasRoomDown)
            {
                bool wallAlreadyExistsDown = false;
                foreach (GameObject wall in generatedWalls)
                {
                    if ((wall.transform.position - new Vector3(r.getCentre().x, r.getCentre().y, r.getCentre().z - roomHeight / 2)).magnitude < 5)
                        wallAlreadyExistsDown = true;
                }
                if (!wallAlreadyExistsDown)
                {
                    GameObject randomWall = walls[Random.Range(0, walls.Count)];
                    generatedWalls.Add(Instantiate(randomWall, new Vector3(r.getCentre().x, r.getCentre().y + randomWall.transform.localScale.y / 2, r.getCentre().z - roomHeight / 2), Quaternion.Euler(0, -180, 0)));
                }
            }
            if (!hasRoomLeft)
            {
                bool wallAlreadyExistsLeft = false;
                foreach (GameObject wall in generatedWalls)
                {
                    if ((wall.transform.position - new Vector3(r.getCentre().x - roomWidth / 2, r.getCentre().y, r.getCentre().z)).magnitude < 5)
                        wallAlreadyExistsLeft = true;
                }
                if (!wallAlreadyExistsLeft)
                {
                    GameObject randomWall = walls[Random.Range(0, walls.Count)];
                    generatedWalls.Add(Instantiate(randomWall, new Vector3(r.getCentre().x - roomWidth / 2, r.getCentre().y + randomWall.transform.localScale.y / 2, r.getCentre().z), Quaternion.Euler(0, -90, 0)));
                }
            }
            if (!hasRoomRight)
            {
                bool wallAlreadyExistsRight = false;
                foreach (GameObject wall in generatedWalls)
                {
                    if ((wall.transform.position - new Vector3(r.getCentre().x + roomWidth / 2, r.getCentre().y, r.getCentre().z)).magnitude < 5)
                        wallAlreadyExistsRight = true;
                }
                if (!wallAlreadyExistsRight)
                {
                    GameObject randomWall = walls[Random.Range(0, walls.Count)];
                    generatedWalls.Add(Instantiate(randomWall, new Vector3(r.getCentre().x + roomWidth / 2, r.getCentre().y + randomWall.transform.localScale.y / 2, r.getCentre().z), Quaternion.Euler(0, 90, 0)));
                }
            }
        }
    }

    private void GenerateSpawnpoints()
    {
        foreach (GameObject room in generatedRoomsGameobjects) {
            foreach (Transform child in room.transform)
            {
                if(child.gameObject.name == "EnemySpawnPoints")
                {
                    foreach(Transform spawnPoints in child)
                    {
                        enemySpawnpoints.Add(spawnPoints.gameObject);
                    }
                }
                else if (child.gameObject.name == "PortalEffect")
                {
                    EnemySpawnController.instance.portalEffects.Add(child.gameObject.GetComponent<ChildPartileSystemsController>());
                }
            }
        }
        print(EnemySpawnController.instance);
        print(EnemySpawnController.instance.spawners);
        print(enemySpawnpoints);
        foreach (GameObject spawnPoint in enemySpawnpoints) {
            EnemySpawnController.instance.spawners.Add(spawnPoint);
        }
    }

    private void GenerateRoom(Room room, int numberOfNeighbourRooms)
    {
        Room left = null;
        Room right = null;
        Room up = null;
        Room down = null;
        for (int i = 0; i < numberOfNeighbourRooms; i++)
        {
            if (newRoomChance <= 0)
                continue;
            switch (i)
            {
                case 0:
                    bool roomAlreadyExistsUp = false;
                    foreach(Room r in generatedRooms)
                    {
                        if ((r.getCentre() - new Vector3(room.getCentre().x, room.getCentre().y, room.getCentre().z + roomHeight)).magnitude < 10)
                            roomAlreadyExistsUp = true;
                    }
                    if (roomAlreadyExistsUp)
                        continue;
                    break;
                case 1:
                    bool roomAlreadyExistsDown = false;
                    foreach (Room r in generatedRooms)
                    {
                        if ((r.getCentre() - new Vector3(room.getCentre().x, room.getCentre().y, room.getCentre().z - roomHeight)).magnitude < 10)
                            roomAlreadyExistsDown = true;
                    }
                    if (roomAlreadyExistsDown)
                        continue;
                    break;
                case 2:
                    bool roomAlreadyExistsLeft = false;
                    foreach (Room r in generatedRooms)
                    {
                        if ((r.getCentre() - new Vector3(room.getCentre().x - roomWidth, room.getCentre().y, room.getCentre().z)).magnitude < 10)
                            roomAlreadyExistsLeft = true;
                    }
                    if (roomAlreadyExistsLeft)
                        continue;
                    break;
                case 3:
                    bool roomAlreadyExistsRight = false;
                    foreach (Room r in generatedRooms)
                    {
                        if ((r.getCentre() - new Vector3(room.getCentre().x + roomWidth, room.getCentre().y, room.getCentre().z)).magnitude < 10)
                            roomAlreadyExistsRight = true;
                    }
                    if (roomAlreadyExistsRight)
                        continue;
                    break;
            }
            int chance = Random.Range(0, 100);
            if(chance < newRoomChance || generatedRooms.Count <= 2)
            {
                if(generatedRooms.Count > 2) newRoomChance -= 5;
                GameObject randomRoom = rooms[Random.Range(0, rooms.Count)];
                Vector3 spawnPoint = Vector3.zero;
                switch (i) {
                    case 0:
                        spawnPoint = new Vector3(room.getCentre().x, room.getCentre().y, room.getCentre().z + roomHeight);
                        up = new Room(spawnPoint, randomRoom);
                        break;
                    case 1:
                        spawnPoint = new Vector3(room.getCentre().x, room.getCentre().y, room.getCentre().z - roomHeight);
                        down = new Room(spawnPoint, randomRoom);
                        break;
                    case 2:
                        spawnPoint = new Vector3(room.getCentre().x - roomWidth, room.getCentre().y, room.getCentre().z);
                        left = new Room(spawnPoint, randomRoom);
                        break;
                    case 3:
                        spawnPoint = new Vector3(room.getCentre().x + roomWidth, room.getCentre().y, room.getCentre().z);
                        right = new Room(spawnPoint, randomRoom);
                        break;
                }
            }
        }
        List<int> indexes = new List<int> { 0, 1, 2, 3 };
        for(int i = 0; i < 4; i++)
        {
            int roomIndex = Random.Range(0, indexes.Count);
            int nextRoom = indexes[roomIndex];
            indexes.RemoveAt(roomIndex);
            switch (nextRoom)
            {
                case 0:
                    if (up != null)
                    {
                        generatedRooms.Add(up);
                        generatedRoomsGameobjects.Add(Instantiate(up.getPrefab(), up.getCentre(), Quaternion.identity));

                        GenerateRoom(up, 4);
                    }
                    break;
                case 1:
                    if (down != null)
                    {
                        generatedRooms.Add(down);
                        generatedRoomsGameobjects.Add(Instantiate(down.getPrefab(), down.getCentre(), Quaternion.identity));

                        GenerateRoom(down, 4);
                    }
                    break;
                case 2:
                    if (left != null)
                    {
                        generatedRooms.Add(left);
                        generatedRoomsGameobjects.Add(Instantiate(left.getPrefab(), left.getCentre(), Quaternion.identity));

                        GenerateRoom(left, 4);
                    }
                    break;
                case 3:
                    if (right != null)
                    {
                        generatedRooms.Add(right);
                        generatedRoomsGameobjects.Add(Instantiate(right.getPrefab(), right.getCentre(), Quaternion.identity));

                        GenerateRoom(right, 4);
                    }
                    break;
            }
        }
    }

}
