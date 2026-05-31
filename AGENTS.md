# AGENTS.md

本文件记录 WindowsShade 项目中 AI 协作开发需要遵守的本地规则。后续 AI 处理任务前应先阅读本文件；如果用户的最新明确指令与本文冲突，以用户最新指令为准。

## 项目概览

- 项目根目录：`D:\Projects\Github\#qizl\WindowsShade`
- 主程序目录：`WindowsShade\`
- 主程序项目：`WindowsShade\WindowsShade.csproj`
- 当前解决方案文件：`WindowsShade.slnx`
- 依赖库：`SharpSerializer\SharpSerializer.csproj`
- 测试项目：`WindowsShade.Tests\WindowsShade.Tests.csproj`
- 目标框架：.NET Framework 4.8
- UI 技术：WinForms

历史上的 WPF 项目、BrightnessTrainAPI、本地自动调节和训练数据相关逻辑已移除。后续开发不要重新引入这些旧链路，除非用户明确要求恢复。

## 文件编码与编辑

- 新建和修改文本文件时优先使用 UTF-8，避免引入乱码。
- 修改已有文件时尽量保持原文件的换行、语言和局部风格，不做无关格式化。
- 在 Windows PowerShell 中读取或写入包含中文的文件时，显式指定 UTF-8，避免依赖默认编码。
- 不要把 Markdown、C#、XML、配置文件中的中文改写成 `\uXXXX` 或 HTML 实体转义，除非原文件已经采用该形式。
- 不修改 `bin\`、`obj\`、`.vs\`、`packages\` 下由工具生成或还原的产物，除非用户明确要求处理。
- 手工编辑代码优先使用小范围补丁，避免把设计器文件、资源文件或项目文件整体重排。

## 项目结构与职责

- `WindowsShade\FormMain.cs` 负责主窗体、托盘菜单、配置加载保存、多屏遮罩列表和计时器协调。
- `WindowsShade\Views\FormShade.cs` 负责单个显示器的遮罩窗口行为。
- `WindowsShade\Models\` 保存配置、显示器信息、屏幕亮度和公共模型。
- `SharpSerializer\` 是配置序列化依赖库，日常功能开发不应随意改动。
- `WindowsShade.Tests\` 是测试项目；新增可测试逻辑时优先补充或更新测试。

保持主程序为轻量 WinForms 工具。新增功能时优先沿用现有窗体、模型和配置保存方式，不引入新的框架、后台服务或复杂依赖。

## 遮罩窗口规则

遮罩功能是项目核心，修改时必须保持以下行为：

- 遮罩窗口应为无边框全屏窗口，覆盖对应 `Screen.Bounds`，包括任务栏区域。
- 遮罩应鼠标穿透，不影响用户点击底层窗口、任务栏、托盘和按钮。
- 遮罩显示、置顶和透明度调整时不得抢占输入焦点。
- 遮罩被其他窗口覆盖后，应通过轻量方式恢复顶层顺序。
- 遮罩透明度由 layered window alpha 控制，不要混用 WinForms `Opacity` 造成行为不一致。
- 截图和常见系统捕获场景中应默认排除遮罩，优先使用 `SetWindowDisplayAffinity(..., WDA_EXCLUDEFROMCAPTURE)`。
- 不通过全局键盘钩子拦截 `PrintScreen`、`Win+Shift+S` 等快捷键，除非用户明确要求采用该路线。

涉及 `CreateParams`、扩展窗口样式、P/Invoke 或 `SetWindowPos` 的改动，必须特别注意焦点、置顶、点击穿透、多屏和截图行为的回归风险。

## 配置与用户体验

- 不随意改变配置文件结构；需要新增配置项时先确认兼容旧配置。
- 保留现有托盘菜单、自动隐藏、自动显示遮罩、多屏启用和单屏透明度体验。
- UI 不新增选项时，不要为了内部实现细节暴露新设置。
- 屏幕分辨率、排列和数量变化后，遮罩应重新匹配正确屏幕位置。
- 不支持或不保证覆盖 UAC 安全桌面、锁屏、全屏独占游戏、驱动级截图或外部拍摄等 Windows 限制场景。

## 构建与验证

优先使用 Visual Studio/MSBuild 构建 .NET Framework 项目：

```powershell
& 'D:\Program Files\Microsoft Visual Studio\2026\MSBuild\Current\Bin\MSBuild.exe' ..\WindowsShade.slnx /p:Configuration=Debug /p:Platform="Any CPU" /m
```

只验证主程序时可构建：

```powershell
& 'D:\Program Files\Microsoft Visual Studio\2026\MSBuild\Current\Bin\MSBuild.exe' .\WindowsShade.csproj /p:Configuration=Debug /p:Platform=AnyCPU /m
```

构建前如果本次修改触及会被构建的项目，并且项目存在四段式版本号，按项目规则只递增 Revision：

- `1.0.0.3` -> `1.0.0.4`
- 不改 Major、Minor、Build，除非用户明确要求。
- 不更新 NuGet 依赖版本。

构建结果需要在回复中说明，包括警告和失败原因。当前已知可能出现的历史警告包括：

- `Monitor.No` obsolete 警告。
- `SharpSerializer` 缺少 XML 注释警告。

## 测试重点

修改遮罩、主窗体或屏幕配置逻辑后，至少考虑以下场景：

- 启动后主屏和副屏遮罩覆盖完整屏幕，包括任务栏。
- 遮罩可见时仍可点击任务栏、托盘和底层窗口。
- 输入、拖拽、窗口切换时遮罩不抢焦点。
- 打开置顶窗口后，遮罩能自动恢复到其上方。
- 调整全局透明度、单屏透明度、启用/停用单屏后行为正常。
- 显示器分辨率、排列或数量变化后遮罩位置正确。
- 使用 `Win+Shift+S`、`PrintScreen` 或截图工具时，截图结果尽量不包含遮罩。

无法自动验证的桌面交互，需要在最终回复中明确说明需要用户本机手动验收。

## Git 协作

- 提交前先检查 `git status --short --branch` 和 staged diff，确认没有误带无关文件。
- 用户明确说“先不要 commit”时，不要提交，也不要主动 stage 新改动。
- 当前仓库可能存在用户手动修改或删除的文件，不要擅自恢复、重置或覆盖。
- 如果发现工作区有自己未创建的改动，先判断是否与当前任务相关；无关则不要碰。
- 提交信息优先使用 Conventional Commits，例如：
  - `fix(shade): exclude overlay from capture`
  - `refactor: remove legacy wpf project`
  - `docs: update project guide`
- 用户要求 commit 时，默认提交当前已确认的工作区改动；如只应提交部分文件，先说明分组依据。

## 文档维护

- `README.md` 面向项目使用者，记录项目用途、当前功能、构建方式和重要限制。
- `AGENTS.md` 面向 AI/协作者，记录后续开发默认遵守的规则、边界和验证方式。
- 不把一次性的排查过程、临时尝试或聊天记录写进 `AGENTS.md`；只有对后续开发有稳定复用价值的规则才沉淀到这里。
- 如果后续项目结构、构建入口或核心实现路线变化，应同步更新 `README.md` 和本文件。
