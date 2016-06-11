using UnityEngine;
using System.Collections;

public class M_SliderButton : M_Button 
{
    public Transform fillSlider;
    public bool drawGizmos;
    public float value;
    public bool onChange;
    private float prevValue;
    public bool pressed;

    public float leftEdge;
    public float rightEdge;
    public float yOffset;

    private Transform handle;
    public bool selected = false;
    private float speed = 0.75f;

    protected override void OnStart()
    {
        base.OnStart();
        handle = transform.FindChild("SliderHandle");
    }
    void Update()
    {
        if (!selected)
            return;

        if (pressed)
        {
            float input = GameManager.GetThumbStick(XboxThumbStick.Left).x;
            if (Mathf.Abs(input) > 0.1f)
            {
                if (input < 0)
                    value -= speed * Time.deltaTime;
                else
                    value += speed * Time.deltaTime;

                value = Mathf.Clamp(value, 0.0f, 1.0f);
                SetHandle();
            }
        }

        anim.SetBool("Pressed", pressed);
    }

    public void SetValue(float value)
    {
        this.value = value;
        SetHandle();
    }
    private void SetHandle()
    {
        Vector3 minPos = transform.position; minPos.x += leftEdge; minPos.y += yOffset;
        Vector3 maxPos = transform.position; maxPos.x += rightEdge; maxPos.y += yOffset;
        handle.position = maxPos - minPos;
        handle.position = minPos + (handle.position * value);

        Vector3 scale = fillSlider.localScale;
        scale.x = value;
        fillSlider.localScale = scale;
    }

    void OnDrawGizmos()
    {
        if (!drawGizmos)
            return;

        handle = transform.FindChild("SliderHandle");
        Vector3 minPos = transform.position; minPos.x += leftEdge; minPos.y += yOffset;
        Vector3 maxPos = transform.position; maxPos.x += rightEdge; maxPos.y += yOffset;
        SetHandle();

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(handle.position, 0.1f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(minPos, 0.1f);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(maxPos, 0.1f);
    }

    void OnValidate()
    {
        value = Mathf.Clamp(value, 0.0f, 1.0f);
    }

    public override void OnPress()
    {
        pressed = !pressed;
    }
    public override void OnSelect()
    {
        base.OnSelect();
        selected = true;
    }
    public override void OnDeSelect()
    {
        base.OnDeSelect();
        selected = false;
    }
}
