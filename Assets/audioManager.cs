
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class audioManager : MonoBehaviour
{
    
    public Audio[] audios;


    private void Awake()
    {
        foreach(Audio audi in audios)
        {
            audi.source = gameObject.AddComponent<AudioSource>();

            audi.source.loop = audi.loop;
            audi.source.volume = audi.volume;
            audi.source.pitch = audi.pitch;
            audi.source.clip = audi.audioClip;

        }
    }

    public void Play(string name)
    {

        Audio a = System.Array.Find(audios, audi => name == audi.audioName);

        if(a == null)
        {
            Debug.Log(name + " Not Found");
            return;
        }

        a.source.Play();

    }

    public void Stop(string name)
    {

        Audio a = System.Array.Find(audios, audi => name == audi.audioName);

        if (a == null)
        {
            Debug.Log(name + " Not Found");
            return;
        }

        a.source.Stop();

    }

}
