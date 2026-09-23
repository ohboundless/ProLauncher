using UAssetEditor.Binary;
using UAssetEditor.Unreal.Properties.Types;

namespace Boundless.Swapper.Swappers;

public class CharacterPartSwapper
{
    private const string DataCosmeticsPath = "FortniteGame/Content/Balance/DefaultGameDataCosmetics.uasset";
    
    public static void SetCharacterParts(string[] parts)
    {
        var dataCosmetics = USystem.ReadAsset(DataCosmeticsPath, out var containerPath);
        var array = dataCosmetics["DefaultGameDataCosmetics"]["DefaultCharacterParts"].GetValue<ArrayProperty>();
        
        array.Value.Clear();
        foreach (var part in parts)
        {
            array.AddItem(SoftObjectProperty.Create(part));
        }

        var writer = new Writer();
        dataCosmetics.WriteAll(writer);

        SC.LaunchConfig.SaveAssetData(DataCosmeticsPath, containerPath, writer.ToArray());
    }
}