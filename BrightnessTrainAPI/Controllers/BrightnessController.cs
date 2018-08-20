using BrightnessTrainAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BrightnessTrainAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrightnessController : ControllerBase
    {
        [HttpPost]
        [Route("Items")]
        public ActionResult<List<BrightnessData>> Get(List<BrightnessData> src)
        {
            // 0.最少2个样本数据
            if (src.Count == 0) return new List<BrightnessData>();
            if (src.Count == 1)
            {
                src.Add(new BrightnessData() { Time = (Convert.ToInt32(src[0].Time) - 1).ToString().PadLeft(6, '0'), Value = (byte)(src[0].Value - 1) });
                src.Add(new BrightnessData() { Time = (Convert.ToInt32(src[0].Time) + 1).ToString().PadLeft(6, '0'), Value = (byte)(src[0].Value + 1) });
            }

            // 1.保存训练数据到文件
            var dataPath = Common.GetBrightnessTrainedFileName(HttpContext.Connection.RemoteIpAddress.ToString().Replace(':', '.'));
            using (StreamWriter stream = new StreamWriter(dataPath, true))
            {
                foreach (var item in src)
                    stream.WriteLine($"{item.Time},{item.Value}");
                stream.Flush();
                stream.Close();
            }

            // 2.输入训练数据与待处理数据
            var brightnessTrain = new BrightnessTrain(dataPath);

            var times = new List<float>();
            for (float s = 0; s < 2400; s += 10)
            {
                times.Add(s * 100);

                //if (s)
            }
            var r = brightnessTrain.Train(times).ToList();

            // 3.处理新生成的数据
            var result = new List<BrightnessData>();
            for (int i = 0; i < times.Count; i++)
                result.Add(new BrightnessData()
                {
                    Time = (times[i] / 100).ToString().PadLeft(4, '0'),
                    Value = Convert.ToByte(r[i])
                });

            return result;
        }

        private int getNumber(int number, int position)
        {
            int power = (int)Math.Pow(10, position);
            return (number - number / power * power) * 10 / power;
        }

        [HttpGet]
        [Route("HealthCheck")]
        public ActionResult Get() => Ok("Ok");
    }
}
