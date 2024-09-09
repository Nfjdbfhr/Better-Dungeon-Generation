using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DungeonGenerator : MonoBehaviour
{

    public int maxNumOfRooms = 9;
    public int minNumOfRooms = 4;
    public int numOfRows = 3;
    public int numOfCols = 3;
    public int maxRoomSize = 25;
    public int minRoomSize = 8;

    public GameObject floorTile;
    public GameObject centerTile;
    public GameObject startTile;
    public GameObject endTile;

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
        if (maxNumOfRooms != numOfRows * numOfCols)
        {
            Debug.LogError("Incorrect grid parameters: maxNumOfRooms must equal numOfRows * numOfCols");
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
        rooms.Clear();
    }

    public void GenerateFloor()
    {   
        int skipsLeft = maxNumOfRooms - minNumOfRooms;

        for (int i = 0; i < maxNumOfRooms; i++)
        {
            if (skipsLeft > 0 && Random.Range(0, 3) == 2)
            {
                skipsLeft--;
                rooms.Add(new Room(maxRoomSize, minRoomSize));
                rooms[i].isEmpty = true;
                continue;
            }

            rooms.Add(new Room(maxRoomSize, minRoomSize));
            rooms[i].GenerateRoom();
        }

        ChooseStartAndEndRoom();
    }

    public void ChooseStartAndEndRoom()
    {
        List<Room> nonEmptyRooms = rooms.Where(room => !room.isEmpty).ToList();
        
        int startRoomIndex = Random.Range(0, nonEmptyRooms.Count);
        nonEmptyRooms[startRoomIndex].isStartRoom = true;
        nonEmptyRooms[startRoomIndex].roomSpace[nonEmptyRooms[startRoomIndex].centerX, nonEmptyRooms[startRoomIndex].centerY] = 3;

        nonEmptyRooms.RemoveAt(startRoomIndex);

        int endRoomIndex = Random.Range(0, nonEmptyRooms.Count);
        nonEmptyRooms[endRoomIndex].roomSpace[nonEmptyRooms[endRoomIndex].centerX, nonEmptyRooms[endRoomIndex].centerY] = 4;

        assignNeighborRooms();
    }

    public void assignNeighborRooms()
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            if (rooms[i].isEmpty)
            {
                continue;
            }

            if (i - 1 >= 0)
            {
                if (!rooms[i - 1].isEmpty)
                {
                    rooms[i].leftNeighbor = rooms[i - 1];
                }
            }
            if (i + 1 < rooms.Count)
            {
                if (!rooms[i + 1].isEmpty)
                {
                    rooms[i].rightNeighbor = rooms[i + 1];
                }
            }
            if (i - 3 >= 0)
            {
                if (!rooms[i - 3].isEmpty)
                {
                    rooms[i].aboveNeighbor = rooms[i - 3];
                }
            }
            if (i + 3 < rooms.Count)
            {
                if (!rooms[i + 3].isEmpty)
                {
                    rooms[i].belowNeighbor = rooms[i + 3];
                }
            }
        }

        DrawDungeon(CombineRoomArrays());
    }

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
                    PlaceRoomInDungeon(floor, roomIndex, row, col);
                }
            }
            else 
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
                else if (floor[row, col] == 3)
                {
                    floorObjects.Add(Instantiate(startTile, new Vector3(row, 0, col), Quaternion.identity));
                }
                else if (floor[row, col] == 4)
                {
                    floorObjects.Add(Instantiate(endTile, new Vector3(row, 0, col), Quaternion.identity));
                }
            }
        }
    }
}
