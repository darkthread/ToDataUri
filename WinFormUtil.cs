using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToDataUri
{
    public class WinFormUtil : IDisposable
    {
        Form form;
        Label lbl;
        int startX;
        public WinFormUtil()
        {
            form = new Form()
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.CenterScreen,
                TopMost = true,
                ControlBox = false,
                ShowInTaskbar = false,
                Opacity = 0.8,
                Width = 0,
                Height = 0
            };
            lbl = new Label()
            {
                Padding = new Padding(8),
                AutoSize = true,
                Font = new Font("微軟正黑體", 12, FontStyle.Bold),
                BackColor = Color.Coral
            };
            form.Controls.Add(lbl);
            form.Show();
            startX = form.Left;
            form.Hide();
        }
        public void ShowInfo(string message, int autoHideDelay = 2000)
        {
            form.Show();
            lbl.Text = message;
            form.Width = lbl.Width;
            form.Height = lbl.Height;
            form.Left = startX - form.Width / 2;
            form.Refresh();
            Thread.Sleep(autoHideDelay);
            form.Hide();
        }
        public void Alert(string message) { 
            MessageBox.Show(message, "Image To Data Uri"); 
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}