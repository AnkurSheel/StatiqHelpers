namespace StatiqHelpers.Modules.ReadingTime
{
    public interface IReadingTimeService
    {
        ReadingTimeData GetReadingTime(string content, int wordsPerMinute);
    }
}
