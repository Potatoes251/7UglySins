using UnityEngine;

[CreateAssetMenu(fileName = "CharID", menuName = "Scriptable Objects/CharID")]
public class CharID : ScriptableObject
{
    public string CharName;
    public string gender;
    public string causeOfDeath;
    public string request;

    public string[] definingMoments;
}
