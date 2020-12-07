using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
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
        string[] WordData;
        List<string> Words;
        List<string> falling_Words;
        List<Point> falling_Word_Pos;
        System.Object ThreadLock = new System.Object();
        Thread GameThread, m_thr;
        System.Threading.Timer TimeCounter;
        Image Ocean, Ocean_red, Ocean_red2, LightHouse, LightHouse_red, LightHouse_red2, City, City_red, City_red2;
        RankingForm RankFrm;
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
            HARD = 200
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
            NormalLevel.Checked = true;
            drop_Height = 25;
            floor_Height = 100;
            floor_Rect = new Rectangle(new Point(0, ClientRectangle.Height - floor_Height),
                new Size(ClientRectangle.Width, floor_Height));
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

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            DrawScreen();
        }

        private void connDB()
        {
            string connStr = File.ReadAllText(@"..\..\..\dbconnect.txt");  //"server=localhost;user=root;database=world;port=3306;password=******";
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
                    WordData = File.ReadAllLines(@"..\..\..\한글.txt");
                    Words = new List<string>(WordData.OrderBy(i => Guid.NewGuid()).ToList());
                    //Console.WriteLine(ex.ToString());
                    MessageBox.Show("DB 쿼리 실패로 텍스트 파일 로드.");
                    MessageBox.Show(ex.ToString());
                }

            }
        }

        private void ResetGame()
        {
            connDB();
            //WordData = File.ReadAllLines(@"..\..\..\한글.txt");
            //Words = new List<string>(WordData.OrderBy(i => Guid.NewGuid()).ToList());
            //
            /*
            MessageBox.Show("shuffle finished");
            StringBuilder sb = new StringBuilder();
            foreach (string word in Words)
            {
                sb.Append(word);
                sb.Append("\n");
            }
            File.WriteAllText(@"..\..\..\shuffled.txt", sb.ToString());
            MessageBox.Show("saved shuffle");
            */
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
                    /*
                    if (Menu_Item.Checked)
                    {
                        switch (Menu_Item.Text)
                        {
                            case "쉬움":
                                Level = (int)Difficulty.EASY;
                                break;
                            case "보통":
                                Level = (int)Difficulty.NORMAL;
                                break;
                            case "어려움":
                                Level = (int)Difficulty.HARD;
                                break;
                        }
                    }
                    */
                }
            }
        }

        private void DrawScreen()
        {
            lock (ThreadLock)
            {
                Graphics g = this.CreateGraphics();
                using (BufferedGraphics bg = BufferedGraphicsManager.Current.Allocate(g, this.ClientRectangle))
                {
                    switch (Level)
                    {
                        case (int)DIFFICULTY.EASY:
                            if(HealthBar.Value >= 70)
                            {
                                bg.Graphics.DrawImage(Ocean, 0, 0); 
                            }
                            else if(HealthBar.Value >= 30)
                            {
                                bg.Graphics.DrawImage(Ocean_red, 0, 0);
                            }
                            else
                            {
                                bg.Graphics.DrawImage(Ocean_red2, 0, 0);
                            }
                            break;
                        case (int)DIFFICULTY.NORMAL:
                            if (HealthBar.Value >= 70)
                            {
                                bg.Graphics.DrawImage(LightHouse, 0, 0);
                            }
                            else if (HealthBar.Value >= 30)
                            {
                                bg.Graphics.DrawImage(LightHouse_red, 0, 0);
                            }
                            else
                            {
                                bg.Graphics.DrawImage(LightHouse_red2, 0, 0);
                            }
                            break;
                        case (int)DIFFICULTY.HARD:
                            if (HealthBar.Value >= 70)
                            {
                                bg.Graphics.DrawImage(City, 0, 0);
                            }
                            else if (HealthBar.Value >= 30)
                            {
                                bg.Graphics.DrawImage(City_red, 0, 0);
                            }
                            else
                            {
                                bg.Graphics.DrawImage(City_red2, 0, 0);
                            }
                            break;
                    }

                    bg.Graphics.FillRectangle(Brushes.Black, floor_Rect);
                    string strScore = string.Format("점수 : {0}", Score);
                    string strTime = string.Format("남은 시간 : {0}", Time);
                    Font font = new Font("맑은 고딕", 12);
                    StringFormat strFmt = new StringFormat();
                    strFmt.Alignment = StringAlignment.Far;
                    bg.Graphics.DrawString(strScore, font, Brushes.LightGreen,
                        new RectangleF(ClientRectangle.Width - 120, 25, 120, 20), strFmt);
                    bg.Graphics.DrawString(strTime, font, Brushes.LightGreen, new Point(0, 25));
                    for (int i = 0; i < falling_Words.Count; ++i)
                    {
                        bg.Graphics.DrawString(falling_Words[i], font, Brushes.LightGreen, falling_Word_Pos[i]);
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
            /*
            try
            {
                
            }
            catch(Exception ex)
            {
                //MessageBox.Show("error hi");
            }
            */
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
                x = rand.Next(ClientRectangle.Width - text_Width);
                y = 50;
                falling_Words.Add(temp_Word.ToString());
                falling_Word_Pos.Add(new Point(x, y));
                for (int i = falling_Words.Count - 1; i >= 0; --i)
                {
                    falling_Word_Pos[i] = new Point(falling_Word_Pos[i].X, falling_Word_Pos[i].Y + drop_Height);
                    if (falling_Word_Pos[i].Y + text_Height >= ClientRectangle.Height - floor_Height)
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
            //open_rankingForm();
            //m_thr = new Thread(open_rankingForm);
            //m_thr.Start();
            //open_rankingForm();
            //MessageBox.Show("level : " + Difficulty + "\n" + "score : " + Score.ToString());
            if (InvokeRequired)
            {
                this.Invoke(new Action(() => open_rankingForm()));
                return;
            }
            if (GameThread.IsAlive)
            {
                GameThread.Abort();
            }
        }

        private void Show_Ranking_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("ranking");
            string connStr = File.ReadAllText(@"..\..\..\dbconnect.txt");  //"server=localhost;user=root;database=world;port=3306;password=******";
            using (MySqlConnection connection = new MySqlConnection(connStr))
            {
                try//예외 처리
                {
                    connection.Open();
                    string sql = "SELECT `name`, `score`, `difficulty` FROM score ORDER BY `score` DESC LIMIT 10";

                    MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(sql, connection);

                    DataTable dt = new DataTable();
                    mySqlDataAdapter.Fill(dt);

                    string m_str="\tname\t\tscore\t\tdifficulty\n\n";
                    string difficulty_changer = "";
                    string name_format = "";
                    int i = 1;
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["difficulty"].ToString() == "0")
                        {
                            difficulty_changer = "Easy";
                        }
                        else if (row["difficulty"].ToString() == "1")
                        {
                            difficulty_changer = "Normal";
                        }
                        else
                        {
                            difficulty_changer = "Hard";
                        }
                        name_format = row["name"].ToString();
                        if (name_format.Length < 5)
                            name_format += "\t";
                        m_str += i.ToString()+".\t"+name_format + "\t" + row["score"].ToString() + "\t\t" + difficulty_changer + "\n";
                        i++;
                        //MessageBox.Show(row["name"].ToString() +"d" +row["score"].ToString() + "d" + row["difficulty"].ToString());
                    }
                    MessageBox.Show(m_str);
                    //Words = new List<string>(Database.OrderBy(i => Guid.NewGuid()));

                }
                catch (Exception ex)
                {
                    //WordData = File.ReadAllLines(@"..\..\..\한글.txt");
                    //Words = new List<string>(WordData.OrderBy(i => Guid.NewGuid()).ToList());
                    //Console.WriteLine(ex.ToString());
                    MessageBox.Show("랭킹 DB 쿼리 실패");
                    //MessageBox.Show(ex.ToString());
                }

            }
        }

        public void InsertRanking(string name)
        {
            string connStr = File.ReadAllText(@"..\..\..\dbconnect.txt");  //"server=localhost;user=root;database=world;port=3306;password=******";
            using (MySqlConnection connection = new MySqlConnection(connStr))
            {
                try//예외 처리
                {
                    connection.Open();
                    string ssql = "INSERT INTO `hangul`.`score` (`name`, `score`, `difficulty`) VALUES ('"+name+"', '"+Score.ToString()+"', '"+(Level-1).ToString()+"')";
                    MySqlCommand mcmd = new MySqlCommand(ssql, connection);
                    mcmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("랭킹 DB 입력 실패");
                }

            }
        }

        private void open_rankingForm()
        {
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.Name == "RankingForm")
                {
                    frm.Activate();
                    return;
                }
            }

            RankFrm = new RankingForm();
            RankFrm.Owner = this;
            RankFrm.parent = this;

            RankFrm.Show();
        }

        private void QuitGame()
        {
            bRunning = false;
            if (GameThread.IsAlive)
            {
                GameThread.Abort();
            }
            open_rankingForm();
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
            //MessageBox.Show("level : " + Difficulty + "\n" + "score : " + Score.ToString());
        }

        private void WordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (bRunning && e.KeyCode == Keys.Enter)
            {
                int idx = falling_Words.FindIndex(x => x.Equals(WordBox.Text));
                if (idx != -1)
                {
                    falling_Words.RemoveAt(idx);
                    falling_Word_Pos.RemoveAt(idx);
                    Score += word_Score;
                    DrawScreen();
                }
                WordBox.Text = "";
            }
        }
    }
}
