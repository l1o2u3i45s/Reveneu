using System;
using System.Drawing;
using System.Windows.Forms;

using MySql.Data.MySqlClient;           // MySQL 元件

namespace Reveneu
{
    public partial class Add : Form
    {
        string connStr = "";

        public Add()
        {
            InitializeComponent();
        }

        private void Add_Load(object sender, EventArgs e)
        {
            connStr = Reveneu.connStr;

            string yy = (System.DateTime.Now.Year - 1911).ToString();
            string mm = System.DateTime.Now.Month.ToString();
            if (System.DateTime.Now.Month < 10)
                mm = "0" + mm;
            string dd = System.DateTime.Now.Day.ToString();
            if (System.DateTime.Now.Day < 10)
                dd = "0" + dd;

            maskedTextBox1.Text = yy + mm + dd;

            if (Reveneu.vendor_code == "A000000")
                comboBox1.Enabled = true;
            else
                comboBox1.Enabled = false;

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

            toolStripStatusLabel1.Text = "";

            maskedTextBox1.ReadOnly = true;
            button3.Visible = false;
            label10.Visible = false;
        }

        // 離開
        private void button4_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

        // 新增
        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";
            textBox8.Text = "";
            if (Reveneu.vendor_code == "A000000")
            {
                comboBox1.Enabled = true;
                maskedTextBox1.ReadOnly = false;
            }
            else
            {
                comboBox1.Enabled = false;
                maskedTextBox1.ReadOnly = true;
            }

            button1.Enabled = false;
            button2.Enabled = false;

            button3.Visible = true;

            textBox1.Focus();

            label10.Text = "- 新增 -";
            label10.ForeColor = Color.Red;
            label10.Visible = true;

            button5.Visible = true;

            toolStripStatusLabel1.Text = "";

            Check_for_Last();
        }

        private void Check_for_Last()
        {
            string str_date = maskedTextBox1.Text.Replace("/", "");
            int int_last_num = 0;

            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();

            string SQL = "select count(*) from otc ";
            SQL += "where rpt_date = '" + str_date + "'";
            MySqlCommand cmd = new MySqlCommand(SQL, conn);
            MySqlDataReader myData = cmd.ExecuteReader();

            if (myData.Read())
            {
                string str_last_num = myData[0].ToString();
                int_last_num = Int16.Parse(str_last_num);
            }

            cmd.Dispose();

            conn.Close();
            conn.Dispose();

            if (int_last_num == 5)
            {
                Reveneu.message = "你/妳 是最後一位填資料的,";
                Reveneu.message += "\n記得要將排名上傳到 Line";
                Message frm = new Message();
                frm.ShowDialog();

                frm.Dispose();
                frm.Close();
            }
        }

        // 修改
        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";
            textBox8.Text = "";
            if (Reveneu.vendor_code == "A000000")
            {
                comboBox1.Enabled = true;
                comboBox1.Text = "佑昌藥局";
            }
            else
            {
                comboBox1.Enabled = false;
            }

            maskedTextBox1.ReadOnly = true;
            textBox1.Focus();

            button1.Enabled = false;
            button2.Enabled = false;

            button3.Visible = true;

            label10.Text = "- 修改 -";
            label10.ForeColor = Color.Red;
            label10.Visible = true;

            button5.Visible = true;

