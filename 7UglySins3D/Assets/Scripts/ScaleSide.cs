using UnityEngine;


public struct WeightData
{
    public Weight[] weight;
    public int count;
};

public class ScaleSide : MonoBehaviour
{
    [SerializeField]
    private SwitchScale SwitchScale;
    private Weight[] weights = new Weight[5];
    private int count = 0;

    [SerializeField]
    private GameObject plateau;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Weight"))
        {
            int index = collision.collider.GetComponent<Weight>().index;
            weights[index] = collision.collider.GetComponent<Weight>();
            count++;
            SwitchScale.CheckScale();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Weight"))
        {
            int index = collision.collider.GetComponent<Weight>().index;
            weights[index] = null;
            count--;
            SwitchScale.CheckScale();
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

    public void MoveWeight(bool resetScale = true)
    {
        Debug.Log("moving weights");
        for (int i = 0; i < weights.Length; i++)
        {
            if (weights[i] != null)
            {
                weights[i].transform.position = plateau.transform.position;
            }
        }
        if (resetScale)
        {
            ResetScale();
        }
    }
}
