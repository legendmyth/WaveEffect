namespace WaveEffect
{
    partial class FrmMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.tbWavelength = new System.Windows.Forms.TrackBar();
            this.tbAmplitude = new System.Windows.Forms.TrackBar();
            this.lblWaveLength = new System.Windows.Forms.Label();
            this.lblAmplitude = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.tbWavelength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbAmplitude)).BeginInit();
            this.SuspendLayout();
            // 
            // tbWavelength
            // 
            this.tbWavelength.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbWavelength.Location = new System.Drawing.Point(137, 0);
            this.tbWavelength.Maximum = 100;
            this.tbWavelength.Minimum = 1;
            this.tbWavelength.Name = "tbWavelength";
            this.tbWavelength.Size = new System.Drawing.Size(862, 45);
            this.tbWavelength.TabIndex = 3;
            this.tbWavelength.Value = 1;
            this.tbWavelength.Scroll += new System.EventHandler(this.tbWavelength_Scroll);
            // 
            // tbAmplitude
            // 
            this.tbAmplitude.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbAmplitude.Location = new System.Drawing.Point(137, 51);
            this.tbAmplitude.Maximum = 100;
            this.tbAmplitude.Minimum = 1;
            this.tbAmplitude.Name = "tbAmplitude";
            this.tbAmplitude.Size = new System.Drawing.Size(862, 45);
            this.tbAmplitude.TabIndex = 5;
            this.tbAmplitude.Value = 2;
            this.tbAmplitude.Scroll += new System.EventHandler(this.tbAmplitude_Scroll);
            // 
            // lblWaveLength
            // 
            this.lblWaveLength.Location = new System.Drawing.Point(31, 0);
            this.lblWaveLength.Name = "lblWaveLength";
            this.lblWaveLength.Size = new System.Drawing.Size(100, 23);
            this.lblWaveLength.TabIndex = 6;
            this.lblWaveLength.Text = "波长";
            this.lblWaveLength.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblAmplitude
            // 
            this.lblAmplitude.Location = new System.Drawing.Point(31, 51);
            this.lblAmplitude.Name = "lblAmplitude";
            this.lblAmplitude.Size = new System.Drawing.Size(100, 23);
            this.lblAmplitude.TabIndex = 8;
            this.lblAmplitude.Text = "振幅";
            this.lblAmplitude.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(999, 450);
            this.Controls.Add(this.lblAmplitude);
            this.Controls.Add(this.lblWaveLength);
            this.Controls.Add(this.tbAmplitude);
            this.Controls.Add(this.tbWavelength);
            this.Name = "FrmMain";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.tbWavelength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbAmplitude)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TrackBar tbWavelength;
        private System.Windows.Forms.TrackBar tbAmplitude;
        private System.Windows.Forms.Label lblWaveLength;
        private System.Windows.Forms.Label lblAmplitude;
    }
}

