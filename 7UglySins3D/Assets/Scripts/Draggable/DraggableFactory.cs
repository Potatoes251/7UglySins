using UnityEngine;

public static class DraggableFactory
{
    public static DraggableController Spawn(
        Vector3 position,
        Quaternion rotation,
        GameObject genericDraggablePrefab,
        DraggableVisualConfig visual,
        DraggableParams parameters,
        Transform parent = null)
    {
        var go = Object.Instantiate(genericDraggablePrefab, position, rotation, parent);
        var ctrl = go.GetComponent<DraggableController>();
        ctrl.visual = visual;
        ctrl.parameters = parameters;

#if UNITY_EDITOR
        // Force a rebuild of visuals when spawned in editor play mode
        var method = typeof(DraggableController).GetMethod("OnValidate",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (Application.isPlaying == false && method != null) method.Invoke(ctrl, null);
#endif
        return ctrl;
    }
}
