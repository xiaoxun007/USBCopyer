# StorageService（USBCopyer 高隐藏性定制版）变更记录

## 2026-10-28 fork 定制（v5.11-gh1）

本 fork（xiaoxun007/USBCopyer）在 kenvix/USBCopyer V5.11 基础上的高隐藏性改造：

1. **程序改名**：程序名 / 进程名 / 窗口标题 / 托盘提示统一改为 `StorageService`，编译产物 `StorageService.exe`
2. **输出目录改名**：默认复制目录 `USBCopyerData` → `StorageServiceData`；配置目录 `USBCopyerSystem` → `StorageServiceSystem`
3. **默认隐藏启动**：无参数启动即隐藏（无窗口、无托盘图标），仅显式 `/gui` 参数显示界面
4. **系统风格图标**：Win10 风格蓝色齿轮图标（主图标 icon.ico、托盘空闲 icon_small.ico、托盘工作 working_small.ico、设置界面 WorkingIcon.ico）
5. **程序集元数据中性化**：AssemblyInfo 的公司/版权/描述不再包含原工具名与原作者信息
6. **注册表自启动项中性化**：`HKCU\...\Run` 下的值名由 `USBCopyer` 改为 `StorageService`
7. **移除外部链接**：设置界面与捐赠窗口中指向原作者域名/项目页的跳转已移除
8. **CI 编译发布**：GitHub Actions 自动编译 Release 标准版（.NET Framework 4.8，覆盖 Win7+），手动触发发布 GitHub Release（资产名带版本号）

原版功能（复制引擎、黑白名单、自定义分类规则、回调、Git 版本控制等）全部保留。

## 2026-10-28 v5.11-gh2

- **exe 图标删除**：移除 `ApplicationIcon` 与 `icon.ico`，资源管理器显示 Windows 默认程序图标
- **托盘图标透明化**：`icon_small.ico` / `working_small.ico` 改为全透明，托盘入口保留（后台运行、右键菜单可用）但无可见图标
- 版本代码同步为 5.11.2.0

## 2026-10-28 v5.11-gh3

- **程序集友好名称**：AssemblyTitle/Description/Product = `StorageService`（任务管理器"进程"页显示干净名称，无 .exe、无版本号、无空格）
- **界面去作者化**：设置窗口底部"作者：Kenvix" → `StorageService`；"捐赠 StorageService""开源信息"按钮与捐赠图片入口移除（点击行为清空）；托盘菜单"作者：Kenvix"项移除
- **检查更新指向本 fork**：点击打开 `xiaoxun007/USBCopyer` Releases 页（不指向上游）
- 版本代码同步 5.11.3.0

## 上游变更记录

### Horange 的 Pull Request（上游）
2021/10/5
## 更变内容：触屏优化
1. 在设置扩展名时可以从下拉框中选择
2. 可直接保存配置并重启
