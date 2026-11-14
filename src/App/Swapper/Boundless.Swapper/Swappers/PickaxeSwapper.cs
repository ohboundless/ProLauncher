using System.Data;
using UAssetEditor.Binary;
using UAssetEditor.Unreal.Objects;
using UAssetEditor.Unreal.Properties.Structs;
using UAssetEditor.Unreal.Properties.Types;
using UAssetEditor.Utils;

namespace Boundless.Swapper.Swappers;

public static class PickaxeSwapper
{
    private static string[] PropertiesToTake =
    {
        "FortItemComponentData_WeaponAttachedParticles",
        "FortItemComponentData_WeaponGenericImpactSound",
        "FortItemComponentData_WeaponIdleEffect",
        "FortItemComponentData_WeaponImpactEffects",
        "FortItemComponentData_WeaponImpactSounds",
        "FortItemComponentData_WeaponMaterialOverrides",
        "FortItemComponentData_WeaponMeleeAnimClass",
        "FortItemComponentData_WeaponMeleeAnimTrails",
        "FortItemComponentData_WeaponMeleeSingleAnimToPlay",
        "FortItemComponentData_WeaponMeleeSwingEffect",
        "FortItemComponentData_WeaponPrimaryFireSounds",
        "FortItemComponentData_WeaponReloadSounds",
        "ItemComponentData_Icon",
        "FortItemComponentData_LargeIcon",
        "ItemComponentData_OwnedGameplayTags"
    };

    private static Dictionary<string, string> BigAssetByContainer = new()
    {
        {
            "pakchunk20-WindowsClient.utoc",
            "FortniteGame/Content/Athena/SupplyDrops/FN_Birthday_Supply_Drop_5/Textures/T_Gadget_FN_Birthday_Supply_Drop_5_Balloon_D.ubulk"
        },
        {
            "pakchunk100-WindowsClient.utoc",
            "FortniteGame/Plugins/GameFeatures/BRCosmetics/Content/Characters/Player/Male/Large/Bodies/M_LRG_UnluckyRoll/Textures/T_UnluckyRoll_Body_FX.ubulk"
        },
    };
    
    public static void SwapPickaxe(string from, string to)
    {
        var fromAsset = USystem.ReadAsset(from, out var containerPath);
        var toAsset = USystem.ReadAsset(to, out _);

        var fromExportName = fromAsset.Name.Split('/').Last();
        var fromExport = fromAsset.Exports[fromExportName];
        
        var toExportName = toAsset.Name.Split('/').Last();
        var toExport = toAsset.Exports[toExportName];

        if (fromExport == null)
            throw new NoNullAllowedException($"Could not get export using name '{fromExportName}'");
        
        if (toExport == null)
            throw new NoNullAllowedException($"Could not get export using name '{toExportName}'");
        
        // Swap model
        var fromMesh = fromExport["WeaponMeshOverride"] ?? throw new NoNullAllowedException($"Could not get WeaponMeshOverride from '{fromExportName}'");
        var toMesh = toExport["WeaponMeshOverride"] ?? throw new NoNullAllowedException($"Could not get WeaponMeshOverride from '{toExportName}'");
        fromMesh.Value = toMesh.Value;
        
        var dataList = fromExport["DataList"]?.GetValue<ArrayProperty>();
        var toDataList = toExport["DataList"]?.GetValue<ArrayProperty>();
        var list = new List<FInstancedStruct>();
        
        // Gather properties
        foreach (var item in toDataList.Value)
        {
            var struc = (FInstancedStruct)item.As<StructProperty>().Value;
    
            if (!PropertiesToTake.Any(x => struc.Class.Name.Contains(x)))
                continue;

            list.Add(struc);
        }

        var newList = new List<object>();

        // Add all non-cosmetic related data
        foreach (var item in dataList.Value)
        {
            var struc = (FInstancedStruct)item.As<StructProperty>().Value!;
            
            if (!PropertiesToTake.Any(x => struc.Class.Name.Contains(x)))
                newList.Add(item);
        }
        
        // Add To asset data
        foreach (var property in list)
        {
            var entry = toAsset.ImportMap[-property.Index.Index - 1];
            var importMap = fromAsset.ImportMap.ToList();
            
            importMap.Add(entry);
            fromAsset.ImportMap = importMap.ToArray();

            property.Index = new FPackageIndex(fromAsset, -(importMap.Count - 1) - 1);
            newList.Add(new StructProperty
            {
                Type = "InstancedStruct",
                Value = property
            });
        }

        dataList.Value = newList;
        
        var writer = new Writer();
        fromAsset.WriteAll(writer);
        
        SC.LaunchConfig.SaveAssetData(SC.CUE4Parse.Provider.FixPath(from), containerPath, writer.ToArray(), "Pickaxe");
    }

    public static void RevertAll()
    {
        SC.LaunchConfig.RemoveAssetBySwapType("Pickaxe");
    }
}