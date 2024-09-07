using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{

    public int numOfRooms = 9;
    public int numOfRows = 3;
    public int numOfCols = 3;
    public int maxRoomSize = 25;
    public int minRoomSize = 8;

    public GameObject floorTile;
    public GameObject centerTile;
    public GameObject startTile;
    public GameObject endTIle;

    public List<Room> rooms = new List<Room>();
    public List<GameObject> floorObjects = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        StartGenerating();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGenerating()
    {
        if (numOfRooms != numOfRows * numOfCols)
        {
            Debug.LogError("Incorrect grid parameters: numOfRooms must equal numOfRows * numOfCols");
            return;
        }

        ClearPreviousGeneration();
        GenerateFloor();
    }

    public void ClearPreviousGeneration()
    {
        foreach (GameObject tile in floorObjects)
        {
            Destroy(tile);
        }

        floorObjects.Clear();
    }

    public void GenerateFloor()
    {   
        for (int i = 0; i < numOfRooms; i++)
        {
            rooms.Add(new Room(maxRoomSize, minRoomSize));
            rooms[i].GenerateRoom();
        }

        DrawDungeon(CombineRoomArrays());
    }

    public int[,] CombineRoomArrays()
    {
        int dungeonWidth = numOfCols * maxRoomSize;
        int dungeonHeight = numOfRows * maxRoomSize;
        int[,] floor = new int[dungeonHeight, dungeonWidth];  // Full dungeon floor array

        // Place each room in its corresponding spot on the grid in a snaking pattern
        for (int row = 0; row < numOfRows; row++)
        {
            if (row % 2 == 0)  // Even row, go left to right
            {
                for (int col = 0; col < numOfCols; col++)
                {
                    int roomIndex = row * numOfCols + col;
                    PlaceRoomInDungeon(floor, roomIndex, row, col);
                }
            }
            else  // Odd row, go right to left
            {
                for (int col = numOfCols - 1; col >= 0; col--)
                {
                    int roomIndex = row * numOfCols + (numOfCols - 1 - col);
                    PlaceRoomInDungeon(floor, roomIndex, row, col);
                }
            }
        }

        return floor;
    }

    // Helper method to place a room in the dungeon array
    private void PlaceRoomInDungeon(int[,] floor, int roomIndex, int row, int col)
    {
        Room room = rooms[roomIndex];

        // Place the room's tiles in the correct part of the floor array
        for (int roomRow = 0; roomRow < maxRoomSize; roomRow++)
        {
            for (int roomCol = 0; roomCol < maxRoomSize; roomCol++)
            {
                floor[row * maxRoomSize + roomRow, col * maxRoomSize + roomCol] = room.roomSpace[roomRow, roomCol];
            }
        }
    }


    public void DrawDungeon(int[,] floor)
    {
        for (int row = 0; row < floor.GetLength(0); row++)
        {
            for (int col = 0; col < floor.GetLength(1); col++)
            {
                if (floor[row, col] == 1)
                {
                    floorObjects.Add(Instantiate(floorTile, new Vector3(row, 0, col), Quaternion.identity));
                }
                else if (floor[row, col] == 2)
                {
                    floorObjects.Add(Instantiate(centerTile, new Vector3(row, 0, col), Quaternion.identity));
                }
            }
        }
    }
}
