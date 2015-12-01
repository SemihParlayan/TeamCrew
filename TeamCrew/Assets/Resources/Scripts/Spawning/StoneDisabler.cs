using UnityEngine;
using System.Collections;

public class StoneDisabler : MonoBehaviour 
{
	//Data
    public LayerMask rockMask;
    public LayerMask frogMask;

	//Components
	
	//References
    private IEnumerator ActivateStone(Collider2D col, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        RaycastHit2D hit = Physics2D.BoxCast(col.transform.position, Vector2.one * 4, 0, Vector2.zero, 1f, frogMask);

        if (hit)
        {
            StartCoroutine(ActivateStone(col, 0.25f));
        }
        else
            col.enabled = true;
    }
    public void DisableStoneAt(Vector2 pos)
    {
        RaycastHit2D[] hit = Physics2D.BoxCastAll(pos, Vector2.one * 3, 0, Vector2.up, 20f, rockMask);

        if (hit.Length > 0)
        {
            for (int i = 0; i < hit.Length; i++)
            {
                Collider2D col = hit[i].transform.GetComponent<Collider2D>();

                if (col)
                {
                    col.enabled = false;
                    StartCoroutine(ActivateStone(col, 1.5f));
                }
            }
        }
    }
}
