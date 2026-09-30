using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpongeBobTycoon.Model
{
    public class GameTimer : ITimer
        {
            private readonly System.Timers.Timer _timer;

            public bool Enabled
            {
                get => _timer.Enabled;
                set => _timer.Enabled = value;
            }

            public double Interval
            {
                get => _timer.Interval;
                set => _timer.Interval = value;
            }

            public event EventHandler? Elapsed;

            public GameTimer()
            {
                _timer = new System.Timers.Timer();
                _timer.Elapsed += (sender, e) =>
                {
                    Elapsed?.Invoke(sender, e);
                };
            }

            public void Start()
            {
                _timer.Start();
            }

            public void Stop()
            {
                _timer.Stop();
            }
        }
    
}
