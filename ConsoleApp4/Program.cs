using CameraConfiguration;

public class Program
{
    public static void Main()
    {
        List<CameraSettings> cameras = new List<CameraSettings>
    {
        new CameraSettings("左上", 1000, 10000),
        new CameraSettings("右上", 2000, 20000),
        new CameraSettings("左下", 3000, 20000),
        new CameraSettings("右下", 4000, 20000)
    };
        Func<CameraSettings, bool> condition = IsRightCamera;
        List<CameraSettings> cameraSettings = cameras.Where(condition).ToList();

        List<CameraSettings> names = cameras
            .Where(x => x.ExposureTime > 2500 && (x.Name == "右上" || x.Name == "右下"))
            .ToList();

        string message = "";

        while (true)
        {
            ShowCameras(cameras);

            List<CameraSettings> camerasCopy = CopyCameras(cameras);

            List<string> cameras1 = cameras.Select(x => x.Name).ToList();

            camerasCopy[0].TrySetExposure(1234, out message);

            ShowCameras(camerasCopy);

            Console.WriteLine("输入相机名字表示更改，输入Save表示保存，输入exit表示退出");

            string? name = Console.ReadLine();

            //假地址

            string filePath = "";

            if (name == null || name == "exit") break;

            if (name == "load")
            {
                bool loaded = CameraConfigFile.TryLoadCameras(filePath, out List<CameraSettings> newCameras, out message);

                if (loaded)
                {
                    Console.WriteLine("相机配置加载成功");
                    cameras = newCameras;
                    continue;
                }
                Console.WriteLine(message);
                continue;
            }

            if (name == "Save")
            {
                bool IsSave = CameraConfigFile.TrySaveCameras(cameras, filePath, out message);

                if (IsSave)
                {
                    message = "保存成功";
                    Console.WriteLine(message);
                    continue;
                }
                Console.WriteLine(message);
                continue;
            }

            int count = CountCameraByName(name, cameras);

            if (count == 0)
            {
                message = "未找到该相机:" + name;
                Console.WriteLine(message);
                continue;
            }

            if (count > 1)
            {
                message = "相机重复:" + name;
                Console.WriteLine(message);
                continue;
            }

            CameraSettings? celected = FindCameraByName(name, cameras);

            if (celected is null)
            {
                Console.WriteLine($"此相机为空：{name}");
                continue;
            }

            Console.WriteLine("请输入要修改的曝光值");

            string? str = Console.ReadLine();

            if (str == null || str == "exit")
                break;

            bool exposureRight = TryParseExposure(str, out double exposureTime, out message);

            if (!exposureRight)
            {
                Console.WriteLine(message);
                continue;
            }

            Console.WriteLine($"请确认需要请修改的相机是否为：{name}?y or n");

            string? userIput = Console.ReadLine();

            if (userIput == "exit" || userIput is null) break;

            if (userIput == "n")
            {
                Console.WriteLine("此次修改已取消");
                continue;
            }

            if (userIput != "y")
            {
                Console.WriteLine("此次修改已取消");
                continue;
            }

            bool result = celected.TrySetExposure(exposureTime, out message);

            if (result)
            {
                Console.WriteLine("修改成功");
                continue;
            }

            Console.WriteLine(message);
        }

        Console.WriteLine("程序已经退出");
    }
    //确认同名相机数量
    static int CountCameraByName(string cameraName, List<CameraSettings> cameraSettings)
    {
        return cameraSettings.Count(x => x.Name == cameraName);
    }
    //用名字找相机
    static CameraSettings? FindCameraByName(string cameraName, List<CameraSettings> cameras)
    {
        return cameras.FirstOrDefault(x => x.Name == cameraName);
    }

    //展示相机名字
    static void ShowCameras(List<CameraSettings> cameras)
    {
        foreach (CameraSettings camera in cameras)
        {
            ShowCamera(camera);
        }
    }

    //展示相机名字
    static void ShowCamera(CameraSettings camera)
    {
        Console.WriteLine($"相机名字：{camera.Name},曝光值：{camera.ExposureTime},相机曝光限制{camera.MaxMicroseconds}");
    }
    //尝试解析曝光字符串
    static bool TryParseExposure(
        string str,
        out double exposureTime,
        out string error)
    {
        error = "";
        exposureTime = 0;

        if (!double.TryParse(str, out double value))
        {
            error = "曝光时间必须是数字。";
            return false;
        }

        exposureTime = value;
        return true;
    }

    //判断是否大于2500
    static bool IsRightCamera(CameraSettings camera)
    {
        return camera.Name[0] == '右';
    }
    //复制相机列表
    public static List<CameraSettings> CopyCameras(List<CameraSettings> cameras)
    {
        List<CameraSettings> cameraSettings = new List<CameraSettings>();
        foreach (CameraSettings camera in cameras)
        {
            cameraSettings.Add(camera);
        }
        return cameraSettings;
    }
}
