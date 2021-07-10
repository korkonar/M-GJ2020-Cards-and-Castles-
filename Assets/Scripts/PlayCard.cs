using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class PlayCard : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    GameManager gameManager;
    private Vector3 origin;
    public CardType type;
    public int cost;
    public int life;
    public Sprite ObsSprite;
    public Sprite CardSprite;
    public bool enableddd=true;
    public enum CardType{
        Wall,
        Spike,
        Ice,
        Push
    }
    // Start is called before the first frame update
    void Start()
    {
        gameManager=GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerDown(PointerEventData eventData){
        if(enabled){
            //print("down");
            origin=this.transform.position;
            this.transform.DOScale(1,0.5f);
            this.GetComponent<Image>().sprite=ObsSprite;
            gameManager.gridManager.grid.ToggleCollision();
        }
    }
    public void OnDrag(PointerEventData eventData){
        if(enabled){
            //print("drag");
            this.transform.position=Input.mousePosition;
            int x,y;
            x=Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).x/(gameManager.gridManager.spacing * (float)gameManager.gridManager.spriteSize / 100));
            y=Mathf.RoundToInt(-Camera.main.ScreenToWorldPoint(Input.mousePosition).y/(gameManager.gridManager.spacing * (float)gameManager.gridManager.spriteSize / 100));
            //print(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            //if(gameManager.gridManager.dimensions.x > x && x>=0 && gameManager.gridManager.dimensions.y > y && y>=0 ){
            //    print(x+", "+y+":"+gameManager.gridManager.grid.cellArray[x,y].GetComponent<Cell>().CollisionMode1);
            //}
        }
    }
    public void OnPointerUp(PointerEventData eventData){
        if(enabled){
            //print("up");
            int x,y;
            x=Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).x/(gameManager.gridManager.spacing * (float)gameManager.gridManager.spriteSize / 100));
            y=Mathf.RoundToInt(-Camera.main.ScreenToWorldPoint(Input.mousePosition).y/(gameManager.gridManager.spacing * (float)gameManager.gridManager.spriteSize / 100));
            this.transform.position=origin;
            if(gameManager.gridManager.dimensions.x > x && x>=0 && gameManager.gridManager.dimensions.y > y && y>=0 ){
                if(gameManager.Mana>=cost){
                    switch(type){
                        case CardType.Wall:
                            if(gameManager.gridManager.grid.cellArray[x,y].GetComponent<Cell>().CollisionMode1==Cell.CollisionMode.Floor){
                                gameManager.cast(cost);
                                gameManager.gridManager.grid.cellArray[x,y].GetComponent<Cell>().CollisionMode1=Cell.CollisionMode.Wall;
                                gameManager.gridManager.grid.cellArray[x,y].GetComponent<SpriteRenderer>().sprite=ObsSprite;
                                gameManager.gridManager.grid.cellArray[x,y].GetComponent<Cell>().life=life;
                                gameManager.AddedWalls.Add(gameManager.gridManager.grid.cellArray[x,y]);
                                gameManager.sound.PlayWallPlace();
                                Destroy(this.gameObject,0.05f);
                            }
                        break;
                        case CardType.Spike:
                            if(gameManager.gridManager.grid.cellArray[x,y].GetComponent<Cell>().CollisionMode1==Cell.CollisionMode.Floor){
                                gameManager.cast(cost);
                                gameManager.gridManager.grid.cellArray[x,y].GetComponent<Cell>().CollisionMode1=Cell.CollisionMode.Trap;
                                gameManager.gridManager.grid.cellArray[x,y].GetComponent<Cell>().life=life;
                                gameManager.gridManager.grid.cellArray[x,y].GetComponent<SpriteRenderer>().sprite=ObsSprite;
                                gameManager.sound.PlayTrapPlace();
                                Destroy(this.gameObject,0.05f);
                            }
                        break;
                         case CardType.Ice:
                            if(gameManager.gridManager.grid.cellArray[x,y].GetComponent<Cell>().CollisionMode1==Cell.CollisionMode.Floor){
                                gameManager.cast(cost);
                                gameManager.gridManager.grid.cellArray[x,y].GetComponent<Cell>().CollisionMode1=Cell.CollisionMode.Ice;
                                gameManager.gridManager.grid.cellArray[x,y].GetComponent<Cell>().life=life;
                                gameManager.gridManager.grid.cellArray[x,y].GetComponent<SpriteRenderer>().sprite=ObsSprite;
                                gameManager.sound.PlayIcePlace();
                                Destroy(this.gameObject,0.05f);
                            }
                        break;
                        case CardType.Push:
                            gameManager.cast(cost);
                            gameManager.sound.PlayPush();
                            for(int i=0;i<gameManager.Enemies.Count;i++){
                                int ex=Mathf.RoundToInt(gameManager.Enemies[i].transform.position.x/(gameManager.gridManager.spacing * (float)gameManager.gridManager.spriteSize / 100));
                                int ey=Mathf.RoundToInt(-gameManager.Enemies[i].transform.position.y/(gameManager.gridManager.spacing * (float)gameManager.gridManager.spriteSize / 100));
                                if(x+1==ex && y==ey){
                                    if(gameManager.gridManager.grid.cellArray[x+2,y].GetComponent<Cell>().CollisionMode1>0){
                                        gameManager.Enemies[i].transform.DOMove(gameManager.gridManager.grid.GetWorldPosition(x+2,y),0.3f);
                                        gameManager.ActOnEnemy(ex,ey,ex+1,ey);
                                    }
                                }
                                if(x-1==ex && y==ey){
                                    if(gameManager.gridManager.grid.cellArray[x-2,y].GetComponent<Cell>().CollisionMode1>0){
                                        gameManager.Enemies[i].transform.DOMove(gameManager.gridManager.grid.GetWorldPosition(x-2,y),0.3f);
                                        gameManager.ActOnEnemy(ex,ey,ex-1,ey);
                                    }
                                }
                                if(x==ex && y+1==ey){
                                    if(gameManager.gridManager.grid.cellArray[x,y+2].GetComponent<Cell>().CollisionMode1>0){
                                        gameManager.Enemies[i].transform.DOMove(gameManager.gridManager.grid.GetWorldPosition(x,y+2),0.3f);
                                        gameManager.ActOnEnemy(ex,ey,ex,ey+1);
                                    }
                                }
                                if(x==ex && y-1==ey){
                                    if(gameManager.gridManager.grid.cellArray[x,y-2].GetComponent<Cell>().CollisionMode1>0){
                                        gameManager.Enemies[i].transform.DOMove(gameManager.gridManager.grid.GetWorldPosition(x,y-2),0.3f);
                                        gameManager.ActOnEnemy(ex,ey,ex,ey-1);
                                    }
                                }
                            }
                            Destroy(this.gameObject,0.05f);
                        break;
                    }
                    //gameManager.gridManager.grid.ToggleCollision();

                }
            }
            gameManager.gridManager.grid.ToggleCollision();
            this.GetComponent<Image>().sprite=CardSprite;
            this.transform.DOScale(2,0.5f);
        }
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(this.transform.localScale.y==2.0f){
            this.transform.DOScale(3,0.2f);
            this.transform.DOLocalMoveY(transform.localPosition.y+50,0.2f);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(this.transform.localScale.y==3.0f){
            this.transform.DOScale(2,0.2f);
            this.transform.DOLocalMoveY(transform.localPosition.y-50,0.2f);
        }
    }

}
