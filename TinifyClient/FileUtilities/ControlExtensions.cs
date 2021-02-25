using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TinifyClient.FileUtilities
{
    public static class ControlExtensions
    {
        /// <summary>
        /// Executes the Action asynchronously on the UI thread, does not block execution on the calling thread.
        /// </summary>
        /// <param name="control">the control for which the update is required</param>
        /// <param name="action">action to be performed on the control</param>
        public static void InvokeOnUiThreadIfRequired(this Control control, Action action)
        {
            if (control.Disposing || control.IsDisposed || !control.IsHandleCreated)
            {
                return;
            }

            if (control.InvokeRequired)
            {
                control.BeginInvoke(action);
            }
            else
            {
                action.Invoke();
            }
        }
        public static void InvokeOnUiThreadIfRequired(this Control control, Action action, int waitinMilliseconds)
        {
            if (control == null)
                return;
            if (control.Disposing || control.IsDisposed || !control.IsHandleCreated)
            {
                return;
            }
            var task = Task.Run(async () => await Task.Delay(waitinMilliseconds).ContinueWith(t => { control.BeginInvoke(action); }));
            task.Wait();
        }
        
    }
}
