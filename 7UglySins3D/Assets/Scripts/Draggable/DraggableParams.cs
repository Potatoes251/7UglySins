using UnityEngine;

[CreateAssetMenu(fileName = "DraggableParams", menuName = "Drag/Draggable Params", order = 1)]
public class DraggableParams : ScriptableObject
{
    [Header("Drag Plane")]
    public float liftHeight = 0.15f;
    public bool keepLiftOnRelease = true;
    public bool makeKinematicWhileDragging = true;

    [Header("Drop To Ground On Release")]
    public bool dropToGroundOnRelease = false;
    public LayerMask groundMask = ~0;

    [Header("Bounds Clamp")]
    [Tooltip("Optional direct reference to a BoxCollider in the scene (usually leave null).")]
    public BoxCollider allowedArea;
    [Tooltip("If true and allowedArea is null, auto-find a BoxCollider GameObject by this tag at runtime.")]
    public bool autoFindPlayAreaByTag = true;
    public string playAreaTag = "PlayArea";
    public float innerPadding = 0f;

    [Header("Picking")]
    public LayerMask draggableMask = ~0;
}
