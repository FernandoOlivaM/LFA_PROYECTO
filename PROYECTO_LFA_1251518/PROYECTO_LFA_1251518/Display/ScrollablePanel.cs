
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PROYECTO_LFA_1251518
{
    public class ScrollablePanel : Panel
    {
        private bool enableAutoHorizontal = true;
        private bool enableAutoVertical = true;
        private bool visibleAutoHorizontal = true;
        private bool visibleAutoVertical = true;
        private int autoScrollHorizontalMinimum = 0;
        private int autoScrollHorizontalMaximum = 100;
        private int autoScrollVerticalMinimum = 0;
        private int autoScrollVerticalMaximum = 100;
        private const int SB_LINEUP = 0;
        private const int SB_LINEDOWN = 1;
        private const int SB_PAGEUP = 2;
        private const int SB_PAGEDOWN = 3;
        private const int SB_THUMBPOSITION = 4;
        private const int SB_THUMBTRACK = 5;
        private const int SB_TOP = 6;
        private const int SB_BOTTOM = 7;
        private const int SB_ENDSCROLL = 8;
        private const int WM_HSCROLL = 276;
        private const int WM_VSCROLL = 277;
        private const int WM_MOUSEWHEEL = 522;
        private const int WM_NCCALCSIZE = 131;
        private const int WM_PAINT = 15;
        private const int WM_SIZE = 5;
        private const uint SB_HORZ = 0;
        private const uint SB_VERT = 1;
        private const uint SB_CTL = 2;
        private const uint SB_BOTH = 3;
        private const uint ESB_DISABLE_BOTH = 3;
        private const uint ESB_ENABLE_BOTH = 0;
        private const int MK_LBUTTON = 1;
        private const int MK_RBUTTON = 2;
        private const int MK_SHIFT = 4;
        private const int MK_CONTROL = 8;
        private const int MK_MBUTTON = 16;
        private const int MK_XBUTTON1 = 32;
        private const int MK_XBUTTON2 = 64;

        public event ScrollEventHandler ScrollHorizontal;

        public event ScrollEventHandler ScrollVertical;

        public event MouseEventHandler ScrollMouseWheel;

        public ScrollablePanel()
        {
            this.Click += new EventHandler(this.ScrollablePanel_Click);
            this.AutoScroll = true;
        }

        public int AutoScrollHPos
        {
            get
            {
                return ScrollablePanel.GetScrollPos(this.Handle, 0);
            }
            set
            {
                ScrollablePanel.SetScrollPos(this.Handle, 0, value, true);
            }
        }

        public int AutoScrollVPos
        {
            get
            {
                return ScrollablePanel.GetScrollPos(this.Handle, 1);
            }
            set
            {
                ScrollablePanel.SetScrollPos(this.Handle, 1, value, true);
            }
        }

        public int AutoScrollHorizontalMinimum
        {
            get
            {
                return this.autoScrollHorizontalMinimum;
            }
            set
            {
                this.autoScrollHorizontalMinimum = value;
                ScrollablePanel.SetScrollRange(this.Handle, 0, this.autoScrollHorizontalMinimum, this.autoScrollHorizontalMaximum, true);
            }
        }

        public int AutoScrollHorizontalMaximum
        {
            get
            {
                return this.autoScrollHorizontalMaximum;
            }
            set
            {
                this.autoScrollHorizontalMaximum = value;
                ScrollablePanel.SetScrollRange(this.Handle, 0, this.autoScrollHorizontalMinimum, this.autoScrollHorizontalMaximum, true);
            }
        }

        public int AutoScrollVerticalMinimum
        {
            get
            {
                return this.autoScrollVerticalMinimum;
            }
            set
            {
                this.autoScrollVerticalMinimum = value;
                ScrollablePanel.SetScrollRange(this.Handle, 1, this.autoScrollHorizontalMinimum, this.autoScrollHorizontalMaximum, true);
            }
        }

        public int AutoScrollVerticalMaximum
        {
            get
            {
                return this.autoScrollVerticalMaximum;
            }
            set
            {
                this.autoScrollVerticalMaximum = value;
                ScrollablePanel.SetScrollRange(this.Handle, 1, this.autoScrollHorizontalMinimum, this.autoScrollHorizontalMaximum, true);
            }
        }

        public bool EnableAutoScrollHorizontal
        {
            get
            {
                return this.enableAutoHorizontal;
            }
            set
            {
                this.enableAutoHorizontal = value;
                if (value)
                    ScrollablePanel.EnableScrollBar(this.Handle, 0U, 0U);
                else
                    ScrollablePanel.EnableScrollBar(this.Handle, 0U, 3U);
            }
        }

        public bool EnableAutoScrollVertical
        {
            get
            {
                return this.enableAutoVertical;
            }
            set
            {
                this.enableAutoVertical = value;
                if (value)
                    ScrollablePanel.EnableScrollBar(this.Handle, 1U, 0U);
                else
                    ScrollablePanel.EnableScrollBar(this.Handle, 1U, 3U);
            }
        }

        public bool VisibleAutoScrollHorizontal
        {
            get
            {
                return this.visibleAutoHorizontal;
            }
            set
            {
                this.visibleAutoHorizontal = value;
                ScrollablePanel.ShowScrollBar(this.Handle, 0, value);
            }
        }

        public bool VisibleAutoScrollVertical
        {
            get
            {
                return this.visibleAutoVertical;
            }
            set
            {
                this.visibleAutoVertical = value;
                ScrollablePanel.ShowScrollBar(this.Handle, 1, value);
            }
        }

        private int getSBFromScrollEventType(ScrollEventType type)
        {
            int num = -1;
            switch (type)
            {
                case ScrollEventType.SmallDecrement:
                    num = 0;
                    break;
                case ScrollEventType.SmallIncrement:
                    num = 1;
                    break;
                case ScrollEventType.LargeDecrement:
                    num = 2;
                    break;
                case ScrollEventType.LargeIncrement:
                    num = 3;
                    break;
                case ScrollEventType.ThumbPosition:
                    num = 4;
                    break;
                case ScrollEventType.ThumbTrack:
                    num = 5;
                    break;
                case ScrollEventType.First:
                    num = 6;
                    break;
                case ScrollEventType.Last:
                    num = 7;
                    break;
                case ScrollEventType.EndScroll:
                    num = 8;
                    break;
            }
            return num;
        }

        private ScrollEventType getScrollEventType(IntPtr wParam)
        {
            ScrollEventType scrollEventType;
            switch (ScrollablePanel.LoWord((int)wParam))
            {
                case 0:
                    scrollEventType = ScrollEventType.SmallDecrement;
                    break;
                case 1:
                    scrollEventType = ScrollEventType.SmallIncrement;
                    break;
                case 2:
                    scrollEventType = ScrollEventType.LargeDecrement;
                    break;
                case 3:
                    scrollEventType = ScrollEventType.LargeIncrement;
                    break;
                case 4:
                    scrollEventType = ScrollEventType.ThumbPosition;
                    break;
                case 5:
                    scrollEventType = ScrollEventType.ThumbTrack;
                    break;
                case 6:
                    scrollEventType = ScrollEventType.First;
                    break;
                case 7:
                    scrollEventType = ScrollEventType.Last;
                    break;
                case 8:
                    scrollEventType = ScrollEventType.EndScroll;
                    break;
                default:
                    scrollEventType = ScrollEventType.EndScroll;
                    break;
            }
            return scrollEventType;
        }

        protected override void WndProc(ref Message msg)
        {
            base.WndProc(ref msg);
            if (msg.HWnd != this.Handle)
                return;
            switch (msg.Msg)
            {
                case 276:
                    try
                    {
                        this.ScrollHorizontal((object)this, new ScrollEventArgs(this.getScrollEventType(msg.WParam), ScrollablePanel.GetScrollPos(this.Handle, 0)));
                        break;
                    }
                    catch (Exception)
                    {
                        break;
                    }
                case 277:
                    try
                    {
                        this.ScrollVertical((object)this, new ScrollEventArgs(this.getScrollEventType(msg.WParam), ScrollablePanel.GetScrollPos(this.Handle, 1)));
                        break;
                    }
                    catch (Exception)
                    {
                        break;
                    }
                case 522:
                    if (!this.VisibleAutoScrollVertical)
                        break;
                    try
                    {
                        int delta = ScrollablePanel.HiWord((int)msg.WParam);
                        int y = ScrollablePanel.HiWord((int)msg.LParam);
                        int x = ScrollablePanel.LoWord((int)msg.LParam);
                        MouseButtons button;
                        switch (ScrollablePanel.LoWord((int)msg.WParam))
                        {
                            case 1:
                                button = MouseButtons.Left;
                                break;
                            case 2:
                                button = MouseButtons.Right;
                                break;
                            case 16:
                                button = MouseButtons.Middle;
                                break;
                            case 32:
                                button = MouseButtons.XButton1;
                                break;
                            case 64:
                                button = MouseButtons.XButton2;
                                break;
                            default:
                                button = MouseButtons.None;
                                break;
                        }
                        this.ScrollMouseWheel((object)this, new MouseEventArgs(button, 1, x, y, delta));
                        break;
                    }
                    catch (Exception)
                    {
                        break;
                    }
            }
        }

        public void performScrollHorizontal(ScrollEventType type)
        {
            int fromScrollEventType = this.getSBFromScrollEventType(type);
            if (fromScrollEventType == -1)
                return;
            ScrollablePanel.SendMessage(this.Handle, 276U, (UIntPtr)(ulong)fromScrollEventType, (IntPtr)0);
        }

        public void performScrollVertical(ScrollEventType type)
        {
            int fromScrollEventType = this.getSBFromScrollEventType(type);
            if (fromScrollEventType == -1)
                return;
            ScrollablePanel.SendMessage(this.Handle, 277U, (UIntPtr)(ulong)fromScrollEventType, (IntPtr)0);
        }

        private void ScrollablePanel_Click(object sender, EventArgs e)
        {
            this.Focus();
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetSystemMetrics(int code);

        [DllImport("user32.dll")]
        public static extern bool EnableScrollBar(IntPtr hWnd, uint wSBflags, uint wArrows);

        [DllImport("user32.dll")]
        public static extern int SetScrollRange(
          IntPtr hWnd,
          int nBar,
          int nMinPos,
          int nMaxPos,
          bool bRedraw);

        [DllImport("user32.dll")]
        public static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);

        [DllImport("user32.dll")]
        public static extern int GetScrollPos(IntPtr hWnd, int nBar);

        [DllImport("user32.dll")]
        public static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(
          IntPtr hWnd,
          uint Msg,
          UIntPtr wParam,
          IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern int HIWORD(IntPtr wParam);

        private static int MakeLong(int LoWord, int HiWord)
        {
            return HiWord << 16 | LoWord & (int)ushort.MaxValue;
        }

        private static IntPtr MakeLParam(int LoWord, int HiWord)
        {
            return (IntPtr)(HiWord << 16 | LoWord & (int)ushort.MaxValue);
        }

        private static int HiWord(int number)
        {
            return ((long)number & 2147483648L) == 2147483648L ? number >> 16 : number >> 16 & (int)ushort.MaxValue;
        }

        private static int LoWord(int number)
        {
            return number & (int)ushort.MaxValue;
        }
    }
}
