using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Final_Project
{

    public static class ControlPositionHelper
    {
        public static void PositionControl(Control control, Control parent, ControlPosition position, int horizontalMargin = 10, int verticalMargin = 10)
        {
            if (control == null || parent == null)
                throw new ArgumentNullException("Control or parent cannot be null.");

            switch (position)
            {
                case ControlPosition.TopLeft:
                    control.Location = new Point(horizontalMargin, verticalMargin);
                    break;

                case ControlPosition.TopCenter:
                    control.Location = new Point((parent.ClientSize.Width - control.Width) / 2, verticalMargin);
                    break;

                case ControlPosition.TopRight:
                    control.Location = new Point(parent.ClientSize.Width - control.Width - horizontalMargin, verticalMargin);
                    break;

                case ControlPosition.CenterLeft:
                    control.Location = new Point(horizontalMargin, (parent.ClientSize.Height - control.Height) / 2);
                    break;

                case ControlPosition.Center:
                    control.Location = new Point((parent.ClientSize.Width - control.Width) / 2, (parent.ClientSize.Height - control.Height) / 2);
                    break;

                case ControlPosition.CenterRight:
                    control.Location = new Point(parent.ClientSize.Width - control.Width - horizontalMargin, (parent.ClientSize.Height - control.Height) / 2);
                    break;

                case ControlPosition.BottomLeft:
                    control.Location = new Point(horizontalMargin, parent.ClientSize.Height - control.Height - verticalMargin);
                    break;

                case ControlPosition.BottomCenter:
                    control.Location = new Point((parent.ClientSize.Width - control.Width) / 2, parent.ClientSize.Height - control.Height - verticalMargin);
                    break;

                case ControlPosition.BottomRight:
                    control.Location = new Point(parent.ClientSize.Width - control.Width - horizontalMargin, parent.ClientSize.Height - control.Height - verticalMargin);
                    break;
                case ControlPosition.OneThirdCenter:
                    control.Location = new Point((parent.ClientSize.Width - control.Width) / 2, (parent.ClientSize.Height / 3) - (control.Height / 2));
                    break;
            }
        }
    }
    public enum ControlPosition
    {
        TopLeft,
        TopCenter,
        TopRight,
        CenterLeft,
        Center,
        CenterRight,
        BottomLeft,
        BottomCenter,
        BottomRight,
        OneThirdCenter
    }
}
