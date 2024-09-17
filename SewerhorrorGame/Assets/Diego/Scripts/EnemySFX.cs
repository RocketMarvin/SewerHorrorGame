using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySFX : MonoBehaviour
{
    public AudioClip[] breathingSounds, smallBreathingSounds;
    public AudioSource bigRoar, sniffing;
    public AudioSource selectedBreath;

    private int randomNum;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RandomBreathSound()
	{
        randomNum = Random.Range(0, breathingSounds.Length);
        selectedBreath.clip = breathingSounds[randomNum];
        selectedBreath.Play();
	}

    public void RandomBreathSoundSmall()
	{
        randomNum = Random.Range(0, breathingSounds.Length);
        selectedBreath.clip = smallBreathingSounds[randomNum];
        selectedBreath.Play();
    }

    public void BigRoarSound() => bigRoar.Play();

    public void SniffingStart() => sniffing.Play();
    public void SniffingPause() => sniffing.Pause();
    public void SniffingUnPause() => sniffing.UnPause();
}
