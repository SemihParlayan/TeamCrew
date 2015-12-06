using UnityEngine;
using System.Collections;
using XInputDotNetPure;

public class Vibration : MonoBehaviour
{
    public static Vibration instance;
    private bool[] vibrating;

    void Awake()
    {
        if (instance == null)
            instance = this;

        vibrating = new bool[4];
    }

    public void SetVibration(int index, float leftMotor, float rightMotor, float time)
    {
        return;
        if (vibrating[index])
        {
            return;
        }
        vibrating[index] = true;

        GamePad.SetVibration((PlayerIndex)index, leftMotor, rightMotor);
        StartCoroutine(ResetVibration((PlayerIndex)index, time));
    }
    private IEnumerator ResetVibration(PlayerIndex index, float time)
    {
        yield return new WaitForSeconds(time);

        GamePad.SetVibration(index, 0f, 0f);
        vibrating[(int)index] = false;
    }
}
