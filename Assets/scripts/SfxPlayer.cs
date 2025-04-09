using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxPlayer : MonoBehaviour
{
    public AudioSource src;
    public AudioClip sfx1, sfx2, sfx3;

    public void button1()
    {
        src.clip = sfx1;
        src.Play();
    }
    public void button2()
    {
        src.clip = sfx2;
        src.Play();
    }

    public void button3()
    {
        src.clip = sfx3;
        src.Play();
    }


}



