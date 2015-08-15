using UnityEngine;
using System.Collections;

public class StoneDisabler : MonoBehaviour 
{
	//Data
    public LayerMask rockMask;

	//Components
	
	//References

    private IEnumerator ActivateStone(Collider2D col, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        col.enabled = true;
    }
    public void DisableStoneAt(Vector2 pos)
    {
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.up, 20, rockMask);

        if (hit)
        {
            Collider2D col = hit.transform.GetComponent<Collider2D>();

            if (col)
            {
                col.enabled = false;
                StartCoroutine(ActivateStone(col, 1.5f));
            }
        }
    }
}
