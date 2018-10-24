namespace ConstraintTrlangles
{
    partial class Form1
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
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.rb_AutoTriangle = new System.Windows.Forms.RadioButton();
            this.rb_ManualTriangle = new System.Windows.Forms.RadioButton();
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.richTextBox = new System.Windows.Forms.RichTextBox();
            this.btn_StartTriangle = new System.Windows.Forms.Button();
            this.btn_Clear = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.groupBox.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox
            // 
            this.pictureBox.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox.Location = new System.Drawing.Point(0, 59);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(635, 512);
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            // 
            // rb_AutoTriangle
            // 
            this.rb_AutoTriangle.AutoSize = true;
            this.rb_AutoTriangle.Location = new System.Drawing.Point(21, 18);
            this.rb_AutoTriangle.Name = "rb_AutoTriangle";
            this.rb_AutoTriangle.Size = new System.Drawing.Size(83, 16);
            this.rb_AutoTriangle.TabIndex = 1;
            this.rb_AutoTriangle.TabStop = true;
            this.rb_AutoTriangle.Text = "自动三角网";
            this.rb_AutoTriangle.UseVisualStyleBackColor = true;
            // 
            // rb_ManualTriangle
            // 
            this.rb_ManualTriangle.AutoSize = true;
            this.rb_ManualTriangle.Location = new System.Drawing.Point(110, 18);
            this.rb_ManualTriangle.Name = "rb_ManualTriangle";
            this.rb_ManualTriangle.Size = new System.Drawing.Size(83, 16);
            this.rb_ManualTriangle.TabIndex = 2;
            this.rb_ManualTriangle.TabStop = true;
            this.rb_ManualTriangle.Text = "手动三角网";
            this.rb_ManualTriangle.UseVisualStyleBackColor = true;
            // 
            // groupBox
            // 
            this.groupBox.Controls.Add(this.rb_ManualTriangle);
            this.groupBox.Controls.Add(this.rb_AutoTriangle);
            this.groupBox.Location = new System.Drawing.Point(10, 8);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(221, 40);
            this.groupBox.TabIndex = 3;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "展示方式";
            // 
            // richTextBox
            // 
            this.richTextBox.Dock = System.Windows.Forms.DockStyle.Right;
            this.richTextBox.Location = new System.Drawing.Point(635, 59);
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.Size = new System.Drawing.Size(140, 512);
            this.richTextBox.TabIndex = 5;
            this.richTextBox.Text = "";
            // 
            // btn_StartTriangle
            // 
            this.btn_StartTriangle.Location = new System.Drawing.Point(579, 16);
            this.btn_StartTriangle.Name = "btn_StartTriangle";
            this.btn_StartTriangle.Size = new System.Drawing.Size(91, 32);
            this.btn_StartTriangle.TabIndex = 4;
            this.btn_StartTriangle.Text = "生成三角网";
            this.btn_StartTriangle.UseVisualStyleBackColor = true;
            // 
            // btn_Clear
            // 
            this.btn_Clear.Location = new System.Drawing.Point(676, 16);
            this.btn_Clear.Name = "btn_Clear";
            this.btn_Clear.Size = new System.Drawing.Size(87, 32);
            this.btn_Clear.TabIndex = 6;
            this.btn_Clear.Text = "清空";
            this.btn_Clear.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.groupBox);
            this.panel1.Controls.Add(this.pictureBox);
            this.panel1.Controls.Add(this.richTextBox);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(775, 571);
            this.panel1.TabIndex = 7;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btn_Clear);
            this.panel2.Controls.Add(this.btn_StartTriangle);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(775, 59);
            this.panel2.TabIndex = 3;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(775, 571);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.groupBox.ResumeLayout(false);
            this.groupBox.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.RadioButton rb_AutoTriangle;
        private System.Windows.Forms.RadioButton rb_ManualTriangle;
        private System.Windows.Forms.GroupBox groupBox;
        private System.Windows.Forms.RichTextBox richTextBox;
        private System.Windows.Forms.Button btn_StartTriangle;
        private System.Windows.Forms.Button btn_Clear;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
    }
}

