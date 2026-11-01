using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
namespace CameraEditor;

public partial class MainWindow : Window
{
    private CameraEditorViewModel _viewModel = new CameraEditorViewModel();
    public MainWindow()
    {
        InitializeComponent();

        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        DataContext = _viewModel;
        _viewModel.SelectedCamera = _viewModel.Cameras[0];
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        Debug.WriteLine($"收到属性变化通知：{e.PropertyName}");
    }
}