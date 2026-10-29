using System.Text.Json;

namespace CameraConfiguration;

public static class CameraConfigFile
{
    //保存相机
    public static bool TrySaveCameras(
        List<CameraSettings> cameras,
        string filePath,
        out string error)
    {
        try
        {
            JsonSerializerOptions options =
                new JsonSerializerOptions();

            options.WriteIndented = true;

            string json = JsonSerializer.Serialize(cameras, options);

            File.WriteAllText(filePath, json);

            error = "";
            return true;
        }
        catch (UnauthorizedAccessException ex)
        {
            error = "无法写入文件，请检查访问权限：" + ex.Message;
            return false;
        }
        catch (IOException ex)
        {
            error = "保存文件失败：" + ex.Message;
            return false;
        }
    }
    //加载相机
    public static bool TryLoadCameras(string filePath, out List<CameraSettings> loadCameras, out string error)
    {
        error = "";

        loadCameras = new List<CameraSettings>();

        try
        {
            string json = File.ReadAllText(filePath);
            List<CameraSettings>? cameras = JsonSerializer.Deserialize<List<CameraSettings>>(json);

            if (cameras == null || cameras.Count == 0)
            {
                error = "配置中没有可用相机";
                return false;
            }

            HashSet<string> strings = new HashSet<string>();

            foreach (CameraSettings camera in cameras)
            {
                if (camera is null)
                {
                    error = "列表配置中存在空选项";
                    return false;
                }

                bool right = strings.Add(camera.Name);

                if (!right)
                {
                    error = "配置中存在重名相机：" + camera.Name;
                    return false;
                }
            }

            loadCameras = cameras;
            error = "加载成功";
            return true;
        }
        catch (IOException ex)
        {
            error = "读取配置文件失败：" + ex.Message;
            return false;
        }
        catch (UnauthorizedAccessException ex)
        {
            error = "无法访问配置文件，请检查权限：" + ex.Message;
            return false;
        }
        catch (JsonException ex)
        {
            error = "JSON 格式或数据类型不正确：" + ex.Message;
            return false;
        }
        catch (ArgumentException ex)
        {
            error = "配置参数或文件路径不合法：" + ex.Message;
            return false;
        }
    }

}