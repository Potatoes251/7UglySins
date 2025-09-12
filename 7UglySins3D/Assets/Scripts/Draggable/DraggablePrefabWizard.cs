/*using UnityEngine;
using UnityEditor;

public class DraggablePrefabWizard : EditorWindow
{
    [Header("Configs")]
    public DraggableVisualConfig visualConfig;
    public DraggableParams paramsConfig;

    [Header("Output")]
    public string prefabName = "Draggable_New";
    public DefaultAsset outputFolder;

    [Header("Root Rigidbody")]
    public bool addRigidbody = true;
    public bool useGravity = true;
    public bool startKinematic = false;

    [MenuItem("Assets/Create/Drag/Create Draggable Prefab (SO-driven)...", priority = 0)]
    public static void Open()
    {
        var w = CreateInstance<DraggablePrefabWizard>();
        w.titleContent = new GUIContent("Draggable Prefab (SO-driven)");
        w.minSize = new Vector2(520, 260);
        w.ShowUtility();
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Create a draggable prefab from Visual + Params Scriptables", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        visualConfig = (DraggableVisualConfig)EditorGUILayout.ObjectField("Visual Config", visualConfig, typeof(DraggableVisualConfig), false);
        paramsConfig = (DraggableParams)EditorGUILayout.ObjectField("Params Config", paramsConfig, typeof(DraggableParams), false);

        EditorGUILayout.Space();
        prefabName = EditorGUILayout.TextField("Prefab Name", prefabName);
        outputFolder = (DefaultAsset)EditorGUILayout.ObjectField("Output Folder", outputFolder, typeof(DefaultAsset), false);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Root Rigidbody", EditorStyles.boldLabel);
        addRigidbody = EditorGUILayout.Toggle("Add Rigidbody", addRigidbody);
        using (new EditorGUI.DisabledScope(!addRigidbody))
        {
            useGravity = EditorGUILayout.Toggle("  Use Gravity", useGravity);
            startKinematic = EditorGUILayout.Toggle("  Start Kinematic", startKinematic);
        }

        GUILayout.FlexibleSpace();
        using (new EditorGUI.DisabledScope(!CanCreate()))
        {
            if (GUILayout.Button("Create Prefab", GUILayout.Height(28)))
                CreatePrefab();
        }

        if (!CanCreate())
            EditorGUILayout.HelpBox("Assign both Scriptables, output folder, and name.", MessageType.Info);
    }

    bool CanCreate() => visualConfig && paramsConfig && outputFolder && !string.IsNullOrEmpty(prefabName);

    void CreatePrefab()
    {
        string folderPath = AssetDatabase.GetAssetPath(outputFolder);
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            EditorUtility.DisplayDialog("Invalid Folder", "Please select a valid project folder.", "OK");
            return;
        }

        // Resolve a collider mesh for the root MeshCollider
        Mesh colliderMesh = visualConfig ? visualConfig.collisionMesh : null;
        if (!colliderMesh && visualConfig)
        {
            if (visualConfig.modelPrefab)
            {
                var mf = visualConfig.modelPrefab.GetComponentInChildren<MeshFilter>(true);
                if (mf) colliderMesh = mf.sharedMesh;
            }
            if (!colliderMesh && visualConfig.meshes != null && visualConfig.meshes.Length > 0)
            {
                colliderMesh = visualConfig.meshes[0];
            }
        }

        // Root
        var root = new GameObject(prefabName);

        var mc = root.AddComponent<MeshCollider>();
        mc.sharedMesh = colliderMesh;
        mc.convex = true;

        if (addRigidbody)
        {
            var rb = root.AddComponent<Rigidbody>();
            rb.useGravity = useGravity;
            rb.isKinematic = startKinematic;
        }

        var ctrl = root.AddComponent<DraggableController>();
        ctrl.visual = visualConfig;
        ctrl.parameters = paramsConfig;

        // Build visuals safely outside OnValidate
        ctrl.RebuildVisualsInEditorNow();

        string outPath = AssetDatabase.GenerateUniqueAssetPath($"{folderPath}/{prefabName}.prefab");
        var saved = PrefabUtility.SaveAsPrefabAsset(root, outPath);
        Object.DestroyImmediate(root);

        if (saved)
        {
            AssetDatabase.SaveAssets(); AssetDatabase.Refresh();
            Selection.activeObject = saved; EditorGUIUtility.PingObject(saved);
            EditorUtility.DisplayDialog("Success", $"Created prefab:\n{outPath}", "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Error", "Failed to save prefab.", "OK");
        }
    }
}
*/