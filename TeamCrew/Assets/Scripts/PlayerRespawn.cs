using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerRespawn : MonoBehaviour 
{
    public Transform prefab;
    public Image arrow;
    public Text text;
    public float timer;
    public float deathPositionX;
    public bool Respawning
    {
        get
        {
            return respawning;
        }
        set
        {
            respawning = value;
            if (value == true)
            {
                arrow.gameObject.SetActive(true);
                text.text = Mathf.RoundToInt(timer).ToString();
            }
            else
            {
                arrow.gameObject.SetActive(false);
            }
        }
    }
    private bool respawning;

    public bool AllowRespawn(float newRespawnTime)
    {
        bool allow = (timer <= 0 && Respawning);

        if (allow)
        {
            timer = newRespawnTime;
            Respawning = false;
        }

        return allow;
    }

    public void UpdateRespawn(float targetX)
    {
        if (Respawning)
        {
            timer -= Time.deltaTime;
            text.text = timer.ToString("F1");

            Vector3 worldpos = Camera.main.ScreenToWorldPoint(arrow.rectTransform.position);
            worldpos.x = Mathf.Lerp(worldpos.x, targetX, Time.deltaTime);
            arrow.rectTransform.position = Camera.main.WorldToScreenPoint(worldpos);
        }
    }
}
