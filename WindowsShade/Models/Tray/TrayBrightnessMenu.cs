using System;
using System.Windows.Forms;

namespace WindowsShade.Models
{
    internal sealed class TrayBrightnessMenu : IDisposable
    {
        private readonly NotifyIcon _notifyIcon;
        private readonly ContextMenuStrip _contextMenu;
        private readonly Action _openMainClick;
        private readonly Action _closeClick;
        private TrayBrightnessMenuView _view;

        public event Action<bool> ShadeEnabledChanged;
        public event Action<int> AlphaBrightnessChanged;
        public event Action<int> SystemBrightnessChanged;

        public TrayBrightnessMenu(NotifyIcon notifyIcon, ContextMenuStrip contextMenu, Action openMainClick, Action closeClick)
        {
            this._notifyIcon = notifyIcon;
            this._contextMenu = contextMenu;
            this._openMainClick = openMainClick;
            this._closeClick = closeClick;
        }

        public void Initialize()
        {
            // 托盘菜单使用自绘弹窗，避免 ContextMenuStrip 在遮罩置顶和 DPI 切换时出现缩放、阴影或定位不一致。
            this._contextMenu.Opening += (sender, e) => e.Cancel = true;
            this._notifyIcon.ContextMenuStrip = null;
            this.ensureView();
        }

        public void ShowAtCursor()
        {
            this.ensureView();
            this._view.ShowAtCursor();
        }

        public void SetState(int alpha, bool shadeEnabled, bool systemEnabled, int systemMaximum, int systemValue)
        {
            this.ensureView();
            this._view.SetState(alpha, shadeEnabled, systemEnabled, systemMaximum, systemValue);
        }

        public void Invalidate()
        {
            if (this._view != null)
                this._view.Invalidate();
        }

        public void Dispose()
        {
            if (this._view != null)
                this._view.Dispose();
        }

        private void ensureView()
        {
            if (this._view != null)
                return;

            this._view = new TrayBrightnessMenuView(
                this._openMainClick,
                this._closeClick,
                shadeEnabled => this.ShadeEnabledChanged?.Invoke(shadeEnabled),
                alpha => this.AlphaBrightnessChanged?.Invoke(alpha),
                system => this.SystemBrightnessChanged?.Invoke(system));
        }
    }
}
