using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerRespawn : MonoBehaviour 
{
    public Transform prefab;
    public Image arrow;
    public Text text;
    [HideInInspector]
    public float timer;
    [HideInInspector]
    public float deathPositionX;
    public float deathCount;
    public bool Respawning
    {
        get
        {
            return respawning;
        }
        set
        {
            if (value == true)
            {
                if (!inactive)
                {
                    arrow.gameObject.SetActive(true);
                    text.text = Mathf.RoundToInt(timer).ToString();
                    respawning = value;
                }
                else
                {
                    respawning = !value;
                }
            }
            else
            {
                respawning = value;
                arrow.gameObject.SetActive(false);
            }
        }
    }
    private bool respawning;
    public bool inactive;

    public bool AllowRespawn(float newRespawnTime)
    {
        if (inactive)
        {
            timer = newRespawnTime;
            Respawning = false;
            return false;
        }

        if (timer <= 0)
        {
            timer = newRespawnTime;
            Respawning = false;
            return true;
        }
        return false;
    }

    public void UpdateRespawn(float targetX)
    {
        if (Respawning)
        {
            timer -= Time.deltaTime;
            text.text = Mathf.RoundToInt(timer).ToString();

            Vector3 worldpos = Camera.main.ScreenToWorldPoint(arrow.rectTransform.position);
            worldpos.x = Mathf.Lerp(worldpos.x, targetX, Time.deltaTime);
            arrow.rectTransform.position = Camera.main.WorldToScreenPoint(worldpos);
        }
    }
}
