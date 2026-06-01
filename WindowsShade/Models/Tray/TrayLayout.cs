using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsShade.Models
{
    internal struct TrayLayout
    {
        private const float BaseDpi = 144F;

        public Size PopupSize;
        public Padding PanelPadding;
        public Rectangle ShadeCheckBoxBounds;
        public Rectangle AlphaCaptionBounds;
        public Rectangle AlphaValueBounds;
        public Rectangle AlphaTrackBarBounds;
        public Rectangle SystemCaptionBounds;
        public Rectangle SystemValueBounds;
        public Rectangle SystemTrackBarBounds;
        public Rectangle OpenMainBounds;
        public Rectangle CloseBounds;
        public float CaptionFontSize;
        public float ValueFontSize;
        public float MenuFontSize;
        public int MenuTextVerticalPadding;
        public int MenuTextLeftPadding;
        public int SliderTrackHeight;
        public int SliderThumbRadius;

        public static TrayLayout Create(float dpiScale, Rectangle workingArea)
        {
            // 低 DPI 与高 DPI 菜单的视觉调参完全拆开，后续修 1080 不应改动 4K 布局。
            if (dpiScale < 1.25F)
                return CreateCompact(workingArea);

            return CreateRegular(dpiScale, workingArea);
        }

        private static TrayLayout CreateCompact(Rectangle workingArea)
        {
            // 1080p/100% 菜单需要更小字体、更细滑块和更紧凑高度，避免托盘附近显得过大。
            const float scale = 0.70F;
            var minWidth = scaleValue(292, scale);
            var preferredWidth = scaleValue(310, scale);
            var maxWidth = Math.Max(minWidth, workingArea.Width - scaleValue(24, scale));
            var popupWidth = clamp(preferredWidth, minWidth, maxWidth);
            var horizontalPadding = scaleValue(22, scale);
            var checkWidth = Math.Max(TrayCheckBox.PreferredControlWidth, scaleValue(22, scale));
            var contentLeft = horizontalPadding + scaleValue(10, scale);
            var captionLeft = contentLeft;
            var alphaCaptionLeft = contentLeft + checkWidth + scaleValue(6, scale);
            var valueWidth = scaleValue(54, scale);
            var sliderWidth = Math.Max(scaleValue(150, scale), popupWidth - captionLeft - horizontalPadding);
            var captionY = scaleValue(14, scale);
            var captionHeight = scaleValue(32, scale);
            var alphaTrackY = scaleValue(52, scale);
            var systemCaptionY = scaleValue(94, scale);
            var systemTrackY = scaleValue(134, scale);
            var menuTop = scaleValue(174, scale);
            var menuHeight = scaleValue(49, scale);
            var checkHeight = scaleValue(TrayCheckBox.PreferredControlHeight, scale);
            var contentHeight = menuTop + menuHeight * 2 + scaleValue(8, scale);
            var popupHeight = Math.Min(contentHeight, Math.Max(contentHeight, workingArea.Height - scaleValue(24, scale)));
            var trackBarHeight = scaleValue(22, scale);
            var trackBarBounds = new Rectangle(captionLeft, 0, sliderWidth, trackBarHeight);
            var valueX = popupWidth - horizontalPadding - valueWidth;

            return new TrayLayout
            {
                PopupSize = new Size(popupWidth, popupHeight),
                PanelPadding = new Padding(horizontalPadding, scaleValue(10, scale), horizontalPadding, scaleValue(10, scale)),
                ShadeCheckBoxBounds = new Rectangle(contentLeft, captionY + scaleValue(2, scale) + (captionHeight - checkHeight) / 2, checkWidth, checkHeight),
                AlphaCaptionBounds = new Rectangle(alphaCaptionLeft, captionY, scaleValue(80, scale), captionHeight),
                AlphaValueBounds = new Rectangle(valueX, captionY, valueWidth, captionHeight),
                AlphaTrackBarBounds = new Rectangle(trackBarBounds.X, alphaTrackY, trackBarBounds.Width, trackBarBounds.Height),
                SystemCaptionBounds = new Rectangle(captionLeft, systemCaptionY, valueX - captionLeft, captionHeight),
                SystemValueBounds = new Rectangle(valueX, systemCaptionY, valueWidth, captionHeight),
                SystemTrackBarBounds = new Rectangle(trackBarBounds.X, systemTrackY, trackBarBounds.Width, trackBarBounds.Height),
                OpenMainBounds = new Rectangle(scaleValue(8, scale), menuTop, popupWidth - scaleValue(16, scale), menuHeight),
                CloseBounds = new Rectangle(scaleValue(8, scale), menuTop + menuHeight, popupWidth - scaleValue(16, scale), menuHeight),
                CaptionFontSize = 9F,
                ValueFontSize = 9F,
                MenuFontSize = 10F,
                MenuTextVerticalPadding = 8,
                MenuTextLeftPadding = contentLeft - scaleValue(8, scale),
                SliderTrackHeight = 3,
                SliderThumbRadius = 5
            };
        }

        private static TrayLayout CreateRegular(float dpiScale, Rectangle workingArea)
        {
            // 4K/150% 等高 DPI 菜单保持较宽松布局，优先保证字体和控件观感稳定。
            var scale = Math.Max(1F, dpiScale / (BaseDpi / 96F));
            var minWidth = scaleValue(300, scale);
            var preferredWidth = scaleValue(344, scale);
            var maxWidth = Math.Max(minWidth, workingArea.Width - scaleValue(24, scale));
            var popupWidth = clamp(preferredWidth, minWidth, maxWidth);
            var horizontalPadding = scaleValue(26, scale);
            var checkWidth = scaleValue(22, scale);
            var contentLeft = scaleValue(30, scale);
            var captionLeft = scaleValue(55, scale);
            var valueWidth = scaleValue(54, scale);
            var valueGap = scaleValue(10, scale);
            var sliderWidth = Math.Max(scaleValue(150, scale), popupWidth - horizontalPadding * 2);
            var captionY = scaleValue(12, scale);
            var captionHeight = scaleValue(30, scale);
            var alphaTrackY = scaleValue(58, scale);
            var systemCaptionY = scaleValue(102, scale);
            var systemTrackY = scaleValue(148, scale);
            var menuTop = scaleValue(188, scale);
            var menuHeight = scaleValue(55, scale);
            var checkHeight = scaleValue(TrayCheckBox.PreferredControlHeight, scale);
            var contentHeight = menuTop + menuHeight * 2 + scaleValue(12, scale);
            var popupHeight = Math.Min(scaleValue(310, scale), Math.Max(contentHeight, workingArea.Height - scaleValue(24, scale)));
            var trackBarHeight = scaleValue(22, scale);
            var trackBarWidth = sliderWidth;
            var valueX = popupWidth - horizontalPadding - valueWidth;

            return new TrayLayout
            {
                PopupSize = new Size(popupWidth, popupHeight),
                PanelPadding = new Padding(horizontalPadding, scaleValue(14, scale), horizontalPadding, scaleValue(14, scale)),
                ShadeCheckBoxBounds = new Rectangle(contentLeft, captionY + scaleValue(2, scale) + (captionHeight - checkHeight) / 2, checkWidth, checkHeight),
                AlphaCaptionBounds = new Rectangle(captionLeft, captionY, scaleValue(60, scale), captionHeight),
                AlphaValueBounds = new Rectangle(valueX, captionY, valueWidth, captionHeight),
                AlphaTrackBarBounds = new Rectangle(horizontalPadding, alphaTrackY, trackBarWidth, trackBarHeight),
                SystemCaptionBounds = new Rectangle(horizontalPadding, systemCaptionY, valueX - horizontalPadding, captionHeight),
                SystemValueBounds = new Rectangle(valueX, systemCaptionY, valueWidth, captionHeight),
                SystemTrackBarBounds = new Rectangle(horizontalPadding, systemTrackY, sliderWidth, trackBarHeight),
                OpenMainBounds = new Rectangle(scaleValue(8, scale), menuTop, popupWidth - scaleValue(16, scale), menuHeight),
                CloseBounds = new Rectangle(scaleValue(8, scale), menuTop + menuHeight, popupWidth - scaleValue(16, scale), menuHeight),
                CaptionFontSize = 11F,
                ValueFontSize = 11F,
                MenuFontSize = 13F,
                MenuTextVerticalPadding = 8,
                MenuTextLeftPadding = 22,
                SliderTrackHeight = 4,
                SliderThumbRadius = 7
            };
        }

        private static int clamp(int value, int minimum, int maximum)
        {
            return Math.Max(minimum, Math.Min(maximum, value));
        }

        private static int scaleValue(int value, float scale)
        {
            return Math.Max(1, (int)Math.Round(value * scale));
        }
    }
}
