using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCharacters : MonoBehaviour
{

    public void StartMoving(Vector3 endPoint)
    {
        StartCoroutine(MoveAndDestroy(endPoint, 1f));
    }

    private IEnumerator MoveAndDestroy(Vector3 endPoint, float duration)
    {
        EventManager.TriggerEvent(GameEvent.NewCharacter, new Dictionary<string, object> { });
        Vector3 start = transform.position;
        float time = 0f;

        while (time < duration)
        {
            transform.position = Vector3.Lerp(start, endPoint, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = endPoint;
        Destroy(gameObject);
    }
}
