using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;

using ADR.Common;

namespace ADR.Physics
{
    // Mock implementation
    public class Car
    {
        public Lane Lane { get; set; }
        public ReactiveProperty<int> LocationY { get; private set; }

        public Car(int initialY)
        {
            this.LocationY = new ReactiveProperty<int>(initialY);
        }

        public void MoveForward(int distance)
        {
            this.LocationY.Value += distance;
        }
    }
}
