using System.Collections.Generic;
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

    private Color[] colors = new Color[] { Color.black , Color.white, Color.aliceBlue, Color.beige, Color.purple };

    public void SpawnWheight(CharID id)
    {
        if (weightPrefab == null)
        {
            Debug.LogWarning("Missing weight prefab in wheight manager");
            return;
        }
        for (int i = 0; i < id.definingMoments.Length; i++)
        {
            Transform parent = id.definingMoments[i].isSinful ? hellSpawner : heavenSpawner;

            Vector3 randomOffset = new Vector3(
            Random.Range(-.05f, .05f),   // X range
            0f,                      // keep Y the same
            Random.Range(-.05f, .05f));    // Z range
            Vector3 spawnPos = parent.position + randomOffset;

            weights[i] = Instantiate(weightPrefab, spawnPos, Quaternion.identity, parent);

            weights[i].GetComponent<Weight>().index = i;
            weights[i].GetComponent<Weight>().isSinful = id.definingMoments[i].isSinful;
            weights[i].GetComponent<Weight>().isMoral = id.definingMoments[i].isMoral;
            weights[i].GetComponent<Weight>().manager = this;
            weights[i].GetComponentInChildren<Renderer>().material.color = colors[i];
        }
    }

    private void OnEnable()
    {
        EventManager.StartListening(GameEvent.GoNext, DeleteWheight);
    }

    public void DeleteWheight(Dictionary<string, object> data)
    {
        for (int i = 0; i < weights.Length; i++)
        { 
            Destroy(weights[i]);
        }
    }

    public void ResetWeight(int index)
    {
        if (weights[index].GetComponent<Weight>().isSinful)
        {
            weights[index].transform.position = hellSpawner.position;
            weights[index].transform.rotation = hellSpawner.rotation;
        }
        else
        {
            weights[index].transform.position = heavenSpawner.position;
            weights[index].transform.rotation = heavenSpawner.rotation;
        }
    }

}
