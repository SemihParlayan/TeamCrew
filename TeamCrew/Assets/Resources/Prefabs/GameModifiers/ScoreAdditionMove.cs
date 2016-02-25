using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreAdditionMove : MonoBehaviour 
{
    private KOTH koth;
    private Rigidbody2D body;
    private Image targetImage;
    private bool move;
    private float movementSpeed;
    private float score = 0;
    private int player = -1;
    private bool winningScore = false;

    void Awake()
    {
        movementSpeed = 0.2f;

        move = false;
        body = GetComponent<Rigidbody2D>();
        StartMoving();
    }

    public void Initialize(KOTH koth, Image image, float score, int player, bool winningScore = false)
    {
        this.winningScore = winningScore;
        this.koth = koth;
        this.player = player;
        this.score = score;
        this.targetImage = image;

        if (winningScore)
        {
            transform.localScale = Vector3.one * 2f;
        }
    }
    void StartMoving()
    {
        move = true;
        body.gravityScale = 0f;
    }
    void Update()
    {
        if (!move || targetImage == null)
            return;

        Vector3 barCenter = targetImage.rectTransform.position;
        barCenter.x += (targetImage.rectTransform.localScale.y * 100) / 2;
        Vector3 target = Camera.main.ScreenToWorldPoint(barCenter);
        target.z = transform.position.z;

        
        Vector3 targetDir = (target - transform.position);
        movementSpeed += 0.1f;

        if (!winningScore)
        {
            Vector3 vel = body.velocity;
            vel = Vector3.MoveTowards(vel, targetDir * movementSpeed, Time.deltaTime * 100.0f);
            body.velocity = vel;
        }
        else
        {
            transform.position += targetDir.normalized * 0.15f;
        }

        float dist = Vector3.Distance(target, transform.position);
        if (dist < 1f)
        {
            body.velocity = Vector3.zero;
            move = false;
            koth.AddScoreToUI(player, score);
            Destroy(gameObject);
        }
    }
}
