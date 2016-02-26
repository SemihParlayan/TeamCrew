using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[System.Serializable]
public class ScoreKeeper
{
    [HideInInspector]
    public Color frogColor;
    public Image image;
    public Text scoreText;
    public Animator anim;
    public float targetScore;
    public float score;
    public float percent;
    public bool active;
}
public class KOTH : MonoBehaviour 
{
    //Public
    public float maximumBarWidth = 4f;
    public float minimumBarWidth = 0.75f;
    public float scoreIncreaseInterval = 0.25f;
    public int scoreIncreaseAmount = 10;

    [Range(0, 1f)]
    public float winningGripPercent;

    public Transform KOTHParent;
    public ParticleSystem particleSystem;
    public TextMesh scoreAdditionPrefab;
    public ScoreKeeper[] keepers;

    //Private
    private GameManager gameManager;
    private float scoreTimer;
    private bool spawnedWinningScore;

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
        if (this.enabled)
            UpdateColumns();

        if (!gameManager.gameActive || !gameManager.tutorialComplete)
            return;

        IncreaseTopFrogScore();
    }

    public void OnGameStart()
    {
        spawnedWinningScore = false;
    }
    public void ActivateKeepers()
    {        
        KOTHParent.gameObject.SetActive(enabled);


        for (int i = 0; i < gameManager.frogsReady.Length; i++)
        {
            keepers[i].active = gameManager.frogsReady[i];

            keepers[i].score = keepers[i].targetScore = 1;
            keepers[i].image.gameObject.SetActive(keepers[i].active);
            keepers[i].image.color = keepers[i].frogColor;
            keepers[i].scoreText.gameObject.SetActive(keepers[i].active);
            keepers[i].scoreText.text = "0";
            keepers[i].percent = 1.0f / 4.0f;
        }
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

        SetParticlePosition(frog.player);

        scoreTimer += Time.deltaTime;
        if (scoreTimer >= scoreIncreaseInterval)
        {
            scoreTimer -= scoreIncreaseInterval;
            IncreaseScore(frog.player);
        }
    }
    public void IncreaseScore(int player, float amount = -1, bool winningScore = false)
    {
        if (amount == -1)
            amount = scoreIncreaseAmount;

        //Spawn text prefab

        if (winningScore)
        {
            ScoreKeeper maxScoreKeeper = keepers[0];
            for (int i = 1; i < keepers.Length; i++)
            {
                if (keepers[i].targetScore > maxScoreKeeper.targetScore)
                {
                    maxScoreKeeper = keepers[i];
                }
            }

            amount = Mathf.RoundToInt(maxScoreKeeper.targetScore * winningGripPercent);
        }
        SpawnTextComponent(player, amount, winningScore);
    }

    public void AddScoreToUI(int player, float amount)
    {
        keepers[player].targetScore += amount;
        keepers[player].anim.SetTrigger("score");

        //Set score into text
        keepers[player].scoreText.text = (keepers[player].targetScore - 1).ToString();

        UpdateColumns();
    }

    private void SetParticlePosition(int player)
    {
        //Set particles position and color
        Vector3 pos = keepers[player].image.rectTransform.position;
        float width = (keepers[player].image.rectTransform.localScale.y * 100);
        pos.x += width * 0.95f;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(pos);
        particleSystem.transform.position = worldPos;
        particleSystem.startColor = keepers[player].frogColor;
    }
    private void SpawnTextComponent(int player, float scoreAmount, bool winningScore)
    {
        if (spawnedWinningScore)
            return;
        spawnedWinningScore = winningScore;

        Vector3 spawnPos = Vector3.zero;
        FrogPrototype frog = gameManager.playerScripts[player];
        if (frog !=null)
        {
            spawnPos = frog.transform.position;
        }

        scoreAdditionPrefab.color = keepers[player].frogColor;
        scoreAdditionPrefab.text = scoreAmount.ToString();
        GameObject o = Instantiate(scoreAdditionPrefab.gameObject, spawnPos, Quaternion.identity) as GameObject;

        //Set target position of object
        o.GetComponent<ScoreAdditionMove>().Initialize(this, keepers[player].image, scoreAmount, player, winningScore);
    }
    private void UpdateColumns()
    {
        //Activate frogs that are playing
        float totalScore = 0;
        if (gameManager != null)
        {
            for (int i = 0; i < gameManager.frogsReady.Length; i++)
            {
                keepers[i].active = gameManager.frogsReady[i];

                keepers[i].image.gameObject.SetActive(keepers[i].active);
                keepers[i].scoreText.gameObject.SetActive(keepers[i].active);

                if (keepers[i].active)
                {
                    //Add to totalscore
                    totalScore += keepers[i].score;

                    //Move towards target score
                    keepers[i].score = Mathf.MoveTowards(keepers[i].score, keepers[i].targetScore, ((spawnedWinningScore) ? 0.02f : Time.deltaTime) * 30.0f);
                    keepers[i].scoreText.text = Mathf.RoundToInt(keepers[i].score - 1).ToString();
                }
            }
        }
        else
        {
            for (int i = 0; i < keepers.Length; i++)
            {
                totalScore += keepers[i].score;
            }
        }


        //Set width of bar to current score
        for (int i = 0; i < keepers.Length; i++)
        {
            if (!keepers[i].active)
                continue;

            //Calculate percent of total
            float percent = Mathf.Clamp((float)keepers[i].score / (float)totalScore, 0f, 1f);
            keepers[i].percent = percent;

            //Set width to percent
            Vector3 scale = keepers[i].image.rectTransform.localScale;
            scale.y = minimumBarWidth + maximumBarWidth * keepers[i].percent;
            keepers[i].image.rectTransform.localScale = scale;

            //Set text to center of bar
            Vector3 textPos = keepers[i].scoreText.rectTransform.position;
            Vector3 imagePos = keepers[i].image.rectTransform.position;

            textPos.x = imagePos.x + (scale.y * 100) / 2;
            textPos.y = imagePos.y;
            keepers[i].scoreText.rectTransform.position = textPos;
        }
    }

    void OnValidate()
    {
        keepers[0].targetScore = Mathf.Clamp(keepers[0].targetScore, 1, float.MaxValue);
        keepers[1].targetScore = Mathf.Clamp(keepers[1].targetScore, 1, float.MaxValue);
        keepers[2].targetScore = Mathf.Clamp(keepers[2].targetScore, 1, float.MaxValue);
        keepers[3].targetScore = Mathf.Clamp(keepers[3].targetScore, 1, float.MaxValue);

        keepers[0].score = Mathf.Clamp(keepers[0].targetScore, 1, float.MaxValue);
        keepers[1].score = Mathf.Clamp(keepers[1].targetScore, 1, float.MaxValue);
        keepers[2].score = Mathf.Clamp(keepers[2].targetScore, 1, float.MaxValue);
        keepers[3].score = Mathf.Clamp(keepers[3].targetScore, 1, float.MaxValue);

        UpdateColumns();
    }

}
