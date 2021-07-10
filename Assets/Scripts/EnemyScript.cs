using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyScript : MonoBehaviour
{
    GameManager gameManager;
    public int health;
    public Slider healthSlider;
    public int steps;
    public int currSteps;
    public int spawnTurn;
    // Start is called before the first frame update
    void Start()
    {
        healthSlider.maxValue=health;
        healthSlider.value=health;
        gameManager=GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void takeDamage(int amount){
        health-=amount;
        if(health<=0){
            health=0;
            Destroy(this.gameObject,0.5f);
            gameManager.SpawnedEnemies--;
            gameManager.Enemies.Remove(this.gameObject);
            gameManager.starting=true;
        }
        healthSlider.value=health;
        healthSlider.gameObject.SetActive(true);
        StartCoroutine(disableSlider(1.0f));
    }
    IEnumerator disableSlider(float sec){
        yield return new WaitForSeconds(sec);
        healthSlider.gameObject.SetActive(false);
        yield return null;
        
    }
}
