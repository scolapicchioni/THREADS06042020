using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WpfApplication {
    public static class MyExtensions {
        public static Task<int> ClickAsync(this Button btn) {
            var taskCompletionSource = new TaskCompletionSource<int>();
            btn.Click += (sender, e) => {
                taskCompletionSource.SetResult(0);
            };
            return taskCompletionSource.Task;
        }

        
    }
}
