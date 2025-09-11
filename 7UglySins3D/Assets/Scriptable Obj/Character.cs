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
            if(spawner != null && id != null)
            {
                //Debug.Log("enters");
                spawner.SpawnWheight(id);
            }
            spawnedObject = Instantiate(model, spawnPoint);
            id.dialogs[0].DisplayDialog();
        }
    }

    public void Exits(bool goToHell = true)
    {
        if (spawnedObject != null)
        {
            if (id.dialogs.Length < 3)
            {
                //Debug.LogWarning("Not enough dialogs in : " + id.CharName);
                return;
            }

            Vector3 start = spawnedObject.transform.position;
            Vector3 end;

            if (goToHell)
            {
                id.dialogs[2].DisplayDialog();
                end = start + 3.5f * Vector3.forward;
            }
            else
            {
                id.dialogs[1].DisplayDialog();
                end = start + 3.5f * Vector3.back;
            }
            if (spawnedObject.GetComponent<MoveCharacters>() == null)
            {
                //Debug.LogWarning("character missing its moving scripts");
                return;
            }
            spawnedObject.GetComponent<MoveCharacters>().StartMoving(end);
        }
    }
}
