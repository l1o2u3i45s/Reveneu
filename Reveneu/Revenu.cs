using System;
using System.Windows.Forms;

using MySql.Data.MySqlClient;           // MySQL 元件

namespace Reveneu
{
    public partial class Reveneu : Form
    {
        public static string message = "";
        public static string vendor_code = "";
        public static string connStr = "";

        public Reveneu()
        {
            InitializeComponent();
        }

        private void 離開ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

        private void Reveneu_Load(object sender, EventArgs e)
        {
            connStr = "server=rx1.chifeng.idv.tw;port=3310;uid=pharm;pwd=qwaszx101819;database=rx_results";
            //connStr = "server=localhost;uid=root;pwd=2iixoguu;database=a000000";

            toolStripStatusLabel1.Text = "";

            string[] args = Environment.GetCommandLineArgs();   // 取得程式執行的參數
                                                                // args[0] 是程式名稱
            vendor_code = args[1];

            if (vendor_code != "A000000")
            {
                目標設定ToolStripMenuItem.Visible = false;
            }

            try
            {
                MySqlConnection conn = new MySqlConnection(connStr);
                conn.Open();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                switch (ex.Number.ToString())
                {
                    case "0":
                        message = "無法連線到資料庫";
                        Message form = new Message();
                        form.ShowDialog();

                        form.Dispose();
                        form.Close();

                        this.Dispose();
                        this.Close();
                        break;

                    case "1042":
                        message = "資料庫未啟動 或 網路斷線";
                        message += "\n請將要填的資料放到 Line 上";
                        message += "\n由資訊人員代填資料";
                        Message frm = new Message();
                        frm.ShowDialog();

                        frm.Dispose();
                        frm.Close();

                        this.Dispose();
                        this.Close();
                        break;

                    default:
                        message = ex.Message.ToString();
                        Message fm = new Message();
                        fm.ShowDialog();

                        fm.Dispose();
                        fm.Close();

                        this.Dispose();
                        this.Close();
                        break;
                }
            }
        }

        private void 新增ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Add frm = new Add();
            frm.MdiParent = this;
            frm.Show();
        }

        private void 修正調劑台整月數字ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Update frm = new Update();
            frm.MdiParent = this;
            frm.Show();
        }

        private void OTC歷史資料ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OTC_History frm = new OTC_History();
            frm.MdiParent = this;
            frm.Show();
        }

        private void 調劑台歷史資料ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Pres_History frm = new Pres_History();
            frm.MdiParent = this;
            frm.Show();
        }

        private void OTC每日排名ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OTC_Ranking frm = new OTC_Ranking();
            frm.MdiParent = this;
            frm.Show();
        }

        private void 調劑台每日排名ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Pres_Ranking frm = new Pres_Ranking();
            frm.MdiParent = this;
            frm.Show();
        }

        private void OTC目標ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OTC_Goal frm = new OTC_Goal();
            frm.MdiParent = this;
            frm.Show();
        }

        private void 調劑台目標ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Pres_Goal frm = new Pres_Goal();
            frm.MdiParent = this;
            frm.Show();
        }

        private void 排除合作診所每日排名ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Ranking frm = new Ranking();
            frm.MdiParent = this;
            frm.Show();
        }

        private void 不含診所目標ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Pure_Goal frm = new Pure_Goal();
            frm.MdiParent = this;
            frm.Show();
        }

        private void 每日毛利ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
