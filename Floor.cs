using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor
{

    public int[,] floor;

    public int floorNum;

    public int floorStartX;
    public int floorStartY;
    public int floorEndX;
    public int floorEndY;

    public List<Room> rooms = new List<Room>();

    public Floor(int floorNumber, int[,] floorSpace)
    {
        floorNum = floorNumber;
        floor = floorSpace;
    }
}
