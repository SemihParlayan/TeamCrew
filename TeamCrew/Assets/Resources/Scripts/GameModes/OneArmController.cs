using UnityEngine;
using System.Collections;

public class OneArmController : MonoBehaviour 
{
    public GameObject leftUpper;
    public GameObject leftLower;
    public GameObject leftHand;

    public GameObject rightUpper;
    public GameObject rightLower;
    public GameObject rightHand;

    void Awake()
    {
    }

    public void SetDisabledArmState(bool state, int arm = -1)
    {
        FrogPrototype frog = GetComponent<FrogPrototype>();

        if (frog == null)
            return;

        leftUpper.SetActive(true);
        leftLower.SetActive(true);
        leftHand.SetActive(true);

        rightUpper.SetActive(true);
        rightLower.SetActive(true);
        rightHand.SetActive(true);

        //frog.leftGripScript.disabled = false;
        //frog.rightGripScript.disabled = false;

        if (state && arm != -1)
        {
            if (arm == 0)
            {
                leftUpper.SetActive(false);
                leftLower.SetActive(false);
                leftHand.SetActive(false);
                //frog.leftGripScript.disabled = true;
            }
            else
            {
                rightUpper.SetActive(false);
                rightLower.SetActive(false);
                rightHand.SetActive(false);
                //frog.rightGripScript.disabled = true;
            }
        }
    }
}
