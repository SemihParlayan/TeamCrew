using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Level
{
    [HideInInspector]
    public string name = "Level";
    [Range(0f, 1f)]
    public float activationPercent;

    public Level(string name)
    {
        this.name = name;
    }
}

[System.Serializable]
public class LayerData
{
    public int[] playOnLevels;
    public AudioClip audioClip;
}

public class MusicLayer
{
    public AudioSource m_audioSource;
    public bool m_isActive;
    public float m_goalVolume;
    public int[] m_playOnLevels;
    float m_fadeSpeed;


    public MusicLayer(MusicManager musicManager, float fadeTime)
    {
        m_audioSource = musicManager.gameObject.AddComponent<AudioSource>();
        m_audioSource.loop = true;
        m_audioSource.volume = 0.0f;
        m_isActive = false;
        m_fadeSpeed = 1.0f / fadeTime;
    }

    public void SetGoalVolume(float goalVolume)
    {
        m_goalVolume = goalVolume;
    }

    public void UpdateLayer()
    {
        if (Mathf.Abs(m_audioSource.volume - m_goalVolume) > 0.01f)
        {
            float direction = Mathf.Sign(m_goalVolume - m_audioSource.volume);
            m_audioSource.volume += m_fadeSpeed * direction * Time.deltaTime;
        }
    }

    public bool ShouldPlay(int level)
    {
        for (int i = 0; i < m_playOnLevels.Length; i++)
        {
            if (m_playOnLevels[i] == level)
                return true;
        }
        return false;
    }
}

public class MusicManager : MonoBehaviour
{
	public float m_defaultFadeTime = 1.0f;
	public LayerData[] m_layerData;
    public List<Level> levels;

	private List<MusicLayer> m_musicLayers;
    private GameManager gameManager;
    public int m_currentLevel = -1;
    public float percentageClimbed = 0.0f;
	

	void Start()
	{
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
		m_musicLayers = new List<MusicLayer> ();
		for (int i = 0; i < m_layerData.Length; i++) {
			MusicLayer musicLayer = new MusicLayer(this, m_defaultFadeTime);
			musicLayer.m_audioSource.clip = m_layerData[i].audioClip;
			musicLayer.m_playOnLevels = m_layerData[i].playOnLevels;
			m_musicLayers.Add(musicLayer);
		}

		for (int i = 0; i < m_musicLayers.Count; i++)
		{
			m_musicLayers[i].m_audioSource.Play();
			m_musicLayers[i].m_isActive = false;
		}
	}

	void SetLevel(int level)
	{
		if (level <= m_currentLevel || level > (m_musicLayers.Count - 1)) // ändra till "==" om musiken ska gå tillbaka till ett lägre lager
			return;

        if (level < 0)
        {
            for (int i = 0; i < m_musicLayers.Count; i++)
            {
                m_musicLayers[i].SetGoalVolume(0.0f);
                m_musicLayers[i].m_isActive = false;
            }
            return;
        }

        Debug.Log("Setting level " + level);
		for (int i = 0; i < m_musicLayers.Count; i++)
		{
			MusicLayer musicLayer = m_musicLayers[i];
			if (musicLayer.ShouldPlay(level) && !musicLayer.m_isActive)
			{
				musicLayer.SetGoalVolume(1.0f);
				musicLayer.m_isActive = true;
			}
			else if (!musicLayer.ShouldPlay(level) && musicLayer.m_isActive)
			{
				musicLayer.SetGoalVolume(0.0f);
				musicLayer.m_isActive = false;
			}
		}
		m_currentLevel = level;
	}

	void Update()
	{
        percentageClimbed = GameManager.GetClimbedHeight();
		//TestMusic();

        int maxLevel = -1;
        for (int i = 0; i < m_musicLayers.Count; i++)
        {
            m_musicLayers[i].UpdateLayer();

            if (gameManager.gameActive && percentageClimbed > levels[i].activationPercent)
            {
                maxLevel = i;
            }
        }
        if (!gameManager.tutorialComplete || !gameManager.gameActive)
            maxLevel = 0;
        SetLevel(maxLevel);
	}

	void TestMusic()
	{
		if (Input.GetKeyDown (KeyCode.Alpha0)) 
			SetLevel(0);

		else if (Input.GetKeyDown (KeyCode.Alpha1)) 
			SetLevel(1);

		else if (Input.GetKeyDown(KeyCode.Alpha2))
			SetLevel(2);

		else if (Input.GetKeyDown(KeyCode.Alpha3))
			SetLevel(3);

		else if (Input.GetKeyDown(KeyCode.Alpha4))
			SetLevel(4);

		else if (Input.GetKeyDown(KeyCode.Alpha5))
			SetLevel(5);
	}

    public void SetPercentageClimbed(float value)
    {
        percentageClimbed = value;
    }

    void OnValidate()
    {
        while (levels.Count < m_layerData.Length)
        {
            levels.Add(new Level("Level: " + levels.Count));
        }
        while (levels.Count > m_layerData.Length)
        {
            levels.Remove(levels.Last());
        }
    }
}
































