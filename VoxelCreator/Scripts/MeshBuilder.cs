using System.Collections.Generic;
using UnityEngine;

namespace BB.VoxelCreator {
  public class VoxelMeshGen {
    public int width, height, depth;
    public float voxelSize;
    public List<Color> palette;
    public int[,,] blocks;
    public int[,,] blocksLeft;
    public int[,,] blocksRight;
    public int[,,] blocksFront;
    public int[,,] blocksBack;
    public int[,,] blocksTop;
    public int[,,] blocksBottom;

    public Mesh outMesh;
  }

  public class MeshBuilder {
    public static void Generate(ref VoxelMeshGen meshGen) {
      if (meshGen.voxelSize == 0.0f) {
        Debug.LogError("Wrong voxel size. It's cannot be 0");
        return;
      }

      List<Vector3> positions = new List<Vector3>();
      List<Vector3> normals = new List<Vector3>();
      List<Color> colors = new List<Color>();
      List<int> indices = new List<int>();

      int[,,] blocksUsed = new int[meshGen.width, meshGen.height, meshGen.depth];

      float voxelSize = meshGen.voxelSize;
      float voxelHalfSize = meshGen.voxelSize / 2.0f;

      for (int x = 0; x < meshGen.width; ++x) {
        for (int y = 0; y < meshGen.height; ++y) {
          for (int z = 0; z < meshGen.depth; ++z) {
            int currentBlock = meshGen.blocks[x, y, z];
            if (currentBlock <= 0)
              continue;

            Color blockColor = meshGen.palette[currentBlock - 1];

            if (!IsBlockHovered(meshGen, x, y, z, Direction.Front)) {
              if ((blocksUsed[x, y, z] & (int)Direction.Front) == 0) {
                int numBlocksY = 0;
                int numBlocksX = 0;

                for (int _y = y; _y < meshGen.height; ++_y) {
                  bool failed = false;

                  for (int _x = x; _x < meshGen.width; ++_x) {
                    if (_y == y) {
                      if (meshGen.blocks[_x, _y, z] == currentBlock && !IsBlockHovered(meshGen, _x, _y, z, Direction.Front)) {
                        ++numBlocksX;
                      } else {
                        break;
                      }
                    } else {
                      if (meshGen.blocks[_x, _y, z] != currentBlock || IsBlockHovered(meshGen, _x, _y, z, Direction.Front)) {
                        failed = true;
                        break;
                      }
                    }
                  }

                  if (failed)
                    break;

                  ++numBlocksY;
                }

                for (int _x = x; _x < x + numBlocksX; ++_x) {
                  for (int _y = y; _y < y + numBlocksY; ++_y) {
                    blocksUsed[_x, _y, z] |= (int)Direction.Front;
                  }
                }

                // geometry
                positions.Add(new Vector3((x * voxelSize) - voxelHalfSize, (y * voxelSize) - voxelHalfSize, (z * voxelSize) + voxelHalfSize));
                positions.Add(new Vector3((x * voxelSize) - voxelHalfSize, (y * voxelSize) + voxelHalfSize + (voxelSize * numBlocksY - voxelSize), (z * voxelSize) + voxelHalfSize));
                positions.Add(new Vector3((x * voxelSize) + voxelHalfSize + (numBlocksX * voxelSize - voxelSize), (y * voxelSize) + voxelHalfSize + (voxelSize * numBlocksY - voxelSize), (z * voxelSize) + voxelHalfSize));
                positions.Add(new Vector3((x * voxelSize) + voxelHalfSize + (numBlocksX * voxelSize - voxelSize), (y * voxelSize) - voxelHalfSize, (z * voxelSize) + voxelHalfSize));

                normals.Add(new Vector3(0, 0, 1));
                normals.Add(new Vector3(0, 0, 1));
                normals.Add(new Vector3(0, 0, 1));
                normals.Add(new Vector3(0, 0, 1));

                colors.Add(blockColor);
                colors.Add(blockColor);
                colors.Add(blockColor);
                colors.Add(blockColor);

                int startVertexIndex = positions.Count - 4;
                indices.Add(startVertexIndex + 0);
                indices.Add(startVertexIndex + 2);
                indices.Add(startVertexIndex + 1);

                indices.Add(startVertexIndex + 3);
                indices.Add(startVertexIndex + 2);
                indices.Add(startVertexIndex + 0);
              }
            }

            if (!IsBlockHovered(meshGen, x, y, z, Direction.Back)) {
              if ((blocksUsed[x, y, z] & (int)Direction.Back) == 0) {
                int numBlocksY = 0;
                int numBlocksX = 0;

                for (int _y = y; _y < meshGen.height; ++_y) {
                  bool failed = false;

                  for (int _x = x; _x < meshGen.width; ++_x) {
                    if (_y == y) {
                      if (meshGen.blocks[_x, _y, z] == currentBlock && !IsBlockHovered(meshGen, _x, _y, z, Direction.Back)) {
                        ++numBlocksX;
                      } else {
                        break;
                      }
                    } else {
                      if (meshGen.blocks[_x, _y, z] != currentBlock || IsBlockHovered(meshGen, _x, _y, z, Direction.Back)) {
                        failed = true;
                        break;
                      }
                    }
                  }

                  if (failed)
                    break;

                  ++numBlocksY;
                }

                for (int _x = x; _x < x + numBlocksX; ++_x) {
                  for (int _y = y; _y < y + numBlocksY; ++_y) {
                    blocksUsed[_x, _y, z] |= (int)Direction.Back;
                  }
                }

                // geometry
                positions.Add(new Vector3((x * voxelSize) - voxelHalfSize, (y * voxelSize) - voxelHalfSize, (z * voxelSize) - voxelHalfSize));
                positions.Add(new Vector3((x * voxelSize) - voxelHalfSize, (y * voxelSize) + voxelHalfSize + (voxelSize * numBlocksY - voxelSize), (z * voxelSize) - voxelHalfSize));
                positions.Add(new Vector3((x * voxelSize) + voxelHalfSize + (numBlocksX * voxelSize - voxelSize), (y * voxelSize) + voxelHalfSize + (voxelSize * numBlocksY - voxelSize), (z * voxelSize) - voxelHalfSize));
                positions.Add(new Vector3((x * voxelSize) + voxelHalfSize + (numBlocksX * voxelSize - voxelSize), (y * voxelSize) - voxelHalfSize, (z * voxelSize) - voxelHalfSize));

                normals.Add(new Vector3(0, 0, -1));
                normals.Add(new Vector3(0, 0, -1));
                normals.Add(new Vector3(0, 0, -1));
                normals.Add(new Vector3(0, 0, -1));

                colors.Add(blockColor);
                colors.Add(blockColor);
                colors.Add(blockColor);
                colors.Add(blockColor);

                int startVertexIndex = positions.Count - 4;
                indices.Add(startVertexIndex + 0);
                indices.Add(startVertexIndex + 1);
                indices.Add(startVertexIndex + 2);

                indices.Add(startVertexIndex + 2);
                indices.Add(startVertexIndex + 3);
                indices.Add(startVertexIndex + 0);
              }
            }

            if (!IsBlockHovered(meshGen, x, y, z, Direction.Top)) {
              if ((blocksUsed[x, y, z] & (int)Direction.Top) == 0) {
                int numBlocksX = 0;
                int numBlocksZ = 0;

                for (int _x = x; _x < meshGen.width; ++_x) {
                  bool failed = false;

                  for (int _z = z; _z < meshGen.depth; ++_z) {
                    if (_x == x) {
                      if (meshGen.blocks[_x, y, _z] == currentBlock && !IsBlockHovered(meshGen, _x, y, _z, Direction.Top)) {
                        ++numBlocksZ;
                      } else {
                        break;
                      }
                    } else {
                      if (meshGen.blocks[_x, y, _z] != currentBlock || IsBlockHovered(meshGen, _x, y, _z, Direction.Top)) {
                        failed = true;
                        break;
                      }
                    }
                  }

                  if (failed)
                    break;

                  ++numBlocksX;
                }

                for (int _x = x; _x < x + numBlocksX; ++_x) {
                  for (int _z = z; _z < z + numBlocksZ; ++_z) {
                    blocksUsed[_x, y, _z] |= (int)Direction.Top;
                  }
                }

                // geometry
                positions.Add(new Vector3((x * voxelSize) - voxelHalfSize, (y * voxelSize) + voxelHalfSize, (z * voxelSize) - voxelHalfSize));
                positions.Add(new Vector3((x * voxelSize) - voxelHalfSize, (y * voxelSize) + voxelHalfSize, (z * voxelSize) + voxelHalfSize + (numBlocksZ * voxelSize - voxelSize)));
                positions.Add(new Vector3((x * voxelSize) + voxelHalfSize + (numBlocksX * voxelSize - voxelSize), (y * voxelSize) + voxelHalfSize, (z * voxelSize) + voxelHalfSize + (numBlocksZ * voxelSize - voxelSize)));
                positions.Add(new Vector3((x * voxelSize) + voxelHalfSize + (numBlocksX * voxelSize - voxelSize), (y * voxelSize) + voxelHalfSize, (z * voxelSize) - voxelHalfSize));

                normals.Add(new Vector3(0, 1, 0));
                normals.Add(new Vector3(0, 1, 0));
                normals.Add(new Vector3(0, 1, 0));
                normals.Add(new Vector3(0, 1, 0));

                colors.Add(blockColor);
                colors.Add(blockColor);
                colors.Add(blockColor);
                colors.Add(blockColor);

                int startVertexIndex = positions.Count - 4;
                indices.Add(startVertexIndex + 0);
                indices.Add(startVertexIndex + 1);
                indices.Add(startVertexIndex + 2);

                indices.Add(startVertexIndex + 2);
                indices.Add(startVertexIndex + 3);
                indices.Add(startVertexIndex + 0);
              }
            }

            if (!IsBlockHovered(meshGen, x, y, z, Direction.Bottom)) {
              if ((blocksUsed[x, y, z] & (int)Direction.Bottom) == 0) {
                int numBlocksX = 0;
                int numBlocksZ = 0;

                for (int _x = x; _x < meshGen.width; ++_x) {
                  bool failed = false;

                  for (int _z = z; _z < meshGen.depth; ++_z) {
                    if (_x == x) {
                      if (meshGen.blocks[_x, y, _z] == currentBlock && !IsBlockHovered(meshGen, _x, y, _z, Direction.Bottom)) {
                        ++numBlocksZ;
                      } else {
                        break;
                      }
                    } else {
                      if (meshGen.blocks[_x, y, _z] != currentBlock || IsBlockHovered(meshGen, _x, y, _z, Direction.Bottom)) {
                        failed = true;
                        break;
                      }
                    }
                  }

                  if (failed)
                    break;

                  ++numBlocksX;
                }

                for (int _x = x; _x < x + numBlocksX; ++_x) {
                  for (int _z = z; _z < z + numBlocksZ; ++_z) {
                    blocksUsed[_x, y, _z] |= (int)Direction.Bottom;
                  }
                }

                // geometry
                positions.Add(new Vector3((x * voxelSize) - voxelHalfSize, (y * voxelSize) - voxelHalfSize, (z * voxelSize) - voxelHalfSize));
                positions.Add(new Vector3((x * voxelSize) - voxelHalfSize, (y * voxelSize) - voxelHalfSize, (z * voxelSize) + voxelHalfSize + (numBlocksZ * voxelSize - voxelSize)));
                positions.Add(new Vector3((x * voxelSize) + voxelHalfSize + (numBlocksX * voxelSize - voxelSize), (y * voxelSize) - voxelHalfSize, (z * voxelSize) + voxelHalfSize + (numBlocksZ * voxelSize - voxelSize)));
                positions.Add(new Vector3((x * voxelSize) + voxelHalfSize + (numBlocksX * voxelSize - voxelSize), (y * voxelSize) - voxelHalfSize, (z * voxelSize) - voxelHalfSize));

                normals.Add(new Vector3(0, -1, 0));
                normals.Add(new Vector3(0, -1, 0));
                normals.Add(new Vector3(0, -1, 0));
                normals.Add(new Vector3(0, -1, 0));

                colors.Add(blockColor);
                colors.Add(blockColor);
                colors.Add(blockColor);
                colors.Add(blockColor);

                int startVertexIndex = positions.Count - 4;
                indices.Add(startVertexIndex + 0);
                indices.Add(startVertexIndex + 2);
                indices.Add(startVertexIndex + 1);

                indices.Add(startVertexIndex + 3);
                indices.Add(startVertexIndex + 2);
                indices.Add(startVertexIndex + 0);
              }
            }

            if (!IsBlockHovered(meshGen, x, y, z, Direction.Right)) {
              if ((blocksUsed[x, y, z] & (int)Direction.Right) == 0) {
                int numBlocksY = 0;
                int numBlocksZ = 0;

                for (int _y = y; _y < meshGen.height; ++_y) {
                  bool failed = false;

                  for (int _z = z; _z < meshGen.depth; ++_z) {
                    if (_y == y) {
                      if (meshGen.blocks[x, _y, _z] == currentBlock && !IsBlockHovered(meshGen, x, _y, _z, Direction.Right)) {
                        ++numBlocksZ;
                      } else {
                        break;
                      }
                    } else {
                      if (meshGen.blocks[x, _y, _z] != currentBlock || IsBlockHovered(meshGen, x, _y, _z, Direction.Right)) {
                        failed = true;
                        break;
                      }
                    }
                  }

                  if (failed)
                    break;

                  ++numBlocksY;
                }

                for (int _y = y; _y < y + numBlocksY; ++_y) {
                  for (int _z = z; _z < z + numBlocksZ; ++_z) {
                    blocksUsed[x, _y, _z] |= (int)Direction.Right;
                  }
                }

                // geometry
                positions.Add(new Vector3((x * voxelSize) + voxelHalfSize, (y * voxelSize) - voxelHalfSize, (z * voxelSize) - voxelHalfSize));
                positions.Add(new Vector3((x * voxelSize) + voxelHalfSize, (y * voxelSize) - voxelHalfSize, (z * voxelSize) + voxelHalfSize + (numBlocksZ * voxelSize - voxelSize)));
                positions.Add(new Vector3((x * voxelSize) + voxelHalfSize, (y * voxelSize) + voxelHalfSize + (numBlocksY * voxelSize - voxelSize), (z * voxelSize) + voxelHalfSize + (numBlocksZ * voxelSize - voxelSize)));
                positions.Add(new Vector3((x * voxelSize) + voxelHalfSize, (y * voxelSize) + voxelHalfSize + (numBlocksY * voxelSize - voxelSize), (z * voxelSize) - voxelHalfSize));

                normals.Add(new Vector3(1, 0, 0));
                normals.Add(new Vector3(1, 0, 0));
                normals.Add(new Vector3(1, 0, 0));
                normals.Add(new Vector3(1, 0, 0));

                colors.Add(blockColor);
                colors.Add(blockColor);
                colors.Add(blockColor);
                colors.Add(blockColor);

                int startVertexIndex = positions.Count - 4;
                indices.Add(startVertexIndex + 0);
                indices.Add(startVertexIndex + 2);
                indices.Add(startVertexIndex + 1);

                indices.Add(startVertexIndex + 3);
                indices.Add(startVertexIndex + 2);
                indices.Add(startVertexIndex + 0);
              }
            }

            if (!IsBlockHovered(meshGen, x, y, z, Direction.Left)) {
              if ((blocksUsed[x, y, z] & (int)Direction.Left) == 0) {
                int numBlocksY = 0;
                int numBlocksZ = 0;

                for (int _y = y; _y < meshGen.height; ++_y) {
                  bool failed = false;

                  for (int _z = z; _z < meshGen.depth; ++_z) {
                    if (_y == y) {
                      if (meshGen.blocks[x, _y, _z] == currentBlock && !IsBlockHovered(meshGen, x, _y, _z, Direction.Left)) {
                        ++numBlocksZ;
                      } else {
                        break;
                      }
                    } else {
                      if (meshGen.blocks[x, _y, _z] != currentBlock || IsBlockHovered(meshGen, x, _y, _z, Direction.Left)) {
                        failed = true;
                        break;
                      }
                    }
                  }

                  if (failed)
                    break;

                  ++numBlocksY;
                }

                for (int _y = y; _y < y + numBlocksY; ++_y) {
                  for (int _z = z; _z < z + numBlocksZ; ++_z) {
                    blocksUsed[x, _y, _z] |= (int)Direction.Left;
                  }
                }

                // geometry
                positions.Add(new Vector3((x * voxelSize) - voxelHalfSize, (y * voxelSize) - voxelHalfSize, (z * voxelSize) - voxelHalfSize));
                positions.Add(new Vector3((x * voxelSize) - voxelHalfSize, (y * voxelSize) - voxelHalfSize, (z * voxelSize) + voxelHalfSize + (numBlocksZ * voxelSize - voxelSize)));
                positions.Add(new Vector3((x * voxelSize) - voxelHalfSize, (y * voxelSize) + voxelHalfSize + (numBlocksY * voxelSize - voxelSize), (z * voxelSize) + voxelHalfSize + (numBlocksZ * voxelSize - voxelSize)));
                positions.Add(new Vector3((x * voxelSize) - voxelHalfSize, (y * voxelSize) + voxelHalfSize + (numBlocksY * voxelSize - voxelSize), (z * voxelSize) - voxelHalfSize));

                normals.Add(new Vector3(-1, 0, 0));
                normals.Add(new Vector3(-1, 0, 0));
                normals.Add(new Vector3(-1, 0, 0));
                normals.Add(new Vector3(-1, 0, 0));

                colors.Add(blockColor);
                colors.Add(blockColor);
                colors.Add(blockColor);
                colors.Add(blockColor);

                int startVertexIndex = positions.Count - 4;
                indices.Add(startVertexIndex + 0);
                indices.Add(startVertexIndex + 1);
                indices.Add(startVertexIndex + 2);

                indices.Add(startVertexIndex + 2);
                indices.Add(startVertexIndex + 3);
                indices.Add(startVertexIndex + 0);
              }
            }
          }
        }
      }

      for (int i = 0; i < positions.Count; ++i) {
        positions[i] += new Vector3(voxelHalfSize, voxelHalfSize, voxelHalfSize);
      }

      Mesh mesh = new Mesh();
      mesh.SetVertices(positions);
      mesh.SetNormals(normals);
      mesh.SetColors(colors);
      mesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);
      mesh.UploadMeshData(true);

