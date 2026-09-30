using UAssetEditor.Binary;
using UAssetEditor.Unreal.Properties.Types;

namespace Boundless.Swapper.Swappers;

public class CharacterPartSwapper
{
    private const string DataCosmetics = "FortniteGame/Content/Balance/DefaultGameDataCosmetics.uasset";
    private const string DefaultBody =
        "FortniteGame/Plugins/GameFeatures/DefaultCosmeticsAthena/Content/Characters/Meshes/CP_Athena_Body_F_Prime_A.uasset";
    private const string NoBackpack =
        "FortniteGame/Content/Characters/CharacterParts/BackPacks/NoBackpack.uasset";
    
    public static void SetCharacterParts(string[] parts)
    {
        var dataCosmetics = USystem.ReadAsset(DataCosmetics, out var containerPath);
        var array = dataCosmetics["DefaultGameDataCosmetics"]["DefaultCharacterParts"].GetValue<ArrayProperty>();
        
        array.Value.Clear();
        foreach (var part in parts)
        {
            array.AddItem(SoftObjectProperty.Create(part));
        }

        var writer = new Writer();
        dataCosmetics.WriteAll(writer);

        SC.LaunchConfig.SaveAssetData(DataCosmetics, containerPath, writer.ToArray());
    }

    public static void InvalidateDefault()
    {
        var bytes = USystem.ReadAssetBytes(DefaultBody, out var containerPath);

        var memoryStream = new MemoryStream(bytes);
        Researcher.FindAndReplace(memoryStream, "/DefaultCosmeticsAthena/Characters/Meshes/CP_Athena_Body_F_Prime_A",
            "/DefaultCosmeticsA111111111racters/M1111111111111111Body_F_Prime_A");
        Researcher.FindAndReplace(memoryStream, "CP_Athena_Body_F_Prime_A", "1111111111111111111111_A");

        SC.LaunchConfig.SaveAssetData(DefaultBody, containerPath, memoryStream.ToArray());
    }
    
    public static void InvalidateBackpack()
    {
        var bytes = USystem.ReadAssetBytes(NoBackpack, out var containerPath);

        var memoryStream = new MemoryStream(bytes);
        Researcher.FindAndReplace(memoryStream, "/Game/Characters/CharacterParts/Backpacks/NoBackpack",
            "/Gam111111111111111111111111111111111111111111ckpack");
        Researcher.FindAndReplace(memoryStream, "NoBackpack", "N1111111ck");

        SC.LaunchConfig.SaveAssetData(NoBackpack, containerPath, memoryStream.ToArray());
    }
}