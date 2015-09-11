using UnityEngine;
using System.Collections;
using System.Linq;

public class SoundMaster : MonoBehaviour {

    public AudioSource[] List;

    Object[] soundComponents;

    public float maxVolume
    {
        set
        {
            for (int i = 0; i < soundComponents.Length; i++)
            {

            }
        }

        get
        {
            return 1;
        }
    }

	void Start () {

        //Add all sound scripts to list
        soundComponents = soundComponents.Concat(FindObjectsOfType(typeof(FallSound))).ToArray();
        soundComponents = soundComponents.Concat(FindObjectsOfType(typeof(FallSoundCam))).ToArray();
        soundComponents = soundComponents.Concat(FindObjectsOfType(typeof(FinalMusic))).ToArray();
        soundComponents = soundComponents.Concat(FindObjectsOfType(typeof(Fireworks))).ToArray();
        soundComponents = soundComponents.Concat(FindObjectsOfType(typeof(ImpactSound))).ToArray();
        soundComponents = soundComponents.Concat(FindObjectsOfType(typeof(MenuMusicController))).ToArray();
        soundComponents = soundComponents.Concat(FindObjectsOfType(typeof(RandomSoundFromList))).ToArray();
        soundComponents = soundComponents.Concat(FindObjectsOfType(typeof(VelocityPitch))).ToArray();
        soundComponents = soundComponents.Concat(FindObjectsOfType(typeof(VelocityVolume))).ToArray();
	}


    
	
	public AudioSource GetSource(int i)
    {
        if (i < List.Length) return List[i];

        Debug.Log("No audio source at specified index");
        return null;
    }
}
