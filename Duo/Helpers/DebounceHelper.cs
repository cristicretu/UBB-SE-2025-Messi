using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.UI.Dispatching;

namespace Duo.Helpers
{
    public static class DebounceHelper
    {
        private static Dictionary<string, System.Timers.Timer> _activeTimers = new Dictionary<string, System.Timers.Timer>();

        public static void Debounce(Action method, int delay = 200, string? key = null) {
          key = key ?? method.GetHashCode().ToString();

          if (_activeTimers.TryGetValue(key, out var existingTimer))
          {
              existingTimer.Stop();
              existingTimer.Dispose();
          }

          var timer = new System.Timers.Timer(delay);
          _activeTimers[key] = timer;

          var dispatcherQueue = DispatcherQueue.GetForCurrentThread();

          timer.Elapsed += (sender, e) => {
            timer.Stop();

            if (_activeTimers.ContainsKey(key))
            {
                _activeTimers.Remove(key);
            }

            dispatcherQueue?.TryEnqueue(() => {
                method();
            });

            timer.Dispose();
          };

          timer.Start();
        }
    }
}