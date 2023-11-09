#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Object = UnityEngine.Object;


public class ParallaxObjectCreator
{
    [MenuItem("GameObject/XR/ScrollableObject",priority = 99)]
    public static void CreatePrefab()
    {
        Object prefab = AssetDatabase.LoadAssetAtPath<Object>( "Assets/Develop/Prefab/Layer/ScrollableObject.prefab" );
        GameObject gameObject = PrefabUtility.InstantiatePrefab(prefab)as GameObject;
        Place(gameObject);
    }

    public static void Place(GameObject gameObject)
    {
        SceneView sceneView = SceneView.lastActiveSceneView;
        gameObject.transform.position = sceneView ? sceneView.pivot : Vector3.zero;
        
        StageUtility.PlaceGameObjectInCurrentStage(gameObject);
        GameObjectUtility.EnsureUniqueNameForSibling(gameObject);
        
        Undo.RegisterCreatedObjectUndo(gameObject,$"Create Object : {gameObject.name}");
        Selection.activeObject = gameObject;

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

    }

    //미구현부 차후 구현 예정
    /*
    public static void SceneViewDragAndDrop(SceneView sceneview)
    {
        UnityEngine.Event current = UnityEngine.Event.current;
        UnityEngine.Object[] references = DragAndDrop.objectReferences;

        if (current.type == EventType.Layout)
            return;
        if (references.Length == 1)
        {
            Texture2D texture2DAsset = references[0] as Texture2D;
            if (texture2DAsset != null)
            {
                Vector2 mousePos = current.mousePosition;
                
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                Handles.BeginGUI();
                GUI.Label(new Rect(mousePos + new Vector2(20f, 20f), new Vector2(400f, 20f)), new GUIContent(string.Format("Create Spine GameObject ({0})", skeletonDataAsset.skeletonJSON.name), SpineEditorUtilities.Icons.skeletonDataAssetIcon));
                Handles.EndGUI();
                
                if (current.type == EventType.DragPerform) {
                    RectTransform rectTransform = (Selection.activeGameObject == null) ? null : Selection.activeGameObject.GetComponent<RectTransform>();
                    Plane plane = (rectTransform == null) ? new Plane(Vector3.back, Vector3.zero) : new Plane(-rectTransform.forward, rectTransform.position);
                    Vector3 spawnPoint = MousePointToWorldPoint2D(mousePos, sceneview.camera, plane);
                    ShowInstantiateContextMenu(texture2DAsset, spawnPoint, null, 0);
                    DragAndDrop.AcceptDrag();
                    current.Use();
                }
            }
        }
        
    }
    
    public static void ShowInstantiateContextMenu (Texture2D texture2DAsset, Vector3 spawnPoint,
        Transform parent, int siblingIndex = 0) {
        GenericMenu menu = new GenericMenu();

        // SkeletonAnimation
        menu.AddItem(new GUIContent("Scrollable"),false,()=>{i});

        // SkeletonGraphic
        System.Type skeletonGraphicInspectorType = System.Type.GetType("Spine.Unity.Editor.SkeletonGraphicInspector");
        if (skeletonGraphicInspectorType != null) {
            MethodInfo graphicInstantiateDelegate = skeletonGraphicInspectorType.GetMethod("SpawnSkeletonGraphicFromDrop", BindingFlags.Static | BindingFlags.Public);
            if (graphicInstantiateDelegate != null)
                menu.AddItem(new GUIContent("SkeletonGraphic (UI)"), false, HandleSkeletonComponentDrop, new SpawnMenuData {
                    skeletonDataAsset = skeletonDataAsset,
                    spawnPoint = spawnPoint,
                    parent = parent,
                    siblingIndex = siblingIndex,
                    instantiateDelegate = System.Delegate.CreateDelegate(typeof(EditorInstantiation.InstantiateDelegate), graphicInstantiateDelegate) as EditorInstantiation.InstantiateDelegate,
                    isUI = true
                });
        }

        menu.ShowAsContext();
    }
    
    static Vector3 MousePointToWorldPoint2D (Vector2 mousePosition, Camera camera, Plane plane) {
        Vector3 screenPos = new Vector3(mousePosition.x, camera.pixelHeight - mousePosition.y, 0f);
        Ray ray = camera.ScreenPointToRay(screenPos);
        float distance;
        bool hit = plane.Raycast(ray, out distance);
        return ray.GetPoint(distance);
    }
    */
    
}

#endif