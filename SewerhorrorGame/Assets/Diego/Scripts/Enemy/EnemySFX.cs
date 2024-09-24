using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySFX : MonoBehaviour
{
    public AudioClip[] breathingSounds, smallBreathingSounds;
    public AudioClip[] walkingSounds;
    public AudioSource bigRoar, sniffing;
    public AudioSource selectedBreath, selectedWalk;

    private int randomNum, randomNum_Walk;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		if (!Enemy.isPlayingSound)
		{
            bigRoar.gameObject.SetActive(true);
            sniffing.gameObject.SetActive(true);
            selectedBreath.gameObject.SetActive(true);
		}
		else
		{
            bigRoar.gameObject.SetActive(false);
            sniffing.gameObject.SetActive(false);
            selectedBreath.gameObject.SetActive(false);
		}
    }

    public void WalkingSound()
	{
        randomNum_Walk = Random.Range(0, walkingSounds.Length);
        selectedWalk.clip = walkingSounds[randomNum_Walk];
        selectedWalk.Play();
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