      meshGen.outMesh = mesh;
    }

    static bool IsBlockHovered(VoxelMeshGen meshGen, int x, int y, int z, Direction direction) {
      bool hovered = false;

      switch (direction) {
        case Direction.Front: {
          if (z + 1 < meshGen.depth) {
            hovered = meshGen.blocks[x, y, z + 1] != 0;
          } else if (z + 1 >= meshGen.depth) {
            if (meshGen.blocksFront != null)
              hovered = meshGen.blocksFront[x, y, 0] != 0;
          }
        } break;

        case Direction.Back: {
          if (z - 1 >= 0) {
            hovered = meshGen.blocks[x, y, z - 1] != 0;
          } else if (z - 1 < 0) {
            if (meshGen.blocksBack != null)
              hovered = meshGen.blocksBack[x, y, meshGen.depth - 1] != 0;
          }
        } break;
    
        case Direction.Top: {
          if (y + 1 < meshGen.height) {
            hovered = meshGen.blocks[x, y + 1, z] != 0;
          } else if (y + 1 >= meshGen.height) {
            if (meshGen.blocksTop != null)
              hovered = meshGen.blocksTop[x, 0, z] != 0;
          }
        } break;

        case Direction.Bottom: {
          if (y - 1 >= 0) {
            hovered = meshGen.blocks[x, y - 1, z] != 0;
          } else if (y - 1 < 0) {
            if (meshGen.blocksBottom != null)
              hovered = meshGen.blocksBottom[x, meshGen.height - 1, z] != 0;
          }
        } break;
    
        case Direction.Right: {
          if (x + 1 < meshGen.width) {
            hovered = meshGen.blocks[x + 1, y, z] != 0;
          } else if (x + 1 >= meshGen.width) {
            if (meshGen.blocksRight != null)
              hovered = meshGen.blocksRight[0, y, z] != 0;
          }
        } break;

        case Direction.Left: {
          if (x - 1 >= 0) {
            hovered = meshGen.blocks[x - 1, y, z] != 0;
          } else if (x - 1 < 0) {
            if (meshGen.blocksLeft != null)
              hovered = meshGen.blocksLeft[meshGen.width - 1, y, z] != 0;
          }
        } break;
      }

      return hovered;
    }
  }
}