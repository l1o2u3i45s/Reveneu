using System;
using System.Windows.Forms;

using MySql.Data.MySqlClient;           // MySQL 元件

namespace Reveneu
{
    public partial class OTC_History : Form
    {
        string connStr = "";

        public OTC_History()
        {
            InitializeComponent();
        }

        private void OTC_History_Load(object sender, EventArgs e)
        {
            connStr = Reveneu.connStr;

            string yy = (System.DateTime.Now.Year - 1911).ToString();
            string mm = System.DateTime.Now.Month.ToString();
            if (System.DateTime.Now.Month < 10)
                mm = "0" + mm;

            maskedTextBox1.Text = yy + mm;

            switch (Reveneu.vendor_code)
            {
                case "A000000":
                    comboBox1.Text = "佑昌藥局";
                    break;

                case "A000001":
                    comboBox1.Text = "佑昌藥局";
                    break;

                case "A000002":
                    comboBox1.Text = "明昌藥局";
                    break;

                case "A000137":
                    comboBox1.Text = "佑東藥局";
                    break;

                case "A000359":
                    comboBox1.Text = "宏昌藥局";
                    break;

                case "A000417":
                    comboBox1.Text = "杏昌藥局";
                    break;

                case "A000128":
                    comboBox1.Text = "和昌藥局";
                    break;
            }

            dataGridView1.Visible = false;

            Query();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Query();
        }

        private void Query()
        {
            dataGridView1.Visible = true;

            string str_date = maskedTextBox1.Text.Replace("/", "");
            string str_store = "";

            switch (comboBox1.Text)
            {
                case "佑昌藥局":
                    str_store = "A000001";
                    break;

                case "明昌藥局":
                    str_store = "A000002";
                    break;

                case "佑東藥局":
                    str_store = "A000137";
                    break;

                case "宏昌藥局":
                    str_store = "A000359";
                    break;

                case "杏昌藥局":
                    str_store = "A000417";
                    break;

                case "和昌藥局":
                    str_store = "A000128";
                    break;

                default:
                    str_store = Reveneu.vendor_code;
                    break;
            }

            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();

            string SQLcmd = "select * from otc where rpt_date like '" + str_date;
            SQLcmd += "%' and rpt_store ='" + str_store + "' ";
            SQLcmd += "order by rpt_date desc";
            MySqlCommand cmd = new MySqlCommand(SQLcmd, conn);
            MySqlDataReader myData = cmd.ExecuteReader();

            int i = 0;
            dataGridView1.Rows.Clear();

            while (myData.Read())
            {
                dataGridView1.Rows.Add();
                dataGridView1.Rows[i].Cells[0].Value = myData[0].ToString();    // 日期
                dataGridView1.Rows[i].Cells[1].Value = myData[2].ToString();    // 當日淨利
                dataGridView1.Rows[i].Cells[2].Value = myData[3].ToString();    // 總調劑人數
                dataGridView1.Rows[i].Cells[3].Value = myData[4].ToString();    // 當日人數
                i++;
            }

            myData.Close();
            cmd.Dispose();

            conn.Close();
            conn.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

        private void maskedTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (maskedTextBox1.Text.Length == 6)
                Query();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Query();
        }
    }
}
