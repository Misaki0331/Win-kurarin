using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;

namespace KyukurarinForm
{
   
    public partial class Forms : Form
    {
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hWnd, int attr, ref int pvAttribute, int cbAttribute);

        public Forms(List<Movement> move,Bitmap map,Arial.Position position= Arial.Position.Center,int x=0,int y=0,bool topmost=false)
        {
            InitializeComponent();
            int titleBarColor = ColorTranslator.ToWin32(Color.FromArgb(254, 201, 215));
            DwmSetWindowAttribute(Handle, 35, ref titleBarColor, sizeof(int));
            Show();
            TopMost = topmost;
            pos = position;
            moves = move;
            ImageSizeW= map.Width;
            ImageSizeH= map.Height;
            this.pictureBox1.Image = map;
            //UpdateFormDisplay(map);
            int h = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            int w = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            h -= 180;
            offsetScaling = h / 480.0;
            offsetTopY = (int)(90);
            offsetTopX = (int)(w - 640.0 * offsetScaling)/2;
            _raws = map.Size;
            Size = new((int)(map.Width*offsetScaling) + WindowOffsetW, (int)(map.Height*offsetScaling) + WindowOffsetH);
            for(int i = 0; i < move.Count; i++)
            {
                if (move[i].Type.Trim() == "S")
                {
                    RawSize = new((int)(ImageSizeW * move[i].MoveFrom) + WindowOffsetW, (int)(ImageSizeH * move[i].MoveFrom) + WindowOffsetH);
                    break;
                }
            }
            RawLocation = new(x, y);
            for (int j = 0; j < moves.Count; j++)
            {
                if (moves[0].TimeStart != moves[j].TimeStart) break;
                if (moves[j].Type.Trim() == "MX" || moves[j].Type.Trim() == "MY" || moves[j].Type.Trim() == "M")
                {
                    switch (moves[j].Type.Trim())
                    {
                        case "MX":
                            System.Diagnostics.Trace.WriteLine($"最初の座標MX {moves[j].MoveFrom} {(double.IsNaN(moves[j].SubValue) ? RawLocation.Y : (float)moves[j].SubValue)}");
                            RawLocation = new((float)moves[j].MoveFrom, double.IsNaN(moves[j].SubValue) ? RawLocation.Y : (float)moves[j].SubValue);
                            break;
                        case "MY":
                            System.Diagnostics.Trace.WriteLine($"最初の座標MY {(double.IsNaN(moves[j].SubValue) ? RawLocation.X : (float)moves[j].SubValue)} {moves[j].MoveFrom}");
                            RawLocation = new(double.IsNaN(moves[j].SubValue) ? RawLocation.X : (float)moves[j].SubValue, (float)moves[j].MoveFrom);
                            break;
                        case "M":
                            System.Diagnostics.Trace.WriteLine($"最初の座標M  {move[j].MoveFrom} {move[j].MoveEnd}");
                            RawLocation = new((float)moves[j].MoveFrom, (float)moves[j].MoveEnd);
                            break;
                    }
                    break;
                }
            }
            for (int j = 0; j < moves.Count; j++)
            {
                if (moves[j].Type.Trim() == "F" && move[0].TimeStart == move[j].TimeStart)
                {
                    Opacity = moves[j].MoveFrom;
                }
            }
            Application.DoEvents();
            Hide();
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int DeleteObject(IntPtr hobject);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int DeleteDC(IntPtr hdc);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int UpdateLayeredWindow(
            IntPtr hwnd,
            IntPtr hdcDst,
            [System.Runtime.InteropServices.In()]
            ref Point pptDst,
            [System.Runtime.InteropServices.In()]
            ref Size psize,
            IntPtr hdcSrc,
            [System.Runtime.InteropServices.In()]
            ref Point pptSrc,
            int crKey,
            [System.Runtime.InteropServices.In()]
            ref BLENDFUNCTION pblend,
            int dwFlags);

        public const byte AC_SRC_OVER = 0;
        public const byte AC_SRC_ALPHA = 1;
        public const int ULW_ALPHA = 2;

        public const int WM_DISPLAYCHANGE = 0x007E;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BLENDFUNCTION
        {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;
        }
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;

                cp.ExStyle = cp.ExStyle | WindowsConst.WS_EX_LAYERED;
                if ((cp.ExStyle & WindowsConst.WS_EX_TRANSPARENT) != 0)
                {
                    cp.ExStyle = cp.ExStyle & WindowsConst.WS_EX_TRANSPARENT;
                }
                return cp;
            }
        }



        const int WindowOffsetW = 16;
        const int WindowOffsetH = 39;
        public void UpdateFormDisplay(Image backgroundImage)
        {
            IntPtr screenDc = GetDC(IntPtr.Zero);
            IntPtr memDc = CreateCompatibleDC(screenDc);
            IntPtr hBitmap = IntPtr.Zero;
            IntPtr oldBitmap = IntPtr.Zero;

            try
            {
                //Display-image
                Bitmap bmp = new Bitmap(backgroundImage);
                hBitmap = bmp.GetHbitmap(Color.FromArgb(0));
                oldBitmap = SelectObject(memDc, hBitmap);

                //Display-rectangle
                Size size = bmp.Size;
                Point pointSource = new Point(0, 0);
                Point topPos = new Point(this.Left, this.Top);

                //Set up blending options
                BLENDFUNCTION blend = new BLENDFUNCTION();
                blend.BlendOp = AC_SRC_OVER;
                blend.BlendFlags = 0;
                blend.SourceConstantAlpha = 255;
                blend.AlphaFormat = AC_SRC_ALPHA;

                UpdateLayeredWindow(this.Handle, screenDc,
                  ref topPos, ref size, memDc, ref pointSource, 0, ref blend, ULW_ALPHA);

                //Clean-up
                bmp.Dispose();
                ReleaseDC(IntPtr.Zero, screenDc);
                if (hBitmap != IntPtr.Zero)
                {
                    SelectObject(memDc, oldBitmap);
                    DeleteObject(hBitmap);
                }
                DeleteDC(memDc);
            }
            catch (Exception)
            {
            }
        }
        int offsetTopX;
        int offsetTopY;
        double offsetScaling;
        int ImageSizeW;
        int ImageSizeH;
        int EndLine = -1;
        Arial.Position pos;
        List<Movement> moves;
        PointF _rawp=new(0,0);
        PointF RawLocation
        {
            get { return _rawp; }
            set
            {
                _rawp = value;
                double x = _rawp.X * offsetScaling + offsetTopX;
                double y = _rawp.Y * offsetScaling + offsetTopY;
                x += Arial.GetXPos(pos, Width);
                y += Arial.GetYPos(pos, Height);
                if (double.IsNaN(x)) throw new ArgumentOutOfRangeException("x","xの値が不明です。");
                if (double.IsNaN(y)) throw new ArgumentOutOfRangeException("y","yの値が不明です。");
                Location = new((int)x, (int)y);

            }
        }
        Size _raws = new(0, 0);
        Size RawSize
        {
            get { return _raws; }
            set
            {
                _raws = value;
                double w = _raws.Width * offsetScaling;
                double h = _raws.Height * offsetScaling;
                if (w == double.NaN) throw new ArgumentOutOfRangeException("wの値が不明です。");
                if (h == double.NaN) throw new ArgumentOutOfRangeException("hの値が不明です。");
                Size = new((int)w, (int)h);
            }
        }
        bool IsShowed = false;
        public int UpdateForm(int time)
        {
            if (IsDisposed) return -1;
            if (moves[0].TimeStart > time) return 1;
            for (int i=-1;i<moves.Count;i++)
            {
                if (i == -1) i = EndLine+1;

                if (EndLine + 1 == moves.Count)
                {
                    Close();
                    return -1;
                }

                var move = moves[i];
                if (!move.IsEnded)
                {

                    if (move.TimeStart > time) break;
                    if (!IsShowed)
                    {
                        IsShowed= true;
                                Show();
                                break;
                    }
                    if (time> move.TimeStart && time > move.TimeEnd && move.TimeEnd != -2147483648)
                    {
                        switch (move.Type.Trim())
                        {
                            case "M":
                                RawLocation = new((float)move.MoveFrom, (float)move.MoveEnd);
                                break;
                            case "MX":
                                RawLocation = new((float)move.MoveEnd, double.IsNaN(move.SubValue) ? RawLocation.Y : (float)move.SubValue);
                                break;
                            case "MY":
                                RawLocation = new(double.IsNaN(move.SubValue) ? RawLocation.X : (float)move.SubValue, (float)move.MoveEnd);
                                break;
                            case "F":
                                Opacity = move.MoveEnd;
                                if (Opacity > 0) this.Show();
                                if (Opacity <= 0)
                                {
                                    for(int j=i+1;j<moves.Count;j++)
                                    {
                                        if (moves[j].Type.Trim() == "MX" || moves[j].Type.Trim() == "MY" || moves[j].Type.Trim() == "M")
                                        {
                                            switch (moves[j].Type.Trim())
                                            {
                                                case "MX":
                                                    System.Diagnostics.Trace.WriteLine($"次の座標MX {moves[j].MoveFrom} {(double.IsNaN(moves[j].SubValue) ? RawLocation.Y : (float)moves[j].SubValue)}");
                                                    RawLocation = new((float)moves[j].MoveFrom, double.IsNaN(moves[j].SubValue) ? RawLocation.Y : (float)moves[j].SubValue);
                                                    break;
                                                case "MY":
                                                    System.Diagnostics.Trace.WriteLine($"次の座標MY {(double.IsNaN(moves[j].SubValue) ? RawLocation.X : (float)moves[j].SubValue)} {moves[j].MoveFrom}");
                                                    RawLocation = new(double.IsNaN(moves[j].SubValue) ? RawLocation.X : (float)moves[j].SubValue, (float)moves[j].MoveFrom);
                                                    break;
                                                case "M":
                                                    System.Diagnostics.Trace.WriteLine($"次の座標M  {moves[j].MoveFrom} {moves[j].MoveEnd}");
                                                    RawLocation = new((float)moves[j].MoveFrom, (float)moves[j].MoveEnd);
                                                    break;
                                            }
                                            Application.DoEvents();
                                            break;
                                        }
                                    }
                                    this.Hide();
                                }
                                break;
                            case "S":
                                RawSize = new((int)(ImageSizeW * move.MoveEnd) + WindowOffsetW, (int)(ImageSizeH * move.MoveEnd) + WindowOffsetH);
                                break;
                        }
                        move.IsEnded = true;
                        continue;
                    }
                    else if (time >= move.TimeStart && move.TimeEnd == -2147483648)
                    {
                        switch (move.Type.Trim())
                        {
                            case "M":
                                RawLocation = new((float)move.MoveFrom, (float)move.MoveEnd);
                                break;
                            case "MX":
                                RawLocation = new((float)move.MoveFrom, double.IsNaN(move.SubValue) ? RawLocation.Y : (float)move.SubValue);
                                break;
                            case "MY":
                                RawLocation = new(double.IsNaN(move.SubValue) ? RawLocation.X : (float)move.SubValue, (float)move.MoveFrom);
                                break;
                            case "F":
                                Opacity = move.MoveFrom;
                                if (Opacity > 0) this.Show();
                                if (Opacity <= 0)
                                {
                                    for (int j = i + 1; j < moves.Count; j++)
                                    {
                                        if (moves[j].Type.Trim() == "MX" || moves[j].Type.Trim() == "MY" || moves[j].Type.Trim() == "M")
                                        {
                                            switch (moves[j].Type.Trim())
                                            {
                                                case "MX":
                                                    System.Diagnostics.Trace.WriteLine($"次の座標MX {moves[j].MoveFrom} {(double.IsNaN(moves[j].SubValue) ? RawLocation.Y : (float)moves[j].SubValue)}");
                                                    RawLocation = new((float)moves[j].MoveFrom, double.IsNaN(moves[j].SubValue) ? RawLocation.Y : (float)moves[j].SubValue);
                                                    break;
                                                case "MY":
                                                    System.Diagnostics.Trace.WriteLine($"次の座標MY {(double.IsNaN(moves[j].SubValue) ? RawLocation.X : (float)moves[j].SubValue)} {moves[j].MoveFrom}");
                                                    RawLocation = new(double.IsNaN(moves[j].SubValue) ? RawLocation.X : (float)moves[j].SubValue, (float)moves[j].MoveFrom);
                                                    break;
                                                case "M":
                                                    System.Diagnostics.Trace.WriteLine($"次の座標M  {moves[j].MoveFrom} {moves[j].MoveEnd}");
                                                    RawLocation = new((float)moves[j].MoveFrom, (float)moves[j].MoveEnd);
                                                    break;
                                            }
                                            Application.DoEvents();
                                            break;
                                        }
                                    }
                                    this.Hide();
                                }
                                break;
                            case "S":
                                RawSize = new((int)(ImageSizeW * move.MoveFrom) + WindowOffsetW, (int)(ImageSizeH * move.MoveFrom) + WindowOffsetH);
                                break;
                        }
                        move.IsEnded = true;
                        continue;
                    }
                    if (i == 0 && move.Type != "F")
                    {
                        //Opacity = 1;
                        //Show();
                    }
                    double per = (double)(time - move.TimeStart) / (move.TimeEnd - move.TimeStart);
                    if (per >= 0 && per <= 1)
                    {
                        per = Easing.GetEasing(move.Easing, per);
                        switch (move.Type.Trim())
                        {
                            case "M":
                                if (double.IsNaN(move.SubValue) || double.IsNaN(move.SubValue2))
                                {
                                    RawLocation = new((float)move.MoveFrom, (float)move.MoveEnd);
                                }
                                else
                                {
                                    RawLocation = new((float)(move.MoveFrom + (move.SubValue - move.MoveFrom) * per), (float)(move.MoveEnd + (move.SubValue2 - move.MoveEnd) * per));
                                }
                                break;
                            case "MX":
                                RawLocation = new((float)(move.MoveFrom + (move.MoveEnd - move.MoveFrom) * per), double.IsNaN(move.SubValue) ? RawLocation.Y : (float)move.SubValue);
                                break;
                            case "MY":
                                RawLocation = new(double.IsNaN(move.SubValue) ? RawLocation.X : (float)move.SubValue, (float)(move.MoveFrom + (move.MoveEnd - move.MoveFrom) * per));
                                break;
                            case "F":
                                Opacity = move.MoveFrom + (move.MoveEnd - move.MoveFrom) * per;
                                if (Opacity > 0) this.Show();
                                if (Opacity <= 0)
                                {
                                    for (int j = i + 1; j < moves.Count; j++)
                                    {
                                        if (moves[j].Type.Trim() == "MX" || moves[j].Type.Trim() == "MY" || moves[j].Type.Trim() == "M")
                                        {
                                            switch (moves[j].Type.Trim())
                                            {
                                                case "MX":
                                                    System.Diagnostics.Trace.WriteLine($"次の座標MX {moves[j].MoveFrom} {(double.IsNaN(moves[j].SubValue) ? RawLocation.Y : (float)moves[j].SubValue)}");
                                                    RawLocation = new((float)moves[j].MoveFrom, double.IsNaN(moves[j].SubValue) ? RawLocation.Y : (float)moves[j].SubValue);
                                                    break;
                                                case "MY":
                                                    System.Diagnostics.Trace.WriteLine($"次の座標MY {(double.IsNaN(moves[j].SubValue) ? RawLocation.X : (float)moves[j].SubValue)} {moves[j].MoveFrom}");
                                                    RawLocation = new(double.IsNaN(moves[j].SubValue) ? RawLocation.X : (float)moves[j].SubValue, (float)moves[j].MoveFrom);
                                                    break;
                                                case "M":
                                                    System.Diagnostics.Trace.WriteLine($"次の座標M  {moves[j].MoveFrom} {moves[j].MoveEnd}");
                                                    RawLocation = new((float)moves[j].MoveFrom, (float)moves[j].MoveEnd);
                                                    break;
                                            }
                                            Application.DoEvents();
                                            break;
                                        }
                                    }
                                    this.Hide();
                                }
                                break;
                            case "S":
                                RawSize = new((int)(ImageSizeW * (move.MoveFrom + (move.MoveEnd - move.MoveFrom) * per)) + WindowOffsetW, (int)(ImageSizeH * (move.MoveFrom + (move.MoveEnd - move.MoveFrom) * per)) + WindowOffsetH);
                                break;
                        }
                    }else if (per > 1)
                    {
                        move.IsEnded = true;
                    }
                }
                if (i == EndLine + 1 && move.IsEnded)
                {
                    EndLine++;
                }
            }
            return 0;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Forms_Load(object sender, EventArgs e)
        {

        }
    }

    public class Movement
    {
        public string Type="";
        public int Easing = 0;
        public int TimeStart = 0;
        public int TimeEnd = -2147483648;
        public double MoveFrom = double.NaN;
        public double MoveEnd = double.NaN;
        public double SubValue = double.NaN;
        public double SubValue2 = double.NaN;
        public bool IsEnded = false;
        public bool IsStarted = false;

    }

    public static class WindowsConst
    {
        public const int WM_NCHITTEST = 0x0084;

        /*
         * Window Styles
         */
        public const int WS_OVERLAPPED = 0x00000000;
        public const long WS_POPUP = 0x80000000L; //unchecked((int) WS_POPUP) 
        public const int WS_CHILD = 0x40000000;
        public const int WS_MINIMIZE = 0x20000000;
        public const int WS_VISIBLE = 0x10000000;
        public const int WS_DISABLED = 0x08000000;
        public const int WS_CLIPSIBLINGS = 0x04000000;
        public const int WS_CLIPCHILDREN = 0x02000000;
        public const int WS_MAXIMIZE = 0x01000000;
        public const int WS_CAPTION = 0x00C00000;/* WS_BORDER | WS_DLGFRAME  */
        public const int WS_BORDER = 0x00800000;
        public const int WS_DLGFRAME = 0x00400000;
        public const int WS_VSCROLL = 0x00200000;
        public const int WS_HSCROLL = 0x00100000;
        public const int WS_SYSMENU = 0x00080000;
        public const int WS_THICKFRAME = 0x00040000;
        public const int WS_GROUP = 0x00020000;
        public const int WS_TABSTOP = 0x00010000;

        public const int WS_MINIMIZEBOX = 0x00020000;
        public const int WS_MAXIMIZEBOX = 0x00010000;

        public const int WS_TILED = WS_OVERLAPPED;
        public const int WS_ICONIC = WS_MINIMIZE;
        public const int WS_SIZEBOX = WS_THICKFRAME;
        public const int WS_TILEDWINDOW = WS_OVERLAPPEDWINDOW;

        /*
         * Common Window Styles
        */
        public const int WS_OVERLAPPEDWINDOW = (WS_OVERLAPPED |
                                         WS_CAPTION |
                                         WS_SYSMENU |
                                         WS_THICKFRAME |
                                         WS_MINIMIZEBOX |
                                         WS_MAXIMIZEBOX);

        public const long WS_POPUPWINDOW = (WS_POPUP |
                                         WS_BORDER |
                                         WS_SYSMENU);

        public const int WS_CHILDWINDOW = (WS_CHILD);


        /*
         * Extended Window Styles
         */
        public const int WS_EX_DLGMODALFRAME = 0x00000001;
        public const int WS_EX_NOPARENTNOTIFY = 0x00000004;
        public const int WS_EX_TOPMOST = 0x00000008;
        public const int WS_EX_ACCEPTFILES = 0x00000010;
        public const int WS_EX_TRANSPARENT = 0x00000020;
        public const int WS_EX_MDICHILD = 0x00000040;
        public const int WS_EX_TOOLWINDOW = 0x00000080;
        public const int WS_EX_WINDOWEDGE = 0x00000100;
        public const int WS_EX_CLIENTEDGE = 0x00000200;
        public const int WS_EX_CONTEXTHELP = 0x00000400;

        public const int WS_EX_RIGHT = 0x00001000;
        public const int WS_EX_LEFT = 0x00000000;
        public const int WS_EX_RTLREADING = 0x00002000;
        public const int WS_EX_LTRREADING = 0x00000000;
        public const int WS_EX_LEFTSCROLLBAR = 0x00004000;
        public const int WS_EX_RIGHTSCROLLBAR = 0x00000000;

        public const int WS_EX_CONTROLPARENT = 0x00010000;
        public const int WS_EX_STATICEDGE = 0x00020000;
        public const int WS_EX_APPWINDOW = 0x00040000;


        public const int WS_EX_OVERLAPPEDWINDOW = (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE);
        public const int WS_EX_PALETTEWINDOW
          = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST);
        public const int WS_EX_LAYERED = 0x00080000;

        public const int WS_EX_NOINHERITLAYOUT = 0x00100000;
        // Disable inheritence of mirroring by children
        public const int WS_EX_LAYOUTRTL = 0x00400000; // Right to left mirroring

        public const int WS_EX_COMPOSITED = 0x02000000;
        public const int WS_EX_NOACTIVATE = 0x08000000;

        /*
         * WM_NCHITTEST and MOUSEHOOKSTRUCT Mouse Position Codes
         */
        public const int HTERROR = (-2);
        public const int HTTRANSPARENT = (-1);
        public const int HTNOWHERE = 0;
        public const int HTCLIENT = 1;
        public const int HTCAPTION = 2;
        public const int HTSYSMENU = 3;
        public const int HTGROWBOX = 4;
        public const int HTSIZE = HTGROWBOX;
        public const int HTMENU = 5;
        public const int HTHSCROLL = 6;
        public const int HTVSCROLL = 7;
        public const int HTMINBUTTON = 8;
        public const int HTMAXBUTTON = 9;
        public const int HTLEFT = 10;
        public const int HTRIGHT = 11;
        public const int HTTOP = 12;
        public const int HTTOPLEFT = 13;
        public const int HTTOPRIGHT = 14;
        public const int HTBOTTOM = 15;
        public const int HTBOTTOMLEFT = 16;
        public const int HTBOTTOMRIGHT = 17;
        public const int HTBORDER = 18;
        public const int HTREDUCE = HTMINBUTTON;
        public const int HTZOOM = HTMAXBUTTON;
        public const int HTSIZEFIRST = HTLEFT;
        public const int HTSIZELAST = HTBOTTOMRIGHT;

        public const int HTOBJECT = 19;
        public const int HTCLOSE = 20;
        public const int HTHELP = 21;

    }
}
