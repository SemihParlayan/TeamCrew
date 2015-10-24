using UnityEngine;
using System.Collections;

public class Fireworks : MonoBehaviour
{
    public AudioSource fireworkSounder;
    public AudioSource applaudes;
    public AudioSource fireworkBackgroundSounder;

    public AudioSource bigExplosion;
    public AudioClip[] fireworks;
    public float duration = 10.0f;

    public void ExplodeBig()
    {
        bigExplosion.Play();
    }
	
	void Update ()
    {
        if(duration > 0){
            duration -= Time.deltaTime;
            if (Random.Range(0.0f, 1.0f) < .03f)
            {
                fireworkSounder.PlayOneShot(fireworks[GetRandSoundIndex()]);
            }

            if(duration < 1)
            {
                fireworkBackgroundSounder.volume = duration;
                applaudes.volume = duration;
            }
        }
	}
    int GetRandSoundIndex()
    { 
        int coolNum = Random.Range(0, fireworks.Length - 1);
        return coolNum;
    }

    public void Reset()
    {
        duration = 10;
    }
        
}
