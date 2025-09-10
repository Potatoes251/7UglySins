using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Scriptable Objects/Character")]
public class Character : ScriptableObject
{
    public CharID id;

    [SerializeField]
    private GameObject model;

    private GameObject spawnedObject = null;

    public void Enters()
    {
        if (model != null)
        {
            spawnedObject = Instantiate(model, new Vector3(0, 2f, 0), Quaternion.identity);
            id.dialogs[0].DisplayDialog();
        }
    }

    public void Exits()
    {
        if (spawnedObject != null)
        {
            id.dialogs[1].DisplayDialog();
            Destroy(spawnedObject);
        }
    }
}
