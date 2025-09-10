using UnityEngine;

[CreateAssetMenu(fileName = "CharID", menuName = "Scriptable Objects/CharID")]
public class CharID : ScriptableObject
{
    [System.Serializable]
    public struct Moments
    {
        public string text;
        public bool isSinful;
        public bool isMoral;
    }

    public string CharName;
    public string gender;
    public string causeOfDeath;
    public string occupation;

    public Moments[] definingMoments;

    public Dialog[] dialogs;
}
