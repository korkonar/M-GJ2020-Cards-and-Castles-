using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject helpPanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadNormal(){
        GetComponent<AudioSource>().pitch=Random.Range(0.95f,1.05f);
        GetComponent<AudioSource>().Play();
        SceneManager.LoadScene(1);
    }

    public void LoadEndless(){
        GetComponent<AudioSource>().pitch=Random.Range(0.95f,1.05f);
        GetComponent<AudioSource>().Play();
        SceneManager.LoadScene(2);
    }

    public void Help(){
        GetComponent<AudioSource>().pitch=Random.Range(0.95f,1.05f);
        GetComponent<AudioSource>().Play();
        if(!helpPanel.activeSelf){
            helpPanel.SetActive(true);
        }else{
            helpPanel.SetActive(false);
        }
    }
}
