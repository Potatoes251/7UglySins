using UnityEngine;

public class WheightManager : MonoBehaviour
{
    [SerializeField]
    private Transform hellSpawner;

    [SerializeField]
    private Transform heavenSpawner;

    [SerializeField]
    private GameObject weightPrefab;

    private GameObject[] weights = new GameObject[5];

    public void SpawnWheight(bool[] hellWheight)
    {
        if (weightPrefab == null)
        {
            Debug.LogWarning("Missing wheight prefab in wheight manager");
            return;
        }
        if (weights.Length > 0)
        {
            DeleteWheight();
        }
        for (int i = 0; i < hellWheight.Length; i++)
        {
            if (hellWheight[i])
            {
                weights[i] = Instantiate(weightPrefab, hellSpawner);
            }
            else
            {
                weights[i] = Instantiate(weightPrefab, heavenSpawner);
            }
        }
    }

    public void DeleteWheight()
    {
        for (int i = 0; i < weights.Length; i++)
        { 
            Destroy(weights[i]);
        }
    }

}
