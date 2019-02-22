using System;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace BrightnessTrainAPI.Models
{
    public class Common
    {
        public static string BrightnessTrainedFolder { get; set; }
        public static string GetBrightnessTrainedFileName(string requester) => Path.Combine(BrightnessTrainedFolder, $"{DateTime.Now.ToString("yyyyMMddss")}-{requester}.txt");

        /// <summary>
        /// 获取客户端IP
        ///     仅实现nginx代理下获取ip
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetClientIP(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress.ToString();

            //if (context.Request.Headers.ContainsKey("X-Real-IP"))
            //    ip = context.Request.Headers["X-Real-IP"].ToString();
            if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
                ip = context.Request.Headers["X-Forwarded-For"].ToString();

            return ip;
        }
    }
}
