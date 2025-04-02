using System;
using System.Drawing;
using System.Drawing.Text;
using System.Reflection;
using System.Windows.Forms;

namespace Final_Project
{
    public class CustomLabel : Label
    {
        private static PrivateFontCollection _fontCollection;
        private Font _customFont;

        public CustomLabel(float fontSize = 12f, FontStyle fontStyle = FontStyle.Regular)
        {
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer, true);

            // Enable AutoSize if desired
            AutoSize = true;

            LoadCustomFont();

            if (_fontCollection != null && _fontCollection.Families.Length > 0)
            {
                var fontFamily = _fontCollection.Families[0];
                if (fontFamily.IsStyleAvailable(fontStyle))
                {
                    _customFont = new Font(fontFamily, fontSize, fontStyle);
                }
                else
                {
                    throw new Exception("FontStyle.Regular is not available for this font.");
                }
            }
            else
            {
                throw new Exception("Custom font loaded but not applied. Verify font loading and assignment.");
            }

            // You can still set this, but we will actually respect it in OnPaint below
            TextAlign = ContentAlignment.MiddleCenter;
        }

        private void LoadCustomFont()
        {
            if (_fontCollection != null) return;

            try
            {
                _fontCollection = new PrivateFontCollection();

                // Replace with your namespace and resource path
                var fontResource = Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("Final_Project.Resources.Fonts.Cubic_11.ttf");

                if (fontResource == null)
                {
                    throw new Exception("Font resource not found. Check the namespace and resource name.");
                }

                byte[] fontData = new byte[fontResource.Length];
                fontResource.Read(fontData, 0, (int)fontResource.Length);
                IntPtr fontPtr = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(fontData.Length);
                System.Runtime.InteropServices.Marshal.Copy(fontData, 0, fontPtr, fontData.Length);

                _fontCollection.AddMemoryFont(fontPtr, fontData.Length);
                System.Runtime.InteropServices.Marshal.FreeCoTaskMem(fontPtr);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading custom font: {ex.Message}");
            }
        }

        public static Font GetCustomFont(float size, FontStyle style = FontStyle.Regular)
        {
            if (_fontCollection == null || _fontCollection.Families.Length == 0)
                throw new Exception("Custom font not loaded.");

            var fontFamily = _fontCollection.Families[0];
            if (!fontFamily.IsStyleAvailable(style))
                throw new Exception($"The style {style} is not available.");

            return new Font(fontFamily, size, style);
        }


        // Override so AutoSize can compute its size with the custom font
        public override Size GetPreferredSize(Size proposedSize)
        {
            if (_customFont == null || string.IsNullOrEmpty(Text))
            {
                return base.GetPreferredSize(proposedSize);
            }

            using (Graphics g = CreateGraphics())
            {
                SizeF textSize = g.MeasureString(Text, _customFont);
                int width = (int)Math.Ceiling(textSize.Width) + Padding.Left + Padding.Right;
                int height = (int)Math.Ceiling(textSize.Height) + Padding.Top + Padding.Bottom;
                return new Size(width, height);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Paint the background (but not default Label text)
            base.OnPaintBackground(e);

            if (_customFont == null) return;

            Graphics g = e.Graphics;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            SizeF textSize = g.MeasureString(Text, _customFont);

            // Compute the correct draw position based on TextAlign and Padding
            PointF pos = ComputeTextPosition(textSize, TextAlign);

            using (var textBrush = new SolidBrush(ForeColor))
            {
                g.DrawString(Text, _customFont, textBrush, pos);
            }
        }

        private PointF ComputeTextPosition(SizeF textSize, ContentAlignment align)
        {
            // Default to top-left
            float x = Padding.Left;
            float y = Padding.Top;

            switch (align)
            {
                case ContentAlignment.TopLeft:
                    x = Padding.Left;
                    y = Padding.Top;
                    break;
                case ContentAlignment.TopCenter:
                    x = (Width - textSize.Width) / 2f;
                    y = Padding.Top;
                    break;
                case ContentAlignment.TopRight:
                    x = Width - textSize.Width - Padding.Right;
                    y = Padding.Top;
                    break;
                case ContentAlignment.MiddleLeft:
                    x = Padding.Left;
                    y = (Height - textSize.Height) / 2f;
                    break;
                case ContentAlignment.MiddleCenter:
                    x = (Width - textSize.Width) / 2f;
                    y = (Height - textSize.Height) / 2f;
                    break;
                case ContentAlignment.MiddleRight:
                    x = Width - textSize.Width - Padding.Right;
                    y = (Height - textSize.Height) / 2f;
                    break;
                case ContentAlignment.BottomLeft:
                    x = Padding.Left;
                    y = Height - textSize.Height - Padding.Bottom;
                    break;
                case ContentAlignment.BottomCenter:
                    x = (Width - textSize.Width) / 2f;
                    y = Height - textSize.Height - Padding.Bottom;
                    break;
                case ContentAlignment.BottomRight:
                    x = Width - textSize.Width - Padding.Right;
                    y = Height - textSize.Height - Padding.Bottom;
                    break;
            }

            return new PointF(x, y);
        }
    }
}
