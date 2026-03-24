using UnityEngine;

public enum BodyStyle
{
    GasGiant,
    LavaWorld,
    IceCrystal,
    CrateredMoon,
    NeonMarble,
    Mountainous,
    Storm,
    Crystalline,
    Oceanic,
    Toxic
}

public static class BodyPresets
{
    static Shader _shader;
    static Shader GetShader()
    {
        if (_shader == null)
        {
            _shader = Resources.Load<Shader>("Shaders/ProceduralBody");
            if (_shader == null)
                _shader = Shader.Find("Custom/ProceduralBody"); // fallback for Editor
        }
        return _shader;
    }

    public static Material CreateMaterial(BodyStyle style, Color colorA, Color colorB, Color rimColor)
    {
        var mat = new Material(GetShader());
        mat.SetColor("_ColorA", colorA);
        mat.SetColor("_ColorB", colorB);
        mat.SetColor("_RimColor", rimColor);

        switch (style)
        {
            case BodyStyle.GasGiant:
                ApplyGasGiant(mat);
                break;
            case BodyStyle.LavaWorld:
                ApplyLavaWorld(mat);
                break;
            case BodyStyle.IceCrystal:
                ApplyIceCrystal(mat);
                break;
            case BodyStyle.CrateredMoon:
                ApplyCrateredMoon(mat);
                break;
            case BodyStyle.NeonMarble:
                ApplyNeonMarble(mat);
                break;
            case BodyStyle.Mountainous:
                ApplyMountainous(mat);
                break;
            case BodyStyle.Storm:
                ApplyStorm(mat);
                break;
            case BodyStyle.Crystalline:
                ApplyCrystalline(mat);
                break;
            case BodyStyle.Oceanic:
                ApplyOceanic(mat);
                break;
            case BodyStyle.Toxic:
                ApplyToxic(mat);
                break;
        }

        return mat;
    }

    static void ApplyGasGiant(Material mat)
    {
        mat.SetFloat("_NoiseScale", 3f);
        mat.SetFloat("_NoiseOctaves", 4f);
        mat.SetFloat("_NoiseLacunarity", 2.2f);
        mat.SetFloat("_NoisePersistence", 0.45f);
        mat.SetFloat("_WarpStrength", 0.6f);
        mat.SetFloat("_WarpScale", 2f);
        mat.SetFloat("_DriftSpeed", 0.03f);
        mat.SetVector("_DriftDirection", new Vector4(1f, 0.1f, 0f, 0f));
        mat.SetFloat("_RimPower", 2.5f);
        mat.SetFloat("_RimIntensity", 1.5f);
        mat.SetFloat("_EmissiveIntensity", 0.15f);
        mat.SetFloat("_EmissiveThreshold", 0.7f);
        mat.SetFloat("_BandStrength", 0.7f);
        mat.SetFloat("_VoronoiStrength", 0f);
        mat.SetFloat("_CrackStrength", 0f);
    }

    static void ApplyLavaWorld(Material mat)
    {
        mat.SetFloat("_NoiseScale", 5f);
        mat.SetFloat("_NoiseOctaves", 3f);
        mat.SetFloat("_NoiseLacunarity", 2.5f);
        mat.SetFloat("_NoisePersistence", 0.5f);
        mat.SetFloat("_WarpStrength", 0.4f);
        mat.SetFloat("_WarpScale", 3f);
        mat.SetFloat("_DriftSpeed", 0.01f);
        mat.SetVector("_DriftDirection", new Vector4(0.5f, 0.3f, 0f, 0f));
        mat.SetFloat("_RimPower", 3f);
        mat.SetFloat("_RimIntensity", 1f);
        mat.SetFloat("_EmissiveIntensity", 1.5f);
        mat.SetColor("_EmissiveColor", new Color(1f, 0.4f, 0f));
        mat.SetFloat("_EmissiveThreshold", 0.4f);
        mat.SetFloat("_BandStrength", 0f);
        mat.SetFloat("_VoronoiStrength", 0f);
        mat.SetFloat("_CrackStrength", 0.8f);
        mat.SetFloat("_VoronoiScale", 6f);
    }

