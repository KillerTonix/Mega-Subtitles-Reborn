namespace Mega_Subtitles_Reborn.Utilities
{
    public class CommandsData
    {
        public string Command { get; set; } = string.Empty;
        public string CachePath { get; set; } = string.Empty;
        public string CurrentPosition { get; set; } = string.Empty;
        public List<string?> Actors { get; set; } = [];
        public DateTime DateAndTime { get; set; } = DateTime.Now;

    }

    public class ReaperData
    {
        public double Start { get; set; } = 0;
        public double End { get; set; } = 0;
        public string Text { get; set; } = string.Empty;
    }
}
