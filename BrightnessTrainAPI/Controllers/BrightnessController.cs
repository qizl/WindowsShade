using BrightnessTrainAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
            return src;

            var dataPath = string.Empty;

            var brightnessTrain = new BrightnessTrain(dataPath);

            var times = new List<float>();
            for (float s = 0; s <= 2400; s += 10)
                times.Add(s);
            var r = brightnessTrain.Train(times).ToList();

            var result = new List<BrightnessData>();
            for (int i = 0; i < times.Count; i++)
                result.Add(new BrightnessData()
                {
                    Time = times[i].ToString().PadLeft(4, '0'),
                    Value = Convert.ToByte(r[i])
                });
            return result;
        }

        [HttpGet]
        [Route("HealthCheck")]
        public ActionResult Get() => Ok("Ok");
    }
}
