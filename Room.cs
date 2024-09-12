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

    public int numOfCoridoors = 0;

    public int[,] roomSpace;

    public List<Room> neighborRooms = new List<Room>();

    public bool isEmpty = false;
    public bool isStartRoom = false;

    // Constructor to initialize the room with maximum and minimum sizes
    public Room(int maxSize, int minSize)
    {
        maxRoomDimensions = maxSize;
        minRoomDimensions = minSize;

        roomSpace = new int[maxRoomDimensions, maxRoomDimensions];
    }

    // Generate the layout of the room
    public void GenerateRoom()
    {
        roomSpace = new int[maxRoomDimensions, maxRoomDimensions];

        int roomLength = Random.Range(minRoomDimensions, maxRoomDimensions + 1);
        int roomHeight = Random.Range(minRoomDimensions, maxRoomDimensions + 1);

        int rowsDown = Random.Range(0, maxRoomDimensions - roomHeight + 1);
        int colsOver = Random.Range(0, maxRoomDimensions - roomLength + 1);

        for (int i = rowsDown; i < rowsDown + roomHeight; i++)
        {
            for (int j = colsOver; j < colsOver + roomLength; j++)
            {
                roomSpace[i, j] = 1; // Mark room space
            }
        }

        FindRoomCenter(rowsDown, colsOver, roomLength, roomHeight); // Find and mark the center of the room
    }

    // Find the center position of the room
    public void FindRoomCenter(int rowsDown, int colsOver, int roomLength, int roomHeight)
    {
        centerX = rowsDown + (roomHeight / 2);
        centerY = colsOver + (roomLength / 2);

        roomSpace[centerX, centerY] = 2; // Mark the center of the room
    }
}
