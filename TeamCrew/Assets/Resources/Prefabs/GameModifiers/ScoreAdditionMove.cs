using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreAdditionMove : MonoBehaviour 
{
    private Rigidbody2D body;
    private Image targetImage;
    private bool move;
    private float movementSpeed;

    void Awake()
    {
        movementSpeed = 0.2f;

        move = false;
        body = GetComponent<Rigidbody2D>();
        StartMoving();
    }

    public void SetTargetPosition(Image image)
    {
        targetImage = image;
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

        movementSpeed += 0.1f;
        Vector3 targetDir = (target - transform.position);
        Vector3 vel = body.velocity;
        vel = Vector3.MoveTowards(vel, targetDir * movementSpeed, Time.deltaTime * 100.0f);
        body.velocity = vel;

        float dist = Vector3.Distance(target, transform.position);
        if (dist < 1f)
        {
            body.velocity = Vector3.zero;
            move = false;
            Destroy(gameObject);
        }
    }
}
