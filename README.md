# WindowsShade

WindowsShade 是一个 Windows 多屏亮度调节工具。它通过在每个显示器上放置独立的系统级遮罩窗口来降低视觉亮度，适合显示器本身亮度调节不方便、或多屏亮度需要分别控制的场景。

<img src="https://qizl.cn/Images/WindowsShade/6366890016291518825946326.png" alt="WindowsShade" />

## 功能

- 支持多显示器独立启用、禁用遮罩。
- 支持全局亮度和单屏亮度调节。
- 遮罩覆盖完整屏幕区域，包括 Windows 任务栏。
- 遮罩窗口鼠标穿透，不影响点击窗口、任务栏和托盘。
- 遮罩窗口不抢焦点，不打断当前输入、拖拽或窗口操作。
- 遮罩会维护顶层顺序，被其他窗口覆盖后会自动恢复。
- 截图和常见系统捕获场景中默认排除遮罩，截图结果尽量保持原始屏幕内容。

## 当前实现

主程序是 WinForms 项目 `WindowsShade`，目标框架为 .NET Framework 4.8。遮罩逻辑集中在 `Views/FormShade.cs`：

- 使用无边框、非激活、工具窗口样式创建遮罩窗口。
- 使用 layered window alpha 控制遮罩透明度。
- 使用 `SetWindowPos(HWND_TOPMOST, ...)` 维护顶层顺序。
- 使用 `SetWindowDisplayAffinity(..., WDA_EXCLUDEFROMCAPTURE)` 尽量从截图/录屏捕获中排除遮罩。

历史上的 WPF 项目、亮度训练 API、本地自动调节和训练数据相关逻辑已移除。

## 构建

使用 Visual Studio 或 MSBuild 构建当前解决方案：

```powershell
MSBuild.exe WindowsShade.slnx /p:Configuration=Debug /p:Platform="Any CPU" /m
```

也可以只构建主程序：

```powershell
MSBuild.exe WindowsShade\WindowsShade.csproj /p:Configuration=Debug /p:Platform=AnyCPU /m
```

解决方案当前包含：

- `WindowsShade`：WinForms 主程序。
- `SharpSerializer`：配置序列化依赖库。
- `WindowsShade.Tests`：测试项目。

## 说明

截图排除依赖 Windows 的窗口显示亲和性能力，优先支持 Windows 10 2004+ 和 Windows 11。它适用于常见系统截图和多数基于 Windows 合成器的捕获方式，但不保证覆盖所有第三方、驱动级或外部拍摄场景。

项目原始介绍：

http://enjoycodes.com/ViewNote/dc7e3d7e-c462-465e-b20e-e4726beafb81
