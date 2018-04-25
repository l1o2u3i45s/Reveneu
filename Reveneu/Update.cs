using System;
using System.Drawing;
using System.Windows.Forms;

using MySql.Data.MySqlClient;           // MySQL 元件

namespace Reveneu
{
    public partial class Update : Form
    {
        string connStr = "";

        public Update()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

        private void Update_Load(object sender, EventArgs e)
        {
            connStr = Reveneu.connStr;

            if (Reveneu.vendor_code == "A000000")
                comboBox1.Enabled = true;
            else
                comboBox1.Enabled = false;

            switch (Reveneu.vendor_code)
            {
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

            toolStripStatusLabel1.Text = "";
        }

        // 儲存更新
        private void button1_Click(object sender, EventArgs e)
        {
            string vendor_code = "";
            switch (comboBox1.Text)
            {
                case "佑昌藥局":
                    vendor_code = "A000001";
                    break;

                case "明昌藥局":
                    vendor_code = "A000002";
                    break;

                case "佑東藥局":
                    vendor_code = "A000137";
                    break;

                case "宏昌藥局":
                    vendor_code = "A000359";
                    break;

                case "杏昌藥局":
                    vendor_code = "A000417";
                    break;

                case "和昌藥局":
                    vendor_code = "A000128";
                    break;
            }

            Clean_to_Zero(vendor_code);
            Save_new_Data(vendor_code);

            toolStripStatusLabel1.Text = "修改成功";
            toolStripStatusLabel1.ForeColor = Color.DarkGreen;
        }

        private void Clean_to_Zero(string vendor_code)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();

            string SQLcmd = "UPDATE presc SET pre_clinic = '0', pre_dose = '0', ";
            SQLcmd += "pre_long_term = '0' where pre_store = '" + vendor_code + "'";
            MySqlCommand cmd = new MySqlCommand(SQLcmd, conn);
            cmd.ExecuteNonQuery();

            cmd.Dispose();

            conn.Close();
            conn.Dispose();
        }

        private void Save_new_Data(string vendor_code)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();

            string SQLcmd = "SELECT * FROM presc WHERE PRE_STORE = '" + vendor_code + "' ";
            SQLcmd += "ORDER BY pre_date desc";
            MySqlCommand cmd = new MySqlCommand(SQLcmd, conn);
            MySqlDataReader myData = cmd.ExecuteReader();

            if (myData.Read())
            {
                MySqlConnection iconn = new MySqlConnection(connStr);
                iconn.Open();

                string iSQL = "UPDATE presc SET pre_clinic = '" + textBox1.Text + "', ";
                iSQL += "pre_dose = '" + textBox2.Text + "', ";
                iSQL += "pre_long_term = '" + textBox3.Text + "' ";
                iSQL += "where pre_date = '" + myData[0].ToString() + "' and ";
                iSQL += "pre_store = '" + vendor_code + "'";
                MySqlCommand icmd = new MySqlCommand(iSQL, iconn);
                icmd.ExecuteNonQuery();

                icmd.Dispose();

                iconn.Close();
                iconn.Dispose();
            }

            myData.Close();
            cmd.Dispose();

            conn.Close();
            conn.Dispose();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                textBox2.Focus();
            }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                textBox3.Focus();
            }
        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1.Focus();
            }
        }
    }
}
