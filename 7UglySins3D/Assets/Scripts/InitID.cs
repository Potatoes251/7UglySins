using TMPro;
using UnityEngine;

public class InitID : MonoBehaviour
{
    public CharID id;

    [SerializeField] private TMP_Text nameToSet;
    [SerializeField] private TMP_Text genderToSet;
    [SerializeField] private TMP_Text causeToSet;
    [SerializeField] private TMP_Text occupationToSet;
    [SerializeField] private TMP_Text[] momentsToSet;

    void OnEnable()
    {
        if (id == null)
        {
            //Debug.LogWarning("id not set");
            return;
        }

        if (nameToSet != null) nameToSet.text = id.CharName;
        if (genderToSet != null) genderToSet.text = id.gender;
        if (causeToSet != null) causeToSet.text = id.causeOfDeath;
        if (occupationToSet != null) occupationToSet.text = id.occupation;

        for (int i = 0; i < id.definingMoments.Length && i < momentsToSet.Length; i++)
        {
            momentsToSet[i].text = id.definingMoments[i].text;
        }
    }
}
