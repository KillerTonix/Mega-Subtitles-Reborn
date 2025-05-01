using System.ComponentModel;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Media;

namespace Mega_Subtitles_Reborn.Utilities.Subtitles
{
    public class SubtitlesEnteries : INotifyPropertyChanged
    {

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
        }

        


        private void UpdateColor()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;

            if (mainWindow.ActorsAndColorsDict != null && _actor != null && mainWindow.ActorsAndColorsDict.TryGetValue(key: _actor, value: out var newColor))
            {
                Color = newColor.ToString();
            }
        }


    }

}
