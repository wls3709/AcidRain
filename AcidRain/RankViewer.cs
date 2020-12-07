using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace AcidRain
{
    public partial class RankViewer : Form
    {
        public Form1 parent;
        public RankViewer()
        {
            InitializeComponent();
        }

        private void RankViewer_Load(object sender, EventArgs e)
        {
            RankList.View = View.Details;
            RankList.GridLines = true;
            int Width = RankList.Width / 4 - 5;
            RankList.Columns.Add("순위", Width, HorizontalAlignment.Right);
            RankList.Columns.Add("이름", Width, HorizontalAlignment.Right);
            RankList.Columns.Add("점수", Width, HorizontalAlignment.Right);
            RankList.Columns.Add("난이도", Width, HorizontalAlignment.Right);
            string connStr = File.ReadAllText(@"..\..\..\dbconnect.txt");
            using (MySqlConnection connection = new MySqlConnection(connStr))
            {
                try//예외 처리
                {
                    connection.Open();
                    string sql = "SELECT `name`, `score`, `difficulty` FROM score ORDER BY `score` DESC, `difficulty` DESC LIMIT 10";

                    MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(sql, connection);

                    DataTable dt = new DataTable();
                    mySqlDataAdapter.Fill(dt);

                    // string m_str = "\tname\t\tscore\t\tdifficulty\n\n";
                    string difficulty_changer = "";
                    string name_format = "";
                    int i = 1;
                    RankList.BeginUpdate();
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["difficulty"].ToString() == "1")
                        {
                            difficulty_changer = "EASY";
                        }
                        else if (row["difficulty"].ToString() == "2")
                        {
                            difficulty_changer = "NORMAL";
                        }
                        else
                        {
                            difficulty_changer = "HARD";
                        }
                        name_format = row["name"].ToString();
                        if (name_format.Length < 5)
                            name_format += "\t";
                        ListViewItem item = new ListViewItem(i.ToString());
                        item.SubItems.Add(name_format);
                        item.SubItems.Add(row["score"].ToString());
                        item.SubItems.Add(difficulty_changer);
                        RankList.Items.Add(item);
                        ++i;
                        // m_str += i.ToString() + ".\t" + name_format + "\t" + row["score"].ToString() + "\t\t" + difficulty_changer + "\n";
                    }
                    RankList.EndUpdate();
                    // MessageBox.Show(m_str);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("랭킹 DB 쿼리 실패");
                    this.Dispose();
                    // MessageBox.Show(ex.ToString());
                }
            }
        }

        private void CloseForm_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
