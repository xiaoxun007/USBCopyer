using System;
using System.IO;
using System.Text;

namespace USBCopyer
{
    /// <summary>
    /// U盘序列号 → 目标文件夹别名映射表（disk_mapping.ini，与 exe 同级）。
    /// 程序只读取与追加，绝不修改用户已写好的行与注释。
    /// </summary>
    public static class DiskMappingHelper
    {
        private const string MappingFileName = "disk_mapping.ini";

        private static string MappingFilePath
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, MappingFileName); }
        }

        private const string Template =
            "# U盘序列号文件夹别名映射表\r\n" +
            "# 格式：磁盘序列号=自定义文件夹名\r\n" +
            "# 示例：80070600=WorkDisk\r\n" +
            "# 程序只会自动追加尚未存在的序列号条目\r\n" +
            "# 已经存在的行、注释不会被程序修改\r\n" +
            "# 别名请勿包含 \\ / : * ? \" < > |\r\n" +
            "# 修改本文件后，下次插入U盘自动生效，无需重启程序\r\n";

        /// <summary>
        /// 解析指定U盘序列号对应的目标文件夹别名。
        /// 命中映射返回别名；未命中时自动在配置文件末尾追加"序列号=序列号"并返回序列号本身。
        /// 任何读取/写入异常均静默降级为返回序列号本身，不影响复制流程。
        /// </summary>
        public static string ResolveAlias(string serialNumber)
        {
            if (string.IsNullOrEmpty(serialNumber))
                return serialNumber;

            try
            {
                string path = MappingFilePath;
                if (!File.Exists(path))
                {
                    File.WriteAllText(path, Template, Encoding.UTF8);
                    return serialNumber;
                }

                string alias = null;
                foreach (string rawLine in File.ReadAllLines(path, Encoding.UTF8))
                {
                    string line = rawLine.Trim();
                    if (line.Length == 0 || line.StartsWith("#"))
                        continue;

                    int eq = line.IndexOf('=');
                    if (eq <= 0)
                        continue;

                    string key = line.Substring(0, eq).Trim();
                    string value = line.Substring(eq + 1).Trim();
                    // 追加粘连修复：历史数据可能因文件末尾缺少换行而形如 "yuwen19DB324C=19DB324C"，
                    // 这里只取到下一个等号之前的内容，避免整串被当作别名
                    int extraEq = value.IndexOf('=');
                    if (extraEq >= 0)
                    {
                        value = value.Substring(0, extraEq).Trim();
                    }
                    // 与黑白名单序列号匹配行为保持一致（忽略大小写）
                    if (string.Equals(key, serialNumber, StringComparison.OrdinalIgnoreCase))
                    {
                        alias = SanitizeAlias(value);
                        break;
                    }
                }

                if (alias != null)
                {
                    // 别名过滤后为空则回退为序列号，避免产生空文件夹名
                    if (alias.Length == 0)
                        alias = serialNumber;
                    Program.log("磁盘别名：命中映射 " + serialNumber + " → " + alias, 0);
                    return alias;
                }

                // 未命中：追加 序列号=序列号（仅追加，不重写文件）；本次直接使用序列号
                try
                {
                    AppendWithNewlineGuard(path, serialNumber + "=" + serialNumber + "\r\n");
                    Program.log("磁盘别名：未命中映射，已追加 " + serialNumber + "=" + serialNumber, 0);
                }
                catch
                {
                    // 静默：追加失败仅本次不持久化，不影响复制
                }
                return serialNumber;
            }
            catch
            {
                return serialNumber;
            }
        }

        /// <summary>
        /// 追加内容前确保文件以换行结尾，避免新条目粘到上一行尾部
        /// （用记事本编辑后，最后一行往往没有尾换行，直接追加会把两行拼成一行）
        /// </summary>
        private static void AppendWithNewlineGuard(string path, string content)
        {
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    if (fs.Length > 0)
                    {
                        fs.Seek(-1, SeekOrigin.End);
                        int last = fs.ReadByte();
                        if (last != '\n' && last != '\r')
                        {
                            content = "\r\n" + content;
                        }
                    }
                }
            }
            File.AppendAllText(path, content, Encoding.UTF8);
        }

        /// <summary>
        /// 过滤Windows路径非法字符，避免生成无法创建的文件夹名。
        /// </summary>
        private static string SanitizeAlias(string alias)
        {
            if (string.IsNullOrEmpty(alias))
                return alias;

            var invalid = new char[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };
            return string.Concat(alias.Split(invalid, StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
