namespace BrightnessTrainAPI.Models
{
    /// <summary>
    /// ML保存数据对象
    /// </summary>
    public class BrightnessData
    {
        /// <summary>
        /// ML输入格式HHmsss，输出HHm0
        /// </summary>
        public string Time { get; set; }
        public byte Value { get; set; }
    }
}
