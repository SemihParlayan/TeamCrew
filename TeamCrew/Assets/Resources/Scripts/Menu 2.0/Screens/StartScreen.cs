using UnityEngine;
using System.Collections;

public class StartScreen : M_Screen
{
    //Data
    private bool fadeToZero;

    //References
    public SpriteRenderer fadeToBlackSprite;

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (fadeToZero)
        {
            Color c = fadeToBlackSprite.color;
            c.a = Mathf.MoveTowards(c.a, 0f, Time.deltaTime);
            fadeToBlackSprite.color = c;
        }
    }

    public override void OnSwitchedTo()
    {
        fadeToZero = false;
        base.OnSwitchedTo();
        Invoke("Fade", 0.5f);
    }

    private void Fade()
    {
        fadeToZero = true;
    }


}
