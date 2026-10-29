using CameraConfiguration;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
namespace CameraEditor;

public partial class MainWindow : Window
{
    private CameraEditorViewModel _viewModel = new CameraEditorViewModel();
    private CameraSettings _camera;

    public MainWindow()
    {
        InitializeComponent();

        _viewModel.PropertyChanged += ViewModel_PropertyChanged;

        DataContext = _viewModel;

        _viewModel.SelectedCamera = _viewModel.Cameras[0];

        _camera = _viewModel.Cameras.First();

        CameraList.ItemsSource = _viewModel.Cameras;

        RoutedEventHandler handelr = ApplyExposure_Click;

        ChangeExposure.Click += handelr;

        CameraList.SelectionChanged += CameraList_SelectionChanged;

        CameraList.SelectedIndex = 0;
    }

    private void ApplyExposure_Click(object sender, RoutedEventArgs e)
    {
        // 第一步：把输入框中的文字转换成数字
        if (!double.TryParse(ExposureInput.Text, out double value))
        {
            StatusText.Text = "曝光时间必须是数字。";
            return;
        }

        // 第二步：由相机配置对象检查范围，并尝试修改
        if (_camera.TrySetExposure(value, out string error))
        {
            StatusText.Text = error;
            return;
        }

        StatusText.Text = "曝光配置修改成功。";
    }
    private void CameraList_SelectionChanged(object sender, RoutedEventArgs e)
    {
        if (CameraList.SelectedItem is not CameraSettings selected)
        {
            DataContext = null;
            ChangeExposure.IsEnabled = false;
            ExposureInput.Text = "";
            StatusText.Text = "请选择相机";
            return;
        }
        _camera = selected;

        ChangeExposure.IsEnabled = true;

        DataContext = _camera;

        StatusText.Text = "";
    }
    private void AddCamera_Click(object sender, RoutedEventArgs e)
    {
        int count = _viewModel.Cameras.Count(x => x.Name == "备用");

        if (count > 0)
        {
            StatusText.Text = "已有此相机";
            return;
        }
        CameraSettings cameraSettings = new CameraSettings("备用", 1314, 88888);

        _viewModel.Cameras.Add(cameraSettings);

        StatusText.Text = $"共有{_viewModel.Cameras.Count}台相机";
    }

    private void RemoveCamera_Click(object sender, RoutedEventArgs e)
    {
        if (_camera is not CameraSettings selected)
        {
            StatusText.Text = "请选择相机";
            return;
        }
        _viewModel.Cameras.Remove(selected);
        CameraList.SelectedIndex = -1;

        StatusText.Text = $"已删除相机：{_camera.Name}。请重新选择相机。";
    }
    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        Debug.WriteLine($"收到属性变化通知：{e.PropertyName}");
    }
}