using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Scriptable Objects/Character")]
public class Character : ScriptableObject
{
    public CharID id;

    [SerializeField]
    private GameObject model;

    private GameObject spawnedObject = null;

    public void Enters(Transform spawnPoint, WheightManager spawner = null)
    {
        if (model != null)
        {
            if(spawner != null)
            {
                spawner.SpawnWheight(CountSin());
            }
            spawnedObject = Instantiate(model, spawnPoint);
            id.dialogs[0].DisplayDialog();
        }
    }

    public bool[] CountSin()
    {
        bool[] sin = new bool[id.definingMoments.Length];

        for (int i = 0; i < id.definingMoments.Length; ++i)
        {
            sin[i] = id.definingMoments[i].isSinful;
        }
        return sin;
    }

    public void Exits(bool goToHell = true)
    {
        if (spawnedObject != null)
        {
            if (id.dialogs.Length < 3)
            {
                Debug.LogWarning("Not enough dialogs in : " + id.CharName);
                Destroy(spawnedObject);
                return;
            }
            if (goToHell)
            {
                id.dialogs[2].DisplayDialog();
            }
            else
            {
                id.dialogs[1].DisplayDialog();
            }

            Destroy(spawnedObject);
        }
    }
}
