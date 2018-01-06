using System.Collections.Generic;
using UnityEngine;

namespace BB.VoxelCreator {
  [CreateAssetMenu(fileName = "VoxelColorPalette", menuName = "Voxel/New Palette")]
  public class VoxelColorPalette : ScriptableObject {
    public List<Color> colors = new List<Color>();
  }
}
