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
        // Rare styles
        new RareStyle("CID_017_Athena_Commando_M", "Parts", "Stage2"),
        new RareStyle("CID_028_Athena_Commando_F", "Material", "Mat3"),
        new RareStyle("CID_029_Athena_Commando_F_Halloween", "Material", "Mat3"),
        new RareStyle("CID_030_Athena_Commando_M_Halloween", "ClothingColor", "Mat1"),
        new RareStyle("CID_116_Athena_Commando_M_CarbideBlack", "Emissive", "Emissive2"),
        
        // Rare characters
        new RareCosmetic("CID_035_Athena_Commando_M_Medieval"),
        new RareCosmetic("CID_052_Athena_Commando_F_PSBlue"),
        new RareCosmetic("CID_084_Athena_Commando_M_Assassin"),
        new RareCosmetic("CID_085_Athena_Commando_M_Twitch"),
        new RareCosmetic("CID_089_Athena_Commando_M_RetroGrey"),
        new RareCosmetic("CID_113_Athena_Commando_M_BlueAce"),
        new RareCosmetic("CID_114_Athena_Commando_F_TacticalWoodland"),
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
        new RareCosmetic("CID_A_101_Athena_Commando_M_TacticalWoodlandBlue"),
        new RareCosmetic("Character_EmailStamp"),
        new RareCosmetic("Character_UnbrokenTrash"),
        new RareCosmetic("Character_CoolSuitable"),
        new RareCosmetic("Character_AsterOrder"),
        new RareCosmetic("Character_CrownOrder"),
        new RareCosmetic("Character_SummerUnsuitable"),
        new RareCosmetic("Character_SmoothSuede"),
        new RareCosmetic("Character_FitVapor"),
        new RareCosmetic("Character_SnowNinjaDark"),
        new RareCosmetic("Character_SnowSoldierFashion"),
        new RareCosmetic("Character_Scribble"),
        new RareCosmetic("Character_BlueJet"),
        new RareCosmetic("Character_MasterKeyOrder"),
        new RareCosmetic("CID_A_421_Athena_Commando_F_BlizzardBomber"),
        new RareCosmetic("CID_A_297_Athena_Commando_F_Network"),
        new RareCosmetic("CID_850_Athena_Commando_F_SkullBriteCube"),
        new RareCosmetic("CID_516_Athena_Commando_M_BlackWidowRogue"),

        // Emotes
        new RareCosmetic("EID_ChewyStave"),
        new RareCosmetic("EID_TouchLine"),
        new RareCosmetic("EID_FlyPie"),
        new RareCosmetic("EID_HiccupPanic"),
        new RareCosmetic("EID_WishingStar"),
        new RareCosmetic("EID_EyeSpire"),
        new RareCosmetic("EID_Avian"),
        new RareCosmetic("EID_Mic_SpinSlide"),
        new RareCosmetic("EID_Drum_Pullups"),
        new RareCosmetic("EID_Bass_KyleThrowBass"),
        new RareCosmetic("EID_Mic_SlipTheMic"),
        new RareCosmetic("EID_Potassium"),
        new RareCosmetic("EID_GroovingSparkle"),
        new RareCosmetic("EID_Hoppin"),
        new RareCosmetic("EID_PlayerEleven"),
        new RareCosmetic("EID_KPopDance03"),
        new RareCosmetic("EID_HipHop01")
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