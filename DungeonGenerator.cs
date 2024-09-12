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
    public int maxNumOfCoridoors = 2;

    // Prefabs for different types of tiles
    public GameObject floorTile;
    public GameObject centerTile;
    public GameObject startTile;
    public GameObject endTile;

    // Lists to keep track of rooms and instantiated tiles
    public List<Room> rooms = new List<Room>();
    public List<Room> nonEmptyRooms = new List<Room>();
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
        nonEmptyRooms = rooms.Where(room => !room.isEmpty).ToList();
        
        int startRoomIndex = Random.Range(0, nonEmptyRooms.Count);
        nonEmptyRooms[startRoomIndex].isStartRoom = true;
        nonEmptyRooms[startRoomIndex].roomSpace[nonEmptyRooms[startRoomIndex].centerX, nonEmptyRooms[startRoomIndex].centerY] = 3; // Start room identifier
        Debug.Log("Start: " + nonEmptyRooms[startRoomIndex].centerX + ", " + nonEmptyRooms[startRoomIndex].centerY);

        int endRoomIndex = 0;
        do
        {
            endRoomIndex = Random.Range(0, nonEmptyRooms.Count);
        }
        while (endRoomIndex == startRoomIndex);

        nonEmptyRooms[endRoomIndex].roomSpace[nonEmptyRooms[endRoomIndex].centerX, nonEmptyRooms[endRoomIndex].centerY] = 4; // End room identifier
        Debug.Log("End: " + nonEmptyRooms[endRoomIndex].centerX + ", " + nonEmptyRooms[endRoomIndex].centerY);

        AssignNeighbors();
    }

    public void AssignNeighbors()
    {
        for (int i = 0; i < nonEmptyRooms.Count; i++)
        {
            int[] closeRooms = findNearestRooms(maxNumOfCoridoors, nonEmptyRooms[i]);

            for (int j = 0; j < closeRooms.Length; j++)
            {
                nonEmptyRooms[i].neighborRooms.Add(nonEmptyRooms[closeRooms[j]]);
            }
        }

        DrawDungeon(GenerateCoridoors(CombineRoomArrays()));
    }

    public int[] findNearestRooms(int numToFind, Room currentRoom)
    {
        var closeRooms = Enumerable.Repeat<int>(1000, numToFind).ToArray();

        for (int i = 0; i < nonEmptyRooms.Count; i++)
        {
            if (nonEmptyRooms[i] == currentRoom)
            {
                continue;
            }

            int distance = (Mathf.Abs(nonEmptyRooms[i].floorCenterX - currentRoom.floorCenterX)) + (Mathf.Abs(nonEmptyRooms[i].floorCenterY - currentRoom.floorCenterY));

            for (int j = 0; j < closeRooms.Length; j++)
            {
                if (distance < closeRooms[j])
                {
                    // Shift elements to the right to make room for the new distance
                    for (int k = closeRooms.Length - 1; k > j; k--)
                    {
                        closeRooms[k] = closeRooms[k - 1];
                    }

                    // Insert the new room index at the correct position
                    closeRooms[j] = i;
                    break; // Exit inner loop after placing the room
                }
            }
        }

        return closeRooms;
    }


    public int[,] GenerateCoridoors(int[,] floor)
    {
        for (int i = 0; i < nonEmptyRooms.Count; i++)
        {
            Room currentRoom = nonEmptyRooms[i];

            if (currentRoom.neighborRooms.Count == 0)
            {
                continue;
            }

            int numOfCoridoors = Random.Range(1, currentRoom.neighborRooms.Count);

            for (int j = 0; j < numOfCoridoors; j++)
            {
                Room neighbor = currentRoom.neighborRooms[Random.Range(0, currentRoom.neighborRooms.Count)];

                if (currentRoom.numOfCoridoors >= maxNumOfCoridoors || neighbor.numOfCoridoors >= maxNumOfCoridoors)
                {
                    if (j != numOfCoridoors - 1 && currentRoom.numOfCoridoors != 0)
                    {
                        currentRoom.neighborRooms.Remove(neighbor);
                        neighbor.neighborRooms.Remove(currentRoom);
                        continue;
                    }
                }

                floor = CreateCoridoor(floor, currentRoom, neighbor);

                currentRoom.numOfCoridoors++;
                neighbor.numOfCoridoors++;
                currentRoom.neighborRooms.Remove(neighbor);
                neighbor.neighborRooms.Remove(currentRoom);
            }
        }

        return floor;
    }

    public int[,] CreateCoridoor(int[,] floor, Room room1, Room room2)
    {
        int startX = room1.floorCenterX;
        int startY = room1.floorCenterY;

        int targetX = room2.floorCenterX;
        int targetY = room2.floorCenterY;

        int currentX = startX;
        int currentY = startY;

        while (currentX != targetX || currentY != targetY)
        {
            int xChange = targetX - currentX;
            int yChange = targetY - currentY;

            if (xChange != 0 && yChange != 0)
            {
                if (Random.Range(0, 2) == 0)
                {
                    if (xChange > 0)
                    {
                        currentX++;
                    }
                    else
                    {
                        currentX--;
                    }
                }
                else
                {
                    if (yChange > 0)
                    {
                        currentY++;
                    }
                    else
                    {
                        currentY--;
                    }
                }
            }
            else if (xChange != 0)
            {
                if (xChange > 0)
                {
                    currentX++;
                }
                else
                {
                    currentX--;
                }
            }
            else if (yChange != 0)
            {
                if (yChange > 0)
                {
                    currentY++;
                }
                else
                {
                    currentY--;
                }
            }

            if (floor[currentX, currentY] == 0)
            {
                floor[currentX, currentY] = 1;
            }
        } 

        return floor;
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
                int x = row * maxRoomSize + roomRow;
                int y = col * maxRoomSize + roomCol;
                floor [x, y] = room.roomSpace[roomRow, roomCol];

                if (floor[x, y] == 2 || floor[x, y] == 3 || floor[x, y] == 4)
                {
                    
                    room.floorCenterX = row * maxRoomSize + roomRow;
                    room.floorCenterY = col * maxRoomSize + roomCol;
                }
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
