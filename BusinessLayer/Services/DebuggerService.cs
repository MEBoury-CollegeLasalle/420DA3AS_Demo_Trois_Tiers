using _420DA3AS_Demo_Trois_Tiers.PresentationLayer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _420DA3AS_Demo_Trois_Tiers.BusinessLayer.Services;
internal class DebuggerService : AbstractService {
    private static DebuggerWindow? WINDOW;
    private static bool IS_INITIALIZED = false;

    public DebuggerService() : base() {

    }

    public static void InitDebugger() {
        WINDOW = new DebuggerWindow();
        WINDOW.Show();
        IS_INITIALIZED = true;
    }

    public static void Shutdown() {
        if (IS_INITIALIZED) {
            WINDOW?.Close();
            WINDOW?.Dispose();
            IS_INITIALIZED = false;
        }
    }

    public static void Info(string message) {
        message = "INFO - " + message;
        if (IS_INITIALIZED) {
            WINDOW?.AddColoredLine(message, Color.DodgerBlue);
        }
        Debug.WriteLine(message);
    }

    public static void Log(string message) {
        message = "LOG - " + message;
        if (IS_INITIALIZED) {
            WINDOW?.AddColoredLine(message, null);
        }
        Debug.WriteLine(message);
    }

    public static void Success(string message) {
        message = "SUCCESS - " + message;
        if (IS_INITIALIZED) {
            WINDOW?.AddColoredLine(message, Color.Lime);
        }
        Debug.WriteLine(message);
    }

    public static void Warn(string message) {
        message = "WARN - " + message;
        if (IS_INITIALIZED) {
            WINDOW?.AddColoredLine(message, Color.Yellow);
        }
        Debug.WriteLine(message);
    }

    public static void Error(string message) {
        message = "ERROR - " + message;
        if (IS_INITIALIZED) {
            WINDOW?.AddColoredLine(message, Color.Red);
        }
        Debug.WriteLine(message);
    }

    public static void Error(Exception exception) {
        string message = "ERROR - " + exception.Message;
        string stackTrace = "STACKTRACE - " + exception.StackTrace;
        if (IS_INITIALIZED) {
            WINDOW?.AddColoredLine(message, Color.Red);
            WINDOW?.AddColoredLine(stackTrace, Color.Red);
        }
        Debug.WriteLine(message);
        Debug.WriteLine(stackTrace);
    }
}
