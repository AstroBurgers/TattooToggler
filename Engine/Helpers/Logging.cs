using System;
using System.IO;
using System.Runtime.CompilerServices;
using Rage;

namespace TattooToggler.Engine.Helpers;

internal static class Logger
{
    internal static void Error(Exception ex, [CallerFilePath] string p = "", [CallerMemberName] string m = "",
        [CallerLineNumber] int l = 0)
    {
        Game.LogTrivial($"[ERROR] TattooToggler: Exception at '{Path.GetFileName(p)}' -> {m} (line {l})");
        Game.LogTrivial($"[ERROR] Message: {ex.Message}");
        Game.LogTrivial($"[ERROR] Type: {ex.GetType().FullName}");
        Game.LogTrivial($"[ERROR] Stack Trace: {ex.StackTrace}");

        Exception inner = ex.InnerException;
        int depth = 1;
        while (inner != null)
        {
            Game.LogTrivial($"[ERROR] Inner Exception (Depth {depth}): {inner.GetType().FullName} - {inner.Message}");
            Game.LogTrivial($"[ERROR] Inner Stack Trace: {inner.StackTrace}");
            inner = inner.InnerException;
            depth++;
        }
    }


    /*internal static void Debug(string msg)
    {
        if (DebugMode)
        {
            Game.LogTrivial($"[DEBUG] TattooToggler: {msg}");
        }
    }*/

    internal static void Normal(string msg) => Game.LogTrivial($"[NORMAL] TattooToggler: {msg}");
}