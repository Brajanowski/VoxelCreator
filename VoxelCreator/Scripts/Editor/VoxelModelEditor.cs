using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BB.VoxelCreator {
  [CustomEditor(typeof(VoxelModel))]
  public class VoxelModelEditor : Editor {
    enum EditMode {
      Place,
      Erase,
      Paint
    };
    EditMode editMode = EditMode.Place;

    int colorIndex = -1;

    public override void OnInspectorGUI() {
      VoxelModel model = (VoxelModel)target;

      model.voxelModelRenderer = (VoxelModelRenderer)EditorGUILayout.ObjectField("Voxel Model Renderer", model.voxelModelRenderer, typeof(VoxelModelRenderer), true);

      if (model.voxelModelRenderer == null) {
        GUILayout.Label("Please set VoxelModelRenderer reference to continue");
        return;
      }

      GUILayout.Label("Size");
      model.width = EditorGUILayout.IntField("Width", model.width);
      model.height = EditorGUILayout.IntField("Height", model.height);
      model.depth = EditorGUILayout.IntField("Depth", model.depth);
      model.voxelSize = EditorGUILayout.FloatField("Voxel Size", model.voxelSize);

      if (GUILayout.Button("Apply new dimensions (WARNING it will destroy current progress)")) {
        model.blocks = new int[model.width * model.height * model.depth];
        model.voxelModelRenderer.Regenerate();
      }

      if (GUILayout.Button("Regenerate mesh")) {
        model.voxelModelRenderer.Regenerate();
      }

      EditorGUILayout.Space();

      model.palette = (VoxelColorPalette)EditorGUILayout.ObjectField("Color palette", model.palette, typeof(VoxelColorPalette), false);
      if (model.palette == null) {
        GUILayout.Label("We need palette to continue our work!");
        return;
      }

      EditorGUILayout.Space();

      if (model.blocks == null) {
        GUILayout.Label("Blocks are not initialized, please apply new dimensions");
        return;
      }

      if (GUILayout.Button("Randomize")) {
        Random.InitState(System.DateTime.Now.Millisecond);
        for (int x = 0; x < model.width; ++x) {
          for (int y = 0; y < model.height; ++y) {
            for (int z = 0; z < model.depth; ++z) {
              model.SetBlock(Random.Range(0, model.palette.colors.Count + 1), x, y, z);
            }
          }
        }

        model.voxelModelRenderer.Regenerate();
      }

      EditorGUILayout.Space();
      GUILayout.Label("Tools (Current mode: " + editMode.ToString() + ")", EditorStyles.boldLabel);

      EditorGUILayout.BeginHorizontal();

      if (GUILayout.Button("Place mode", GUILayout.Width(96), GUILayout.Height(32))) {
        editMode = EditMode.Place;
        //colorIndex = -1;
      }

      if (GUILayout.Button("Erasing mode", GUILayout.Width(96), GUILayout.Height(32))) {
        editMode = EditMode.Erase;
        colorIndex = -1;
      }

      EditorGUILayout.EndHorizontal();

      if (GUILayout.Button("Reset position(must be at 0, 0, 0)")) {
        model.transform.position = Vector3.zero;
      }

      if (GUILayout.Button("Fill")) {
        for (int x = 0; x < model.width; ++x) {
          for (int y = 0; y < model.height; ++y) {
            for (int z = 0; z < model.depth; ++z) {
              model.SetBlock(colorIndex + 1, x, y, z);
            }
          }
        }

        model.voxelModelRenderer.Regenerate();
      }

      if (GUILayout.Button("Generate sphere")) {
        float radius = Mathf.Min(model.width, model.height, model.depth) / 2.0f;
        for (int x = 0; x < model.width; ++x) {
          for (int y = 0; y < model.height; ++y) {
            for (int z = 0; z < model.depth; ++z) {
              if (Vector3.Distance(new Vector3(model.width / 2.0f, model.height / 2.0f, model.depth / 2.0f), new Vector3(x, y, z)) <= radius) {
                model.SetBlock(colorIndex + 1, x, y, z);
              }
            }
          }
        }

        model.voxelModelRenderer.Regenerate();
      }

      if (GUILayout.Button("Generate base")) {
        float radius = Mathf.Min(model.width, model.height, model.depth) / 2.0f;
        for (int x = 0; x < model.width; ++x) {
          for (int z = 0; z < model.depth; ++z) {
            model.SetBlock(colorIndex + 1, x, 0, z);
          }
        }

        model.voxelModelRenderer.Regenerate();
      }

      EditorGUILayout.Space();
      GUILayout.Label("Palette", EditorStyles.boldLabel);
      for (int i = 0; i < model.palette.colors.Count; ++i) {
        EditorGUILayout.BeginVertical(GUILayout.Height(48));
        EditorGUILayout.BeginHorizontal(GUILayout.Width(Screen.width / 2.0f));

        if (GUILayout.Button("Select", GUILayout.Height(32))) {
          colorIndex = i;
          editMode = EditMode.Paint;
        }

        Rect buttonRect = GUILayoutUtility.GetLastRect();

        if (colorIndex == i) {
          EditorGUI.DrawRect(new Rect(buttonRect.x + buttonRect.width + 4, buttonRect.y - 4, 40, 40), Color.red);
        }

        EditorGUI.DrawRect(new Rect(buttonRect.x + buttonRect.width + 8, buttonRect.y, 32, 32), model.palette.colors[i]);
        
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
      }

      EditorGUILayout.Space();
      if (GUILayout.Button("Save as mesh asset")) {
        Mesh meshToSave = (Mesh)Instantiate(model.mesh);

        string path = EditorUtility.SaveFilePanelInProject("Saving voxel mesh", "VM_Unknown", "asset", "Choose path where you wanna save model");
        Debug.Log(path);

        AssetDatabase.CreateAsset(meshToSave, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // create and save prefab with pivot at the center
        GameObject modelParent = new GameObject(path.Substring(path.LastIndexOf('/')));
        GameObject modelPrefab = new GameObject("GFX");
        modelPrefab.AddComponent<MeshRenderer>().sharedMaterial = model.GetComponent<MeshRenderer>().sharedMaterial;
        modelPrefab.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
        modelPrefab.AddComponent<MeshFilter>().mesh = meshToSave;
        modelPrefab.AddComponent<MeshCollider>().sharedMesh = meshToSave;
        modelPrefab.transform.parent = modelParent.transform;
        modelPrefab.transform.localPosition = new Vector3(model.width * (model.voxelSize / 2.0f), model.height * (model.voxelSize / 2.0f), model.depth * (model.voxelSize / 2.0f)) * -1;
        EditorUtility.SetDirty(modelParent);
        PrefabUtility.CreatePrefab(path.Substring(0, path.LastIndexOf('.')) + ".prefab", modelParent);
        DestroyImmediate(modelParent);
        DestroyImmediate(modelPrefab);
      }
    }

    bool CanModelBeEdited(VoxelModel model) {
      if (model.blocks == null ||
          model.palette == null ||
          model.voxelModelRenderer == null ||
          model.voxelSize == 0) {
        return false;
      }
      return true;
    }

    // NOTE(Brajan): in voxels
    Vector3 firstHitPosition;
    bool wasFirstHit;

    void OnSceneGUI() {
      VoxelModel model = (VoxelModel)target;
      if (!CanModelBeEdited(model))
        return;

      float voxelHalfSize = model.voxelSize / 2.0f;
      Handles.DrawWireCube(new Vector3(model.width * (voxelHalfSize), model.height * (voxelHalfSize), model.depth * (voxelHalfSize)), new Vector3(model.width * (model.voxelSize), model.height * (model.voxelSize), model.depth * (model.voxelSize)));

      if (wasFirstHit) {
        Handles.color = Color.red;
        Handles.DrawWireCube(firstHitPosition * model.voxelSize + new Vector3(voxelHalfSize, voxelHalfSize, voxelHalfSize), new Vector3(model.voxelSize, model.voxelSize, model.voxelSize));
        Handles.color = Color.white;
      }


      // TODO(Brajan): Refactor this to avoid double Raycast function call
      Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
      RaycastHit hit;

      if (Physics.Raycast(ray, out hit, 1000)) {
        Vector3 rawPosition;
        if (editMode == EditMode.Place)
          rawPosition = hit.point + (hit.normal * voxelHalfSize);
        else
          rawPosition = hit.point - (hit.normal * voxelHalfSize);

        int x = Mathf.FloorToInt(rawPosition.x / model.voxelSize);
        int y = Mathf.FloorToInt(rawPosition.y / model.voxelSize);
        int z = Mathf.FloorToInt(rawPosition.z / model.voxelSize);

        Handles.color = Color.gray;
        Handles.DrawWireCube(new Vector3(x, y, z) * model.voxelSize + new Vector3(voxelHalfSize, voxelHalfSize, voxelHalfSize), new Vector3(model.voxelSize, model.voxelSize, model.voxelSize));
        Handles.color = Color.white;
      }

      if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.control) {
        if (Physics.Raycast(ray, out hit, 1000)) {
          if (!wasFirstHit) {
            Vector3 rawPosition;
            if (editMode == EditMode.Place)
              rawPosition = hit.point + (hit.normal * voxelHalfSize);
            else
              rawPosition = hit.point - (hit.normal * voxelHalfSize);

            int x = Mathf.FloorToInt(rawPosition.x / model.voxelSize);
            int y = Mathf.FloorToInt(rawPosition.y / model.voxelSize);
            int z = Mathf.FloorToInt(rawPosition.z / model.voxelSize);

            firstHitPosition = new Vector3(x, y, z);

            wasFirstHit = true;
          } else {
            Vector3 rawPosition = hit.point + (hit.normal * voxelHalfSize);

            if (editMode == EditMode.Place)
              rawPosition = hit.point + (hit.normal * voxelHalfSize);
            else
              rawPosition = hit.point - (hit.normal * voxelHalfSize);

            int secondX = Mathf.FloorToInt(rawPosition.x / model.voxelSize);
            int secondY = Mathf.FloorToInt(rawPosition.y / model.voxelSize);
            int secondZ = Mathf.FloorToInt(rawPosition.z / model.voxelSize);

            int startX = (int)firstHitPosition.x > secondX ? secondX : (int)firstHitPosition.x;
            int startY = (int)firstHitPosition.y > secondY ? secondY : (int)firstHitPosition.y;
            int startZ = (int)firstHitPosition.z > secondZ ? secondZ : (int)firstHitPosition.z;

            int endX = (int)firstHitPosition.x > secondX ? (int)firstHitPosition.x : secondX;
            int endY = (int)firstHitPosition.y > secondY ? (int)firstHitPosition.y : secondY;
            int endZ = (int)firstHitPosition.z > secondZ ? (int)firstHitPosition.z : secondZ;
            
            for (int x = startX; x <= endX; ++x) {
              for (int y = startY; y <= endY; ++y) {
                for (int z = startZ; z <= endZ; ++z) {
                  if (editMode == EditMode.Place) {
                    if (x >= 0 && y >= 0 && z >= 0 && x < model.width && y < model.height && z < model.depth && model.GetBlock(x, y, z) == 0)
                      model.SetBlock(colorIndex >= 0 ? colorIndex + 1 : 1, x, y, z);
                  } else {
                    if (x >= 0 && y >= 0 && z >= 0 && x < model.width && y < model.height && z < model.depth && model.GetBlock(x, y, z) > 0) {
                      if (editMode == EditMode.Erase) {
                        model.SetBlock(0, x, y, z);
                      } else if (editMode == EditMode.Paint) {
                        if (colorIndex >= 0) {
                          model.SetBlock(colorIndex + 1, x, y, z);
                        }
                      }
                    }
                  }
                }
              }
            }

            wasFirstHit = false;
          }
        }

        model.voxelModelRenderer.Regenerate();
        Event.current.Use();
      } if (Event.current.type == EventType.MouseDown && Event.current.button == 0) {
        wasFirstHit = false;

        if (Physics.Raycast(ray, out hit, 1000)) {
          if (hit.transform.GetComponent<VoxelModel>() == model) {
            if (editMode == EditMode.Place) {
              Vector3 rawPosition = hit.point + (hit.normal * voxelHalfSize);
              int x = Mathf.FloorToInt(rawPosition.x / model.voxelSize);
              int y = Mathf.FloorToInt(rawPosition.y / model.voxelSize);
              int z = Mathf.FloorToInt(rawPosition.z / model.voxelSize);

              if (x >= 0 && y >= 0 && z >= 0 && x < model.width && y < model.height && z < model.depth && model.GetBlock(x, y, z) == 0)
                model.SetBlock(colorIndex >= 0 ? colorIndex + 1 : 1, x, y, z);
            } else {
              Vector3 rawPosition = hit.point - (hit.normal * voxelHalfSize);
              int x = Mathf.FloorToInt(rawPosition.x / model.voxelSize);
              int y = Mathf.FloorToInt(rawPosition.y / model.voxelSize);
              int z = Mathf.FloorToInt(rawPosition.z / model.voxelSize);

              if (x >= 0 && y >= 0 && z >= 0 && x < model.width && y < model.height && z < model.depth && model.GetBlock(x, y, z) > 0) {
                if (editMode == EditMode.Erase) {
                  model.SetBlock(0, x, y, z);
                } else if (editMode == EditMode.Paint) {
                  if (colorIndex >= 0) {
                    model.SetBlock(colorIndex + 1, x, y, z);
                  }
                }
              }
            }

            model.voxelModelRenderer.Regenerate();
            Event.current.Use();
          }
        }
      } else if (Event.current.type == EventType.layout) {
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
      }

      SceneView.RepaintAll();
    }

    void Update() {
      VoxelModel model = (VoxelModel)target;
      if (!CanModelBeEdited(model))
        return;

      float voxelHalfSize = model.voxelSize / 2.0f;


      //Repaint();
    }

  }
}