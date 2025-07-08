
[System.Serializable]
public struct FoodPhenotypeSettings
{
    public float foodMinSize;
    public float foodMaxSize;
    public float growthSpeed;
    public int sproutFrequency;
    public float sproutDistance;

    public FoodPhenotypeSettings (float foodMinSize, float foodMaxSize, float growthSpeed, int sproutFrequency, float sproutDistance)
    {
        this.foodMinSize = foodMinSize;
        this.foodMaxSize = foodMaxSize;
        this.growthSpeed = growthSpeed;
        this.sproutFrequency = sproutFrequency;
        this.sproutDistance = sproutDistance;
    }
}
