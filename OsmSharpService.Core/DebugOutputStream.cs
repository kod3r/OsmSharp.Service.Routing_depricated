using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace OsmSharp.Tools.Output
{
    /// <summary>
    /// OutputStream implementation for debugging purposes.
    /// </summary>
    public class DebugOutputStream : IOutputStream
    {
        #region IOutputTextStream Members

        /// <summary>
        /// Writes text.
        /// </summary>
        /// <param name="text"></param>
        public void WriteLine(string text)
        {
            Debug.WriteLine(text);
        }

        /// <summary>
        /// Writes text.
        /// </summary>
        /// <param name="text"></param>
        public void Write(string text)
        {
            Debug.Write(text);
        }

        private string _previous_progress_string;

        //private string _previous_key;

        /// <summary>
        /// Reports progress.
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="key"></param>
        /// <param name="message"></param>
        public void ReportProgress(double progress, string key, string message)
        {
            string current_progress_string = message;//string.Format("{0}:{1}", key, message);
            if (current_progress_string == _previous_progress_string)
            {
//#if __ANDROID__
//                Console.WriteLine(); // resetting cursor position not support in Mono for Android!
//#elif WINDOWS_PHONE
//                Console.WriteLine(); // resetting cursor position not support in Mono for Android!
//#elif IOS
//                Console.WriteLine(); // resetting cursor position not supported in Monotouch!
//#else
//                Debug.SetCursorPosition(0, Console.CursorTop);
//#endif
                Debug.WriteLine(string.Empty);
            }
            else
            {
                Debug.WriteLine(string.Empty);
            }

            Debug.Write(string.Format("{0} : {1}%",
                current_progress_string, System.Math.Round(progress * 100, 2).ToString().PadRight(6), key, message));
            _previous_progress_string = current_progress_string;
        }

        #endregion
    }
}
