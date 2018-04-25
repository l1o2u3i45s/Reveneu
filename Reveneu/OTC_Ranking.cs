using System;
using System.Drawing;
using System.Windows.Forms;

using MySql.Data.MySqlClient;           // MySQL 元件

namespace Reveneu
{
    public partial class OTC_Ranking : Form
    {
        string connStr = "";

        public OTC_Ranking()
        {
            InitializeComponent();
        }

        private void maskedTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                Query();
        }

        private void OTC_Ranking_Load(object sender, EventArgs e)
        {
            connStr = Reveneu.connStr;

            string yy = (System.DateTime.Now.Year - 1911).ToString();
            string mm = System.DateTime.Now.Month.ToString();
            if (System.DateTime.Now.Month < 10)
                mm = "0" + mm;
            string dd = System.DateTime.Now.Day.ToString();
            if (System.DateTime.Now.Day < 10)
                dd = "0" + dd;

            maskedTextBox1.Text = yy + mm +  dd;

            dataGridView1.Columns[0].HeaderText = maskedTextBox1.Text.Substring(4, 5);

            toolStripStatusLabel1.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Query();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

        private void Query()
        {
            toolStripStatusLabel1.Text = "程式執行中...";
            toolStripStatusLabel1.ForeColor = Color.Blue;

            process_data();
            show_data();

            maskedTextBox1.Focus();
        }

        private void process_data()
        {
            delete_old_data();
            extract_data();
        }

        private void delete_old_data()
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();

            string SQL = "DELETE FROM ranking ";
            SQL += "WHERE rk_store like '%'";

            MySqlCommand cmd = new MySqlCommand(SQL, conn);
            cmd.ExecuteNonQuery();
            cmd.Dispose();

            conn.Dispose();
            conn.Close();
        }

        private void extract_data()
        {
            string yy = maskedTextBox1.Text.Substring(0, 3);
            string mm = maskedTextBox1.Text.Substring(4, 2);
            string dd = maskedTextBox1.Text.Substring(7, 2);

            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();

            string SQLcmd = "select * from otc where rpt_date = '" + yy + mm + dd + "'";
            MySqlCommand cmd = new MySqlCommand(SQLcmd, conn);
            MySqlDataReader myData = cmd.ExecuteReader();

            while (myData.Read())
            {
                MySqlConnection iconn = new MySqlConnection(connStr);
                iconn.Open();

                decimal prof = decimal.Parse(myData[2].ToString());             // 當日淨利
                decimal num = decimal.Parse(myData[4].ToString());              // 當日人數
                if (num == 0)
                    num = 1;
                decimal per = (prof / (num * 70)) * 100;                        // 當日毛利率
                per = decimal.Round(per, 0);
                string str_per = per.ToString();

                string goal_num = myData[3].ToString();                         // 總人次
                string bus_goal = (Int16.Parse(goal_num) * 70).ToString();      // 營業目標
                string month_profit = get_month_profit(myData[1].ToString());   // 月毛利
                decimal decl_ach_rate = (decimal.Parse(month_profit)
                     / decimal.Parse(bus_goal)) * 100;                          // 達成率
                //decl_ach_rate = decl_ach_rate + (decimal)0.1;
                decl_ach_rate = decimal.Round(decl_ach_rate, 2);
                string ach_rate = decl_ach_rate.ToString();

                string store = "";
                switch (myData[1].ToString())
                {
                    case "A000001":
                        store = "佑昌";
                        break;

                    case "A000002":
                        store = "明昌";
                        break;

                    case "A000137":
                        store = "佑東";
                        break;

                    case "A000359":
                        store = "宏昌";
                        break;

                    case "A000417":
                        store = "杏昌";
                        break;

                    case "A000128":
                        store = "和昌";
                        break;
                }

                string iSQL = "INSERT INTO ranking VALUES ('";
                iSQL += store + "', '";                 // 藥局名稱
                iSQL += myData[2].ToString() + "', '";  // 當日毛利
                iSQL += str_per + "', '";               // 當日毛利率
                iSQL += goal_num + "', '";              // 總人次
                iSQL += bus_goal + "', '";              // 營業目標
                iSQL += month_profit + "', '";          // 月毛利
                iSQL += ach_rate + "')";                // 達成率
                MySqlCommand icmd = new MySqlCommand(iSQL, iconn);
                icmd.ExecuteNonQuery();

                icmd.Dispose();

                iconn.Dispose();
                iconn.Close();
            }

            myData.Close();
            cmd.Dispose();

            conn.Close();
            conn.Dispose();
        }

        // 月毛利
        private string get_month_profit(string store)
        {
            string ret_value = "";

            string yy = maskedTextBox1.Text.Substring(0, 3);
            string mm = maskedTextBox1.Text.Substring(4, 2);
            string dd = maskedTextBox1.Text.Substring(7, 2);

            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();

            string SQLcmd = "select sum(rpt_profit) from otc where rpt_date like '" + yy + mm + "%' ";
            SQLcmd += "and rpt_store = '" + store + "'";
            MySqlCommand cmd = new MySqlCommand(SQLcmd, conn);
            MySqlDataReader myData = cmd.ExecuteReader();

            if (myData.Read())
                ret_value = myData[0].ToString();

            myData.Close();
            cmd.Dispose();

            conn.Dispose();
            conn.Close();

            return ret_value;
        }

