namespace CameraConfiguration;

using System.ComponentModel;

//相机类
public class CameraSettings : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public string Name
    {
        get;
    }
    public double MaxMicroseconds
    {
        get;
    }

    public double ExposureTime
    {
        get; private set;
    }

    public CameraSettings(
        string name,
        double exposureTime,
        double maxMicroseconds)
    {
        string error = "";
        if (string.IsNullOrWhiteSpace(name))
        {
            error = "相机名字为空";
            throw new ArgumentException(error);
        }
        Name = name;
        MaxMicroseconds = maxMicroseconds;

        if (!TrySetExposure(exposureTime, out error))
        {
            throw new ArgumentException(error);
        }
    }

    public bool TrySetExposure(double value, out string error)
    {
        if (!ExposureRules.TryValidate(
            value, MaxMicroseconds, out error))
        {
            return false;
        }

        if (ExposureTime == value)
        {
            return true;
        }

        ExposureTime = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ExposureTime)));
        return true;
    }
    //复制相机的副本
    public CameraSettings CreateCopy()
    {
        return new CameraSettings(
            Name,
            ExposureTime,
            MaxMicroseconds
            );
    }
}
