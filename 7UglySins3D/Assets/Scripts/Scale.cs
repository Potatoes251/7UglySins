using System.Collections.Generic;
using UnityEngine;

public enum ScaleState
{
    left,
    right,
    middle
}

public class Scale : MonoBehaviour
{
    public ScaleSide hell;
    public ScaleSide heaven;


    void OnEnable()
    {
        EventManager.StartListening(GameEvent.GoNext, Evaluate);
    }

    void OnDisable()
    {
        EventManager.StopListening(GameEvent.GoNext, Evaluate);
    }

    private void Evaluate(Dictionary<string, object> data)
    {
        Debug.Log("evaluate");
        WeightData hellData = hell.GetWeightData();
        WeightData heavenData = heaven.GetWeightData();

        if (hellData.count + heavenData.count < 5)
        {
            EventManager.TriggerEvent(GameEvent.MissingWeight, new Dictionary<string, object> { });
            //Debug.LogWarning("wheight(s) missing on the scale only " + (hellData.count + heavenData.count) + " present");
            return;
        }
        else if(hellData.count + heavenData.count > 5)
        {
            EventManager.TriggerEvent(GameEvent.MissingWeight, new Dictionary<string, object> { });
            //Debug.LogWarning("too many weights on the scale : hell : " + hellData.count + "hell : " + heavenData.count);
            return;
        }

        bool goToHell = hellData.count > 2;

        int obedience = 0;
        int morality = 0;

        for (int i = 0; i < hellData.weight.Length; i++)
        {
            if (hellData.weight[i] != null)
            {
                obedience = hellData.weight[i].isSinful ? ++obedience : --obedience;
                morality = hellData.weight[i].isMoral ? --morality : ++morality;
            }
        }
        for (int i = 0; i < heavenData.weight.Length; i++)
        {
            if (heavenData.weight[i] != null)
            {
                obedience = heavenData.weight[i].isSinful ? --obedience : ++obedience;
                morality = heavenData.weight[i].isMoral ? ++morality : --morality;
            }
        }
        Debug.Log("sentencing");
        EventManager.TriggerEvent(GameEvent.Sentencing, new Dictionary<string, object> { 
            {"obedience", obedience },{ "morality", morality }, { "goToHell", goToHell } });

        hell.ResetScale();
        heaven.ResetScale();
    }

    public ScaleState CheckScale()
    {
        WeightData hellData = hell.GetWeightData();
        WeightData heavenData = heaven.GetWeightData();
        Debug.Log("hell : " + hellData.count + " heaven : " + heavenData.count);
        if (hellData.count == heavenData.count)
        {
            return ScaleState.middle;
        }
        else if (hellData.count < heavenData.count)
        {
            return ScaleState.right;
        }
        else
        {
            return ScaleState.left;
        }
    }

    public void MoveWeight(Weight[] weightsHell, Weight[] weightHeaven)
    {
        hell.MoveWeight(weightsHell);
        heaven.MoveWeight(weightHeaven);
    }
}
