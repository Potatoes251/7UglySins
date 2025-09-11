using UnityEngine;


public struct WeightData
{
    public Weight[] weight;
    public int count;
};

public class ScaleSide : MonoBehaviour
{
    private Weight[] weights = new Weight[5];
    private int count = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weight"))
        {
            int index = other.GetComponent<Weight>().index;
            weights[index] = other.GetComponent<Weight>();
            count++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Weight"))
        {
            int index = other.GetComponent<Weight>().index;
            weights[index] = null;
            count--;
        }
    }

    public void ResetScale()
    {
        count = 0;
    }

    public WeightData GetWeightData()
    {
        WeightData data = new WeightData();
        data.weight = weights;
        data.count = count;

        return data;
    }
}
