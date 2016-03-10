using UnityEngine;
using System.Collections;

public class M_SliderButton : M_Button 
{
    public Transform fillSlider;
    public bool drawGizmos;
    public float value;

    public float leftEdge;
    public float rightEdge;
    public float yOffset;

    private Transform handle;
    private bool selected = false;
    private float speed = 0.004f;

    protected override void OnStart()
    {
        base.OnStart();
        handle = transform.FindChild("SliderHandle");
    }
    void Update()
    {
        if (!selected)
            return;

        float input = GameManager.GetThumbStick(XboxThumbStick.Left).x;
        if (Mathf.Abs(input) > 0.1f)
        {
            if (input < 0)
                value -= speed;
            else
                value += speed;

            value = Mathf.Clamp(value, 0.0f, 1.0f);
            SetHandle();
        }
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
