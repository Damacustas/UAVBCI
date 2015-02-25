using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UAV
{
    public class Velocity
    {
        public Velocity()
        {
            X = Y = Z = 0.0f;
        }

        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }
    }
}
