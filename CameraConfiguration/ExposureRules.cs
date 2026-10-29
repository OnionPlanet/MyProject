namespace CameraConfiguration;


//曝光规则
static class ExposureRules
{
    public static bool TryValidate(double value, double maxNumber, out string error)
    {
        if (!double.IsFinite(maxNumber) || maxNumber <= 0)
        {
            error = "曝光时间上限必须是有限正数。";
            return false;
        }

        if (!double.IsFinite(value) || value > maxNumber || value <= 0)
        {
            error = "曝光时间必须是有限正数，且不能超过"
                    + maxNumber + "微秒。";
            return false;
        }

        error = "";
        return true;
    }
}