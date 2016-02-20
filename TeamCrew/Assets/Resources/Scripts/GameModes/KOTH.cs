using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[System.Serializable]
public class ScoreKeeper
{
    [HideInInspector]
    public Color frogColor;
    [HideInInspector]
    public Color glowColor;
    public Image image;
    public float targetScore;
    public float score;
    public float percent;
    public bool active;
}
public class KOTH : MonoBehaviour 
{
    //Public
    public float height = 4f;
    public float bottomOffset = 0f;
    public int scoreIncreaseAmount = 100;
    public Transform KOTHParent;
    public TextMesh scoreAdditionPrefab;
    public ScoreKeeper[] keepers;

    //Private
    private GameManager gameManager;
    private bool glowing;
    private float scoreTimer;

    void Awake()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

        scoreAdditionPrefab.transform.GetChild(0).GetComponent<TrailRenderer>().sortingLayerName = "Text";
        scoreAdditionPrefab.transform.GetChild(0).GetComponent<TrailRenderer>().sortingOrder = 50;

        scoreAdditionPrefab.GetComponent<MeshRenderer>().sortingLayerName = "Text";
        scoreAdditionPrefab.GetComponent<MeshRenderer>().sortingOrder = 100;
    }

    void Update()
    {
        if (!gameManager.gameActive || !gameManager.tutorialComplete)
            return;

        IncreaseTopFrogScore();
        Glow();
    }

    public void OnGameStart()
    {
        InvokeRepeating("Pulse", 0.0f, 0.75f);
        KOTHParent.gameObject.SetActive(enabled);
    }
    public void DisableKeepers()
    {
        CancelInvoke();
        KOTHParent.gameObject.SetActive(false);
    }

    private void IncreaseTopFrogScore()
    {
        Transform topFrog = GameManager.GetTopFrog();
        if (topFrog == null)
            return;

        FrogPrototype frog = topFrog.GetComponent<FrogPrototype>();
        if (frog == null)
            return;

        scoreTimer += Time.deltaTime;
        if (scoreTimer >= 2.0f)
        {
            scoreTimer -= 2.0f;
            IncreaseScore(frog.player);
        }

        UpdateColumns();
    }
    private void IncreaseScore(int player)
    {
        keepers[player].targetScore += scoreIncreaseAmount;

        //Spawn text prefab
        Vector3 spawnPos = Vector3.zero;
        FrogPrototype frog = gameManager.playerScripts[player];
        if (frog !=null)
        {
            spawnPos = frog.transform.position;
        }


        scoreAdditionPrefab.color = keepers[player].frogColor;
        scoreAdditionPrefab.text = scoreIncreaseAmount.ToString();
        GameObject o = Instantiate(scoreAdditionPrefab.gameObject, spawnPos, Quaternion.identity) as GameObject;

        int xDir = (Random.Range(0, 2) == 0) ? -1 : 1;
        o.GetComponent<Rigidbody2D>().AddForce(new Vector2(xDir * 150, 1 * 300));
    }

    private void Pulse()
    {
        glowing = !glowing;
    }
    private void Glow()
    {
        ScoreKeeper topKeeper = null;
        float maxScore = -1;
        for (int i = 0; i < keepers.Length; i++)
        {
            if (keepers[i].targetScore > maxScore)
            {
                maxScore = keepers[i].targetScore;
                topKeeper = keepers[i];
            }
        }

        for (int i = 0; i < keepers.Length; i++)
        {
            ScoreKeeper keeper = keepers[i];
            if (keeper == topKeeper && glowing)
            {
                keeper.image.color = Color.Lerp(keeper.image.color, keeper.glowColor, Time.deltaTime * 7.0f);
            }
            else
            {
                keeper.image.color = Color.Lerp(keeper.image.color, keeper.frogColor, Time.deltaTime * 7.0f);
            }
        }
    }
    private void UpdateColumns()
    {
        float totalScore = 0;
        for (int i = 0; i < keepers.Length; i++)
        {
            if (!keepers[i].active)
                continue;

            keepers[i].score = Mathf.MoveTowards(keepers[i].score, keepers[i].targetScore, Time.deltaTime * 500);
        }

        for (int i = 0; i < gameManager.frogsReady.Length; i++)
        {
            keepers[i].active = gameManager.frogsReady[i];
            if (keepers[i].active)
            {
                keepers[i].image.gameObject.SetActive(true);
                totalScore += keepers[i].score;
            }
            else
            {
                keepers[i].image.gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < keepers.Length; i++)
        {
            if (!keepers[i].active)
                continue;

            float percent = 0;
            if (totalScore > 0)
                percent = (float)keepers[i].score / (float)totalScore;

            keepers[i].percent = percent;

            Vector3 scale = keepers[i].image.rectTransform.localScale;
            scale.y = height * keepers[i].percent;
            keepers[i].image.rectTransform.localScale = scale;
        }


        Image prev = null;
        int startIndex = 0;
        for (int i = 0; i < keepers.Length; i++)
        {
            if (!keepers[i].active)
                continue;

            startIndex = i;
            prev = keepers[i].image;
            break;
        }

        float y = bottomOffset;
        SetRectY(prev.rectTransform, y);
        for (int i = startIndex + 1; i < keepers.Length; i++)
        {
            if (!keepers[i].active)
                continue;

            y += prev.rectTransform.localScale.y * 100;
            SetRectY(keepers[i].image.rectTransform, y);

            prev = keepers[i].image;

        }
    }

    private void SetRectY(RectTransform rect, float y)
    {
        Vector3 pos = rect.localPosition;
        pos.y = y;
        rect.localPosition = pos;
    }

    void OnValidate()
    {
        keepers[0].targetScore = Mathf.Clamp(keepers[0].targetScore, 1, float.MaxValue);
        keepers[1].targetScore = Mathf.Clamp(keepers[1].targetScore, 1, float.MaxValue);
        keepers[2].targetScore = Mathf.Clamp(keepers[2].targetScore, 1, float.MaxValue);
        keepers[3].targetScore = Mathf.Clamp(keepers[3].targetScore, 1, float.MaxValue);
    }

}
