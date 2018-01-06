using UnityEngine;

namespace BB.VoxelCreator {
  [RequireComponent(typeof(MeshRenderer))]
  [RequireComponent(typeof(MeshFilter))]
  [RequireComponent(typeof(MeshCollider))]
  public class VoxelModelRenderer : MonoBehaviour {
    public VoxelModel model;
    public bool buildMeshAtStart = false;

    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    MeshCollider meshCollider;

    void Awake() {
      ValidateComponents();
    }

    void Start() {
      if (buildMeshAtStart)
        Regenerate();
    }

    public void Regenerate() {
      ValidateComponents();

      model.GenerateMesh();
      meshFilter.mesh = model.mesh;
      meshCollider.sharedMesh = model.mesh;
    }

    void ValidateComponents() {
      if (meshRenderer == null)
        meshRenderer = GetComponent<MeshRenderer>();
      if (meshFilter == null)
        meshFilter = GetComponent<MeshFilter>();
      if (meshCollider == null)
        meshCollider = GetComponent<MeshCollider>();
    }
  }
}