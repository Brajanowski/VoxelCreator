using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BB.VoxelCreator {
  [CustomEditor(typeof(VoxelColorPalette))]
  public class VoxelColorPaletteEditor : Editor {
    public override void OnInspectorGUI() {
      VoxelColorPalette palette = (VoxelColorPalette)target;
    
      GUILayout.Label("Palette");
      for (int i = 0; i < palette.colors.Count; ++i) {
        EditorGUILayout.BeginHorizontal();
        palette.colors[i] = EditorGUILayout.ColorField(palette.colors[i]);
        if (GUILayout.Button("X", EditorStyles.miniButton)) {
          palette.colors.RemoveAt(i);
          --i;
        }
        EditorGUILayout.EndHorizontal();
      }

      if (GUILayout.Button("Add a new color")) {
        palette.colors.Add(Color.white);
      }

      EditorUtility.SetDirty(palette);
    }
  }
}