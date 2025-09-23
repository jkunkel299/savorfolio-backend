namespace Tests.Helpers;

public static class TestFileHelper
{
    public static string GetProjectPath(string relativePath)
    {
        // Get directory where tests are running (bin folder)
        var basePath = AppContext.BaseDirectory;

        // Walk up until we find the Tests folder
        var dir = Directory.GetParent(basePath)!;
        while (dir != null && !dir.Name.Equals("Tests", StringComparison.OrdinalIgnoreCase))
        {
            dir = dir.Parent;
        }

        if (dir == null)
            throw new DirectoryNotFoundException("Could not locate /Tests directory.");

        return Path.Combine(dir.FullName, relativePath);
    }
}