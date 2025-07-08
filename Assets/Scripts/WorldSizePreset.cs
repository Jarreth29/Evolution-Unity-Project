using UnityEngine;

public struct WorldSizePreset
{
    public WorldSizeEnum worldSizeName;
    public Vector2 worldDimensions;

    public WorldSizePreset(WorldSizeEnum name, float size)
    {
        worldSizeName = name;
        worldDimensions = new Vector2 (size, size);
    }
}
