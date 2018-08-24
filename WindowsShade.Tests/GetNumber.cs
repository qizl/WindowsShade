using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace WindowsShade.Tests
{
    [TestClass]
    public class GetNumber
    {
        [TestMethod]
        public void Get()
        {
            var n = 1230;
            var tenN = this.getNumber(n,3);
        }

        /// <summary>
        /// 参考：https://blog.csdn.net/daonidedie/article/details/41643157
        /// </summary>
        /// <param name="number"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        private int getNumber(int number, int position)
        {
            int power = (int)Math.Pow(10, position);
            return (number - number / power * power) * 10 / power;
        }
    }
}
