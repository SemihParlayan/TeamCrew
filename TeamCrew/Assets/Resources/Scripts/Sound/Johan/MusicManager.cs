using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


[System.Serializable]
public class MusicLayer
{
    [HideInInspector]
    public string name;

    public string instrument;
    public AudioClip audioClip;
    public AudioMixerGroup audioGroup;
    [Range(0, 100)]
    public int start = 0;
    [Range(0, 100)]
    public int end = 0;
    public bool requireTutorialComplete = true;

    private AudioSource m_audioSource;
    private float goalVolume;
    private float fadeSpeed;

    public void Initialize(MusicManager musicManager, float fadeTime)
    {
        m_audioSource = musicManager.gameObject.AddComponent<AudioSource>();
        m_audioSource.clip = audioClip;
        m_audioSource.outputAudioMixerGroup = audioGroup;
        m_audioSource.loop = true;
        m_audioSource.volume = 0.0f;
        m_audioSource.Play();
        fadeSpeed = 1.0f / fadeTime;
        goalVolume = 0.0f;
    }

    public void SetGoalVolume(float goalVolume)
    {
        this.goalVolume = goalVolume;
    }

    public void UpdateLayer(bool forceQuit, float percentageClimbed, bool tutorialComplete)
    {
        //Is percentage withing play interval?
        if (percentageClimbed >= start && percentageClimbed <= end)
        {
            SetGoalVolume(1f);
        }
        else
        {
            SetGoalVolume(0f);
        }

        //Is tutorial required and finished?
        if (requireTutorialComplete && !tutorialComplete)
            SetGoalVolume(0f);

        //Is game active?
        if (forceQuit)
            SetGoalVolume(0f);

        if (Mathf.Abs(m_audioSource.volume - goalVolume) > 0.01f)
        {
            float direction = Mathf.Sign(goalVolume - m_audioSource.volume);
            m_audioSource.volume += fadeSpeed * direction * Time.deltaTime;
        }
    }
}

public class MusicManager : MonoBehaviour
{
    public bool debugMode;
    public bool debugTutorialComplete;

    public M_Sounds menuMusicManager;
	public List<MusicLayer> musicLayers;
	public float defaultFadeTime = 1.0f;
    public float percentageClimbed = 0.0f;

    private GameManager gameManager;

	void Awake()
	{
        menuMusicManager = GameObject.FindObjectOfType<M_Sounds>();
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

        for (int i = 0; i < musicLayers.Count; i++) 
        {
            musicLayers[i].Initialize(this, defaultFadeTime);
		}
	}

	void Update()
	{
        if (!debugMode)
            percentageClimbed = GameManager.GetClimbedHeight() * 100;

        for (int i = 0; i < musicLayers.Count; i++)
        {
            bool tutorialComplete = (debugMode) ? debugTutorialComplete : gameManager.tutorialComplete;
            if (percentageClimbed > 30)
                tutorialComplete = true;

            bool forceQuit = (debugMode) ? false : menuMusicManager.playMenuMusic;
            musicLayers[i].UpdateLayer(forceQuit, percentageClimbed, tutorialComplete);
        }
	}

    void OnValidate()
    {
        if (Application.isPlaying)
            return;
        foreach (MusicLayer m in musicLayers)
        {
            m.name = m.instrument + " |  % " + m.start + " - " + m.end + " | Tutorial Complete: " + m.requireTutorialComplete;
        }
    }
}
































