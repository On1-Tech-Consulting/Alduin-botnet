﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace Alduin.Server.Modules
{
    public class ConfigTor
    {
        public static string Tor = "tor";
        public static string AlduinWebPort = "44359";

        public static string TorBaseFolder = GetPathes.Get_TorPath();
        public static string TorrcPath = TorBaseFolder + @"\Data\Tor\torrc";
        public static string TorPath = TorBaseFolder + @"\tor.exe";
        
        public static void StartTor()
        {
            KillTor();
            if (!File.Exists(TorrcPath))
            {
                CreateTorrc();
            }
            Process[] p;
            p = Process.GetProcessesByName(Tor);
            if (!(p.Length > 0))
                StartTorProccess();
        }
        public static void KillTor()
        {
            foreach (Process proc in Process.GetProcessesByName(Tor))
            {
                try
                {
                    proc.Kill();
                }
                catch(Exception e)
                {
                    WriteOutput("Tor error: " + e.ToString());
                }
            }
        }
        private static void StartTorProccess()
        {
            Console.WriteLine("Starting tor...<br>\r\n");
            ServerFileManager.FileAppendTextWithDate(GetPathes.Get_LogPath() + @"\Log.txt", "Starting tor...<br>\r\n");
            var Process = new Process();
            Process.StartInfo.FileName = TorPath;
            Process.StartInfo.Arguments = "-f " + TorrcPath;
            Process.StartInfo.UseShellExecute = false;
            Process.StartInfo.RedirectStandardOutput = true;
            Process.StartInfo.CreateNoWindow = true;
            Process.StartInfo.WorkingDirectory = TorBaseFolder;
            Process.StartInfo.RedirectStandardOutput = true;
            Process.Start();
            Process.PriorityClass = ProcessPriorityClass.BelowNormal;

            while (!Process.StandardOutput.EndOfStream)
            {
                WriteOutput(Process.StandardOutput.ReadLine());
            }
        }
        private static void WriteOutput(string data)
        {
            Console.WriteLine(data);
            ServerFileManager.FileAppendTextWithDate(GetPathes.Get_LogPath() + @"\Log.txt", data + "<br>\r\n");
        }
        public static int ToSec(int sec)
        {
            return sec * 1000;
        }
        public static void CreateTorrc()
        {
            string filestring = @"
ControlPort 9151
DataDirectory " + TorBaseFolder + @"
DirPort 9030
ExitPolicy reject *:*
HashedControlPassword 16:4E1F1599005EB8F3603C046EF402B00B6F74C008765172A774D2853FD4
HiddenServiceDir " + TorBaseFolder + @"
HiddenServicePort " + AlduinWebPort + @" 127.0.0.1:5000
Log notice stdout
Nickname Alduin
SocksPort 9150";
            FileStream fs = File.Create(TorrcPath);
            var info = new UTF8Encoding(false).GetBytes(filestring);
            fs.Write(info, 0, info.Length);
            fs.Close();
        }
        public static void CreateTorrc(string data)
        {
            FileStream fs = File.Create(TorrcPath);
            var info = new UTF8Encoding(false).GetBytes(data);
            fs.Write(info, 0, info.Length);
            fs.Close();
        }
    }
}
