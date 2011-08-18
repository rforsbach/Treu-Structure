using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Canguro.View
{
    /// <summary>
    /// Panel para entrada rápida de datos
    /// (Para mejorarlo y pasarlo como controles dentro de DirectX,
    /// pasar el código a ScenePanel y manejar Sprites)
    /// </summary>
    public class SmallPanel : Panel
    {
        private Label title;
        private Label subtitle;
        private Label tracker;
        private TextBox data;
        private Label sample;
        private int manualDataLength;

        public SmallPanel()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            title = new Label();
            subtitle = new Label();
            tracker = new Label();
            data = new TextBox();
            sample = new Label();
            manualDataLength = 0;

            this.SuspendLayout();

            // title
            this.title.AutoSize = true;
            this.title.Font = new System.Drawing.Font("Comic Sans MS", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.title.Location = new System.Drawing.Point(4, 4);
            this.title.Name = "title";
            this.title.Size = new System.Drawing.Size(52, 21);
            this.title.TabIndex = 0;
            this.title.Text = "Title";
            
            // subtitle
            this.subtitle.AutoSize = true;
            this.subtitle.Location = new System.Drawing.Point(108, 11);
            this.subtitle.Name = "subtitle";
            this.subtitle.Size = new System.Drawing.Size(35, 13);
            this.subtitle.TabIndex = 1;
            this.subtitle.Text = "Subtitle";

            // data
            this.data.AutoSize = true;
            this.data.Location = new System.Drawing.Point(158, 7);
            this.data.Name = "data";
            this.data.Size = new System.Drawing.Size(35, 13);
            this.data.TabIndex = 1;
            this.data.Text = "TextBox";
            this.data.Visible = false;

            // sample
            this.sample.AutoSize = true;
            this.sample.Location = new System.Drawing.Point(208, 11);
            this.sample.Name = "sample";
            this.sample.Size = new System.Drawing.Size(35, 13);
            this.sample.TabIndex = 1;
            this.sample.Text = "(Sample)";

            // SmallPanel
            this.Controls.Add(this.title);
            this.Controls.Add(this.subtitle);
            this.Controls.Add(this.data);
            this.Controls.Add(this.sample);
            this.ResumeLayout();

            this.data.KeyDown += new KeyEventHandler(data_KeyDown);
            this.data.KeyPress += new KeyPressEventHandler(data_KeyPress);
        }

        void data_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
                e.Handled = true;
        }

        void data_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                EnterData(this, new EnterDataEventArgs(this.data.Text));
                // Flush data
                this.data.Text = "";
            }
        }

        public string TrackingText
        {
            get
            {
                return tracker.Text;
            }
            set
            {
                tracker.Text = value;
            }
        }

        public void Start(string title, string subtitle, int manualDataLength, string dataSample)
        {
            this.SuspendLayout();
            this.title.Text = title;
            this.title.ForeColor = Canguro.Properties.Settings.Default.SmallPanelForeColor;
            this.subtitle.Text = subtitle;
            this.subtitle.Left = this.title.Left + this.title.Width + 5;
            this.subtitle.ForeColor = Canguro.Properties.Settings.Default.SmallPanelForeColor; // System.Drawing.Color.FromArgb(64, 64, 64);
            this.manualDataLength = manualDataLength;

            if (manualDataLength > 0)
            {
                this.data.Left = this.subtitle.Left + this.subtitle.Width + 3;
                this.data.Text = "";
                this.data.Width = Math.Max(dataSample.Length, manualDataLength) * 8;
                this.data.Visible = true;
                this.data.SelectAll();
                this.data.Focus();

                this.sample.Text = "(" + Culture.Get("forExample") + dataSample + ")";
                this.sample.Left = this.data.Left + this.data.Width + 2;
                this.sample.ForeColor = Canguro.Properties.Settings.Default.SmallPanelForeColor; // System.Drawing.Color.FromArgb(64, 64, 64);
                this.sample.Visible = (dataSample.Length > 0);
            }
            else
            {
                this.data.Visible = false;
                this.sample.Visible = false;
            }
            
            this.BackColor = Canguro.Properties.Settings.Default.BackColor;
            
            this.Height = 32;
            this.ResumeLayout();
        }

        public void Stop()
        {
            this.data.Visible = false;
            this.Height = 0;
            this.Parent.Focus();
        }

        public event EnterDataEventHandler EnterData;
    }
}
