using System;
using UAV.Common;

namespace UAV.Simulation
{
    public class HistoryItem
    {
        public Vector2D Value { get; set; }
        public double Epoch { get; set; }

        public HistoryItem(Vector2D val, double epoch)
        {
            Value = val;
            Epoch = epoch;
        }
    }
}

