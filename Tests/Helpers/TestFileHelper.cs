namespace Tests.Helpers;

public static class TestFileHelper
{
    public static string GetProjectPath(string relativePath)
    {
        var path = Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..", "Tests", relativePath
        );

        return path;
    }
}