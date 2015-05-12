using UnityEngine;
using System.Collections;

public class Fireworks : MonoBehaviour
{
    public AudioSource fireworkSounder;
    public AudioSource applaudes;
    public AudioSource fireworkBackgroundSounder;

    public AudioClip[] fireworks;
    public float duration = 10.0f;

	void Start ()
    {
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
        else
        {
            //this.enabled = false;
            enabled = false;
        }
	}
    int GetRandSoundIndex()
    { 
        int coolNum = Random.Range(0, fireworks.Length - 1);
        Debug.Log(coolNum);
        return coolNum;
    }
        
}
