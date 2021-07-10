using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SoundManager : MonoBehaviour
{
    public int numSources;
    AudioSource[] sources;
    int sourceIndex;
    public AudioClip Win;
    public AudioClip Lose;
    public AudioClip Click;
    public AudioClip hurt;
    public AudioClip icePlace;
    public AudioClip IceSlip;
    public AudioClip Push;
    public AudioClip TrapPlace;
    public AudioClip WallPlace;
    public AudioClip WallDestroy;
    // Start is called before the first frame update
    void Start()
    {
        sources=new AudioSource[numSources];
        for(int i=0;i<numSources;i++){
            sources[i]=this.gameObject.AddComponent<AudioSource>();
            sources[i].loop=false;
            sources[i].volume=0.6f;
        }
        sourceIndex=0;
    }

    public void PlayHurt(){
        sources[sourceIndex].clip=hurt;
        sources[sourceIndex].pitch=Random.Range(0.85f,1.1f);
        sources[sourceIndex++].Play();
        testIndexReset();
    }

    public void PlayOver(){
        sources[sourceIndex].clip=Lose;
        sources[sourceIndex].pitch=Random.Range(0.85f,1.1f);
        sources[sourceIndex++].Play();
        testIndexReset();
    }

    public void PlayWin(){
        sources[sourceIndex].clip=Win;
        sources[sourceIndex].pitch=Random.Range(0.85f,1.1f);
        sources[sourceIndex++].Play();
        testIndexReset();
    }

    public void PlayClick(){
        sources[sourceIndex].clip=Click;
        sources[sourceIndex].pitch=Random.Range(0.85f,1.1f);
        sources[sourceIndex++].Play();
        testIndexReset();
    }

    public void PlayIcePlace(){
        sources[sourceIndex].clip=icePlace;
        sources[sourceIndex].pitch=Random.Range(0.85f,1.1f);
        sources[sourceIndex++].Play();
        testIndexReset();
    }

    public void PlayIceSlip(){
        sources[sourceIndex].clip=IceSlip;
        sources[sourceIndex].pitch=Random.Range(0.85f,1.1f);
        sources[sourceIndex++].Play();
        testIndexReset();
    }

    public void PlayPush(){
        sources[sourceIndex].clip=Push;
        sources[sourceIndex].pitch=Random.Range(0.85f,1.1f);
        sources[sourceIndex++].Play();
        testIndexReset();
    }

    public void PlayTrapPlace(){
        sources[sourceIndex].clip=TrapPlace;
        sources[sourceIndex].pitch=Random.Range(0.85f,1.1f);
        sources[sourceIndex++].Play();
        testIndexReset();
    }

    public void PlayWallDestroy(){
        sources[sourceIndex].clip=WallDestroy;
        sources[sourceIndex].pitch=Random.Range(0.85f,1.1f);
        sources[sourceIndex++].Play();
        testIndexReset();
    }

    public void PlayWallPlace(){
        sources[sourceIndex].clip=WallPlace;
        sources[sourceIndex].pitch=Random.Range(0.85f,1.1f);
        sources[sourceIndex++].Play();
        testIndexReset();
    }

    void testIndexReset(){
        if(sourceIndex>=numSources)sourceIndex=0;
    }
}
