using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UAV.Joystick;

namespace UAV
{
    class Program
    {
        static void Main(string[] args)
        {
            var js = new Joystick.Joystick();
            js.Initialize("/dev/input/js0");
            js.ReadInput();
        }
    }
}
