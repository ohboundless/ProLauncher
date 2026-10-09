namespace Boundless.Swapper;

public static class Statics
{
    public static string DataDirectory
    {
        get
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
                       @"\BoundlessLauncher\.data";
            
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }
    }
}