        // 將資料顯示於dataGridView
        private void show_data()
        {
            dataGridView1.Columns[0].HeaderText = maskedTextBox1.Text.Substring(4, 5);

            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();

            string SQLcmd = "select * from ranking order by rk_ach_rate desc";
            MySqlCommand cmd = new MySqlCommand(SQLcmd, conn);
            MySqlDataReader myData = cmd.ExecuteReader();

            dataGridView1.Rows.Clear();

            int i = 0;

            while (myData.Read())
            {
                dataGridView1.Rows.Add();
                dataGridView1.Rows[i].Cells[0].Value = myData[0].ToString();        // 藥局名稱
                dataGridView1.Rows[i].Cells[1].Value = myData[1].ToString();        // 當日淨利
                dataGridView1.Rows[i].Cells[2].Value = myData[2].ToString() + "%";  // 毛利率
                dataGridView1.Rows[i].Cells[3].Value = myData[3].ToString();        // 總人次
                dataGridView1.Rows[i].Cells[4].Value = myData[4].ToString();        // 營業目標
                dataGridView1.Rows[i].Cells[5].Value = myData[5].ToString();        // 月毛利
                dataGridView1.Rows[i].Cells[6].Value = myData[6].ToString() + "%";  // 月達成率
                dataGridView1.Rows[i].Cells[7].Value = (i + 1).ToString();            // 名次

                i++;
            }

            if (i > 1)
            {
                string day_profit_sum = get_day_profit_sum();
                string total_num = get_total_num();
                string total_goal = get_total_goal();
                string total_profit = get_total_profit();
                if (total_profit == "")
                    total_profit = "1";
                decimal total_ranking = (decimal.Parse(total_profit) / decimal.Parse(total_goal)) * 100;
                total_ranking = decimal.Round(total_ranking, 2);
                string ranking = total_ranking.ToString() + "%";

                dataGridView1.Rows.Add();
                dataGridView1.Rows[i].Cells["Column1"].Value = "合計";              // 藥局名稱
                dataGridView1.Rows[i].Cells["Column2"].Value = day_profit_sum;      // 當日淨利
                dataGridView1.Rows[i].Cells["Column3"].Value = "";                  // 毛利率
                dataGridView1.Rows[i].Cells["Column4"].Value = "";                  // 總人次
                dataGridView1.Rows[i].Cells["Column5"].Value = total_goal;          // 營業目標
                dataGridView1.Rows[i].Cells["Column6"].Value = total_profit;        // 月毛利
                dataGridView1.Rows[i].Cells["Column7"].Value = ranking;             // 月達成率
                dataGridView1.Rows[i].Cells["Column8"].Value = "";                  // 名次

                dataGridView1.CurrentCell = dataGridView1.Rows[i].Cells["Column8"]; // 游標位置
            }

            myData.Close();
            cmd.Dispose();

            conn.Close();
            conn.Dispose();

            toolStripStatusLabel1.Text = "";

            button2.Focus();
        }

        // 當日毛利加總
        private string get_day_profit_sum()
        {
            string ret_value = "";

            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();

            string SQLcmd = "select sum(rk_profit) from ranking";
            MySqlCommand cmd = new MySqlCommand(SQLcmd, conn);
            MySqlDataReader myData = cmd.ExecuteReader();

            if (myData.Read())
                ret_value = myData[0].ToString();

            myData.Close();
            cmd.Dispose();

            conn.Dispose();
            conn.Close();

            return ret_value;
        }

        // 總人次加總
        private string get_total_num()
        {
            string ret_value = "";

            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();

            string SQLcmd = "select sum(rk_num) from ranking";
            MySqlCommand cmd = new MySqlCommand(SQLcmd, conn);
            MySqlDataReader myData = cmd.ExecuteReader();

            if (myData.Read())
                ret_value = myData[0].ToString();

            myData.Close();
            cmd.Dispose();

            conn.Dispose();
            conn.Close();

            return ret_value;
        }

        // 營業目標加總
        private string get_total_goal()
        {
            string ret_value = "";

            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();

            string SQLcmd = "select sum(rk_goal) from ranking";
            MySqlCommand cmd = new MySqlCommand(SQLcmd, conn);
            MySqlDataReader myData = cmd.ExecuteReader();

            if (myData.Read())
                ret_value = myData[0].ToString();

            myData.Close();
            cmd.Dispose();

            conn.Dispose();
            conn.Close();

            return ret_value;
        }

        // 月毛利加總
        private string get_total_profit()
        {
            string ret_value = "";

            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();

            string SQLcmd = "select sum(rk_up_to_month) from ranking";
            MySqlCommand cmd = new MySqlCommand(SQLcmd, conn);
            MySqlDataReader myData = cmd.ExecuteReader();

            if (myData.Read())
                ret_value = myData[0].ToString();

            myData.Close();
            cmd.Dispose();

            conn.Dispose();
            conn.Close();

            return ret_value;
        }

        private void maskedTextBox1_TextChanged(object sender, EventArgs e)
        {
            //if (maskedTextBox1.Text.Length == 9)
            //    Query();
        }
    }
}
