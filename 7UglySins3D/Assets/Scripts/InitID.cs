using NUnit.Framework.Interfaces;
using TMPro;
using UnityEngine;

public class InitID : MonoBehaviour
{
    [SerializeField] private CharID id;

    [SerializeField] private TMP_Text nameToSet;
    [SerializeField] private TMP_Text genderToSet;
    [SerializeField] private TMP_Text causeToSet;
    [SerializeField] private TMP_Text requestToSet;

    void Start()
    {
        if (id == null)
        {
            Debug.Log("id not set");
            return;
        }

        if (nameToSet != null) nameToSet.text = id.CharName;
        if (genderToSet != null) genderToSet.text = id.gender;
        if (causeToSet != null) causeToSet.text = id.causeOfDeath;
        if (requestToSet != null) requestToSet.text = id.request;
    }
}
