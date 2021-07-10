using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class PickCard : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    GameManager gameManager;
    static int cardsPicked=0;
    // Start is called before the first frame update
    void Start()
    {
        gameManager=GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData){
        //print("clicked");
        gameManager.sound.PlayClick();
        this.transform.parent=GameObject.FindGameObjectWithTag("Deck").transform;
        this.transform.localScale= new Vector3(2,2,1);
        this.GetComponent<PlayCard>().enabled=true;
        cardsPicked++;
        if(cardsPicked>=2){
            gameManager.cardPicked();
            cardsPicked=0;
        }
        this.enabled=false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(this.transform.localScale.y==4.0f){
            this.transform.DOScale(6,0.2f);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(this.transform.localScale.y==6.0f){
            this.transform.DOScale(4,0.2f);
        }
    }
}
