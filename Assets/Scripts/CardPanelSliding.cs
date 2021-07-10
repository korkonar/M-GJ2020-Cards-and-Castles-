using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

public class CardPanelSliding : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler
{
    GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager=GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(this.transform.position.y==0.0f){
            this.transform.DOMoveY(this.transform.position.y+100,0.5f);
            this.transform.DOScale(1.2f,0.5f);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(this.transform.position.y==100.0f){
            this.transform.DOMoveY(this.transform.position.y-100,0.5f);
            this.transform.DOScale(1.2f,0.5f);
        }
    }
}
