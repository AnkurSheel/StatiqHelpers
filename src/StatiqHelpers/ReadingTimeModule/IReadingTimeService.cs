namespace StatiqHelpers.ReadingTimeModule
{
    public interface IReadingTimeService
    {
        ReadingTimeData GetReadingTime(string content, int wordsPerMinute);
    }
}
