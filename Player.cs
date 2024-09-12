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
            if (generator.currentFloor[currentX - 1, currentY] != 0)
            {
                currentX--;
            }
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            if (generator.currentFloor[currentX, currentY - 1] != 0)
            {
                currentY--;
            }
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            if (generator.currentFloor[currentX + 1, currentY] != 0)
            {
                currentX++;
            }
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            if (generator.currentFloor[currentX, currentY + 1] != 0)
            {
                currentY++;
            }
        }

        if (oldX != currentX || oldY != currentY)
        {
            transform.position = new Vector3(currentX, 1, currentY);
        }
    }
}