namespace StatiqHelpers.ImageHelpers;

public interface IGitService
{
    IReadOnlyList<string> GetModifiedFiles(string rootPath);
}
