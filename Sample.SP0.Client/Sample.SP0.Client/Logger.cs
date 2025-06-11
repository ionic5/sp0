using Sample.SP0.Client.Core;
using Sample.SP0.Client.View;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.SP0.Client
{
    public class Logger : ILogger
    {
        private readonly DebugConsolePanel _debugConsolePanel;

        public Logger(DebugConsolePanel debugConsolePanel)
        {
            _debugConsolePanel = debugConsolePanel;
        }

        public void Info(string message)
        {
            _debugConsolePanel.AddDebugMessage($"{System.DateTime.Now} [INFO] {message}");
        }

        public void Warn(string message)
        {
            StackTrace stackTrace = new StackTrace(true);
            var filteredStack = stackTrace.GetFrames()
                                          .Where(frame => frame.GetMethod().DeclaringType?.Namespace?.StartsWith("Sample.SP0.Client") == true)
                                          .Select(frame => $"{frame.GetMethod().DeclaringType}.{frame.GetMethod().Name} (Line {frame.GetFileLineNumber()})")
                                          .ToList();

            filteredStack.RemoveAt(0);
            string formattedStackTrace = string.Join("\n", filteredStack);
            _debugConsolePanel.AddDebugMessage($"{DateTime.Now} [WARN] {message}\nFiltered StackTrace:\n{formattedStackTrace}");
        }
    }
}
