using NAudio.Wave;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Resources;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

namespace KyukurarinForm
{
    public partial class MainForm : Form
    {
        WaveOut wav;
        WaveStream wavstr;
        MemoryStream memstr;
        void LoadData()
        {
            string str = asset.String1;
            var lines=str.Split('\n');
            string tempstr = "";
            string format = "";
            for(int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (string.IsNullOrEmpty(line)) continue;
                if(!line.StartsWith(" "))
                {
                    if (tempstr != string.Empty&&format!=string.Empty)
                    {
                        var data = format.Split(',');
                        var img = asset.ResourceManager.GetObject(data[3].Replace("\"", "").Replace("\\","_").Replace(".png","").Replace(" ", "_"));
                        if (img == null) throw new ArgumentException($"{(data[3].Replace("\"", "").Replace("\\", "_").Replace(".png", "").Replace(" ","_"))} is Missing.");

                        Bitmap a = new((Bitmap)img);
                        bool t = false;
                        //if (data[3].Contains("sb\\f\\") || data[3].Contains("sb\\c\\") || data[3].Contains("sb\\d\\") || data[3].Contains("sb\\a\\")) t = true;
                        double.TryParse(data[4], out double x);
                        double.TryParse(data[5], out double y);
                        forms.Add(new(GetMovements(tempstr), a,x:(int)x,y:(int)y,topmost:t));
                        Text = $"{i}/{line.Length} オブジェクト数 : {forms.Count} ";
                        System.Diagnostics.Trace.WriteLine($"{i}/{line.Length} オブジェクト数 : {forms.Count} ");
                    }
                    format = line;
                    tempstr = "";
                }
                else
                {
                    tempstr += line + "\n";
                }
                
            }
            if (tempstr != string.Empty && format != string.Empty)
            {
                var data = format.Split(',');
                var img = asset.ResourceManager.GetObject(data[3].Replace("\"", "").Replace("\\", "_").Replace(".png", "").Replace(" ", "_"));
                if (img == null) throw new ArgumentException($"{(data[3].Replace("\"", "").Replace("\\", "_").Replace(".png", "").Replace(" ", "_"))} is Missing.");

                Bitmap a = new((Bitmap)img);
                bool t = false;
                if (data[3].Contains("sb\\f\\") || data[3].Contains("sb\\c\\") || data[3].Contains("sb\\d\\") || data[3].Contains("sb\\a\\")) t = true;
                double.TryParse(data[4], out double x);
                double.TryParse(data[5], out double y); 
                forms.Add(new(GetMovements(tempstr), a, x: (int)x, y: (int)y, topmost: t));

                Text = $"オブジェクト数 : {forms.Count}";

            }
            Text = $"Winくらりん";
        }
        public MainForm()
        {
            InitializeComponent(); 
            wav = new NAudio.Wave.WaveOut();  // 要Dispose
            memstr = new MemoryStream(asset.audio)
            {
                Position = 0  // 先頭位置からの再生を意味するが、streamを新規にnewするときは不要なはず
            };
            wavstr = new Mp3FileReader(memstr);
            wav.Init(wavstr);
            this.Show();
            TopMost = true;
            
        }
        List<Forms> forms= new();
        List<Movement> GetMovements(string str)
        {
            List<Movement> list = new List<Movement>();
            var lines=str.Split('\n');
            foreach (var line in lines)
            {
                var arg = line.Split(',');
                Movement m = new Movement();
                if (arg.Length == 0) continue;
                m.Type= arg[0];
                switch (m.Type.Trim())
                {
                    case "S":
                    case "F":
                        m.MoveFrom = 1;
                        m.MoveEnd = 1;
                        break;
                }
                if (arg.Length > 1) int.TryParse(arg[1], out m.Easing);
                if (arg.Length > 2) if(!int.TryParse(arg[2], out m.TimeStart)) m.TimeStart = 0;
                if (arg.Length > 3) if(!int.TryParse(arg[3], out m.TimeEnd)) m.TimeEnd = -2147483648;
                if (arg.Length > 4) if(!double.TryParse(arg[4], out m.MoveFrom))m.MoveFrom = 0;
                if (arg.Length > 5) if (!double.TryParse(arg[5], out m.MoveEnd))m.MoveEnd=double.NaN;
                if (arg.Length > 6) if(!double.TryParse(arg[6], out m.SubValue))m.SubValue=double.NaN;
                if (arg.Length > 7) double.TryParse(arg[7], out m.SubValue2);
                if (double.IsNaN(m.MoveFrom))
                {
                    switch (m.Type.Trim())
                    {
                        case "S":
                        case "F":
                            m.MoveFrom = 1; break;
                        case "MX":
                        case "MY":
                            m.MoveFrom = 0; break;
                    }
                }
                if (arg.Length == 5)
                {
                    switch (m.Type.Trim())
                    {
                        case "MX":
                        case "MY":
                        case "S":
                        case "F":
                            m.MoveEnd = m.MoveFrom;
                            break;
                    }
                }
                if(m.TimeStart>MaxTime)MaxTime= m.TimeStart;
                if(m.TimeEnd>MaxTime)MaxTime= m.TimeEnd;
                list.Add(m);
            }
            return list;
        }
        int MaxTime = 0;
        Stopwatch sw= new Stopwatch();
        private void timer1_Tick(object sender, EventArgs e)
        {
            int disposedcount = -1;
            for (int i = disposedcount+1; i < forms.Count; i++)
            {
                int a=forms[i].UpdateForm((int)sw.ElapsedMilliseconds);
                if (a == -1&&disposedcount+1==i) disposedcount = i;
            }
            if (sw.ElapsedMilliseconds > MaxTime + 1000) Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Start();
            sw.Start();
            wav.Play();
            Hide();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            button1.Enabled = false;
            LoadData();
            button1.Enabled = true;
        }
    }
}