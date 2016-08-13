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

    private GameManager gameManager;

    void Start()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }
    public void ExplodeBig()
    {
        bigExplosion.Play();
        for (int i = 0; i < gameManager.frogsReady.Length; i++)
        {
            if (gameManager.frogsReady[i])
                Vibration.instance.SetVibration(i, 1f, 1f, 1f);
        }
    }

    void Update()
    {
        if (duration > 0)
        {
            duration -= Time.deltaTime;
            if (Random.Range(0.0f, 1.0f) < .03f)
            {
                fireworkSounder.PlayOneShot(fireworks[GetRandSoundIndex()]);
            }

            if (duration < 1)
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
