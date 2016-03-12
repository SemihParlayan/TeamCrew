using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerRespawn : MonoBehaviour 
{
    public Transform prefab;
    public Image arrow;
    private Text text;
    [HideInInspector]
    public float timer;
    //[HideInInspector]
    public float deathPositionX;
    [HideInInspector]
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
    public bool respawning;
    [HideInInspector]
    public bool inactive;
    public bool allowRespawn = true;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        text = arrow.transform.GetChild(0).GetComponent<Text>();
    }

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

    public void DisableRespawn(float howLong)
    {
        allowRespawn = false;
        Invoke("EnableRespawn", howLong);
    }
    private void EnableRespawn()
    {
        allowRespawn = true;
    }
    public void UpdateRespawnArrowPosition(float targetX)
    {
        timer -= Time.deltaTime;
        text.text = Mathf.RoundToInt(timer).ToString();

        Vector3 worldpos = cam.ScreenToWorldPoint(arrow.rectTransform.position);
        worldpos.x = Mathf.Lerp(worldpos.x, targetX, Time.deltaTime);
        arrow.rectTransform.position = cam.WorldToScreenPoint(worldpos);
    }
}
