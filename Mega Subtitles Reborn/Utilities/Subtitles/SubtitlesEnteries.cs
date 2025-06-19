using System.ComponentModel;
using System.Windows;

namespace Mega_Subtitles_Reborn.Utilities.Subtitles
{
    public class SubtitlesEnteries : INotifyPropertyChanged
    {
        private static readonly MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;


        public string Layer { get; set; } = "";
        public string Style { get; set; } = "";
        public string Effect { get; set; } = "";
        public int Number { get; set; } = 1;
        public string Start { get; set; } = "";
        public string End { get; set; } = "";
        public string Text { get; set; } = "";


        private string? _color;
        public string? Color
        {
            get => _color;
            set
            {
                _color = value;
                OnPropertyChanged(nameof(Color));
            }
        }


        private string? _actor;
        public string? Actor
        {
            get => _actor;
            set
            {
                _actor = value;
                OnPropertyChanged(nameof(Actor));
                UpdateColor(); // Update the color based on the new actor
                UpdateActorLineCounts.Recalculate();
            }
        }

        private string? _comment;
        public string? Comment
        {
            get => _comment;
            set
            {
                _comment = value;
                OnPropertyChanged(nameof(Comment));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            if (propertyName == "Actor")
                UpdateActorLineCounts.Recalculate();
        }


        private void UpdateColor()
        {
            if (mainWindow.ActorsAndColorsDict != null && _actor != null && mainWindow.ActorsAndColorsDict.TryGetValue(key: _actor, value: out var newColor))
                Color = newColor.ToString();
        }


    }

}
