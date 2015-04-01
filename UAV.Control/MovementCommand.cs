using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UAV
{
    public class MovementCommand
    {
        public MovementCommand()
        {
            Pitch = Yaw = Roll = Gaz = 0.0f;
        }

        public float Pitch { get; set; }

        public float Yaw { get; set; }

        public float Roll { get; set; }

        public float Gaz { get; set; }
    }
}
