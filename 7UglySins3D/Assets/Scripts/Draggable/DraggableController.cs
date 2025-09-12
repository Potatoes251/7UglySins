using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine.InputSystem;


/// <summary>
/// SO-driven draggable: multi-mesh proof visuals, single root MeshCollider, Y-locked dragging.
/// Safe in Edit Mode via deferred rebuilds; no stripping or mutation of artist assets.
/// </summary>
[ExecuteAlways]
[RequireComponent(typeof(Collider))]
public class DraggableController : MonoBehaviour
{
    [Header("Configs")]
    public DraggableVisualConfig visual;
    public DraggableParams parameters;

    Vector3 _pendingPhysicsPos;
    bool _hasPendingPhysicsPos;
    RigidbodyInterpolation _prevInterpolation;

    [Header("Scene Refs (auto if null)")]
    [Tooltip("Container for all renderers. Auto-created if missing.")]
    public Transform visualRoot;
    [Tooltip("Root MeshCollider used for picking/drag. Auto-created if missing.")]
    public MeshCollider meshCollider;

    // Runtime caches
    Camera _cam;
    Rigidbody _rb;
    Collider _col;

    bool _dragging;
    float _targetY;
    Plane _dragPlane;
    Vector3 _xzOffset;
    Vector2 _selfXZExtents;
    BoxCollider _allowedArea;

    bool _rebuildScheduled;

    static Vector2 _restoreScreenPos;
    static bool _hasRestoreScreenPos;

    // Cursor control (shared across all draggables in scene)
    static int _cursorHideRefs;
    static bool _cursorPrevVisible;
    static CursorLockMode _cursorPrevLock;

    // optional: toggle via params (fallback true if params is null)
    bool HideCursorFlag => parameters ? parameters.hideCursorWhileDragging : true;
    bool ConfineCursorFlag => parameters ? parameters.confineCursorWhileDragging : true;





    void MoveImmediate(Vector3 worldPos)
    {
        if (_rb == null) _rb = GetComponent<Rigidbody>();

        // If dragging a kinematic body, set position directly for 1:1 cursor tracking.
        if (_dragging && _rb && _rb.isKinematic)
        {
            _rb.position = worldPos;          // exact, no physics timestep delay
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            return;
        }

        // If we have a non-kinematic body, queue MovePosition for FixedUpdate.
        if (_rb && !_rb.isKinematic)
        {
            _pendingPhysicsPos = worldPos;
            _hasPendingPhysicsPos = true;
            return;
        }

        // No rigidbody: move the transform directly.
        transform.position = worldPos;
    }

    // ===== Editor/Init =====
    void OnEnable()
    {
        EnsureBasics();
#if UNITY_EDITOR
        if (!Application.isPlaying) ScheduleEditorRebuild();
#endif
    }

    void OnValidate()
    {
        EnsureBasics();
#if UNITY_EDITOR
        if (!Application.isPlaying) { ScheduleEditorRebuild(); return; }
#endif
        if (Application.isPlaying) ApplyInternal(playMode: true);
    }

    void Awake()
    {
        _cam = Camera.main;
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<Collider>();
        CacheSelfExtentsXZ();
        RefreshAllowedArea();
    }

    void EnsureBasics()
    {
        if (visualRoot == null)
        {
            var t = transform.Find("Visual");
            if (t == null)
            {
                var go = new GameObject("Visual");
                go.transform.SetParent(transform, false);
                visualRoot = go.transform;
            }
            else visualRoot = t;
        }
        if (meshCollider == null)
        {
            meshCollider = GetComponent<MeshCollider>();
            if (meshCollider == null) meshCollider = gameObject.AddComponent<MeshCollider>();
            meshCollider.convex = true;
        }
    }

