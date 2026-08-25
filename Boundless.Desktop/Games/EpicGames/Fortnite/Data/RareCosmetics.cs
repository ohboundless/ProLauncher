namespace Boundless.Desktop.Games.EpicGames.Fortnite.Data;

internal class RareCosmetic(string id)
{
    public string Id = id.ToLower();
}

internal sealed class RareStyle(string id, string channel, string tag) : RareCosmetic(id)
{
    public string Channel = channel.ToLower();
    public string Tag = tag.ToLower();
}

public static class RareCosmetics
{
    private static RareCosmetic[] List = new[]
    {
        new RareStyle("CID_017_Athena_Commando_M", "Parts", "Stage2"),
        new RareStyle("CID_028_Athena_Commando_F", "Material", "Mat3"),
        new RareStyle("CID_029_Athena_Commando_F_Halloween", "Material", "Mat3"),
        new RareStyle("CID_030_Athena_Commando_M_Halloween", "ClothingColor", "Mat1"),
        new RareCosmetic("CID_035_Athena_Commando_M_Medieval"),
        new RareCosmetic("CID_052_Athena_Commando_F_PSBlue"),
        new RareCosmetic("CID_084_Athena_Commando_M_Assassin"),
        new RareCosmetic("CID_085_Athena_Commando_M_Twitch"),
        new RareCosmetic("CID_089_Athena_Commando_M_RetroGrey"),
        new RareCosmetic("CID_113_Athena_Commando_M_BlueAce"),
        new RareCosmetic("CID_114_Athena_Commando_F_TacticalWoodland"),
        new RareStyle("CID_116_Athena_Commando_M_CarbideBlack", "Emissive", "Emissive2"), // TODO double check rarity
        new RareCosmetic("CID_138_Athena_Commando_M_PSBurnout"),
        new RareCosmetic("CID_095_Athena_Commando_M_Founder"),
        new RareCosmetic("CID_096_Athena_Commando_F_Founder"),
        new RareCosmetic("CID_096_Athena_Commando_F_Founder"),
        new RareCosmetic("CID_175_Athena_Commando_M_Celestial"),
        new RareCosmetic("CID_174_Athena_Commando_F_CarbideWhite"),
        new RareCosmetic("CID_183_Athena_Commando_M_ModernMilitaryRed"),
        new RareCosmetic("CID_342_Athena_Commando_M_StreetRacerMetallic"),
        new RareCosmetic("CID_296_Athena_Commando_M_Math"),
        new RareCosmetic("CID_313_Athena_Commando_M_KpopFashion"),
        new RareCosmetic("CID_360_Athena_Commando_M_TechOpsBlue"),
        new RareCosmetic("CID_386_Athena_Commando_M_StreetOpsStealth"),
        new RareCosmetic("CID_371_Athena_Commando_M_SpeedyMidnight"),
        new RareCosmetic("CID_434_Athena_Commando_F_StealthHonor"),
        new RareCosmetic("CID_441_Athena_Commando_F_CyberScavengerBlue"),
        new RareCosmetic("CID_479_Athena_Commando_F_Davinci"),
        new RareCosmetic("CID_657_Athena_Commando_F_TechOpsBlue"),
        new RareCosmetic("CID_711_Athena_Commando_M_LongShorts"),
        new RareCosmetic("CID_757_Athena_Commando_F_WildCat"),
        new RareCosmetic("CID_783_Athena_Commando_M_AquaJacket"),
        new RareCosmetic("CID_926_Athena_Commando_F_StreetFashionDiamond"),
        new RareCosmetic("CID_A_101_Athena_Commando_M_TacticalWoodlandBlue")
        // Maybe more idk
    };

    public static bool IsRare(string id, string? channel, string? tag)
    {
        id = id.ToLower();
        
        foreach (var item in List)
        {
            if (item.Id != id)
                continue;

            if (item is RareStyle style)
            {
                // We can lower it here because this will only get called once
                channel = channel?.ToLower();
                tag = tag?.ToLower();
                
                if (style.Channel == channel && style.Tag == tag)
                    return true;
            }
            else
                return true;
        }

        return false;
    }
}