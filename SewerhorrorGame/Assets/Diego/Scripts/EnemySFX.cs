using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySFX : MonoBehaviour
{
    public AudioClip[] breathingSounds;
    public AudioSource bigRoar;
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

    public void BigRoarSound() => bigRoar.Play();
}