    void FixedUpdate()
    {
        if (_hasPendingPhysicsPos && _rb && !_rb.isKinematic)
        {
            _rb.MovePosition(_pendingPhysicsPos);
            _hasPendingPhysicsPos = false;
        }
    }

#if UNITY_EDITOR
    void ScheduleEditorRebuild()
    {
        if (_rebuildScheduled) return;
        _rebuildScheduled = true;
        EditorApplication.delayCall += () =>
        {
            if (this == null) return;
            _rebuildScheduled = false;
            if (!Application.isPlaying) ApplyInternal(playMode: false, forceImmediateEditorDestroy: true);
        };
    }

    [ContextMenu("Rebuild Visuals Now (Editor)")]
    public void RebuildVisualsInEditorNow()
    {
        if (Application.isPlaying) return;
        ApplyInternal(playMode: false, forceImmediateEditorDestroy: true);
    }
#endif

    // ===== Core apply (safe both in editor and play) =====
    void ApplyInternal(bool playMode, bool forceImmediateEditorDestroy = false)
    {
        EnsureBasics();

        // 1) Clear previous visuals safely
        ClearChildrenOf(visualRoot, playMode, forceImmediateEditorDestroy);

        // 2) Build visuals (multi-mesh proof)
        Mesh firstFoundMesh = null;

        if (visual != null && visual.modelPrefab != null)
        {
#if UNITY_EDITOR
            var inst = (GameObject)PrefabUtility.InstantiatePrefab(visual.modelPrefab, visualRoot);
#else
            var inst = Instantiate(visual.modelPrefab, visualRoot);
#endif
            inst.transform.localPosition = Vector3.zero;
            inst.transform.localRotation = Quaternion.identity;
            inst.transform.localScale = Vector3.one;

            // Optional global material override
            if (visual.materialsOverride != null && visual.materialsOverride.Length > 0)
            {
                foreach (var mr in inst.GetComponentsInChildren<MeshRenderer>(true))
                    mr.sharedMaterials = visual.materialsOverride;
            }

            var mf = inst.GetComponentInChildren<MeshFilter>(true);
            if (mf && mf.sharedMesh) firstFoundMesh = mf.sharedMesh;
        }
        else if (visual != null && visual.meshes != null && visual.meshes.Length > 0)
        {
            for (int i = 0; i < visual.meshes.Length; i++)
            {
                var m = visual.meshes[i];
                if (!m) continue;

                var go = new GameObject($"Part_{i}");
                go.transform.SetParent(visualRoot, false);

                var mf = go.AddComponent<MeshFilter>();
                mf.sharedMesh = m;

                var mr = go.AddComponent<MeshRenderer>();
                if (visual.materialsOverride != null && visual.materialsOverride.Length > 0)
                {
                    var mat = i < visual.materialsOverride.Length ? visual.materialsOverride[i] : visual.materialsOverride[0];
                    mr.sharedMaterial = mat;
                }

                if (firstFoundMesh == null) firstFoundMesh = m;
            }
        }

        // 3) Root collider mesh
        Mesh colMesh = (visual != null && visual.collisionMesh != null) ? visual.collisionMesh : firstFoundMesh;
        meshCollider.sharedMesh = colMesh;
        meshCollider.convex = true;

#if UNITY_EDITOR
        if (!playMode)
        {
            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(meshCollider);
        }
#endif

        // 4) Refresh caches for drag/clamp
        _col = GetComponent<Collider>();
        CacheSelfExtentsXZ();
        RefreshAllowedArea();
    }

    static void ClearChildrenOf(Transform root, bool playMode, bool forceImmediateEditorDestroy)
    {
        for (int i = root.childCount - 1; i >= 0; i--)
        {
            var child = root.GetChild(i);
#if UNITY_EDITOR
            if (!playMode && forceImmediateEditorDestroy)
                Object.DestroyImmediate(child.gameObject);
            else if (!playMode)
                Object.DestroyImmediate(child.gameObject); // deferred via delayCall already
            else
                Object.Destroy(child.gameObject);
#else
            if (playMode) Object.Destroy(child.gameObject);
            else Object.Destroy(child.gameObject);
#endif
        }
    }

