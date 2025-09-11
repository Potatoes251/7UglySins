using UnityEngine;

public class SwitchScale : MonoBehaviour
{
    [SerializeField]
    private GameObject balancedScale;
    [SerializeField]
    private GameObject leftScale;
    [SerializeField]
    private GameObject rightScale;

    private float delay = 1f;
    private float timeLeft = 0f;

    private bool switched = false;


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

    public void SwitchLeft()
    {
        switched = true;
        balancedScale.SetActive(false);
        leftScale.SetActive(true);
        rightScale.SetActive(false);
        leftScale.GetComponent<Scale>().MoveWeight();
    }

    public void SwitchRight()
    {
        switched = true;
        balancedScale.SetActive(false);
        leftScale.SetActive(false);
        rightScale.SetActive(true);
        rightScale.GetComponent<Scale>().MoveWeight();
    }

    public void SwitchMiddle()
    {
        switched = true;
        balancedScale.SetActive(true);
        leftScale.SetActive(false);
        rightScale.SetActive(false);
        balancedScale.GetComponent<Scale>().MoveWeight();
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
                    SwitchRight();
                    break;

                case ScaleState.left:
                    SwitchLeft();
                    break;

                case ScaleState.middle:
                    break;

                default:
                    Debug.LogWarning("Scale state not handled");
                    break;
            }
        }
        else if (leftScale.activeSelf)
        {
            switch (leftScale.GetComponent<Scale>().CheckScale())
            {
                case ScaleState.right:
                    SwitchRight();
                    break;

                case ScaleState.left:
                    break;

                case ScaleState.middle:
                    SwitchMiddle();
                    break;

                default:
                    Debug.LogWarning("Scale state not handled");
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
                    SwitchLeft();
                    break;

                case ScaleState.middle:
                    SwitchMiddle();
                    break;

                default:
                    Debug.LogWarning("Scale state not handled");
                    break;
            }
        }
        else
        {
            Debug.LogWarning("No scale active");
        }
    }
}
