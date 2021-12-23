namespace StatiqHelpers.ReadingTimeModule
{
    public record ReadingTimeData(int Minutes, int Seconds, int Words)
    {
        public int RoundedMinutes
            => Seconds < 30
                ? Minutes
                : Minutes + 1;
    }
}
