using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DungeonGenerator : MonoBehaviour
{
    // Parameters for dungeon generation
    public int maxNumOfRooms = 9;
    public int minNumOfRooms = 4;
    public int numOfRows = 3;
    public int numOfCols = 3;
    public int maxRoomSize = 25;
    public int minRoomSize = 8;

    // Prefabs for different types of tiles
    public GameObject floorTile;
    public GameObject centerTile;
    public GameObject startTile;
    public GameObject endTile;

    // Lists to keep track of rooms and instantiated tiles
    public List<Room> rooms = new List<Room>();
    public List<GameObject> floorObjects = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        StartGenerating(); // Begin dungeon generation
    }

    // Update is called once per frame (not used in this script)
    void Update()
    {
        
    }

    // Main method to start generating the dungeon
    public void StartGenerating()
    {
        // Validate grid parameters
        if (maxNumOfRooms != numOfRows * numOfCols)
        {
            Debug.LogError("Incorrect grid parameters: maxNumOfRooms must equal numOfRows * numOfCols");
            return;
        }

        ClearPreviousGeneration(); // Clear previous dungeon
        GenerateFloor(); // Generate the new dungeon floor
    }

    // Clear any previously generated dungeon tiles
    public void ClearPreviousGeneration()
    {
        foreach (GameObject tile in floorObjects)
        {
            Destroy(tile); // Remove each tile from the scene
        }

        floorObjects.Clear(); // Clear the list of floor objects
        rooms.Clear(); // Clear the list of rooms
    }

    // Generate the dungeon floor by creating rooms and placing them
    public void GenerateFloor()
    {   
        int skipsLeft = maxNumOfRooms - minNumOfRooms;

        for (int i = 0; i < maxNumOfRooms; i++)
        {
            if (skipsLeft > 0 && Random.Range(0, 3) == 2)
            {
                skipsLeft--;
                rooms.Add(new Room(maxRoomSize, minRoomSize));
                rooms[i].isEmpty = true; // Mark room as empty
                continue;
            }

            rooms.Add(new Room(maxRoomSize, minRoomSize));
            rooms[i].GenerateRoom(); // Generate the room layout
        }

        ChooseStartAndEndRoom(); // Assign start and end rooms
    }

    // Randomly select start and end rooms
    public void ChooseStartAndEndRoom()
    {
        List<Room> nonEmptyRooms = rooms.Where(room => !room.isEmpty).ToList();
        
        int startRoomIndex = Random.Range(0, nonEmptyRooms.Count);
        nonEmptyRooms[startRoomIndex].isStartRoom = true;
        nonEmptyRooms[startRoomIndex].roomSpace[nonEmptyRooms[startRoomIndex].centerX, nonEmptyRooms[startRoomIndex].centerY] = 3; // Start room identifier

        nonEmptyRooms.RemoveAt(startRoomIndex);

        int endRoomIndex = Random.Range(0, nonEmptyRooms.Count);
        nonEmptyRooms[endRoomIndex].roomSpace[nonEmptyRooms[endRoomIndex].centerX, nonEmptyRooms[endRoomIndex].centerY] = 4; // End room identifier

        assignNeighborRooms(); // Assign neighbors to rooms
    }

    // Assign neighboring rooms to each room
    public void assignNeighborRooms()
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            if (rooms[i].isEmpty)
            {
                continue;
            }

            if (i - 1 >= 0 && !rooms[i - 1].isEmpty)
            {
                rooms[i].leftNeighbor = rooms[i - 1];
            }
            if (i + 1 < rooms.Count && !rooms[i + 1].isEmpty)
            {
                rooms[i].rightNeighbor = rooms[i + 1];
            }
            if (i - 3 >= 0 && !rooms[i - 3].isEmpty)
            {
                rooms[i].aboveNeighbor = rooms[i - 3];
            }
            if (i + 3 < rooms.Count && !rooms[i + 3].isEmpty)
            {
                rooms[i].belowNeighbor = rooms[i + 3];
            }
        }

        DrawDungeon(CombineRoomArrays()); // Draw the dungeon based on the combined room arrays
    }

    // Combine room arrays into a single dungeon floor array
    public int[,] CombineRoomArrays()
    {
        int dungeonWidth = numOfCols * maxRoomSize;
        int dungeonHeight = numOfRows * maxRoomSize;
        int[,] floor = new int[dungeonHeight, dungeonWidth];

        for (int row = 0; row < numOfRows; row++)
        {
            if (row % 2 == 0)
            {
                for (int col = 0; col < numOfCols; col++)
                {
                    int roomIndex = row * numOfCols + col;
                    PlaceRoomInDungeon(floor, roomIndex, row, col); // Place the room in the dungeon
                }
            }
            else 
            {
                for (int col = numOfCols - 1; col >= 0; col--)
                {
                    int roomIndex = row * numOfCols + (numOfCols - 1 - col);
                    PlaceRoomInDungeon(floor, roomIndex, row, col); // Place the room in the dungeon
                }
            }
        }

        return floor;
    }

    // Place a room's tiles in the dungeon array
    private void PlaceRoomInDungeon(int[,] floor, int roomIndex, int row, int col)
    {
        Room room = rooms[roomIndex];

        for (int roomRow = 0; roomRow < maxRoomSize; roomRow++)
        {
            for (int roomCol = 0; roomCol < maxRoomSize; roomCol++)
            {
                floor[row * maxRoomSize + roomRow, col * maxRoomSize + roomCol] = room.roomSpace[roomRow, roomCol];
                room.floorCenterX = row * maxRoomSize + roomRow;
                room.floorCenterY = col * maxRoomSize + roomCol;
            }
        }
    }

    // Instantiate and place the tiles in the game world
    public void DrawDungeon(int[,] floor)
    {
        for (int row = 0; row < floor.GetLength(0); row++)
        {
            for (int col = 0; col < floor.GetLength(1); col++)
            {
                GameObject tilePrefab = null;

                switch (floor[row, col])
                {
                    case 1:
                        tilePrefab = floorTile;
                        break;
                    case 2:
                        tilePrefab = centerTile;
                        break;
                    case 3:
                        tilePrefab = startTile;
                        break;
                    case 4:
                        tilePrefab = endTile;
                        break;
                }

                if (tilePrefab != null)
                {
                    floorObjects.Add(Instantiate(tilePrefab, new Vector3(row, 0, col), Quaternion.identity));
                }
            }
        }
    }
}
