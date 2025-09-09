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

    public void Exits(bool goToHell = true)
    {
        if (spawnedObject != null)
        {
            if (id.dialogs.Length < 3)
            {
                Debug.LogWarning("Not enough dialogs in : " + id.CharName);
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
