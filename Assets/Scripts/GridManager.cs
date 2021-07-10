using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GridManager : MonoBehaviour {
    /// <summary>
    /// TopLeft position of the grid
    /// </summary>
    public Vector3 gridPos;
    public Grid grid;

    /// <summary>
    /// Object placed at every cell.
    /// </summary>
    public GameObject Cell;

    /// <summary>
    /// Spacing between cells. 1 is no space in between, 1.1 is 10% spacing between cells etc.
    /// </summary>
    public float spacing;
    public int spriteSize;
    public Sprite[] allTileSprites;
    public string nameFile;

    public Vector2 dimensions;
    // Start is called before the first frame update
    void Awake() {
        grid = new Grid(gridPos, Cell, (int)dimensions.x, (int)dimensions.y, spacing * (float)spriteSize / 100);
        grid.allTileSprites = allTileSprites;
        LoadLevel(nameFile);
    }

    // Temporary input handling. Probably should go in it's own class.
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            grid.ToggleCollision();
        }
    }

    /// <summary>
    /// Loads in the level and the collisionmap.
    /// </summary>
    /// <param name="fileName"> Name of the level without extension or path to it.</param>
    void LoadLevel(string fileName) {
        string path = @"Assets/Levels/";
        grid.ReadCollisionsFromFile(path + fileName + "Col.txt");
        grid.ReadSpritesFromFile(path + fileName + "Sprites.txt");
    }

    /// Lmao put this here to prevent last minute merge conflicts
    /// 
    public void LoadMenu() {
        GetComponent<AudioSource>().pitch = Random.Range(0.95f, 1.05f);
        GetComponent<AudioSource>().Play();
        SceneManager.LoadScene(0);
    }
}
