using CameraConfiguration;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;

namespace CameraEditor
{
    public class CameraEditorViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public ObservableCollection<CameraSettings> Cameras { get; }
        private CameraSettings? _selectedCamera;
        private string? _exposureTimeInput = "";
        private string? _statusMessage = "";
        private readonly string _configFilePath = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
    "相机配置",
    "cameras.json");
        //命令
        public RelayCommand ApplyExposureCommand { get; }
        public RelayCommand AddCameraCommand { get; }
        public RelayCommand RemoveCameraCommand { get; }
        public RelayCommand SaveCamerasCommand { get; }
        public RelayCommand RestExposureInputCommand { get; }

        private void AddBackupCamera()
        {
            AddCamera(new CameraSettings("备用", 1314, 88888));
        }
        private bool CanRemoveCamera()
        {
            return SelectedCamera != null;

        }
        private bool CanApplyExposure()
        {
            return SelectedCamera != null && !string.IsNullOrEmpty(ExposureTimeInput);
        }
        private bool CanRestExposureTimenInput()
        {
            return SelectedCamera != null;
        }

        public CameraSettings? SelectedCamera
        {
            get => _selectedCamera;
            set
            {
                if (_selectedCamera == value) return;
                _selectedCamera = value;

                if (value is null)
                {
                    ExposureTimeInput = "";
                    StatusMessage = "当前未选中相机";
                }
                else
                {
                    ExposureTimeInput = value.ExposureTime.ToString();
                    StatusMessage = "";
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedCamera)));
                ApplyExposureCommand.NotifyCanExecuteChanged();
                RemoveCameraCommand.NotifyCanExecuteChanged();
                RestExposureInputCommand.NotifyCanExecuteChanged();
            }
        }
        public string? StatusMessage
        {
            get => _statusMessage;
            set
            {
                if (_statusMessage == value) return;
                _statusMessage = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StatusMessage)));
            }
        }
        public string? ExposureTimeInput
        {
            get => _exposureTimeInput;
            set
            {
                if (_exposureTimeInput == value) return;
                _exposureTimeInput = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ExposureTimeInput)));
                ApplyExposureCommand.NotifyCanExecuteChanged();
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

            ApplyExposureCommand = new RelayCommand(ApplyExposure, CanApplyExposure);
            AddCameraCommand = new RelayCommand(AddBackupCamera);
            RemoveCameraCommand = new RelayCommand(RemoveCamera, CanRemoveCamera);
            SaveCamerasCommand = new RelayCommand(SaveCameras);
            RestExposureInputCommand = new RelayCommand(RestExposureInput, CanRestExposureTimenInput);
            LoadCameras();

            SelectedCamera = Cameras[0];
        }
        //修改曝光
        public void ApplyExposure()
        {
            if (SelectedCamera is null)
            {
                StatusMessage = "请先选择相机。";
                return;
            }

            if (!double.TryParse(_exposureTimeInput, out double value))
            {
                StatusMessage = "曝光时间必须是数字。";
                return;
            }

            if (value == SelectedCamera.ExposureTime)
            {
                StatusMessage = "当前值未产生变化";
                return;
            }

            if (!SelectedCamera.TrySetExposure(value, out string error))
            {
                StatusMessage = error;
                return;
            }

            StatusMessage = "曝光配置修改成功。";
        }
        //添加相机
        public void AddCamera(CameraSettings camera)
        {
            if (camera is null)
            {
                StatusMessage = "传入的相机配置为空";
                return;
            }
            int count = Cameras.Count(x => x.Name == camera.Name);
            if (count > 0)
            {
                StatusMessage = $"已存在此相机：{camera.Name}";
                return;
            }
            Cameras.Add(camera);
            SelectedCamera = camera;
            StatusMessage = $"已添加此相机：{camera.Name},共有{Cameras.Count}台相机";
        }

        //删除相机
        public void RemoveCamera()
        {
            if (SelectedCamera is not CameraSettings selected)
            {
                StatusMessage = "未选中相机";
                return;
            }
            Cameras.Remove(selected);
            SelectedCamera = null;
            if (Cameras.Count > 0) SelectedCamera = Cameras[0];
            StatusMessage = $"已删除相机：{selected.Name}，剩余{Cameras.Count}台相机";
        }
        //保存相机
        private void SaveCameras()
        {
            if (!CameraConfigFile.TrySaveCameras(Cameras.ToList(), _configFilePath, out string error))
            {
                StatusMessage = error;
                return;
            }
            StatusMessage = "配置已保存";
        }
        //恢复输入
        private void RestExposureInput()
        {
            if (SelectedCamera is null)
            {
                StatusMessage = "未正确选择相机";
                return;
            }
            ExposureTimeInput = SelectedCamera?.ExposureTime.ToString();
            StatusMessage = "";
        }
        //加载配置
        private void LoadCameras()
        {
            Cameras.Clear();
            if (CameraConfigFile.TryLoadCameras(_configFilePath, out List<CameraSettings> items, out string error))
            {
                foreach (var item in items)
                {
                    Cameras.Add(item);
                }
                StatusMessage = "配置加载成功";
                return;
            }
            StatusMessage = "配置加载失败";
        }
    }
}