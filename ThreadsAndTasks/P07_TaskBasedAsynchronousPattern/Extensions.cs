using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace P07_TaskBasedAsynchronousPattern
{
    static class Extensions {
        public static TaskAwaiter<int> GetAwaiter(this Process process) {
            var tcs = new TaskCompletionSource<int>();
            process.EnableRaisingEvents = true;
            process.Exited += (s, e) => tcs.TrySetResult(process.ExitCode);
            if (process.HasExited) tcs.TrySetResult(process.ExitCode);
            return tcs.Task.GetAwaiter();
        }
    }
}
