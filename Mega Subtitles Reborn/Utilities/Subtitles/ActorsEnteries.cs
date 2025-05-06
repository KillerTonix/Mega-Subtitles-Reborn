using System.ComponentModel;
using System.Windows;

namespace Mega_Subtitles_Reborn.Utilities.Subtitles
{
    public class ActorsEnteries : INotifyPropertyChanged
    {
        public int ActorsNumber { get; set; } = 1;

        private string? _color;
        public string? ActorsColor
        {
            get => _color;
            set
            {
                _color = value;
                OnPropertyChanged(nameof(ActorsColor));
            }
        }

        private string? _actor;
        public string? Actors
        {
            get => _actor;
            set
            {
                _actor = value;
                OnPropertyChanged(nameof(Actors));
                UpdateColor(); // Update the color based on the new actor
            }
        }

        public int ActorsLineCount { get; set; } = 0;



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
                ActorsColor = newColor.ToString();
            }
        }

    }
}
