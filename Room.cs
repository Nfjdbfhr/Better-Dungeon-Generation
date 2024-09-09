using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    private int maxRoomDimensions;
    private int minRoomDimensions;

    public int centerX;
    public int centerY;
    public int floorCenterX;
    public int floorCenterY;

    public int[,] roomSpace;

    public List<Room> neigborRooms = new List<Room>();

    public Room aboveNeighbor;
    public Room belowNeighbor;
    public Room leftNeighbor;
    public Room rightNeighbor;

    public bool isEmpty = false;
    public bool isStartRoom = false;

    public Room(int maxSize, int minSize)
    {
        maxRoomDimensions = maxSize;
        minRoomDimensions = minSize;

        roomSpace = new int [maxRoomDimensions, maxRoomDimensions];
    }

    public void GenerateRoom()
    {
        roomSpace = new int [maxRoomDimensions, maxRoomDimensions];

        int roomLength = Random.Range(minRoomDimensions, maxRoomDimensions + 1);
        int roomHeight = Random.Range(minRoomDimensions, maxRoomDimensions + 1);

        int rowsDown = Random.Range(0, maxRoomDimensions - roomHeight + 1);
        int colsOver = Random.Range(0, maxRoomDimensions - roomLength + 1);

        for (int i = rowsDown; i < rowsDown + roomHeight; i++)
        {
            for (int j = colsOver; j < colsOver + roomLength; j++)
            {
                roomSpace[i, j] = 1;
            }
        }

        FindRoomCenter(rowsDown, colsOver, roomLength, roomHeight);
    }

    public void FindRoomCenter(int rowsDown, int colsOver, int roomLength, int roomHeight)
    {
        centerX = rowsDown + (roomHeight / 2);
        centerY = colsOver + (roomLength / 2);

        roomSpace[centerX, centerY] = 2;
    }
}
