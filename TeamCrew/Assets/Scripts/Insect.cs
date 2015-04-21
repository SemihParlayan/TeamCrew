using UnityEngine;
using System.Collections;

public enum MovementType
{
    NONE,
    RANDOM,
    SIN,
}

public class Insect : MonoBehaviour
{
    public MovementType movementMode;
    public AudioSource soundSource;
    public float MinSittingTime;
    public float MaxSittingTime;
    public float speed;

    public float intenseForce;
    public float normalForce;


    float sittingTime; //timer
    bool paralyzed;

    Transform hand;

    //Sin curve variables
    float startY;
    float swing;
    int cycleStart;
    float frequency;

    //Movement types
    //bool[] movementTypes;
    bool curve;
    bool jitter;

    float thing;

    float targetY;

	void Start ()
    {
        paralyzed = false;

        intenseForce = 1500;
        normalForce = 500;
       targetY = startY = transform.position.y;
        swing = 1.0f;
        cycleStart = Random.Range(0, 360);
        frequency = 1.0f;

        //movement
        curve = true;
        jitter = true;
        targetY = 20;
	}
	
	
	void Update ()
    {
        if(paralyzed)
        {
            CircleCollider2D c = hand.GetComponent<CircleCollider2D>();
            if (c)
            {
                transform.position = hand.position - hand.right * 0.5f;
            }
        }
        else
        {
            if(sittingTime > 0)
            {
                sittingTime -= Time.deltaTime;
                if(sittingTime  <= 0)
                {
                    sittingTime = 0;
                    movementMode = MovementType.SIN;
                }
            }

            Rigidbody2D body = GetComponent<Rigidbody2D>();
            thing += Time.deltaTime;
            if(curve)      
            {
                
                if (transform.position.y < targetY)
                {
                    body.AddForce(new Vector2(0, intenseForce));
                    //soundSource.pitch = intenseForce/normalForce;
                }
                else if (body.velocity.y < 0)
                {
                    body.AddForce(new Vector2(0, normalForce));
                    //soundSource.pitch = 1;

                }
            }
            if (jitter)
            {

            }
        }
	}

    public void SetParalyze(bool state)
    {
        paralyzed = state;
    }
    public void SetHand(Transform hand)
    {
        this.hand = hand;
       //transform.parent = hand;
       //transform.localPosition = Vector3.zero;
       //transform.localScale = Vector3.one;
    }

    void OnTriggerEnter2D(Collider2D c)
    {
        if (c.transform.tag == "Grip" && Random.Range(0,2) == 1)
        {
            movementMode = MovementType.NONE;
            sittingTime = Random.Range(MinSittingTime, MaxSittingTime);

            //Todo: Change sprite to sitting!
        }
    }
}
