#nullable enable
using System;
using System.IO;
using System.Runtime.CompilerServices;
using Godot;

namespace SubstanceAbuse.Common;

public enum LogLevel
{
    Info,
    Warning,
    Error
}

/// <summary>Единая точка вывода игровых сообщений в консоль.</summary>
public static class GameLog
{
    /// <summary>Хук для игровой консоли (ConsoleManager). Получает готовую строку.</summary>
    public static event Action<LogLevel, string>? MessageLogged;

    public static void Info(string message,
        [CallerMemberName] string member = "", [CallerFilePath] string file = "")
        => Write(LogLevel.Info, message, member, file);

    public static void Warn(string message,
        [CallerMemberName] string member = "", [CallerFilePath] string file = "")
        => Write(LogLevel.Warning, message, member, file);

    public static void Error(string message,
        [CallerMemberName] string member = "", [CallerFilePath] string file = "")
        => Write(LogLevel.Error, message, member, file);

    public static void Error(Exception ex,
        [CallerMemberName] string member = "", [CallerFilePath] string file = "")
        => Write(LogLevel.Error, $"{ex.GetType().Name}: {ex.Message}", member, file);

    private static void Write(LogLevel level, string message, string member, string file)
    {
        string source = $"{Path.GetFileNameWithoutExtension(file)}.{member}";
        string text = $"[{level.ToString().ToUpperInvariant()}] {source}: {message}";

        switch (level)
        {
            case LogLevel.Error:
                GD.PushError(text);
                break;
            case LogLevel.Warning:
                GD.PushWarning(text);
                break;
            default:
                GD.Print(text);
                break;
        }

        try
        {
            MessageLogged?.Invoke(level, text);
        }
        catch (Exception ex)
        {
            GD.PrintErr($"GameLog subscriber failed: {ex.Message}");
        }
    }
}
