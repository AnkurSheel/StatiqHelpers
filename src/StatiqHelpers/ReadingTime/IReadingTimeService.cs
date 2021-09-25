namespace StatiqHelpers.ReadingTime
{
    public interface IReadingTimeService
    {
        ReadingTimeData GetReadingTime(string content, int wordsPerMinute);
    }
}
