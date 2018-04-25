using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using MySql.Data.MySqlClient;           // MySQL 元件

namespace Reveneu
{
    public partial class Pure_Goal : Form
    {
        string connStr = "";

        public Pure_Goal()
        {
            InitializeComponent();
        }

        private void Pure_Goal_Load(object sender, EventArgs e)
        {
            connStr = Reveneu.connStr;

            string yy = (System.DateTime.Now.Year - 1911).ToString();
            string mm = System.DateTime.Now.Month.ToString();
            if (System.DateTime.Now.Month < 10)
                mm = "0" + mm;

            maskedTextBox1.Text = yy + mm;

            comboBox1.Text = "A000001 - 佑昌";

            dataGridView1.Visible = false;

            toolStripStatusLabel1.Text = "";
        }

        // 新增
        private void button1_Click(object sender, EventArgs e)
        {
            if (data_validation())
            {
                string str_date = maskedTextBox1.Text.Replace("/", "");

                MySqlConnection conn = new MySqlConnection(connStr);
                conn.Open();

                string SQL = "INSERT INTO pres_goal VALUES ('";
                SQL += str_date + "', '";
                SQL += comboBox1.Text.Substring(0, 7) + "', '";
                SQL += textBox1.Text + "')";

                MySqlCommand cmd = new MySqlCommand(SQL, conn);
                cmd.ExecuteNonQuery();

                cmd.Dispose();

                conn.Close();
                conn.Dispose();

                toolStripStatusLabel1.Text = "新增成功";
                toolStripStatusLabel1.ForeColor = Color.DarkGreen;
            }
        }

        private bool data_validation()
        {
            bool ret_val = true;

            string str_date = maskedTextBox1.Text.Replace("/", "");
            if (str_date == "")
            {
                toolStripStatusLabel1.Text = "年度月份不可空白";
                toolStripStatusLabel1.ForeColor = Color.Red;
                ret_val = false;
            }

            if (textBox1.Text == "")
            {
                toolStripStatusLabel1.Text = "目標值不可空白";
                toolStripStatusLabel1.ForeColor = Color.Red;
                ret_val = false;
            }

            if (textBox1.Text != "" && Int16.Parse(textBox1.Text) == 0)
            {
                toolStripStatusLabel1.Text = "目標值必須是數字";
                toolStripStatusLabel1.ForeColor = Color.Red;
                ret_val = false;
            }

            return ret_val;
        }

        // 修改
        private void button2_Click(object sender, EventArgs e)
        {
            string str_date = maskedTextBox1.Text.Replace("/", "");

            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();

            string SQL = "UPDATE pres_goal SET ";
            SQL += "pg_num = '" + textBox1.Text + "' ";
            SQL += "WHERE pg_date = '" + str_date + "' and ";
            SQL += "pg_store = '" + comboBox1.Text.Substring(0, 7) + "'";

            MySqlCommand cmd = new MySqlCommand(SQL, conn);
            cmd.ExecuteNonQuery();
            cmd.Dispose();

            conn.Dispose();
            conn.Close();

            toolStripStatusLabel1.Text = "資料修改成功";
            toolStripStatusLabel1.ForeColor = Color.DarkGreen;
        }

        // 刪除
        private void button3_Click(object sender, EventArgs e)
        {
            string str_date = maskedTextBox1.Text.Replace("/", "");

            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();

            string SQL = "DELETE FROM pres_goal ";
            SQL += "WHERE pg_date = '" + str_date + "' and ";
            SQL += "pg_store = '" + comboBox1.Text.Substring(0, 7) + "'";

            MySqlCommand cmd = new MySqlCommand(SQL, conn);
            cmd.ExecuteNonQuery();
            cmd.Dispose();

            conn.Dispose();
            conn.Close();

            toolStripStatusLabel1.Text = "資料刪除成功";
            toolStripStatusLabel1.ForeColor = Color.DarkGreen;
        }

        // 查詢
        private void button4_Click(object sender, EventArgs e)
        {
            dataGridView1.Visible = true;

            string str_date = maskedTextBox1.Text.Replace("/", "");

            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();

            string SQLcmd = "select * from pres_goal where pg_date = '" + str_date + "'";
            MySqlCommand cmd = new MySqlCommand(SQLcmd, conn);
            MySqlDataReader myData = cmd.ExecuteReader();
            int i = 0;
            while (myData.Read())
            {
                dataGridView1.Rows.Add();

                dataGridView1.Rows[i].Cells[0].Value = myData[0].ToString();

                string temp = "";
                switch (myData[1].ToString())
                {
                    case "A000001":
                        temp = "佑昌藥局";
                        break;

                    case "A000002":
                        temp = "明昌藥局";
                        break;

                    case "A000137":
                        temp = "佑東藥局";
                        break;

                    case "A000359":
                        temp = "宏昌藥局";
                        break;

                    case "A000417":
                        temp = "杏昌藥局";
                        break;

                    case "A000128":
                        temp = "和昌藥局";
                        break;
                }

                dataGridView1.Rows[i].Cells[1].Value = temp;
                dataGridView1.Rows[i].Cells[2].Value = myData[2].ToString();

                i++;
            }

            myData.Close();
            cmd.Dispose();

            conn.Close();
            conn.Dispose();
        }

        // 離開
        private void button5_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowIndex = dataGridView1.CurrentCell.RowIndex;

            maskedTextBox1.Text = dataGridView1.Rows[rowIndex].Cells[0].Value.ToString();
            //maskedTextBox1.Text = maskedTextBox1.Text.Substring(0, 3) + "/" + textBox1.Text.Substring(3, 2);

            string store = dataGridView1.Rows[rowIndex].Cells[1].Value.ToString();
            switch (store)
            {
                case "佑昌藥局":
                    comboBox1.Text = "A000001 - 佑昌";
                    break;

                case "明昌藥局":
                    comboBox1.Text = "A000002 - 明昌";
                    break;

                case "和昌藥局":
                    comboBox1.Text = "A000128 - 和昌";
                    break;

                case "佑東藥局":
                    comboBox1.Text = "A000137 - 佑東";
                    break;

                case "宏昌藥局":
                    comboBox1.Text = "A000359 - 宏昌";
                    break;

                case "杏昌藥局":
                    comboBox1.Text = "A000417 - 杏昌";
                    break;
            }
            textBox1.Text = dataGridView1.Rows[rowIndex].Cells[2].Value.ToString();

            dataGridView1.Visible = false;
        }

        private void Pure_Goal_Click(object sender, EventArgs e)
        {
            dataGridView1.Visible = false;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "";
        }

        private void maskedTextBox1_TextChanged(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "";
        }
    }
}
