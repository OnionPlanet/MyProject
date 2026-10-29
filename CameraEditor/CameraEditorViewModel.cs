using CameraConfiguration;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CameraEditor
{
    public class CameraEditorViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public ObservableCollection<CameraSettings> Cameras { get; }
        private CameraSettings? _selectedCamera;
        public CameraSettings? SelectedCamera
        {
            get => _selectedCamera;
            set
            {
                if (_selectedCamera == value) return;
                _selectedCamera = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedCamera)));
            }
        }
        public CameraEditorViewModel()
        {
            Cameras = new ObservableCollection<CameraSettings> {
            new CameraSettings("左上", 1000, 10000),
            new CameraSettings("右上", 2000, 20000),
            new CameraSettings("左下", 3000, 20000),
            new CameraSettings("右下", 4000, 20000)
            };
        }
    }
}