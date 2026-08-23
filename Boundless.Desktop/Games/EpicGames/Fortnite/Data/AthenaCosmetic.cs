namespace Boundless.Desktop.Games.EpicGames.Fortnite.Data;

public class AthenaVariant
{
    public string Channel;
    public string Active;
    public string[] Owned;
}

public class AthenaCosmeticAttributes
{
    public AthenaVariant[] Variants;
}

public class AthenaCosmetic
{
    public string TemplateId;
    public AthenaCosmeticAttributes Attributes;
}