    static void ApplyIceCrystal(Material mat)
    {
        mat.SetFloat("_NoiseScale", 6f);
        mat.SetFloat("_NoiseOctaves", 5f);
        mat.SetFloat("_NoiseLacunarity", 2.8f);
        mat.SetFloat("_NoisePersistence", 0.4f);
        mat.SetFloat("_WarpStrength", 0.2f);
        mat.SetFloat("_WarpScale", 4f);
        mat.SetFloat("_DriftSpeed", 0.005f);
        mat.SetVector("_DriftDirection", new Vector4(0.3f, 0.7f, 0f, 0f));
        mat.SetFloat("_RimPower", 2f);
        mat.SetFloat("_RimIntensity", 1.8f);
        mat.SetFloat("_EmissiveIntensity", 0.4f);
        mat.SetColor("_EmissiveColor", new Color(0.7f, 0.9f, 1f));
        mat.SetFloat("_EmissiveThreshold", 0.65f);
        mat.SetFloat("_BandStrength", 0f);
        mat.SetFloat("_VoronoiStrength", 0f);
        mat.SetFloat("_CrackStrength", 0f);
    }

    static void ApplyCrateredMoon(Material mat)
    {
        mat.SetFloat("_NoiseScale", 4f);
        mat.SetFloat("_NoiseOctaves", 3f);
        mat.SetFloat("_NoiseLacunarity", 2f);
        mat.SetFloat("_NoisePersistence", 0.5f);
        mat.SetFloat("_WarpStrength", 0.1f);
        mat.SetFloat("_WarpScale", 3f);
        mat.SetFloat("_DriftSpeed", 0f);
        mat.SetVector("_DriftDirection", new Vector4(0f, 0f, 0f, 0f));
        mat.SetFloat("_RimPower", 3.5f);
        mat.SetFloat("_RimIntensity", 0.8f);
        mat.SetFloat("_EmissiveIntensity", 0.1f);
        mat.SetFloat("_EmissiveThreshold", 0.8f);
        mat.SetFloat("_BandStrength", 0f);
        mat.SetFloat("_VoronoiStrength", 0.8f);
        mat.SetFloat("_VoronoiScale", 7f);
        mat.SetFloat("_CrackStrength", 0f);
    }

    static void ApplyNeonMarble(Material mat)
    {
        mat.SetFloat("_NoiseScale", 3.5f);
        mat.SetFloat("_NoiseOctaves", 4f);
        mat.SetFloat("_NoiseLacunarity", 2f);
        mat.SetFloat("_NoisePersistence", 0.55f);
        mat.SetFloat("_WarpStrength", 1.2f);
        mat.SetFloat("_WarpScale", 2.5f);
        mat.SetFloat("_DriftSpeed", 0.015f);
        mat.SetVector("_DriftDirection", new Vector4(0.7f, 0.5f, 0f, 0f));
        mat.SetFloat("_RimPower", 2f);
        mat.SetFloat("_RimIntensity", 2f);
        mat.SetFloat("_EmissiveIntensity", 0.6f);
        mat.SetFloat("_EmissiveThreshold", 0.5f);
        mat.SetFloat("_BandStrength", 0f);
        mat.SetFloat("_VoronoiStrength", 0f);
        mat.SetFloat("_CrackStrength", 0f);
    }

    static void ApplyMountainous(Material mat)
    {
        mat.SetFloat("_NoiseScale", 7f);
        mat.SetFloat("_NoiseOctaves", 4f);
        mat.SetFloat("_NoiseLacunarity", 2.5f);
        mat.SetFloat("_NoisePersistence", 0.45f);
        mat.SetFloat("_WarpStrength", 0.15f);
        mat.SetFloat("_WarpScale", 3f);
        mat.SetFloat("_DriftSpeed", 0f);
        mat.SetVector("_DriftDirection", new Vector4(0f, 0f, 0f, 0f));
        mat.SetFloat("_RimPower", 3f);
        mat.SetFloat("_RimIntensity", 1f);
        mat.SetFloat("_EmissiveIntensity", 0.1f);
        mat.SetFloat("_EmissiveThreshold", 0.8f);
        mat.SetFloat("_BandStrength", 0f);
        mat.SetFloat("_VoronoiStrength", 0.6f);
        mat.SetFloat("_VoronoiScale", 12f);
        mat.SetFloat("_CrackStrength", 0f);
    }

