using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BB.VoxelCreator {
  public class VoxelModel : MonoBehaviour {
    public VoxelModelRenderer voxelModelRenderer;

    public int width;
    public int height;
    public int depth;
    public float voxelSize;

    [HideInInspector]
    public int[] blocks;
    public VoxelColorPalette palette;

    public Mesh mesh;

    public void GenerateMesh() {
      VoxelMeshGen meshGen = new VoxelMeshGen();
      meshGen.width = width;
      meshGen.height = height;
      meshGen.depth = depth;
      meshGen.voxelSize = voxelSize;
      meshGen.palette = palette.colors;
      meshGen.blocks = GetBlocks3DArray();
      meshGen.blocksFront = null;//new int[meshGen.width, meshGen.height, meshGen.depth];
      meshGen.blocksBack = null;//new int[meshGen.width, meshGen.height, meshGen.depth];
      meshGen.blocksTop = null;//new int[meshGen.width, meshGen.height, meshGen.depth];
      meshGen.blocksBottom = null;//new int[meshGen.width, meshGen.height, meshGen.depth];
      meshGen.blocksRight = null;//new int[meshGen.width, meshGen.height, meshGen.depth];
      meshGen.blocksLeft = null;//new int[meshGen.width, meshGen.height, meshGen.depth];
      MeshBuilder.Generate(ref meshGen);

      mesh = meshGen.outMesh;
    }

    int[,,] GetBlocks3DArray() {
      int[,,] result = new int[width, height, depth];
      for (int x = 0; x < width; ++x) {
        for (int y = 0; y < height; ++y) {
          for (int z = 0; z < depth; ++z) {
            result[x, y, z] = GetBlock(x, y, z);
          }
        }
      }
      return result;
    }

    public int GetBlock(int x, int y, int z) {
      return blocks[x * height * depth + y * depth + z];
    }
   
    public void SetBlock(int id, int x, int y, int z) {
      blocks[x * height * depth + y * depth + z] = id;
    }
  }
}