    // ===== Runtime drag =====
    void Update()
    {
        if (!Application.isPlaying) return;
        if (_cam == null) _cam = Camera.main;
        if (_cam == null) return;

        bool pressed = GetMouseDown();
        bool held = GetMouseHeld();
        bool released = GetMouseUp();

        if (!_dragging && pressed)
        {
            if (TryPick(GetMousePos(), out var hit) && hit.collider == _col)
                BeginDrag(GetMousePos());
        }

        if (_dragging)
        {
            if (held) DragTo(GetMousePos());
            if (released) EndDrag();
        }
    }

    bool TryPick(Vector2 screenPos, out RaycastHit hit)
    {
        Ray ray = _cam.ScreenPointToRay(screenPos);
        int mask = parameters ? parameters.draggableMask : ~0;
        return Physics.Raycast(ray, out hit, 1000f, mask, QueryTriggerInteraction.Ignore);
    }

    void BeginDrag(Vector2 screenPos)
    {

        if (HideCursorFlag) RequestHideCursor(ConfineCursorFlag);

        float lift = parameters ? parameters.liftHeight : 0.15f;
        _targetY = transform.position.y + lift;
        _dragPlane = new Plane(new Vector3(0f, 1f, 0.6f), new Vector3(0f, _targetY, 0f));

        Ray ray = _cam.ScreenPointToRay(screenPos);
        if (_dragPlane.Raycast(ray, out float enter))
        {
            Vector3 planeHit = ray.GetPoint(enter);
            Vector3 objAtY = new Vector3(transform.position.x, _targetY, transform.position.z);
            _xzOffset = objAtY - planeHit;
        }

        MoveImmediate(new Vector3(transform.position.x, _targetY, transform.position.z));

        _rb = GetComponent<Rigidbody>();
        bool makeKin = parameters ? parameters.makeKinematicWhileDragging : true;
        if (_rb && makeKin)
        {
#if UNITY_2022_3_OR_NEWER
            _rb.linearVelocity = Vector3.zero;
#else
        _rb.velocity = Vector3.zero;
#endif
            _rb.angularVelocity = Vector3.zero;

            // turn kinematic + disable interpolation so position writes are exact each frame
            _prevInterpolation = _rb.interpolation;
            _rb.interpolation = RigidbodyInterpolation.None;
            _rb.isKinematic = true;
        }

        _dragging = true;
    }

    void DragTo(Vector2 screenPos)
    {
        Ray ray = _cam.ScreenPointToRay(screenPos);
        if (!_dragPlane.Raycast(ray, out float enter)) return;

        Vector3 p = ray.GetPoint(enter) + _xzOffset;
        p.y = _targetY;

        var area = _allowedArea;
        if (area) p = ClampInsideAreaXZ(p, area, parameters ? parameters.innerPadding : 0f);

        MoveImmediate(p);
    }

    void EndDrag()
    {

        if (parameters && !parameters.keepLiftOnRelease)
        {
            var p = transform.position;
            p.y = _targetY - parameters.liftHeight;
            MoveImmediate(p);
        }

        if (_rb && (parameters ? parameters.makeKinematicWhileDragging : true))
        {
            // restore interpolation and kinematic state
            _rb.isKinematic = false;
            _rb.interpolation = _prevInterpolation;
        }

        if (parameters && parameters.dropToGroundOnRelease)
        {
            Vector3 start = transform.position + Vector3.up * 0.1f;
            if (Physics.Raycast(start, Vector3.down, out RaycastHit hit, 10f, parameters.groundMask, QueryTriggerInteraction.Ignore))
            {
                Vector3 p = transform.position;
                p.y = hit.point.y + meshCollider.bounds.size.y * 0.5f;
                MoveImmediate(p);
            }
        }

        _restoreScreenPos = GetMousePos();
        _hasRestoreScreenPos = true;

        if (HideCursorFlag) ReleaseHideCursor(); // (your existing call)
        _dragging = false;

    }