    static void ApplyStorm(Material mat)
    {
        mat.SetFloat("_NoiseScale", 4f);
        mat.SetFloat("_NoiseOctaves", 5f);
        mat.SetFloat("_NoiseLacunarity", 2.3f);
        mat.SetFloat("_NoisePersistence", 0.55f);
        mat.SetFloat("_WarpStrength", 1.5f);
        mat.SetFloat("_WarpScale", 2f);
        mat.SetFloat("_DriftSpeed", 0.05f);
        mat.SetVector("_DriftDirection", new Vector4(1f, 0.3f, 0f, 0f));
        mat.SetFloat("_RimPower", 2f);
        mat.SetFloat("_RimIntensity", 1.6f);
        mat.SetFloat("_EmissiveIntensity", 1.2f);
        mat.SetColor("_EmissiveColor", new Color(1f, 0.9f, 0.7f));
        mat.SetFloat("_EmissiveThreshold", 0.45f);
        mat.SetFloat("_BandStrength", 0.4f);
        mat.SetFloat("_VoronoiStrength", 0f);
        mat.SetFloat("_CrackStrength", 0f);
    }

    static void ApplyCrystalline(Material mat)
    {
        mat.SetFloat("_NoiseScale", 15f);
        mat.SetFloat("_NoiseOctaves", 3f);
        mat.SetFloat("_NoiseLacunarity", 3f);
        mat.SetFloat("_NoisePersistence", 0.4f);
        mat.SetFloat("_WarpStrength", 0f);
        mat.SetFloat("_WarpScale", 1f);
        mat.SetFloat("_DriftSpeed", 0f);
        mat.SetVector("_DriftDirection", new Vector4(0f, 0f, 0f, 0f));
        mat.SetFloat("_RimPower", 1.5f);
        mat.SetFloat("_RimIntensity", 2.2f);
        mat.SetFloat("_EmissiveIntensity", 0.8f);
        mat.SetColor("_EmissiveColor", new Color(0.9f, 0.95f, 1f));
        mat.SetFloat("_EmissiveThreshold", 0.55f);
        mat.SetFloat("_BandStrength", 0f);
        mat.SetFloat("_VoronoiStrength", 0.7f);
        mat.SetFloat("_VoronoiScale", 14f);
        mat.SetFloat("_CrackStrength", 0f);
    }

    static void ApplyOceanic(Material mat)
    {
        mat.SetFloat("_NoiseScale", 2.5f);
        mat.SetFloat("_NoiseOctaves", 5f);
        mat.SetFloat("_NoiseLacunarity", 2f);
        mat.SetFloat("_NoisePersistence", 0.5f);
        mat.SetFloat("_WarpStrength", 0.3f);
        mat.SetFloat("_WarpScale", 2f);
        mat.SetFloat("_DriftSpeed", 0.02f);
        mat.SetVector("_DriftDirection", new Vector4(0.8f, 0.2f, 0f, 0f));
        mat.SetFloat("_RimPower", 2f);
        mat.SetFloat("_RimIntensity", 1.6f);
        mat.SetFloat("_EmissiveIntensity", 0.2f);
        mat.SetFloat("_EmissiveThreshold", 0.7f);
        mat.SetFloat("_BandStrength", 0.3f);
        mat.SetFloat("_VoronoiStrength", 0.2f);
        mat.SetFloat("_VoronoiScale", 4f);
        mat.SetFloat("_CrackStrength", 0f);
    }

    static void ApplyToxic(Material mat)
    {
        mat.SetFloat("_NoiseScale", 5f);
        mat.SetFloat("_NoiseOctaves", 4f);
        mat.SetFloat("_NoiseLacunarity", 2.2f);
        mat.SetFloat("_NoisePersistence", 0.5f);
        mat.SetFloat("_WarpStrength", 0.8f);
        mat.SetFloat("_WarpScale", 3f);
        mat.SetFloat("_DriftSpeed", 0.04f);
        mat.SetVector("_DriftDirection", new Vector4(0.5f, 0.6f, 0f, 0f));
        mat.SetFloat("_RimPower", 2f);
        mat.SetFloat("_RimIntensity", 1.8f);
        mat.SetFloat("_EmissiveIntensity", 1f);
        mat.SetColor("_EmissiveColor", new Color(0.5f, 1f, 0.2f));
        mat.SetFloat("_EmissiveThreshold", 0.5f);
        mat.SetFloat("_BandStrength", 0f);
        mat.SetFloat("_VoronoiStrength", 0.3f);
        mat.SetFloat("_VoronoiScale", 5f);
        mat.SetFloat("_CrackStrength", 0.5f);
    }
}
