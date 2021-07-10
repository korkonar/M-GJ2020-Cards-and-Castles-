using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public GridManager gridManager;
    public SoundManager sound;
    public int Mana;
    public int maxMana;
    public Transform ManaContainer;
    public GameObject ManaObj;
    public GameObject pickCardPanel;
    public GameObject EndTurnButton;
    public List<GameObject> Enemies;
    private int EnemyIndex = 0;
    public int SpawnedEnemies = 0;
    private int Turn;
    public GameObject VictoryPanel;
    public GameObject DefeatPanel;
    public Sprite floor;
    public List<GameObject> cards;
    public List<GameObject> AddedWalls;
    PathFinding pf;
    bool playerTurn;
    State state;
    bool picking;
    public bool starting;
    bool startingForReal;
    bool ending;
    public bool endless;
    public GameObject hero;
    public GameObject zombie;
    enum State {
        cardPick,
        CardPlay
    }
    // Start is called before the first frame update
    void Start() {
        sound=GetComponent<SoundManager>();
        Turn = 0;
        Mana = maxMana;
        playerTurn = true;
        picking = false;
        AddedWalls = new List<GameObject>();
        if(endless){
            Enemies=new List<GameObject>();
            Enemies.Add(Instantiate(hero,gridManager.grid.GetWorldPosition(1,1),Quaternion.identity));
            SpawnedEnemies++;
        }
        //Enemy=Instantiate(Enemy,gridManager.grid.GetWorldPosition(1,1),Quaternion.identity);
        pf = new PathFinding();
        pf.gridManager = gridManager;
        //List<int> res = pf.FindShortestPath(grid, new Vector3(1,1), new Vector3(8,3));
    }

    // Update is called once per frame
    void Update() {
        bool end = true;
        bool lost = false;
        foreach (GameObject g in Enemies){
            int xc = Mathf.RoundToInt(Enemies[EnemyIndex].transform.position.x / (gridManager.spacing * (float)gridManager.spriteSize / 100));
            int yc = Mathf.RoundToInt(-Enemies[EnemyIndex].transform.position.y / (gridManager.spacing * (float)gridManager.spriteSize / 100));
            if (g.GetComponent<EnemyScript>().health > 0)
                end = false;
            if (xc == gridManager.grid.exit.x && yc == gridManager.grid.exit.y) {
                lost = true;
            }
        }
        if (end && !endless) {
            VictoryPanel.SetActive(true);
            sound.PlayWin();
        } else if (lost) {
            sound.PlayOver();
            DefeatPanel.SetActive(true);
        } else {
            if (playerTurn) {
                switch (state) {
                    case State.cardPick:
                        if (!picking) {
                            pickCardPanel.SetActive(true);

                            int r = Random.Range(0, cards.Count);
                            int rr;
                            int rrr;
                            do {
                                rr = Random.Range(0, cards.Count);
                            } while (rr == r);
                            do {
                                rrr = Random.Range(0, cards.Count);
                            } while (rrr == r && rrr == rr);
                            Instantiate(cards[r], pickCardPanel.transform.GetChild(0));
                            Instantiate(cards[rr], pickCardPanel.transform.GetChild(0));
                            Instantiate(cards[rrr], pickCardPanel.transform.GetChild(0));
                            picking = true;
                        }
                        break;
                    case State.CardPlay:
                        break;
                }
            } else {
                //move enemy etc..
                int x = 0, y = 0;
                int nx = 0, ny = 0;
                if (starting) {
                    if (startingForReal) {
                        if(!endless){
                            foreach (GameObject g in Enemies) {
                                if (g.GetComponent<EnemyScript>().spawnTurn == Turn) {
                                    g.SetActive(true);
                                    SpawnedEnemies++;
                                    print(SpawnedEnemies);
                                }
                            }
                        }else{
                            if(Random.value>0.75f){
                                if(Random.value>0.5){
                                    Enemies.Add(Instantiate(hero,gridManager.grid.GetWorldPosition(1,1),Quaternion.identity));
                                    SpawnedEnemies++;
                                }else{
                                    Enemies.Add(Instantiate(zombie,gridManager.grid.GetWorldPosition(1,1),Quaternion.identity));
                                    SpawnedEnemies++;
                                }
                            }
                        }
                        startingForReal = false;
                    }
                    if(!endless){
                        if (Enemies[EnemyIndex].activeSelf == false) {
                            EnemyIndex++;
                            if (EnemyIndex >= Enemies.Count) {
                                Turn++;
                                StartCoroutine(endTurn(playerTurn));
                                EnemyIndex = 0;
                                ending = true;
                            }
                        }
                    }
                    if (!ending) {
                        Enemies[EnemyIndex].GetComponent<EnemyScript>().currSteps++;

                        x = Mathf.RoundToInt(Enemies[EnemyIndex].transform.position.x / (gridManager.spacing * (float)gridManager.spriteSize / 100));
                        y = Mathf.RoundToInt(-Enemies[EnemyIndex].transform.position.y / (gridManager.spacing * (float)gridManager.spriteSize / 100));

                        List<int> res = pf.FindShortestPath(gridManager.grid, new Vector2Int(x, y), gridManager.grid.exit);
                        Sequence sq = DOTween.Sequence();
                        sq.Append(Enemies[EnemyIndex].transform.DOMove(gridManager.grid.GetWorldPosition(res[res.Count - 1], res[res.Count - 2]), 0.5f));
                        sq.Insert(0, Enemies[EnemyIndex].transform.DOShakeRotation(sq.Duration(),new Vector3(0,0,40),25,70,false));
                        nx = res[res.Count - 1];
                        ny = res[res.Count - 2];
                        
                        sq.OnComplete(() => moveComplete(x, y, nx, ny));
                        if (gridManager.grid.cellArray[nx, ny].GetComponent<Cell>().CollisionMode1 == Cell.CollisionMode.Wall) {
                            sound.PlayWallDestroy();
                            gridManager.grid.cellArray[nx, ny].GetComponent<SpriteRenderer>().sprite = floor;
                            gridManager.grid.cellArray[nx, ny].GetComponent<Cell>().CollisionMode1 = Cell.CollisionMode.Floor;
                        }

                    }
                    starting = false;
                }
                if (Input.GetKeyDown(KeyCode.T) && !ending) {
                    ending = true;
                    Turn++;
                    StartCoroutine(endTurn(playerTurn));
                }
            }
        }
    }

    void moveComplete(int x, int y, int nx, int ny) {
        //do damage/ice ...
        ActOnEnemy(x, y, nx, ny);
        if (Enemies[EnemyIndex].GetComponent<EnemyScript>().currSteps >= Enemies[EnemyIndex].GetComponent<EnemyScript>().steps) {
            Enemies[EnemyIndex].GetComponent<EnemyScript>().currSteps = 0;
            EnemyIndex++;
            if (EnemyIndex >= SpawnedEnemies) {
                Turn++;
                StartCoroutine(endTurn(playerTurn));
                EnemyIndex = 0;
                ending = true;
                starting = false;
            } else {
                starting = true;
            }
        }
    }
    public void ActOnEnemy(int sx, int sy, int nx, int ny) {
        if (gridManager.grid.cellArray[nx, ny].GetComponent<Cell>().CollisionMode1 == Cell.CollisionMode.Trap) {
            Enemies[EnemyIndex].GetComponent<EnemyScript>().takeDamage(1);
            sound.PlayHurt();
            gridManager.grid.cellArray[nx, ny].GetComponent<Cell>().life--;
            if (gridManager.grid.cellArray[nx, ny].GetComponent<Cell>().life <= 0) {
                gridManager.grid.cellArray[nx, ny].GetComponent<SpriteRenderer>().sprite = floor;
                gridManager.grid.cellArray[nx, ny].GetComponent<Cell>().CollisionMode1 = Cell.CollisionMode.Floor;
            }
            if (Enemies[EnemyIndex].GetComponent<EnemyScript>().currSteps < Enemies[EnemyIndex].GetComponent<EnemyScript>().steps) starting = true;
        } else if (gridManager.grid.cellArray[nx, ny].GetComponent<Cell>().CollisionMode1 == Cell.CollisionMode.Ice) {
            starting = false;
            gridManager.grid.cellArray[nx, ny].GetComponent<Cell>().life--;
            if (gridManager.grid.cellArray[nx, ny].GetComponent<Cell>().life <= 0) {
                gridManager.grid.cellArray[nx, ny].GetComponent<SpriteRenderer>().sprite = floor;
                gridManager.grid.cellArray[nx, ny].GetComponent<Cell>().CollisionMode1 = Cell.CollisionMode.Floor;
            }

            int addx = 0;
            int addy = 0;
            int dmg = 0;
            bool moved = false;
            print((nx - sx) + " " + (ny - sy));
            if (nx - sx != 0) {
                do {
                    if (nx - sx < 0) {
                        addx--;
                    } else {
                        addx++;
                    }
                    if (gridManager.grid.cellArray[nx + addx, ny].GetComponent<Cell>().CollisionMode1 > 0) {
                        moved = true;
                        if (gridManager.grid.cellArray[nx + addx, ny].GetComponent<Cell>().CollisionMode1 == Cell.CollisionMode.Trap) {
                            dmg++;
                            gridManager.grid.cellArray[nx + addx, ny].GetComponent<Cell>().life--;
                            if (gridManager.grid.cellArray[nx + addx, ny].GetComponent<Cell>().life <= 0) {
                                StartCoroutine(changeToFloor(nx + addx, ny));
                            }
                        }
                    } else {
                        if (nx - sx < 0) {
                            addx++;
                        } else {
                            addx--;
                        }
                        moved = false;
                    }
                } while (moved);
            } else if (ny - sy != 0) {
                do {
                    if (ny - sy < 0) {
                        addy--;
                    } else {
                        addy++;
                    }
                    if (gridManager.grid.cellArray[nx, ny + addy].GetComponent<Cell>().CollisionMode1 > 0) {
                        moved = true;
                        if (gridManager.grid.cellArray[nx, ny + addy].GetComponent<Cell>().CollisionMode1 == Cell.CollisionMode.Trap) {
                            dmg++;
                            gridManager.grid.cellArray[nx, ny + addy].GetComponent<Cell>().life--;
                            if (gridManager.grid.cellArray[nx, ny + addy].GetComponent<Cell>().life <= 0) {
                                StartCoroutine(changeToFloor(nx, ny + addy));
                            }
                        }
                    } else {
                        if (ny - sy < 0) {
                            addy++;
                        } else {
                            addy--;
                        }
                        moved = false;
                    }
                } while (moved);
            }
            sound.PlayIceSlip();
            Enemies[EnemyIndex].transform.DOMove(gridManager.grid.GetWorldPosition(nx + addx, ny + addy), 0.5f * (Mathf.Abs(addx + addy))).OnComplete(starting1);
            if(dmg>0){
                sound.PlayHurt();
                Enemies[EnemyIndex].GetComponent<EnemyScript>().takeDamage(dmg);
            }
        } else {
            if (Enemies[EnemyIndex].GetComponent<EnemyScript>().currSteps < Enemies[EnemyIndex].GetComponent<EnemyScript>().steps) starting = true;
        }
    }

    IEnumerator changeToFloor(int x, int y) {
        yield return new WaitForSeconds(0.5f);
        sound.PlayHurt();
        gridManager.grid.cellArray[x, y].GetComponent<SpriteRenderer>().sprite = floor;
        gridManager.grid.cellArray[x, y].GetComponent<Cell>().CollisionMode1 = Cell.CollisionMode.Floor;
    }
    void starting1() {
        if (Enemies[EnemyIndex].GetComponent<EnemyScript>().currSteps < Enemies[EnemyIndex].GetComponent<EnemyScript>().steps) starting = true;
    }
    IEnumerator endTurn(bool b) {
        yield return new WaitForSeconds(0.8f);
        playerTurn = !b;
        if (playerTurn) {
            //wall decay
            foreach (GameObject g in AddedWalls) {
                g.GetComponent<Cell>().life--;
                if (g.GetComponent<Cell>().life <= 0) {
                    int x = 0, y = 0;
                    x = Mathf.RoundToInt(g.transform.position.x / (gridManager.spacing * (float)gridManager.spriteSize / 100));
                    y = Mathf.RoundToInt(-g.transform.position.y / (gridManager.spacing * (float)gridManager.spriteSize / 100));
                    gridManager.grid.cellArray[x, y].GetComponent<SpriteRenderer>().sprite = floor;
                    gridManager.grid.cellArray[x, y].GetComponent<Cell>().CollisionMode1 = Cell.CollisionMode.Floor;
                }
            }
            //visuals for enemies spawning
            state = State.cardPick;
            ResetMana();
        } else {
            EndTurnButton.SetActive(false);
            starting = true;
            startingForReal = true;
            ending = false;
            togglePlayable(false);
        }
    }

    public void endTurn1(bool b) {
        playerTurn = !b;
        if (playerTurn) {
            state = State.cardPick;
            ResetMana();
            //wall decay
            List<GameObject> gg = new List<GameObject>();
            foreach (GameObject g in AddedWalls) {
                g.GetComponent<Cell>().life--;
                if (g.GetComponent<Cell>().life <= 0) {
                    int x = 0, y = 0;
                    x = Mathf.RoundToInt(g.transform.position.x / (gridManager.spacing * (float)gridManager.spriteSize / 100));
                    y = Mathf.RoundToInt(-g.transform.position.y / (gridManager.spacing * (float)gridManager.spriteSize / 100));
                    gridManager.grid.cellArray[x, y].GetComponent<SpriteRenderer>().sprite = floor;
                    gridManager.grid.cellArray[x, y].GetComponent<Cell>().CollisionMode1 = Cell.CollisionMode.Floor;
                    gg.Add(g);
                }
            }
            foreach (GameObject g in gg) {
                AddedWalls.Remove(g);
            }
            //visuals for enemies spawning
        } else {
            EndTurnButton.SetActive(false);
            starting = true;
            startingForReal = true;
            ending = false;
            togglePlayable(false);
        }
    }

    public void togglePlayable(bool b) {
        GameObject deck = GameObject.FindGameObjectWithTag("Deck");
        deck.GetComponent<CardPanelSliding>().enabled = b;
        for (int i = 0; i < deck.transform.childCount; i++) {
            deck.transform.GetChild(i).GetComponent<PlayCard>().enableddd = b;
        }
    }

    public void ResetMana() {
        int diff = maxMana - Mana;
        Mana = maxMana;
        for (int i = 0; i < diff; i++) {
            Instantiate(ManaObj, ManaContainer);
        }
    }
    public void cardPicked() {
        for (int i = pickCardPanel.transform.GetChild(0).childCount - 1; i >= 0; i--) {
            Destroy(pickCardPanel.transform.GetChild(0).GetChild(i).gameObject);
        }
        pickCardPanel.SetActive(false);
        state = State.CardPlay;
        picking = false;
        EndTurnButton.SetActive(true);
        togglePlayable(true);
    }
    public bool cast(int cost) {
        if (cost <= Mana) {
            for (int i = 0; i < cost; i++) {
                Destroy(ManaContainer.GetChild(Mana - 1).gameObject);
                Mana--;
            }
            return true;
        } else {
            return false;
        }
    }

    public void LoadMenu() {
        GetComponent<AudioSource>().pitch = Random.Range(0.95f, 1.05f);
        GetComponent<AudioSource>().Play();
        SceneManager.LoadScene(0);
    }
}
