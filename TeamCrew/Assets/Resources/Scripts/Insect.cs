using UnityEngine;
using System.Collections;

public enum FlyState { Chasing, Leaving, Panicking}
public class Insect : MonoBehaviour
{
    //Publics
    public Transform topFrog;
    public Transform bottomFrog;
    public AudioSource soundSource;
    public Animator arrowAnimator;
    public LadybugSpawner spawner;

    [Range(0, 10)]
    public float heightDifferenceLimit;

    [Range(0, 10f)]
    public float bottomFrogTargetOffset;
    [Range(0, 500f)]
    public float chaseSpeed;
    [Range(0, 500f)]
    public float panickSpeed;
    [Range(0, 500f)]
    public float leaveSpeed;

    [Range(0, 30f)]
    public float panickTargetOffset;

    [Range(0, 5f)]
    public float panickTopLimit = 1.25f;

    //Privates
    private Rigidbody2D body;
    private HingeJoint2D gripJoint;
    private GameManager gameManager;
    public FlyState currentState;
    public Vector3 targetChasePosition;
    private bool moveNormalized;
    private int player;

	void Start ()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
        body = GetComponent<Rigidbody2D>();
        gripJoint = GetComponent<HingeJoint2D>();
	}
	void Update ()
    {
        SetTopAndBottomFrog();
        LeaveIfThereIsNoFrogs();

        switch (currentState)
        {
            case FlyState.Chasing:
                Chase();
                break;
            case FlyState.Panicking:
                Panick();
                break;
            case FlyState.Leaving:
                Leave();
                break;
        }
	}
    void FixedUpdate()
    {
        Vector2 directionToTarget = (targetChasePosition - transform.position);
        if (moveNormalized)
            directionToTarget.Normalize();

        float speed = 0;
        switch(currentState)
        {
            case FlyState.Chasing:
                speed = chaseSpeed;
                break;
            case FlyState.Leaving:
                speed = leaveSpeed;
                break;
            case FlyState.Panicking:
                speed = panickSpeed;
                break;
        }
        body.velocity = directionToTarget * Time.fixedDeltaTime * speed;


        //Clamp velocity
        Vector3 vel = body.velocity;

        vel.x = Mathf.Clamp(vel.x, -10f, 10f);

        body.velocity = vel;
    }

    private void Chase()
    {
        //Change to leaving if top and bottomfrog are to close
        float heightDifference = Mathf.Abs(bottomFrog.position.y - topFrog.position.y);
        if (heightDifference <= heightDifferenceLimit)
        {
            ChangeState(FlyState.Leaving);
            return;
        }

        //Set target position
        targetChasePosition = bottomFrog.position;
        targetChasePosition.y += bottomFrogTargetOffset;

        //Leave if bottomfrog is to close to top
        float climbedHeight = Mathf.Clamp(bottomFrog.position.y / GameManager.LevelHeight, 0f, 1f);
        if (climbedHeight >= 0.9f)
        {
            ChangeState(FlyState.Leaving);
        }
    }
    private void Panick()
    {
        Vector2 leftStick = GameManager.GetThumbStick(XboxThumbStick.Left, player);
        Vector2 rightStick = GameManager.GetThumbStick(XboxThumbStick.Right, player);


        Vector3 combination = leftStick + rightStick;

        combination.Normalize();

        targetChasePosition += Vector3.right * combination.x * 0.025f;
    }
    private void Leave()
    {
        float distance = Vector3.Distance(transform.position, targetChasePosition);
        if (distance <= 2f)
        {
            spawner.RemoveFly();
        }
    }

    private void SetTopAndBottomFrog()
    {
        topFrog = GameManager.GetTopFrog();
        bottomFrog = GameManager.GetBottomFrog();
    }
    private void LeaveIfThereIsNoFrogs()
    {
        int activeFrogCounter = 0;
        for (int i = 0; i < GameManager.players.Length; i++)
        {
            if (GameManager.players[i] != null)
                activeFrogCounter++;
        }
        if (activeFrogCounter <= 1)
            ChangeState(FlyState.Leaving);
    }
    private void ChangeState(FlyState state)
    {
        if (currentState == state)
            return;
        currentState = state;

        switch(state)
        {
            case FlyState.Chasing:
                moveNormalized = false;
                break;

            case FlyState.Leaving:
                gripJoint.enabled = false;
                moveNormalized = true;
                int dir = (Random.Range(0, 2) == 0) ? 1 : -1;
                targetChasePosition += Vector3.right * dir * 50f;

                targetChasePosition += Vector3.up * Random.Range(10f, 15f);
                break;

            case FlyState.Panicking:
                Invoke("ActivateArrows", 2.5f);
                moveNormalized = false;
                targetChasePosition += Vector3.up * panickTargetOffset;
                targetChasePosition.y = Mathf.Clamp(targetChasePosition.y, 0, GameManager.LevelHeight - panickTopLimit);
                break;
        }
    }
    public void AddHand(int player)
    {
        this.player = player;
        ChangeState(FlyState.Panicking);
    }
    public void RemoveHand()
    {
        ChangeState(FlyState.Leaving);
    }


    private void ActivateArrows()
    {
        arrowAnimator.SetTrigger("Spawn");
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(targetChasePosition, 0.5f);
    }
}