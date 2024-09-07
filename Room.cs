using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    private int maxRoomDimensions;
    private int minRoomDimensions;

    private int roomCenterX { get; set; }
    private int roomCenterY { get; set; }

    public int[,] roomSpace;

    public List<Room> neigborRooms = new List<Room>();

    public Room(int maxSize, int minSize)
    {
        maxRoomDimensions = maxSize;
        minRoomDimensions = minSize;
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
        int centerX = rowsDown + (roomHeight / 2);
        int centerY = colsOver + (roomLength / 2);

        roomSpace[centerX, centerY] = 2;
    }
}