    Vector3 ClampInsideAreaXZ(Vector3 p, BoxCollider area, float innerPad)
    {
        Bounds b = area.bounds; // world AABB
        float padX = innerPad + _selfXZExtents.x;
        float padZ = innerPad + _selfXZExtents.y;
        p.x = Mathf.Clamp(p.x, b.min.x + padX, b.max.x - padX);
        p.z = Mathf.Clamp(p.z, b.min.z + padZ, b.max.z - padZ);
        return p;
    }

    void CacheSelfExtentsXZ()
    {
        _col = _col ?? GetComponent<Collider>();
        if (_col != null)
        {
            var e = _col.bounds.extents;
            _selfXZExtents = new Vector2(e.x, e.z);
        }
    }

    void RefreshAllowedArea()
    {
        _allowedArea = parameters ? parameters.allowedArea : null;

        if (_allowedArea == null && parameters && parameters.autoFindPlayAreaByTag && !string.IsNullOrEmpty(parameters.playAreaTag))
        {
            var go = GameObject.FindWithTag(parameters.playAreaTag);
            if (go) _allowedArea = go.GetComponent<BoxCollider>();
        }
    }

    static void RequestHideCursor(bool confine)
    {
        if (!Application.isPlaying) return; // don’t touch editor cursor
        if (_cursorHideRefs++ == 0)
        {
            _cursorPrevVisible = Cursor.visible;
            _cursorPrevLock = Cursor.lockState;
            Cursor.visible = false;
            Cursor.lockState = confine ? CursorLockMode.Confined : CursorLockMode.None;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }

    static void ReleaseHideCursor()
    {
        if (!Application.isPlaying) return;
        _cursorHideRefs = Mathf.Max(0, _cursorHideRefs - 1);
        if (_cursorHideRefs == 0)
        {
            Cursor.visible = _cursorPrevVisible;
            Cursor.lockState = _cursorPrevLock;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

            if (_hasRestoreScreenPos && Mouse.current != null)
            {
                Mouse.current.WarpCursorPosition(_restoreScreenPos);
                // No InputState.Change; not needed and not public.
                // If you need to consume the restored position immediately,
                // use _restoreScreenPos instead of reading the mouse this frame.
            }

            _hasRestoreScreenPos = false;
        }
    }

    // --- Input helpers (old & new) ---
    Vector2 GetMousePos()
    {
#if ENABLE_INPUT_SYSTEM
        var m = UnityEngine.InputSystem.Mouse.current;
        if (m != null) return m.position.ReadValue();
#endif
        return Input.mousePosition;
    }
    bool GetMouseDown()
    {
#if ENABLE_INPUT_SYSTEM
        var m = UnityEngine.InputSystem.Mouse.current;
        if (m != null && m.leftButton.wasPressedThisFrame) return true;
#endif
        return Input.GetMouseButtonDown(0);
    }
    bool GetMouseHeld()
    {
#if ENABLE_INPUT_SYSTEM
        var m = UnityEngine.InputSystem.Mouse.current;
        if (m != null && m.leftButton.isPressed) return true;
#endif
        return Input.GetMouseButton(0);
    }
    bool GetMouseUp()
    {
#if ENABLE_INPUT_SYSTEM
        var m = UnityEngine.InputSystem.Mouse.current;
        if (m != null && m.leftButton.wasReleasedThisFrame) return true;
#endif
        return Input.GetMouseButtonUp(0);
    }


    void OnDisable()
    {



#if UNITY_EDITOR
        if (!Application.isPlaying) return;
#endif
        if (_dragging)
        {
            _restoreScreenPos = GetMousePos();
            _hasRestoreScreenPos = true;
            if (HideCursorFlag) ReleaseHideCursor();
        }
        _dragging = false;
    }
}
