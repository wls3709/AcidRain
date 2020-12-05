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
        Thread GameThread;
        System.Threading.Timer TimeCounter;
        enum DIFFICULTY
        {
            EASY = 1,
            NORMAL,
            HARD
        }
        enum DROP_CYCLE
        {
            EASY = 1200,
            NORMAL = 600,
            HARD = 100
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
            ResetGame();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            DrawScreen();
        }

        private void ResetGame()
        {
            WordData = File.ReadAllLines(@"..\..\..\한글.txt");
            Words = new List<string>(WordData.OrderBy(i => Guid.NewGuid()).ToList());
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
            HealthBar.Value = 100;
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
            Graphics g = this.CreateGraphics();
            using (BufferedGraphics bg = BufferedGraphicsManager.Current.Allocate(g, this.ClientRectangle))
            {
                bg.Graphics.Clear(BackColor);
                bg.Graphics.FillRectangle(Brushes.Black, floor_Rect);
                string strScore = string.Format("점수 : {0}", Score);
                string strTime = string.Format("남은 시간 : {0}", Time);
                Font font = new Font("맑은 고딕", 12);
                StringFormat strFmt = new StringFormat();
                strFmt.Alignment = StringAlignment.Far;
                bg.Graphics.DrawString(strScore, font, Brushes.Black,
                    new RectangleF(ClientRectangle.Width - 120, 25, 120, 20), strFmt);
                // bg.Graphics.DrawString(strScore, font, Brushes.Black, new Point(ClientRectangle.Width - 120, 25));
                bg.Graphics.DrawString(strTime, font, Brushes.Black, new Point(0, 25));
                for(int i = 0; i < falling_Words.Count; ++i)
                {
                    bg.Graphics.DrawString(falling_Words[i], font, Brushes.Black, falling_Word_Pos[i]);
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
            GameOver();
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
            Region[] regionArr;
            int x, y;
            int text_Width;
            int text_Height;
            while (bRunning)
            {
                temp_Word.Clear();
                charRangeList.Clear();
                regionList.Clear();
                text_Width = 0;
                text_Height = 0;
                if (HealthBar.Value <= 0 || Time <= 0)
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
                    // regionList = regionArr.ToList();
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
                        HealthBar.Value -= word_Damage;
                    }
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
            if (GameThread.IsAlive)
            {
                GameThread.Abort();
            }
            TimeCounter.Dispose();
            Words.Clear();
            falling_Words.Clear();
            falling_Word_Pos.Clear();
            StartGame.Enabled = true;
            ResignGame.Enabled = false;
            LevelMenu.Enabled = true;
            DrawScreen();
            MessageBox.Show("level : " + Level.ToString() + "\n" + "score : " + Score.ToString());
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
