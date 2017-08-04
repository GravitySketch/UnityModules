﻿using UnityEngine;
using UnityEditor;
using Leap.Unity.Query;

namespace Leap.Unity {

  [CustomPropertyDrawer(typeof(AssetFolder))]
  public class AssetFolderPropertyDrawer : PropertyDrawer {

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
      Rect left, right;
      position.SplitHorizontallyWithRight(out left, out right, position.height);
      left.width -= 2;

      Object folderAsset = null;
      string folderPath = "";

      SerializedProperty folderProp = property.FindPropertyRelative("_assetFolder");
      if (folderProp.hasMultipleDifferentValues) {
        EditorGUI.showMixedValue = true;
      } else {
        folderAsset = folderProp.objectReferenceValue;
        if (folderAsset != null) {
          folderPath = AssetDatabase.GetAssetPath(folderAsset);
        }
      }

      EditorGUI.TextField(left, label, folderPath);

      var content = EditorGUIUtility.IconContent("Folder Icon");

      if (GUI.Button(right, content, GUIStyle.none)) {
        string resultPath = EditorUtility.OpenFolderPanel("Select Folder", folderPath, "");
        if (!string.IsNullOrEmpty(resultPath)) {
          string relativePath = Utils.MakeRelativePath(Application.dataPath, resultPath);
          var asset = AssetDatabase.LoadAssetAtPath<DefaultAsset>(relativePath);

          if (asset != null) {
            folderProp.objectReferenceValue = asset;
          }
        }
      }

      EditorGUI.showMixedValue = false;

      if (position.Contains(Event.current.mousePosition)) {
        var draggedFolder = DragAndDrop.objectReferences.Query().OfType<DefaultAsset>().FirstOrDefault();
        if (draggedFolder != null) {
          switch (Event.current.type) {
            case EventType.DragUpdated:
              DragAndDrop.visualMode = DragAndDropVisualMode.Link;
              break;
            case EventType.DragPerform:
              DragAndDrop.AcceptDrag();
              folderProp.objectReferenceValue = draggedFolder;
              break;
          }
        }
      }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
      return EditorGUIUtility.singleLineHeight;
    }
  }
}
