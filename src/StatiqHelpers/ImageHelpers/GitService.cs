using LibGit2Sharp;

namespace StatiqHelpers.ImageHelpers;

public class GitService : IGitService
{
    public IReadOnlyList<string> GetModifiedFiles(string rootPath)
    {
        using var repo = new Repository(rootPath);
        var status = repo.RetrieveStatus();

        return status.Where(x => x.State != FileStatus.Ignored).Select(x => x.FilePath).ToList();
    }
}
