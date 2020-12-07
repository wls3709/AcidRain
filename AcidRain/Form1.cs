using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace AcidRain
{
    public partial class Form1 : Form
    {
        int Level;
        int Time;
        int Score;
        int floor_Height;
        int drop_Height;
        int drop_Cycle;
        int word_Damage;
        int word_Score;
        bool bRunning;
        Rectangle floor_Rect;
        Rectangle wnd_Rect;
        string[] WordData;
        List<string> Words;
        List<string> falling_Words;
        List<Point> falling_Word_Pos;
        FormWindowState prevState;
        Thread GameThread;
        System.Object ThreadLock = new System.Object();
        System.Threading.Timer TimeCounter;
        Image Ocean, Ocean_red, Ocean_red2, LightHouse, LightHouse_red, LightHouse_red2, City, City_red, City_red2;
        RankUploader RankFrm;
        RankViewer RankView;
        enum DIFFICULTY
        {
            EASY = 1,
            NORMAL,
            HARD
        }
        enum DROP_CYCLE
        {
            EASY = 1200,
            NORMAL = 800,
            HARD = 400
        }
        enum WORD_DAMAGE
        {
            EASY = 10,
            NORMAL = 25,
            HARD = 50
        }
        enum WORD_SCORE
        {
            EASY = 10,
            NORMAL = 50,
            HARD = 100
        }
        public Form1()
        {
            InitializeComponent();
            prevState = this.WindowState;
            NormalLevel.Checked = true;
            drop_Height = 25;
            floor_Height = 100;
            wnd_Rect = this.ClientRectangle;
            floor_Rect = new Rectangle(new Point(0, wnd_Rect.Height - floor_Height),
                new Size(wnd_Rect.Width, floor_Height));
            falling_Words = new List<string>();
            falling_Word_Pos = new List<Point>();
            Ocean = Properties.Resources.ocean_5;
            Ocean_red = Properties.Resources.ocean_5_red;
            Ocean_red2 = Properties.Resources.ocean_5_red2;
            LightHouse = Properties.Resources.lighthouse_2;
            LightHouse_red = Properties.Resources.lighthouse_2_red;
            LightHouse_red2 = Properties.Resources.lighthouse_2_red2;
            City = Properties.Resources.city_2;
            City_red = Properties.Resources.city_2_red;
            City_red2 = Properties.Resources.city_2_red2;
            ResetGame();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            DrawScreen();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (GameThread != null && GameThread.IsAlive)
            {
                GameThread.Abort();
            }
            if (TimeCounter != null)
            {
                TimeCounter.Dispose();
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal && prevState == FormWindowState.Minimized)
            {
                DrawScreen();
            }
            prevState = this.WindowState;
        }

        private void connDB()
        {
            string connStr = File.ReadAllText(@"..\..\..\dbconnect.txt");
            using (MySqlConnection connection = new MySqlConnection(connStr))
            {
                try//예외 처리
                {
                    connection.Open();
                    string sql = "SELECT `word` FROM word_list";

                    MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(sql, connection);
                    DataTable dt = new DataTable();
                    mySqlDataAdapter.Fill(dt);

                    List<string> Database = new List<string>();
                    foreach (DataRow row in dt.Rows)
                    {
                        Database.Add(row["word"].ToString());
                    }
                    Words = new List<string>(Database.OrderBy(i => Guid.NewGuid()));
                    Database.Clear();

                }
                catch (Exception ex)
                {
                    if (Words != null)
                    {
                        return;
                    }
                    WordData = File.ReadAllLines(@"..\..\..\한글.txt");
                    Words = new List<string>(WordData.OrderBy(i => Guid.NewGuid()).ToList());
                    MessageBox.Show("DB 쿼리 실패로 텍스트 파일 로드.");
                    // MessageBox.Show(ex.ToString());
                }
            }
        }

        private void ResetGame()
        {
            connDB();
            // WordData = File.ReadAllLines(@"..\..\..\한글.txt");
            // Words = new List<string>(WordData.OrderBy(i => Guid.NewGuid()).ToList());
            HealthBar.Value = HealthBar.Maximum;
            HealthBar.Update();
            Time = 120;
            Score = 0;
            bRunning = false;
            foreach (ToolStripItem item in LevelMenu.DropDownItems)
            {
                if (item is ToolStripMenuItem)
                {
                    ToolStripMenuItem Menu_Item = item as ToolStripMenuItem;
                    if (Menu_Item.Checked)
                    {
                        switch (Menu_Item.Text)
                        {
                            case "쉬움":
                                Level = (int)DIFFICULTY.EASY;
                                drop_Cycle = (int)DROP_CYCLE.EASY;
                                word_Damage = (int)WORD_DAMAGE.EASY;
                                word_Score = (int)WORD_SCORE.EASY;
                                break;
                            case "보통":
                                Level = (int)DIFFICULTY.NORMAL;
                                drop_Cycle = (int)DROP_CYCLE.NORMAL;
                                word_Damage = (int)WORD_DAMAGE.NORMAL;
                                word_Score = (int)WORD_SCORE.NORMAL;
                                break;
                            case "어려움":
                                Level = (int)DIFFICULTY.HARD;
                                drop_Cycle = (int)DROP_CYCLE.HARD;
                                word_Damage = (int)WORD_DAMAGE.HARD;
                                word_Score = (int)WORD_SCORE.HARD;
                                break;
                        }
                        break;
                    }
                }
            }
            DrawScreen();
        }

        private void LevelMenu_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            foreach (ToolStripItem item in LevelMenu.DropDownItems)
            {
                if (item is ToolStripMenuItem)
                {
                    ToolStripMenuItem Menu_Item = item as ToolStripMenuItem;
                    Menu_Item.Checked = (Menu_Item == e.ClickedItem);
                    ResetGame();
                    DrawScreen();
                }
            }
        }

        private void DrawScreen()
        {
            lock (ThreadLock)
            {
                Graphics g = this.CreateGraphics();
                using (BufferedGraphics bg = BufferedGraphicsManager.Current.Allocate(g, wnd_Rect))
                {
                    Image Bkgnd = null;
                    switch (Level)
                    {
                        case (int)DIFFICULTY.EASY:
                            if(HealthBar.Value > 70)
                            {
                                Bkgnd = Ocean;
                            }
                            else if(HealthBar.Value > 30)
                            {
                                Bkgnd = Ocean_red;
                            }
                            else
                            {
                                Bkgnd = Ocean_red2;
                            }
                            break;
                        case (int)DIFFICULTY.NORMAL:
                            if (HealthBar.Value > 70)
                            {
                                Bkgnd = LightHouse;
                            }
                            else if (HealthBar.Value > 30)
                            {
                                Bkgnd = LightHouse_red;
                            }
                            else
                            {
                                Bkgnd = LightHouse_red2;
                            }
                            break;
                        case (int)DIFFICULTY.HARD:
                            if (HealthBar.Value > 70)
                            {
                                Bkgnd = City;
                            }
                            else if (HealthBar.Value > 30)
                            {
                                Bkgnd = City_red;
                            }
                            else
                            {
                                Bkgnd = City_red2;
                            }
                            break;
                    }
                    bg.Graphics.DrawImage(Bkgnd, 0, 0);
                    bg.Graphics.FillRectangle(Brushes.Black, floor_Rect);
                    string strScore = string.Format("점수 : {0}", Score);
                    string strTime = string.Format("남은 시간 : {0} : {1:D2}", Time / 60, Time % 60);
                    string strDifficulty = null;
                    switch (Level)
                    {
                        case (int)DIFFICULTY.EASY:
                            strDifficulty = string.Format("난이도 : EASY");
                            break;
                        case (int)DIFFICULTY.NORMAL:
                            strDifficulty = string.Format("난이도 : NORMAL");
                            break;
                        case (int)DIFFICULTY.HARD:
                            strDifficulty = string.Format("난이도 : HARD");
                            break;
                    }
                    Font font = new Font("맑은 고딕", 12);
                    Brush brush = Brushes.LightGreen;
                    StringFormat strFmt = new StringFormat();
                    strFmt.Alignment = StringAlignment.Far;
                    bg.Graphics.DrawString(strScore, font, brush,
                        new RectangleF(wnd_Rect.Width - 120, 25, 120, 20), strFmt);
                    bg.Graphics.DrawString(strTime, font, brush, new Point(0, 25));
                    bg.Graphics.DrawString(strDifficulty, font, brush, new Point(0, 45));
                    for (int i = 0; i < falling_Words.Count; ++i)
                    {
                        bg.Graphics.DrawString(falling_Words[i], font, brush, falling_Word_Pos[i]);
                    }
                    /*
                    string str = "문자열 크기 측정 테스트";
                    using (StringFormat format = new StringFormat())
                    {
                        Rectangle rect = new Rectangle(200, 200, 200, 20);
                        format.Alignment = StringAlignment.Center;
                        format.LineAlignment = StringAlignment.Center;
                        bg.Graphics.DrawString(str, font, Brushes.Black, rect, format);
                        List<CharacterRange> charRangeList = new List<CharacterRange>();
                        for (int i = 0; i < str.Length; ++i)
                        {
                            charRangeList.Add(new CharacterRange(i, 1));
                        }
                        format.SetMeasurableCharacterRanges(charRangeList.ToArray());
                        Region[] regionArr = bg.Graphics.MeasureCharacterRanges(str, font, rect, format);
                        for (int i = 0; i < str.Length; ++i)
                        {
                            Rectangle textRect = Rectangle.Round(regionArr[i].GetBounds(bg.Graphics));
                            bg.Graphics.DrawRectangle(Pens.Red, textRect);
                        }
                    }
                    */
                    bg.Render(g);
                    bg.Dispose();
                }
                g.Dispose();
            }
        }

        private void StartGame_Click(object sender, EventArgs e)
        {
            ResetGame();
            TimeCounter = new System.Threading.Timer(Callback, null, 0, 1000);
            GameThread = new Thread(RunGame);
            GameThread.Start();
            bRunning = true;
            StartGame.Enabled = false;
            ResignGame.Enabled = true;
            LevelMenu.Enabled = false;
        }

        private void ResignGame_Click(object sender, EventArgs e)
        {
            QuitGame();
        }

        private void RunGame()
        {
            StringBuilder temp_Word = new StringBuilder();
            Random rand = new Random();
            Rectangle rect = new Rectangle(0, 50, 800, 20);
            Rectangle textRect;
            Font font = new Font("맑은 고딕", 12);
            Graphics g = this.CreateGraphics();
            List<CharacterRange> charRangeList = new List<CharacterRange>();
            List<Region> regionList = new List<Region>();
            // Region[] regionArr;
            int x, y;
            int text_Width;
            int text_Height;
            while (true)
            {
                temp_Word.Clear();
                charRangeList.Clear();
                regionList.Clear();
                text_Width = 0;
                text_Height = 0;
                if (Time <= 0)
                {
                    GameOver();
                    break;
                }
                if (Words.Count <= 0)
                {
                    Words = new List<string>(WordData.OrderBy(i => Guid.NewGuid()).ToList());
                }
                temp_Word.Append(Words[Words.Count - 1]);
                Words.RemoveAt(Words.Count - 1);
                using (StringFormat strFormat = new StringFormat())
                {
                    strFormat.Alignment = StringAlignment.Center;
                    strFormat.LineAlignment = StringAlignment.Center;
                    for (int i = 0; i < temp_Word.Length; ++i)
                    {
                        charRangeList.Add(new CharacterRange(i, 1));
                    }
                    strFormat.SetMeasurableCharacterRanges(charRangeList.ToArray());
                    // regionArr = g.MeasureCharacterRanges(temp_Word.ToString(), font, rect, strFormat);
                    regionList.AddRange(g.MeasureCharacterRanges(temp_Word.ToString(), font, rect, strFormat));
                    for (int i = 0; i < temp_Word.Length; ++i)
                    {
                        // textRect = Rectangle.Round(regionArr[i].GetBounds(g));
                        textRect = Rectangle.Round(regionList[i].GetBounds(g));
                        text_Width += textRect.Width;
                        text_Height = textRect.Height;
                    }
                }
                x = rand.Next(wnd_Rect.Width - (text_Width + 10)) + 5;
                y = 50;
                falling_Words.Add(temp_Word.ToString());
                falling_Word_Pos.Add(new Point(x, y));
                for (int i = falling_Words.Count - 1; i >= 0; --i)
                {
                    falling_Word_Pos[i] = new Point(falling_Word_Pos[i].X, falling_Word_Pos[i].Y + drop_Height);
                    if (falling_Word_Pos[i].Y + text_Height >= wnd_Rect.Height - floor_Height)
                    {
                        falling_Words.RemoveAt(i);
                        falling_Word_Pos.RemoveAt(i);
                        if (HealthBar.InvokeRequired)
                        {
                            HealthBar.Invoke(new MethodInvoker(delegate ()
                            {
                                HealthBar.Value -= word_Damage;
                            }));
                        }
                        else
                        {
                            HealthBar.Value -= word_Damage;
                        }
                    }
                }
                if (HealthBar.Value <= HealthBar.Minimum)
                {
                    GameOver();
                    break;
                }
                DrawScreen();
                Thread.Sleep(drop_Cycle);
            }
        }

        private void Callback(object obj)
        {
            --Time;
            DrawScreen();
        }

        private void GameOver()
        {
            bRunning = false;
            TimeCounter.Dispose();
            Words.Clear();
            falling_Words.Clear();
            falling_Word_Pos.Clear();
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate ()
                {
                    StartGame.Enabled = true;
                    ResignGame.Enabled = false;
                    LevelMenu.Enabled = true;
                }));
            }
            else
            {
                StartGame.Enabled = true;
                ResignGame.Enabled = false;
                LevelMenu.Enabled = true;
            }
            DrawScreen();
            /*
            string Difficulty = null;
            switch (Level)
            {
                case (int)DIFFICULTY.EASY:
                    Difficulty = "EASY";
                    break;
                case (int)DIFFICULTY.NORMAL:
                    Difficulty = "NORMAL";
                    break;
                case (int)DIFFICULTY.HARD:
                    Difficulty = "HARD";
                    break;
            }
            MessageBox.Show("level : " + Difficulty + "\n" + "score : " + Score.ToString());
            */
            if (InvokeRequired)
            {
                this.Invoke(new Action(() => OpenRankUploader()));
            }
            if (GameThread.IsAlive)
            {
                GameThread.Abort();
            }
        }

        private void QuitGame()
        {
            bRunning = false;
            if (GameThread.IsAlive)
            {
                GameThread.Abort();
            }
            TimeCounter.Dispose();
            Words.Clear();
            falling_Words.Clear();
            falling_Word_Pos.Clear();
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate ()
                {
                    StartGame.Enabled = true;
                    ResignGame.Enabled = false;
                    LevelMenu.Enabled = true;
                }));
            }
            else
            {
                StartGame.Enabled = true;
                ResignGame.Enabled = false;
                LevelMenu.Enabled = true;
            }
            DrawScreen();
            /*
            string Difficulty = null;
            switch (Level)
            {
                case (int)DIFFICULTY.EASY:
                    Difficulty = "EASY";
                    break;
                case (int)DIFFICULTY.NORMAL:
                    Difficulty = "NORMAL";
                    break;
                case (int)DIFFICULTY.HARD:
                    Difficulty = "HARD";
                    break;
            }
            MessageBox.Show("level : " + Difficulty + "\n" + "score : " + Score.ToString());
            */
            OpenRankUploader();
        }

        private void Show_Ranking_Click(object sender, EventArgs e)
        {
            OpenRankList();
        }

        public void InsertRanking(string name)
        {
            string connStr = File.ReadAllText(@"..\..\..\dbconnect.txt");
            using (MySqlConnection connection = new MySqlConnection(connStr))
            {
                try//예외 처리
                {
                    connection.Open();
                    string ssql = "INSERT INTO `hangul`.`score` (`name`, `score`, `difficulty`) VALUES ('"+name+"', '"+Score.ToString()+"', '"+Level.ToString()+"')";
                    MySqlCommand mcmd = new MySqlCommand(ssql, connection);
                    mcmd.ExecuteNonQuery();
                    OpenRankList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("랭킹 DB 입력 실패");
                }
            }
        }

        private void OpenRankUploader()
        {
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.Name == "RankUploader")
                {
                    frm.Activate();
                    return;
                }
            }

            RankFrm = new RankUploader();
            RankFrm.Owner = this;
            RankFrm.parent = this;

            RankFrm.Show();
        }

        public void OpenRankList()
        {
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.Name == "RankViewer")
                {
                    frm.Activate();
                    return;
                }
            }

            RankView = new RankViewer();
            RankView.Owner = this;
            RankView.parent = this;

            RankView.Show();
        }

        private void WordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (bRunning)
                {
                    int idx = falling_Words.FindIndex(x => x.Equals(WordBox.Text));
                    if (idx != -1)
                    {
                        falling_Words.RemoveAt(idx);
                        falling_Word_Pos.RemoveAt(idx);
                        Score += word_Score;
                        DrawScreen();
                    }
                }
                WordBox.Text = "";
            }
        }
    }
}
