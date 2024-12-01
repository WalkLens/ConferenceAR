using System;
using System.IO;
using UnityEngine;
using Photon.Pun;

namespace CustomLogger{
    public static class FileLogger
    {
        private static LoggingSetting _setting;
        private static string LogPath => $"{Application.persistentDataPath}/Conference_debug_log.txt";

        static FileLogger(){
            _setting = Resources.Load<LoggingSetting>("LoggingSetting");
            if(_setting == null){
                Debug.LogError("LoggingSetting is not found in Resources folder.");
            }
        }

        public static void Log(string message, object caller = null)
        {
            if (_setting == null || _setting.logLevel == LoggingSetting.LogLevel.None)
                return;

            #if UNITY_EDITOR
                // Unity Editor에서는 Debug.Log 사용
                if (_setting.logLevel == LoggingSetting.LogLevel.Detailed && caller != null)
                {
                    string className = caller.GetType().Name.PadRight(20);
                    string role = GetRoleString().PadRight(6);
                    string userName = PhotonNetwork.NickName.PadRight(15);
                    
                    Debug.Log($"{className} | {role} | {userName} | {message}");
                }
                else
                {
                    Debug.Log(message);
                }
            #else
                string finalMessage;
                if (_setting.logLevel == LoggingSetting.LogLevel.Detailed && caller != null)
                {
                    string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff").PadRight(23);
                    string className = caller.GetType().Name.PadRight(20);
                    string role = GetRoleString().PadRight(6);
                    string userName = PhotonNetwork.NickName.PadRight(15);
                    
                    finalMessage = $"[{timestamp}] " +
                                  $"| {className} " +
                                  $"| {role} " +
                                  $"| {userName} " +
                                  $"| {message}";
                }
                else
                {
                    finalMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}";
                }

                try
                {
                    File.AppendAllText(LogPath, finalMessage + Environment.NewLine);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to write to log file: {e.Message}");
                }
            #endif
        }

        public static void ClearLog()
        {
            #if !UNITY_EDITOR
                try
                {
                    string logPath = LogPath;
                    if (File.Exists(logPath))
                        File.Delete(logPath);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to clear log file: {e.Message}");
                }
            #endif
        }

        private static string GetRoleString(){
            return PhotonNetwork.IsMasterClient ? "Master" : "Client";
        }
    }
}