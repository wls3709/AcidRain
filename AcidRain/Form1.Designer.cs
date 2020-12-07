
namespace AcidRain
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.LevelMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.EasyLevel = new System.Windows.Forms.ToolStripMenuItem();
            this.NormalLevel = new System.Windows.Forms.ToolStripMenuItem();
            this.HardLevel = new System.Windows.Forms.ToolStripMenuItem();
            this.GameMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.StartGame = new System.Windows.Forms.ToolStripMenuItem();
            this.ResignGame = new System.Windows.Forms.ToolStripMenuItem();
            this.HealthBar = new System.Windows.Forms.ProgressBar();
            this.WordBox = new System.Windows.Forms.TextBox();
            this.랭킹ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Show_Ranking = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LevelMenu,
            this.GameMenu,
            this.랭킹ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 1, 0, 1);
            this.menuStrip1.Size = new System.Drawing.Size(931, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // LevelMenu
            // 
            this.LevelMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.EasyLevel,
            this.NormalLevel,
            this.HardLevel});
            this.LevelMenu.Name = "LevelMenu";
            this.LevelMenu.Size = new System.Drawing.Size(55, 22);
            this.LevelMenu.Text = "난이도";
            this.LevelMenu.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.LevelMenu_DropDownItemClicked);
            // 
            // EasyLevel
            // 
            this.EasyLevel.Name = "EasyLevel";
            this.EasyLevel.Size = new System.Drawing.Size(110, 22);
            this.EasyLevel.Text = "쉬움";
            // 
            // NormalLevel
            // 
            this.NormalLevel.Name = "NormalLevel";
            this.NormalLevel.Size = new System.Drawing.Size(110, 22);
            this.NormalLevel.Text = "보통";
            // 
            // HardLevel
            // 
            this.HardLevel.Name = "HardLevel";
            this.HardLevel.Size = new System.Drawing.Size(110, 22);
            this.HardLevel.Text = "어려움";
            // 
            // GameMenu
            // 
            this.GameMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StartGame,
            this.ResignGame});
            this.GameMenu.Name = "GameMenu";
            this.GameMenu.Size = new System.Drawing.Size(71, 22);
            this.GameMenu.Text = "게임 진행";
            // 
            // StartGame
            // 
            this.StartGame.Name = "StartGame";
            this.StartGame.Size = new System.Drawing.Size(126, 22);
            this.StartGame.Text = "게임 시작";
            this.StartGame.Click += new System.EventHandler(this.StartGame_Click);
            // 
            // ResignGame
            // 
            this.ResignGame.Enabled = false;
            this.ResignGame.Name = "ResignGame";
            this.ResignGame.Size = new System.Drawing.Size(126, 22);
            this.ResignGame.Text = "게임 포기";
            this.ResignGame.Click += new System.EventHandler(this.ResignGame_Click);
            // 
            // HealthBar
            // 
            this.HealthBar.Location = new System.Drawing.Point(251, 509);
            this.HealthBar.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.HealthBar.Name = "HealthBar";
            this.HealthBar.Size = new System.Drawing.Size(420, 21);
            this.HealthBar.TabIndex = 1;
            // 
            // WordBox
            // 
            this.WordBox.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.WordBox.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.WordBox.Location = new System.Drawing.Point(383, 468);
            this.WordBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.WordBox.Name = "WordBox";
            this.WordBox.Size = new System.Drawing.Size(159, 26);
            this.WordBox.TabIndex = 2;
            this.WordBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.WordBox_KeyDown);
            // 
            // 랭킹ToolStripMenuItem
            // 
            this.랭킹ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Show_Ranking});
            this.랭킹ToolStripMenuItem.Name = "랭킹ToolStripMenuItem";
            this.랭킹ToolStripMenuItem.Size = new System.Drawing.Size(43, 22);
            this.랭킹ToolStripMenuItem.Text = "랭킹";
            // 
            // Show_Ranking
            // 
            this.Show_Ranking.Name = "Show_Ranking";
            this.Show_Ranking.Size = new System.Drawing.Size(180, 22);
            this.Show_Ranking.Text = "랭킹 보기";
            this.Show_Ranking.Click += new System.EventHandler(this.Show_Ranking_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(931, 557);
            this.Controls.Add(this.WordBox);
            this.Controls.Add(this.HealthBar);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AcidRain";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem LevelMenu;
        private System.Windows.Forms.ToolStripMenuItem EasyLevel;
        private System.Windows.Forms.ToolStripMenuItem NormalLevel;
        private System.Windows.Forms.ToolStripMenuItem HardLevel;
        private System.Windows.Forms.ToolStripMenuItem GameMenu;
        private System.Windows.Forms.ToolStripMenuItem StartGame;
        private System.Windows.Forms.ToolStripMenuItem ResignGame;
        private System.Windows.Forms.ProgressBar HealthBar;
        private System.Windows.Forms.TextBox WordBox;
        private System.Windows.Forms.ToolStripMenuItem 랭킹ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Show_Ranking;
    }
}

