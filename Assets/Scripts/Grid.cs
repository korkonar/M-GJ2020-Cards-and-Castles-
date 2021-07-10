using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Grid {
    Vector3 pos;
    public int width;
    public int height;
    private float cellSize;
    private GameObject cell;
    public GameObject[,] cellArray;
    public Sprite[] allTileSprites;
    public Vector2Int exit = new Vector2Int(3, 3);
    public Vector2Int entrance = new Vector2Int(1, 1);

    public Grid(Vector3 pos, GameObject cell, int width, int height, float cellSize) {
        this.cell = cell;
        this.pos = pos;
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        cellArray = new GameObject[width, height];
        GameObject container=GameObject.FindGameObjectWithTag("Map");

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                GameObject currentCell = Object.Instantiate(cell,container.transform);
                currentCell.GetComponent<Cell>().Init(GetWorldPosition(x, y), cellSize, cellSize);
                cellArray[x, y] = currentCell;
            }
        }
        Debug.Log("Grid initialised");
    }

    public Vector3 GetWorldPosition(int x, int y) {
        Vector3 temp = new Vector3(x, -y) * cellSize;
        return pos + temp;
    }


    public void ToggleCollision() {
        foreach (GameObject cell in cellArray) {
            cell.GetComponent<Cell>().ToggleCollision();
        }
    }

    /// <summary>
    /// Reads the collision values from file
    /// </summary>
    /// <param name="path">The path of the file that has the collision info</param>
    public void ReadCollisionsFromFile(string path) {
        StreamReader sr = new StreamReader(path);
        if (!File.Exists(path)) {
            Debug.Log("Tried to load from non-existent file " + path);
            return;
        }
        for (int y = 0; y < height; y++) {
            if (!sr.EndOfStream) {
                string currentLine = sr.ReadLine();
                for (int x = 0; x < Mathf.Min(width, currentLine.Length); x++) {
                    switch (currentLine[x]) {
                        case 'n':
                            cellArray[x, y].GetComponent<Cell>().CollisionMode1 = Cell.CollisionMode.Wall;
                            break;
                        case 'y':
                            cellArray[x, y].GetComponent<Cell>().CollisionMode1 = Cell.CollisionMode.Floor;
                            break;
                        case 't':
                            cellArray[x, y].GetComponent<Cell>().CollisionMode1 = Cell.CollisionMode.Trap;
                            break;
                        case 'e':
                            cellArray[x, y].GetComponent<Cell>().CollisionMode1 = Cell.CollisionMode.Floor;
                            break;
                        case 'x':
                            cellArray[x, y].GetComponent<Cell>().CollisionMode1 = Cell.CollisionMode.Floor;
                            break;

                        default:
                            Debug.Log("Character in Level Collision file that's unknown");
                            break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Reads the sprites from a file
    /// </summary>
    /// <param name="path">The path of the file that has the sprite info</param>
    public void ReadSpritesFromFile(string path) {
        if (!File.Exists(path)) {
            Debug.Log("Tried to load from non-existent file " + path);
            return;
        }
        StreamReader sr = new StreamReader(path);
        for (int y = 0; y < height; y++) {
            if (!sr.EndOfStream) {
                string currentLine = sr.ReadLine();
                for (int x = 0; x < Mathf.Min(width, currentLine.Length); x++) {
                    switch (currentLine[x]) {
                        case 'n':
                            cellArray[x, y].GetComponent<SpriteRenderer>().sprite = allTileSprites[0];
                            break;
                        case 'y':
                            cellArray[x, y].GetComponent<SpriteRenderer>().sprite = allTileSprites[1];
                            break;
                        case 't':
                            cellArray[x, y].GetComponent<SpriteRenderer>().sprite = allTileSprites[1];
                            break;
                        case 'e':
                            cellArray[x, y].GetComponent<SpriteRenderer>().sprite = allTileSprites[3];
                            entrance = new Vector2Int(x, y);
                            break;
                        case 'x':
                            cellArray[x, y].GetComponent<SpriteRenderer>().sprite = allTileSprites[4];
                            exit = new Vector2Int(x, y);
                            break;
                        default:
                            Debug.Log("Character in Level Collision file that's unknown");
                            break;
                    }
                }
            }
        }
    }
}
