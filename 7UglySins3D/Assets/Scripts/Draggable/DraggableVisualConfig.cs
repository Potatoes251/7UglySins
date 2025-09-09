using UnityEngine;

[CreateAssetMenu(fileName = "DraggableVisual", menuName = "Drag/Draggable Visual", order = 0)]
public class DraggableVisualConfig : ScriptableObject
{
    [Header("Source A: Whole visual prefab (multi-mesh allowed)")]
    [Tooltip("If set, this prefab is instantiated under the 'Visual' child at edit/runtime.")]
    public GameObject modelPrefab;

    [Header("Source B: Explicit meshes (used if modelPrefab is null)")]
    [Tooltip("If you prefer, list meshes here and we'll create one child per mesh.")]
    public Mesh[] meshes;

    [Header("Materials (optional)")]
    [Tooltip("If set and using modelPrefab, these replace materials on all MeshRenderers.\nIf using meshes[], the i-th material is applied to Part_i (fallback to index 0).")]
    public Material[] materialsOverride;

    [Header("Collision (root collider)")]
    [Tooltip("Optional dedicated collision mesh for the root MeshCollider. If null, we use the first mesh we find in the visuals.")]
    public Mesh collisionMesh;
}
