using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Syy.Tools
{
    public class AssetMove : EditorWindow
    {
        [MenuItem("Assets/Asset Move", false, 20)]
        public static void Open()
        {
            var window = CreateWindow<AssetMove>(nameof(AssetMove));
            window.minSize = new Vector2(400, 200);
            window.Show();
        }

        [SerializeField]
        List<Object> _targets = new List<Object>();
        [SerializeField]
        DefaultAsset _folder;
        Vector2 _scrollPos;

        const string Key_LastFolderPath = "Key_LastFolderPath";

        void OnEnable()
        {
            _targets = Selection.objects.ToList();
            string path = EditorUserSettings.GetConfigValue(Key_LastFolderPath);
            _folder = AssetDatabase.LoadAssetAtPath<DefaultAsset>(path);
        }

        void OnGUI()
        {
            OnDragAndDropUI();

            if (_targets == null || _targets.Count == 0)
            {
                EditorGUILayout.LabelField("Source assets is empty. Drag and Drop here.");
                return;
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Source assets");
                if (GUILayout.Button("Clear", GUILayout.Width(100)))
                {
                    _targets.Clear();
                }
            }
            using (var scroll = new EditorGUILayout.ScrollViewScope(_scrollPos))
            {
                _scrollPos = scroll.scrollPosition;
                foreach (var item in _targets)
                {
                    EditorGUILayout.ObjectField(item, typeof(Object), false);
                }
            }

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                _folder = (DefaultAsset)EditorGUILayout.ObjectField("To", _folder, typeof(DefaultAsset), false);
                if (check.changed && _folder != null)
                {
                    string assetPath = AssetDatabase.GetAssetPath(_folder);
                    EditorUserSettings.SetConfigValue(Key_LastFolderPath, assetPath);
                }
            }
            using (new EditorGUI.DisabledScope(_folder == null || _targets.Count == 0))
            {
                if (GUILayout.Button("Move"))
                {
                    foreach (var target in _targets)
                    {
                        if (target == null)
                        {
                            continue;
                        }
                        string oldPath = AssetDatabase.GetAssetPath(target);
                        string fileName = Path.GetFileName(oldPath);
                        string newPath = Path.Combine(AssetDatabase.GetAssetPath(_folder), fileName);
                        Debug.Log($"[Move] from {oldPath}. to {newPath}");
                        AssetDatabase.MoveAsset(oldPath, newPath);
                    }
                }
            }
            EditorGUILayout.Space();
        }

        void OnDragAndDropUI()
        {
            var currentEvent = Event.current;
            var dropArea = new Rect(0, 0, position.width, position.height);
            GUI.Box(dropArea, "");
            switch (currentEvent.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dropArea.Contains(currentEvent.mousePosition)) break;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (currentEvent.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        var newTargets = DragAndDrop.objectReferences.Where(value => !_targets.Contains(value)).ToList();
                        _targets.AddRange(newTargets);
                        DragAndDrop.activeControlID = 0;
                    }
                    Event.current.Use();
                    break;
            }
        }
    }
}
