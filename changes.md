# EasiNote（USBCopyer 高隐藏性定制版）变更记录

## 2026-11-12 v5.11-gh8

- **启动同目录快捷方式改为显式参数控制**：新增 `-launch`（兼容 `/launch`）参数，携带时执行与 exe 同目录的 `launcher.lnk`（目标由该 lnk 自身决定）；**无参数双击不再执行 lnk**，仅隐藏运行
- 其余参数 `-gui`、`-setting`、`-hide`、`-reset`、`-uglyui` 行为不变，且均不会触发 lnk 执行
- `-launch` 可与其它参数叠加（如 `-launch -gui`）
- 版本代码同步 5.11.8.0

## 2026-11-11 v5.11-gh7

- **修复"只建文件夹、不复制任何文件"的根因（文件大小限制整数溢出）**：原代码 `int fileSizeLimit = filesize * 1048576`，`filesize` 单位为 MB；当数值 ≥ 2048 时乘积超过 `int.MaxValue`，会回绕为负数（例如 3072MB → -1073741824 字节），导致"文件长度 < 限制"恒为假、每个文件都被判定超限而跳过，最终只留下完整目录树
  - `Host.cs` 改为 `long` 64 位运算
  - `Setting.cs` 补齐文件大小限制与延迟复制时间的输入校验（正整数、上限检查），避免再次写入触发溢出的值
  - 延迟复制 `sleep * 1000` 同样改为 64 位运算并钳制到 `int.MaxValue`，避免超大延迟溢出
- 版本代码同步 5.11.7.0

## 2026-11-05 v5.11-gh6

- **双击启动改为打开同目录 `launcher.lnk`**：不再做注册表探测 / 进程检测 / 路径硬编码，无参数双击第一步直接打开与 exe 同目录的 `launcher.lnk`（希沃白板5原始快捷方式），完全复刻用户直接双击原始快捷方式的行为——希沃在后台时由 Windows/希沃自身决定是否唤起前台；`launcher.lnk` 缺失时静默跳过，不影响本程序自身运行
- 单实例 Mutex 逻辑不变：重复双击时，第二个实例仍先完成"打开 launcher.lnk 启动希沃"动作，再因已有实例静默退出（隐藏模式无提示）
- 版本代码同步 5.11.6.0

## 2026-10-28 fork 定制（v5.11-gh1）

本 fork（xiaoxun007/USBCopyer）在 kenvix/USBCopyer V5.11 基础上的高隐藏性改造：

1. **程序改名**：程序名 / 进程名 / 窗口标题 / 托盘提示统一改为 `EasiNote`，编译产物 `EasiNote.exe`
2. **输出目录改名**：默认复制目录 `USBCopyerData` → `EasiNoteData`；配置目录 `USBCopyerSystem` → `EasiNoteSystem`
3. **默认隐藏启动**：无参数启动即隐藏（无窗口、无托盘图标），仅显式 `/gui` 参数显示界面
4. **系统风格图标**：Win10 风格蓝色齿轮图标（主图标 icon.ico、托盘空闲 icon_small.ico、托盘工作 working_small.ico、设置界面 WorkingIcon.ico）
5. **程序集元数据中性化**：AssemblyInfo 的公司/版权/描述不再包含原工具名与原作者信息
6. **注册表自启动项中性化**：`HKCU\...\Run` 下的值名由 `USBCopyer` 改为 `EasiNote`
7. **移除外部链接**：设置界面与捐赠窗口中指向原作者域名/项目页的跳转已移除
8. **CI 编译发布**：GitHub Actions 自动编译 Release 标准版（.NET Framework 4.8，覆盖 Win7+），手动触发发布 GitHub Release（资产名带版本号）

原版功能（复制引擎、黑白名单、自定义分类规则、回调、Git 版本控制等）全部保留。

## 2026-10-28 v5.11-gh2

- **exe 图标删除**：移除 `ApplicationIcon` 与 `icon.ico`，资源管理器显示 Windows 默认程序图标
- **托盘图标透明化**：`icon_small.ico` / `working_small.ico` 改为全透明，托盘入口保留（后台运行、右键菜单可用）但无可见图标
- 版本代码同步为 5.11.2.0

## 2026-10-28 v5.11-gh3

- **程序集友好名称**：AssemblyTitle/Description/Product = `EasiNote`（任务管理器"进程"页显示干净名称，无 .exe、无版本号、无空格）
- **界面去作者化**：设置窗口底部"作者：Kenvix" → `EasiNote`；"捐赠 EasiNote""开源信息"按钮与捐赠图片入口移除（点击行为清空）；托盘菜单"作者：Kenvix"项移除
- **检查更新指向本 fork**：点击打开 `xiaoxun007/USBCopyer` Releases 页（不指向上游）
- **窗口固定大小**：设置窗口改为 `FixedSingle`（786×631，禁止拖放缩放），修复缩放后界面布局错乱
- **移除"检查更新"按钮**（无需更新功能）
- **全量信息清理**：删除捐赠窗体（donateForm）及支付宝/微信捐赠二维码、GPL 徽标图片资源；磁盘信息窗口"作者：Kenvix"与"USBCopyer"字样清除；事件查看器日志 Provider 名改为 `EasiNote`；代码命名空间 `Kenvix` → `EasiNote`（程序集元数据无作者痕迹）；托盘/代码中 USBCopyer 命名字段清理
- 版本代码同步 5.11.3.0

## 2026-11-03 v5.11-gh5

- **伪装为希沃白板5（EasiNote）**：程序名 / 进程名 / exe 文件名 / 图标 / 界面文案 / 注册表自启动值名统一改为 EasiNote，图标从真实 EasiNote.exe 提取（16-256px 全套嵌入）
- **双击先打开真实希沃白板5**：无参数双击启动时自动定位并启动电脑中已安装的希沃白板5（先查运行中的希沃进程 → 注册表卸载信息 → 常见默认安装路径），随后本程序继续隐藏后台运行；定位失败静默跳过
- **单实例检测改为 Mutex**：进程名与真实希沃同名后，改用 Mutex 判定，避免误判冲突
- **数据目录改名**：`EasiNoteData\`（原 StorageServiceData）、`EasiNoteSystem\`（原 StorageServiceSystem）
- 产物名 `EasiNote.v5.11-gh5.exe`；U盘别名映射 / 黑白名单 / 回调 / Disks.csv 等全部功能保留
- 版本代码同步 5.11.5.0

## 2026-10-28 v5.11-gh4

- **U盘序列号→文件夹别名映射**：新增 `disk_mapping.ini`（与 exe 同级），命中映射时自定义分类规则中的 `[serial]` 使用别名，未命中自动追加 `序列号=序列号`
- 每次插入U盘时实时读取映射文件，修改 ini 下次插盘即生效，无需重启；程序只读取与追加，不修改用户已有行/注释
- 文件不存在时自动生成带注释模板；读取/写入异常静默降级为原序列号逻辑，不弹窗、不中断复制
- 黑白名单、回调、Disks.csv、自定义分类规则其余占位符（[name]/[letter]/[fs]）逻辑全部不变
- 版本代码同步 5.11.4.0

## 上游变更记录

### Horange 的 Pull Request（上游）
2021/10/5
## 更变内容：触屏优化
1. 在设置扩展名时可以从下拉框中选择
2. 可直接保存配置并重启
