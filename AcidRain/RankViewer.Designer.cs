
namespace AcidRain
{
    partial class RankViewer
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
            this.RankList = new System.Windows.Forms.ListView();
            this.CloseForm = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // RankList
            // 
            this.RankList.HideSelection = false;
            this.RankList.Location = new System.Drawing.Point(13, 15);
            this.RankList.Name = "RankList";
            this.RankList.Size = new System.Drawing.Size(741, 285);
            this.RankList.TabIndex = 0;
            this.RankList.UseCompatibleStateImageBehavior = false;
            // 
            // CloseForm
            // 
            this.CloseForm.Location = new System.Drawing.Point(627, 316);
            this.CloseForm.Name = "CloseForm";
            this.CloseForm.Size = new System.Drawing.Size(127, 33);
            this.CloseForm.TabIndex = 1;
            this.CloseForm.Text = "닫기";
            this.CloseForm.UseVisualStyleBackColor = true;
            this.CloseForm.Click += new System.EventHandler(this.CloseForm_Click);
            // 
            // RankViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(769, 361);
            this.Controls.Add(this.CloseForm);
            this.Controls.Add(this.RankList);
            this.MaximizeBox = false;
            this.Name = "RankViewer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RankViewer";
            this.Load += new System.EventHandler(this.RankViewer_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView RankList;
        private System.Windows.Forms.Button CloseForm;
    }
}