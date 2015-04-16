using UnityEngine;
using System.Collections;

public class SoundMaster : MonoBehaviour {

    public AudioSource[] List;
	void Start () {
	
	}
	
	public AudioSource GetSource(int i)
    {
        if (i < List.Length) return List[i];

        Debug.Log("This message means something went wrong");
        return null;
    }
}
