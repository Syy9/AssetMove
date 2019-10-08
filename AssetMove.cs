using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Syy.Tools
{
    public class AssetMove : EditorWindow
    {
        [MenuItem("Assets/Move", false, 20)]
        public static void Open()
        {
            var window = CreateInstance<AssetMove>();
            window.minSize = new Vector2(400, 200);
            window.ShowUtility();
        }

        Object[] _targets = new Object[0];
        DefaultAsset _folder;
        Vector2 _scrollPos;

        void OnEnable()
        {
            _targets = Selection.objects;
        }

        void OnGUI()
        {
            if (_targets == null)
            {
                EditorGUILayout.LabelField("Source assets is empty");
                return;
            }

            EditorGUILayout.LabelField("Source assets");
            using (var scroll = new EditorGUILayout.ScrollViewScope(_scrollPos))
            {
                _scrollPos = scroll.scrollPosition;
                foreach (var item in _targets)
                {
                    EditorGUILayout.ObjectField(item, typeof(Object), false);
                }
            }

            _folder = (DefaultAsset)EditorGUILayout.ObjectField("To", _folder, typeof(DefaultAsset), false);
            using (new EditorGUI.DisabledScope(_folder == null || _targets.Length == 0))
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
                        Close();
                    }
                }
            }
            EditorGUILayout.Space();
        }
    }
}
