namespace StatiqHelpers.ReadingTime
{
    public record ReadingTimeData
    {
        public ReadingTimeData(int minutes, int seconds, int words)
        {
            Minutes = minutes;
            Seconds = seconds;
            Words = words;
        }

        public int Minutes { get; }

        public int Seconds { get; }

        public int Words { get; }

        public int RoundedMinutes
            => Seconds < 30
                ? Minutes
                : Minutes + 1;
    }
}
