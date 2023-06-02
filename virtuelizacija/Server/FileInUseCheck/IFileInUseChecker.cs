namespace Server.FileInUseCheck
{
    public interface IFileInUseChecker
    {
        bool IsFileInUse(string filePath);
    }
}
