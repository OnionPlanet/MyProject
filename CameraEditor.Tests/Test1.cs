using CameraConfiguration;

namespace CameraEditor.Tests;

[TestClass]
public class CameraEditorViewModelTests
{
    [TestMethod]
    public void ApplyExposure_TooLarge_KeepsOriginalExposure()
    {
        // 准备：初始曝光1000，上限10000
        CameraEditorViewModel viewModel = new CameraEditorViewModel();
        CameraSettings camera = new CameraSettings("测试相机", 1000, 10000);
        viewModel.AddCamera(camera);

        // 执行：尝试提交超过上限的曝光
        viewModel.ExposureTimeInput = "20000";
        viewModel.ApplyExposure();

        // 检查：曝光必须仍然是1000
        Assert.AreEqual(1000d, camera.ExposureTime);
    }
    [TestMethod]
    public void ApplyExposure_ValidInput_ChangesExposure()
    {
        // 准备：初始曝光1000，上限10000
        CameraEditorViewModel viewModel = new CameraEditorViewModel();
        CameraSettings camera = new CameraSettings("测试相机", 1000, 10000);
        viewModel.AddCamera(camera);

        // 执行：尝试提交超过上限的曝光
        viewModel.ExposureTimeInput = "6000";
        viewModel.ApplyExposure();

        // 检查：曝光必须仍然是1000
        Assert.AreEqual(6000d, camera.ExposureTime);
    }
    [TestMethod]
    public void ApplyExposure_AtLimit_ChangesExposure()
    {
        CameraEditorViewModel cameraEditorViewModel = new CameraEditorViewModel();
        CameraSettings camera = new CameraSettings("Test", 10000, 10000);
        cameraEditorViewModel.SelectedCamera = camera;
        cameraEditorViewModel.ApplyExposure();
        Assert.AreEqual(10000, camera.ExposureTime);
    }
}