            toolStripStatusLabel1.Text = "";
        }

        // 儲存
        private void button3_Click(object sender, EventArgs e)
        {
            button5.Visible = false;

            if (textBox1.Text == "")
                textBox1.Text = "0";

            if (textBox2.Text == "")
                textBox2.Text = "0";

            if (textBox3.Text == "")
                textBox3.Text = "0";

            if (textBox4.Text == "")
                textBox4.Text = "0";

            if (textBox5.Text == "")
                textBox5.Text = "0";

            if (textBox6.Text == "")
                textBox6.Text = "0";

            if (textBox7.Text == "")
                textBox7.Text = "0";

            if (textBox8.Text == "")
                textBox8.Text = "0";

            string vendor_code = "";
            if (Reveneu.vendor_code == "A000000")
            {
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
            }
            else
                vendor_code = Reveneu.vendor_code;

            // 若是修改先刪除舊資料
            if (label10.Text == "- 修改 -")
                Perform_delete(vendor_code);

            // 先檢查資料,再執行新增
            if (verify_input())
            {
                // 確認資料未曾輸入
                if (!data_validate(maskedTextBox1.Text, vendor_code))
                {
                    

                    if (Save_OTC(vendor_code))
                    {
                        Save_Prescription(vendor_code);
                        Save_ChugaiMedicine();
                    }
                       
                }
                else
                {
                    toolStripStatusLabel1.Text = "本日資料已輸入過";
                    toolStripStatusLabel1.ForeColor = Color.Red;
                }
            }
        }

        private bool verify_input()
        {
            bool ret_value = false;

            int num;
            string txt = "";

            if (!int.TryParse(textBox1.Text, out num))
            {
                txt = "當日淨利";
            }

            if (!int.TryParse(textBox2.Text, out num))
            {
                txt = "+當月總調劑人數";
            }

            if (!int.TryParse(textBox3.Text, out num))
            {
                txt += "+合作診所收入";
            }

            if (!int.TryParse(textBox4.Text, out num))
            {
                txt += "+配藥毛利收入";
            }

            if (!int.TryParse(textBox5.Text, out num))
            {
                txt += "+慢箋+其他收入";
            }

            if (txt == "")
                ret_value = true;
            else
            {
                ret_value = false;
                txt += " 非數字";
            }

            toolStripStatusLabel1.Text = txt;
            toolStripStatusLabel1.ForeColor = Color.Red;

            return ret_value;
        }

        //儲存中藥資料
        private bool Save_ChugaiMedicine() {
            bool ret_value = false;

            string str_date = maskedTextBox1.Text.Replace("/", "");
            
                MySqlConnection conn = new MySqlConnection(connStr);
                conn.Open();

                string SQL = "";
                SQL = "INSERT INTO TraditionalChineseMedicine VALUES ('";
                SQL += str_date + "', '";
                SQL += comboBox1.Text.Substring(0,2) + "', '";
                SQL += textBox5.Text + "', '";
                SQL += textBox6.Text + "', '";
                SQL += textBox7.Text + "')";

                MySqlCommand cmd = new MySqlCommand(SQL, conn);
                cmd.ExecuteNonQuery();

                cmd.Dispose();

                conn.Close();
                conn.Dispose();

                ret_value = true;
            
          
            return ret_value;
        }
        // 儲存OTC資料
        private bool Save_OTC(string vendor_code)
        {
            bool ret_value = false;

            string str_date = maskedTextBox1.Text.Replace("/", "");
            string str_today_num = get_today_num(str_date, vendor_code);

            if (!str_today_num.Contains("-"))
            {
                MySqlConnection conn = new MySqlConnection(connStr);
                conn.Open();

                string SQL = "";
                SQL = "INSERT INTO otc VALUES ('";
                SQL += str_date + "', '";
                SQL += vendor_code + "', '";
                SQL += textBox1.Text + "', '";
                SQL += textBox2.Text + "', '";
                SQL += str_today_num + "')";

                MySqlCommand cmd = new MySqlCommand(SQL, conn);
                cmd.ExecuteNonQuery();

                cmd.Dispose();

                conn.Close();
                conn.Dispose();

                ret_value = true;
            }
            else
            {
                toolStripStatusLabel1.Text = "總人次輸入錯誤";
                toolStripStatusLabel1.ForeColor = Color.Red;
                ret_value = false;
            }

            return ret_value;
        }

        // 儲存調劑台資料
        private void Save_Prescription(string vendor_code)
        {

            string str_date = maskedTextBox1.Text.Replace("/", "");

            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();

            string SQL = "";
            SQL = "INSERT INTO presc VALUES ('";
            SQL += str_date + "', '";
            SQL += vendor_code + "', '";
            SQL += textBox3.Text + "', '";
            SQL += textBox4.Text + "', '";
            SQL += textBox5.Text + "')";

            MySqlCommand cmd = new MySqlCommand(SQL, conn);
            cmd.ExecuteNonQuery();

            cmd.Dispose();

            conn.Close();
            conn.Dispose();

            toolStripStatusLabel1.Text = "存檔成功";
            toolStripStatusLabel1.ForeColor = Color.DarkGreen;

            recovery();
        }

        // 取得當日人數, 傳入年月資料
        private string get_today_num(string str_date, string store)
        {
            string ret_value = "0";

            string yy = str_date.Substring(0, 3);
            string mm = str_date.Substring(3, 2);
            string dd = str_date.Substring(5, 2);

            // 如果是當月第 一天
            //if (dd == "01")
            if (check_first_day(yy + mm, store))
            {
                ret_value = textBox2.Text;
            }
            else
            {
                MySqlConnection conn = new MySqlConnection(connStr);
                conn.Open();

                string SQL = "select * from otc ";
                SQL += "where rpt_date like '" + yy + mm + "%' and rpt_store = '" + store + "' ";
                SQL += "order by rpt_date desc";
                MySqlCommand cmd = new MySqlCommand(SQL, conn);
                MySqlDataReader myData = cmd.ExecuteReader();

                if (myData.Read())
                {
                    string str_last_num = myData[3].ToString();
                    int int_last_num = Int16.Parse(str_last_num);
                    int cur_num = Int16.Parse(textBox2.Text);
                    ret_value = (cur_num - int_last_num).ToString();
                }

                cmd.Dispose();

                conn.Close();
                conn.Dispose();
            }

            return ret_value;
        }

        private bool check_first_day(string day, string str_store)
        {
            bool ret_value = true;

            string yy = maskedTextBox1.Text.Substring(0, 3);
            string mm = maskedTextBox1.Text.Substring(4, 2);

            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();

            string SQLcmd = "select * from otc where rpt_date like '" + day + "%'";
            SQLcmd += " and rpt_store ='" + str_store + "'";
            MySqlCommand cmd = new MySqlCommand(SQLcmd, conn);
            MySqlDataReader myData = cmd.ExecuteReader();

            if (myData.Read())
                ret_value = false;

            myData.Close();
            cmd.Dispose();

            conn.Close();
            conn.Dispose();

            return ret_value;
        }

        // 刪除當日資料
        private void Perform_delete(string vendor_code)
        {
            try
            {
                Delete_OTC(vendor_code);
                Delete_Prescription(vendor_code);
                Delete_ChineseMedicine();
            }
            catch (Exception e)
            {
                toolStripStatusLabel1.Text = e.Message.ToString();
            }
        }
        //刪除當日中藥資料
        private void Delete_ChineseMedicine()
        {
            string str_date = maskedTextBox1.Text.Replace("/", "");

            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();

            string SQL = "";
            SQL = "DELETE FROM TraditionalChineseMedicine WHERE ";
            SQL += "rpt_date = '" + str_date + "' and ";
            SQL += "rpt_store = '" + comboBox1.Text.Substring(0,2) + "'";

            MySqlCommand cmd = new MySqlCommand(SQL, conn);
            cmd.ExecuteNonQuery();

            cmd.Dispose();

            conn.Close();
            conn.Dispose();
        }
        // 刪除當日OTC資料
        private void Delete_OTC(string vendor_code)
        {
            string str_date = maskedTextBox1.Text.Replace("/", "");

            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();

            string SQL = "";
            SQL = "DELETE FROM otc WHERE ";
            SQL += "rpt_date = '" + str_date + "' and ";
            SQL += "rpt_store = '" + vendor_code + "'";

            MySqlCommand cmd = new MySqlCommand(SQL, conn);
            cmd.ExecuteNonQuery();

            cmd.Dispose();

            conn.Close();
            conn.Dispose();
        }

        private void Delete_Prescription(string vendor_code)
        {
            string str_date = maskedTextBox1.Text.Replace("/", "");

            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();

            string SQL = "";
            SQL = "DELETE FROM presc WHERE ";
            SQL += "pre_date = '" + str_date + "' and ";
            SQL += "pre_store = '" + vendor_code + "'";

            MySqlCommand cmd = new MySqlCommand(SQL, conn);
            cmd.ExecuteNonQuery();

            cmd.Dispose();

            conn.Close();
            conn.Dispose();
        }

        private bool data_validate(string str_date, string str_store)
        {
            bool ret_value = false;

            str_date = str_date.Replace("/", "");

            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();

            string SQLcmd = "select * from otc where rpt_date = '" + str_date;
            SQLcmd += "' and rpt_store ='" + str_store + "'";
            MySqlCommand cmd = new MySqlCommand(SQLcmd, conn);
            MySqlDataReader myData = cmd.ExecuteReader();

            if (myData.Read())
                ret_value = true;

            myData.Close();
            cmd.Dispose();

            conn.Close();
            conn.Dispose();

            return ret_value;
        }

        private void recovery()
        {
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Visible = false;

            label10.Visible = false;
        }

        private void maskedTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                textBox1.Focus();
            }
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
                textBox4.Focus();
            }  
        }

        private void textBox4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                textBox5.Focus();
            }  
        }

        private void textBox5_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button3.Focus();
            }   
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string yy = (System.DateTime.Now.Year - 1911).ToString();
            string mm = System.DateTime.Now.Month.ToString();
            if (System.DateTime.Now.Month < 10)
                mm = "0" + mm;
            string dd = System.DateTime.Now.Day.ToString();
            if (System.DateTime.Now.Day < 10)
                dd = "0" + dd;

            maskedTextBox1.Text = yy + mm + dd;

            if (Reveneu.vendor_code == "A000000")
                comboBox1.Enabled = true;
            else
                comboBox1.Enabled = false;

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

            toolStripStatusLabel1.Text = "";

            maskedTextBox1.ReadOnly = true;
            button3.Visible = false;
            label10.Visible = false;

            button1.Enabled = true;
            button2.Enabled = true;

            button5.Visible = false;
        }
        private void Caculate() {
            if (textBox6.Text != string.Empty && textBox7.Text != string.Empty)
                textBox8.Text = (Convert.ToDouble(textBox6.Text) - Convert.ToDouble(textBox7.Text)).ToString();
        }
        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            Caculate();
        }

        private void textBox6_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Caculate();
                textBox7.Focus();
            }
        }
        
        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            Caculate();
        }
    }
}
