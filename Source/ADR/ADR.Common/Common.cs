using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADR.Common
{
    public enum Lane
    {
        LeftHand,
        RightHand,
    }

    public static class LaneExt
    {
        public static Lane AnotherHand(this Lane lane)
        {
            return lane == Lane.LeftHand ? Lane.RightHand : Lane.LeftHand;
        }
    }

    public static class Logger
    {
        public static void Log(string message)
        {
            Console.WriteLine($"[{DateTime.Now.ToString("hh:mm:ss.fff")}] {message}");
        }
    }
}
