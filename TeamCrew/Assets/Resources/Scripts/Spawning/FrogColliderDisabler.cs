using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FrogColliderDisabler : MonoBehaviour 
{
    private bool collidersAreActive = true;
    private bool reActiveColliders = false;
    public LayerMask rockMask;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            DisableRockColliders();
        }

        if (reActiveColliders && !collidersAreActive)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position + new Vector3(0, -0.7f), 2.7f, rockMask);

            if (hits.Length <= 0)
            {
                EnableRockColliders();
            }
        }
    }

    private void ReActivateColliders()
    {
        reActiveColliders = true;
    }

    public void DisableRockColliders()
    {
        if (!collidersAreActive)
            return;

        collidersAreActive = false;
        Invoke("ReActivateColliders", 1f);


        List<Collider2D> rockColliders = transform.parent.GetComponentsInChildren<Collider2D>().ToList();
        for (int i = 0; i < rockColliders.Count; i++)
        {
            if (rockColliders[i].transform.name != "rock_collider")
            {
                rockColliders.Remove(rockColliders[i]);
                i--;
            }
            else
            {
                rockColliders[i].enabled = false;
            }
        }
    }
    public void EnableRockColliders()
    {
        if (collidersAreActive)
            return;
        collidersAreActive = true;
        reActiveColliders = false;

        List<Collider2D> rockColliders = transform.parent.GetComponentsInChildren<Collider2D>().ToList();
        for (int i = 0; i < rockColliders.Count; i++)
        {
            if (rockColliders[i].transform.name != "rock_collider")
            {
                rockColliders.Remove(rockColliders[i]);
                i--;
            }
            else
            {
                rockColliders[i].enabled = true;
            }
        }
    }
}
