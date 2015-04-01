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
            Forward = Left = TurnLeft = 0.0f;
        }

        public float Forward { get; set; }

        public float Left { get; set; }

        public float TurnLeft { get; set; }
    }
}
