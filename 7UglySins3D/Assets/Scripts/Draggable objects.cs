using UnityEngine;

/// <summary>
/// Drag a 3D object with the mouse on a horizontal plane:
/// - On grab: small lift, then Y stays fixed
/// - Optionally clamps XZ inside a BoxCollider area (trigger recommended)
/// </summary>
[RequireComponent(typeof(Collider))]
public class Drag3DKeepY : MonoBehaviour
{
    [Header("Drag Settings")]
    public float liftHeight = 0.15f;
    public bool keepLiftOnRelease = true;
    public bool makeKinematicWhileDragging = true;

    [Header("Bounds (Optional)")]
    [Tooltip("If assigned, object will be clamped inside this BoxCollider's world bounds (XZ only). Make it a Trigger.")]
    public BoxCollider allowedArea;
    [Tooltip("Extra padding inside the bounds (meters).")]
    public float innerPadding = 0.0f;

    [Header("Optional: Drop to ground on release")]
    public bool dropToGroundOnRelease = false;
    public LayerMask groundMask = ~0;

    Camera _cam;
    Rigidbody _rb;
    bool _dragging;
    float _targetY;
    Vector3 _xzOffset;
    Plane _dragPlane;
    Collider _selfCol;
    Vector2 _selfXZExtents;  // half-size on X and Z to prevent clipping outside

    void Awake()
    {
        _cam = Camera.main;
        _rb = GetComponent<Rigidbody>();
        _selfCol = GetComponent<Collider>();
        CacheSelfExtentsXZ();

        Debug.Log("drag waking up");
    }

    void CacheSelfExtentsXZ()
    {
        if (_selfCol)
        {
            var e = _selfCol.bounds.extents;
            _selfXZExtents = new Vector2(e.x, e.z);
        }
        else
        {
            _selfXZExtents = Vector2.zero;
        }
    }

    void OnMouseDown()
    {
        Debug.Log("clicked");

        if (_cam == null) return;

        _targetY = transform.position.y + liftHeight;
        _dragPlane = new Plane(Vector3.up, new Vector3(0f, _targetY, 0f));

        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
        if (_dragPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPointOnPlane = ray.GetPoint(enter);
            Vector3 objAtPlaneY = new Vector3(transform.position.x, _targetY, transform.position.z);
            _xzOffset = objAtPlaneY - hitPointOnPlane;

            MoveImmediate(new Vector3(transform.position.x, _targetY, transform.position.z));

            if (_rb && makeKinematicWhileDragging)
            {
                _rb.isKinematic = true;
                _rb.linearVelocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
            }

            _dragging = true;
        }
    }

    void OnMouseDrag()
    {
        if (!_dragging || _cam == null) return;

        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
        if (_dragPlane.Raycast(ray, out float enter))
        {
            Vector3 target = ray.GetPoint(enter) + _xzOffset;
            target.y = _targetY;

            if (allowedArea)
                target = ClampInsideAreaXZ(target);

            MoveImmediate(target);
        }
    }

    void OnMouseUp()
    {
        if (!_dragging) return;

        if (dropToGroundOnRelease)
        {
            Vector3 start = transform.position + Vector3.up * 0.1f;
            if (Physics.Raycast(start, Vector3.down, out RaycastHit hit, 10f, groundMask, QueryTriggerInteraction.Ignore))
            {
                Vector3 p = transform.position;
                p.y = hit.point.y;
                MoveImmediate(p);
            }
        }
        else if (!keepLiftOnRelease)
        {
            Vector3 p = transform.position;
            p.y = _targetY - liftHeight;
            MoveImmediate(p);
        }

        if (_rb && makeKinematicWhileDragging)
            _rb.isKinematic = false;

        _dragging = false;
    }

    Vector3 ClampInsideAreaXZ(Vector3 worldTarget)
    {
        Bounds b = allowedArea.bounds; // world-space AABB
        // shrink by padding + this object's half extents so it never pokes through edges
        float padX = innerPadding + _selfXZExtents.x;
        float padZ = innerPadding + _selfXZExtents.y;

        float minX = b.min.x + padX;
        float maxX = b.max.x - padX;
        float minZ = b.min.z + padZ;
        float maxZ = b.max.z - padZ;

        worldTarget.x = Mathf.Clamp(worldTarget.x, minX, maxX);
        worldTarget.z = Mathf.Clamp(worldTarget.z, minZ, maxZ);
        // Y is managed elsewhere
        return worldTarget;
    }

    void MoveImmediate(Vector3 worldPos)
    {
        if (_rb && _rb.isKinematic)
            _rb.MovePosition(worldPos);
        else
            transform.position = worldPos;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        // Visualize allowed area in scene view (wire box)
        if (allowedArea)
        {
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.color = new Color(0f, 0.6f, 1f, 0.35f);
            Gizmos.DrawWireCube(allowedArea.bounds.center, allowedArea.bounds.size);

            // Inner shrunken box (padding + object extents) – approximate preview
            if (_selfCol)
            {
                var e = _selfCol.bounds.extents;
                float padX = innerPadding + e.x;
                float padZ = innerPadding + e.z;
                var b = allowedArea.bounds;
                var innerCenter = b.center;
                var innerSize = new Vector3(
                    Mathf.Max(0.0f, b.size.x - 2f * padX),
                    b.size.y,
                    Mathf.Max(0.0f, b.size.z - 2f * padZ)
                );
                Gizmos.color = new Color(0f, 1f, 0.4f, 0.25f);
                Gizmos.DrawWireCube(innerCenter, innerSize);
            }
        }
    }
#endif
}
