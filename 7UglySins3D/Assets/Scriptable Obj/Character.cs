using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Scriptable Objects/Character")]
public class Character : ScriptableObject
{
    public CharID id;

    public string[] dialogsName;

    public GameObject model;


    public void Enters()
    {
        if (model != null)
        {
            Instantiate(model, new Vector3(0, 2f, 0), Quaternion.identity);
            TextManager.instance.StartDialogs(dialogsName[0]);
        }
    }
}
