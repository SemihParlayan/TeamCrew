using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[System.Serializable]
public class ScoreKeeper
{
    public Image image;
    public float score;
    public float percent;
    public bool active;
}
public class KOTH : MonoBehaviour 
{
    //Public
    public float height = 4f;
    public float bottomOffset = 0f;
    public float scoreIncreaseAmount = 1;
    public Transform KOTHParent;
    public ScoreKeeper[] keepers;

    //Private
    private GameManager gameManager;

    void Awake()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        if (!gameManager.gameActive )//|| !gameManager.tutorialComplete)
            return;

        IncreaseTopFrogScore();
    }

    public void OnGameStart()
    {
        KOTHParent.gameObject.SetActive(enabled);
    }
    public void DisableKeepers()
    {
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

        IncreaseScore(frog.player);
    }
    private void IncreaseScore(int player)
    {
        keepers[player].score += scoreIncreaseAmount;

        UpdateColumns();
    }

    private void UpdateColumns()
    {
        float totalScore = 0;
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
        keepers[0].score = Mathf.Clamp(keepers[0].score, 1, float.MaxValue);
        keepers[1].score = Mathf.Clamp(keepers[1].score, 1, float.MaxValue);
        keepers[2].score = Mathf.Clamp(keepers[2].score, 1, float.MaxValue);
        keepers[3].score = Mathf.Clamp(keepers[3].score, 1, float.MaxValue);

        //UpdateColumns();
    }

}
