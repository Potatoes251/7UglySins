using System.Collections;
using UnityEngine;

public class SwitchScale : MonoBehaviour
{
    [SerializeField]
    private GameObject balancedScale;
    [SerializeField]
    private GameObject leftScale;
    [SerializeField]
    private GameObject rightScale;

    private float delay = .1f;
    private float timeLeft = 0f;

    private bool switched = false;
    private bool isWaiting = false;

    private void Update()
    {
        if (switched)
        {
            timeLeft += Time.deltaTime;
            if (timeLeft > delay)
            {
                timeLeft = 0f;
                switched = false;
                CheckScale();
            }
        }
    }

    public void SwitchLeft(Scale oldScale)
    {
        switched = true;
        leftScale.GetComponent<Scale>().MoveWeight(oldScale.hell.GetWeightData().weight, oldScale.heaven.GetWeightData().weight);
        balancedScale.SetActive(false);
        leftScale.SetActive(true);
        rightScale.SetActive(false);
    }

    public void SwitchRight(Scale oldScale)
    {
        switched = true;
        rightScale.GetComponent<Scale>().MoveWeight(oldScale.hell.GetWeightData().weight, oldScale.heaven.GetWeightData().weight);
        balancedScale.SetActive(false);
        leftScale.SetActive(false);
        rightScale.SetActive(true);
    }

    public void SwitchMiddle(Scale oldScale)
    {
        switched = true;
        balancedScale.GetComponent<Scale>().MoveWeight(oldScale.hell.GetWeightData().weight, oldScale.heaven.GetWeightData().weight);
        balancedScale.SetActive(true);
        leftScale.SetActive(false);
        rightScale.SetActive(false);
    }

    public void CheckScale()
    {
        if (switched)
        {
            return;
        }
        if (balancedScale.activeSelf)
        {
            switch (balancedScale.GetComponent<Scale>().CheckScale())
            {
                case ScaleState.right:
                    SwitchRight(balancedScale.GetComponent<Scale>());
                    break;

                case ScaleState.left:
                    SwitchLeft(balancedScale.GetComponent<Scale>());
                    break;

                case ScaleState.middle:
                    break;

                default:
                    //Debug.LogWarning("Scale state not handled");
                    break;
            }
        }
        else if (leftScale.activeSelf)
        {
            switch (leftScale.GetComponent<Scale>().CheckScale())
            {
                case ScaleState.right:
                    SwitchRight(leftScale.GetComponent<Scale>());
                    break;

                case ScaleState.left:
                    break;

                case ScaleState.middle:
                    SwitchMiddle(leftScale.GetComponent<Scale>());
                    break;

                default:
                    //Debug.LogWarning("Scale state not handled");
                    break;
            }
        }
        else if (rightScale.activeSelf)
        {
            switch (rightScale.GetComponent<Scale>().CheckScale())
            {
                case ScaleState.right:
                    break;

                case ScaleState.left:
                    SwitchLeft(rightScale.GetComponent<Scale>());
                    break;

                case ScaleState.middle:
                    SwitchMiddle(rightScale.GetComponent<Scale>());
                    break;

                default:
                    //Debug.LogWarning("Scale state not handled");
                    break;
            }
        }
        else
        {
            //Debug.LogWarning("No scale active");
        }
    }

    public IEnumerator WaitForCheck()
    {
        if(isWaiting)
        {
            yield break;
        }
        isWaiting = true;

        yield return new WaitForSeconds(delay);

        isWaiting = false;
        CheckScale();
    }
}
