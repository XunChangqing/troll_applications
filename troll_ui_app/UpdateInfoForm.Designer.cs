namespace troll_ui_app
{
    partial class UpdateInfoForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.updateInfoRichTextBox = new System.Windows.Forms.RichTextBox();
            this.updateButton = new System.Windows.Forms.Button();
            this.updateNextTimeButton = new System.Windows.Forms.Button();
            this.skipVersionButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // updateInfoRichTextBox
            // 
            this.updateInfoRichTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.updateInfoRichTextBox.Location = new System.Drawing.Point(13, 13);
            this.updateInfoRichTextBox.Name = "updateInfoRichTextBox";
            this.updateInfoRichTextBox.Size = new System.Drawing.Size(679, 357);
            this.updateInfoRichTextBox.TabIndex = 0;
            this.updateInfoRichTextBox.Text = "";
            // 
            // updateButton
            // 
            this.updateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.updateButton.Location = new System.Drawing.Point(13, 390);
            this.updateButton.Name = "updateButton";
            this.updateButton.Size = new System.Drawing.Size(75, 23);
            this.updateButton.TabIndex = 1;
            this.updateButton.Text = "更新";
            this.updateButton.UseVisualStyleBackColor = true;
            this.updateButton.Click += new System.EventHandler(this.updateButton_Click);
            // 
            // updateNextTimeButton
            // 
            this.updateNextTimeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.updateNextTimeButton.Location = new System.Drawing.Point(95, 390);
            this.updateNextTimeButton.Name = "updateNextTimeButton";
            this.updateNextTimeButton.Size = new System.Drawing.Size(75, 23);
            this.updateNextTimeButton.TabIndex = 2;
            this.updateNextTimeButton.Text = "下次提醒";
            this.updateNextTimeButton.UseVisualStyleBackColor = true;
            this.updateNextTimeButton.Click += new System.EventHandler(this.updateNextTimeButton_Click);
            // 
            // skipVersionButton
            // 
            this.skipVersionButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.skipVersionButton.Location = new System.Drawing.Point(177, 390);
            this.skipVersionButton.Name = "skipVersionButton";
            this.skipVersionButton.Size = new System.Drawing.Size(75, 23);
            this.skipVersionButton.TabIndex = 3;
            this.skipVersionButton.Text = "跳过该版本";
            this.skipVersionButton.UseVisualStyleBackColor = true;
            this.skipVersionButton.Click += new System.EventHandler(this.skipVersionButton_Click);
            // 
            // UpdateInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(704, 425);
            this.Controls.Add(this.skipVersionButton);
            this.Controls.Add(this.updateNextTimeButton);
            this.Controls.Add(this.updateButton);
            this.Controls.Add(this.updateInfoRichTextBox);
            this.Name = "UpdateInfoForm";
            this.Text = "更新版本信息";
            this.Load += new System.EventHandler(this.UpdateInfoForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox updateInfoRichTextBox;
        private System.Windows.Forms.Button updateButton;
        private System.Windows.Forms.Button updateNextTimeButton;
        private System.Windows.Forms.Button skipVersionButton;
    }
}