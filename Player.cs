using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public int currentX;
    public int currentY;

    public DungeonGenerator generator;

    // Start is called before the first frame update
    void Start()
    {
        generator = GameObject.Find("Dungeon Generator").GetComponent<DungeonGenerator>();
    }

    // Update is called once per frame
    void Update()
    {
        int oldX = currentX;
        int oldY = currentY;

        if (Input.GetKeyDown(KeyCode.W))
        {
            if (generator.floors[generator.currentFloorNum].floor[currentX - 1, currentY] != 0)
            {
                currentX--;
            }
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            if (generator.floors[generator.currentFloorNum].floor[currentX, currentY - 1] != 0)
            {
                currentY--;
            }
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            if (generator.floors[generator.currentFloorNum].floor[currentX + 1, currentY] != 0)
            {
                currentX++;
            }
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            if (generator.floors[generator.currentFloorNum].floor[currentX, currentY + 1] != 0)
            {
                currentY++;
            }
        }

        if (oldX != currentX || oldY != currentY)
        {
            transform.position = new Vector3(currentX, 1, currentY);

            if (currentX == generator.floors[generator.currentFloorNum].floorStartX && currentY == generator.floors[generator.currentFloorNum].floorStartY)
            {
                generator.LoadAboveFloor();
            }
            if (currentX == generator.floors[generator.currentFloorNum].floorEndX && currentY == generator.floors[generator.currentFloorNum].floorEndY)
            {
                if (generator.currentFloorNum + 1 == generator.floors.Count)
                {
                    generator.StartGenerating();
                }
                else
                {
                    generator.LoadBelowFloor();
                }
            }
        }
    }
}
