using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Scriptable Objects/Character")]
public class Character : ScriptableObject
{
    public CharID id;

    public string[] dialogsName;

    [SerializeField]
    private GameObject model;

    private GameObject spawnedObject = null;

    public void Enters()
    {
        if (model != null)
        {
            spawnedObject = Instantiate(model, new Vector3(0, 2f, 0), Quaternion.identity);
            TextManager.instance.StartDialogs(dialogsName[0]);
        }
    }

    public void Exits()
    {
        if (spawnedObject != null)
        {
            Destroy(spawnedObject);
        }
    }
}
