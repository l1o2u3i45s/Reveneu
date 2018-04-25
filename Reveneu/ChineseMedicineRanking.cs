using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reveneu
{
    public partial class ChineseMedicineRanking : Form
    {
        string connStr = Reveneu.connStr;
        public static List<string> shopname = new List<string>();
        public static List<string> personcount = new List<string>();
        public ChineseMedicineRanking()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Query();
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
            GetCount();
        }

        private void delete_old_data()
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();

            string SQL = "DELETE FROM pres_ranking ";
            SQL += "WHERE pres_store like '%'";

            MySqlCommand cmd = new MySqlCommand(SQL, conn);
            cmd.ExecuteNonQuery();
            cmd.Dispose();

            conn.Dispose();
            conn.Close();
        }

        private void GetCount()
        {
            string yy = maskedTextBox1.Text.Substring(0, 3);
            string mm = maskedTextBox1.Text.Substring(4, 2);
            string dd = maskedTextBox1.Text.Substring(7, 2);

            if (dd == "01")
            {
                if (mm != "01")
                    mm = (int.Parse(mm) - 1).ToString();
                else
                    yy = (int.Parse(yy) - 1).ToString();
            }
            else
            {
                dd = (int.Parse(dd) - 1).ToString();
            }
            string store = string.Empty;
            string sql = @"select rpt_store,rpt_profit
                            from rx_results.TraditionalChineseMedicine
                            where rpt_date = '"
                + maskedTextBox1.Text.Substring(0, 3) + maskedTextBox1.Text.Substring(4, 2) + maskedTextBox1.Text.Substring(7, 2) + "'";
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();

            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader myData = cmd.ExecuteReader();

            while (myData.Read())
            {
               
                shopname.Add(myData[0].ToString());
                personcount.Add(myData[1].ToString());
            }
        }

        private void extract_data()
        {
            string yy = maskedTextBox1.Text.Substring(0, 3);
            string mm = maskedTextBox1.Text.Substring(4, 2);
            string dd = maskedTextBox1.Text.Substring(7, 2);

            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();

            string SQLcmd = "select * from TraditionalChineseMedicine where rpt_date = '" + yy + mm + dd + "'";
            MySqlCommand cmd = new MySqlCommand(SQLcmd, conn);
            MySqlDataReader myData = cmd.ExecuteReader();
            string vendor_code = string.Empty;
            while (myData.Read())
            {
               
                MySqlConnection iconn = new MySqlConnection(connStr);
                iconn.Open();
                vendor_code = string.Empty;
                switch (myData[1].ToString())
                {
                    case "佑昌":
                        vendor_code = "A000001";
                        break;

                    case "明昌":
                        vendor_code = "A000002";
                        break;

                    case "佑東":
                        vendor_code = "A000137";
                        break;

                    case "宏昌":
                        vendor_code = "A000359";
                        break;

                    case "杏昌":
                        vendor_code = "A000417";
                        break;

                    case "和昌":
                        vendor_code = "A000128";
                        break;
                }
                if (vendor_code == string.Empty) continue;
                decimal prof = decimal.Parse(myData[2].ToString());                 // 當日營收
             
                decimal goal = get_target(vendor_code);                    // 目標

                decimal rate = (prof / goal) * 100;                                 // 當日達成率
                rate = decimal.Round(rate, 2);

                decimal days = get_days(vendor_code);                      // 當月天數

                decimal bus_target = goal * days;                                   // 營業目標

                decimal monthly_profit = get_up_to_day_profit(myData[1].ToString());// 到當日月營收

                decimal ach_rate = (monthly_profit / bus_target) * 100;
                ach_rate = decimal.Round(ach_rate, 2);
                
                string iSQL = "INSERT INTO pres_ranking VALUES ('";
                iSQL += myData[1].ToString() + "', '";                         // 藥局名稱
                iSQL += prof.ToString() + "', '";               // 當日營收
                iSQL += goal.ToString() + "', '";               // 目標
                iSQL += rate.ToString() + "', '";               // 當日達成率
                iSQL += bus_target.ToString() + "', '";         // 營業目標
                iSQL += monthly_profit.ToString() + "', '";     // 月營收
                iSQL += ach_rate.ToString() + "')";             // 達成率

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

        private decimal get_target(string vendor_code)
        {
            
            decimal ret_value = 0;

            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();

            string SQLcmd = "select * from target where tg_store = '" + vendor_code + "'";
            MySqlCommand cmd = new MySqlCommand(SQLcmd, conn);
            MySqlDataReader myData = cmd.ExecuteReader();

            if (myData.Read())
                ret_value = decimal.Parse(myData[2].ToString());

            myData.Close();
            cmd.Dispose();

            conn.Dispose();
            conn.Close();

            return ret_value;
        }

        private decimal get_days(string vendor_code)
        {
            decimal ret_value = 0;

            string str_date = maskedTextBox1.Text.Replace("/", "");
            string yy = maskedTextBox1.Text.Substring(0, 3);
            string mm = maskedTextBox1.Text.Substring(4, 2);

            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();

            string SQLcmd = "select * from presc where pre_store = '" + vendor_code + "' ";
            SQLcmd += "and pre_date like '" + yy + mm + "%'";

            MySqlCommand cmd = new MySqlCommand(SQLcmd, conn);
            MySqlDataReader myData = cmd.ExecuteReader();

            while (myData.Read())
            {
                string curr_date = myData[0].ToString();
                if (string.Compare(curr_date, str_date) <= 0)
                    ret_value = ret_value + 1;
            }

            myData.Close();
            cmd.Dispose();

            conn.Dispose();
            conn.Close();

            return ret_value;
        }

        private decimal get_up_to_day_profit(string vendor_code)
        {
            decimal ret_value = 0;

            string str_date = maskedTextBox1.Text.Replace("/", "");
            string yy = maskedTextBox1.Text.Substring(0, 3);
            string mm = maskedTextBox1.Text.Substring(4, 2);

            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();

            string SQLcmd = "select * from TraditionalChineseMedicine where rpt_store = '" + vendor_code + "' ";
            SQLcmd += "and rpt_date like '" + yy + mm + "%'";

            MySqlCommand cmd = new MySqlCommand(SQLcmd, conn);
            MySqlDataReader myData = cmd.ExecuteReader();

            while (myData.Read())
            {
                string curr_date = myData[0].ToString();
                decimal dec = decimal.Parse(myData[4].ToString());
                if (string.Compare(curr_date, str_date) <= 0)
                    ret_value +=  dec;
            }

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
            List<double> rate = new List<double>();
            string SQLcmd = @"select  a.pres_store,a.pres_profit,SUM(b.rpt_revenue),a.pres_up_to_month,a.pres_ach_rate
                                from rx_results.pres_ranking a,
                                rx_results.TraditionalChineseMedicine b
                                where a.pres_store = b.rpt_store
                                GROUP BY  a.pres_store,a.pres_profit,a.pres_up_to_month,a.pres_ach_rate
                                order by a.pres_up_to_month desc";
            MySqlCommand cmd = new MySqlCommand(SQLcmd, conn);
            MySqlDataReader myData = cmd.ExecuteReader();

            dataGridView1.Rows.Clear();
            double _c1 = 0; double _c2 = 0; double _c3 = 0; double _c4 = 0;
            int i = 0;
            while (myData.Read())
            {

                dataGridView1.Rows.Add();
                dataGridView1.Rows[i].Cells[0].Value = myData[0].ToString();        // 藥局名稱
                dataGridView1.Rows[i].Cells[1].Value = myData[1].ToString();    // 當日營收
                for (int k = 0; k <= shopname.Count - 1; k++)
                {
                    if (shopname[k] == dataGridView1.Rows[i].Cells[0].Value.ToString())
                    {
                        dataGridView1.Rows[i].Cells[2].Value = personcount[k]; //當日毛利
                    }
                       
                }

                dataGridView1.Rows[i].Cells[3].Value = myData[2].ToString();  // 月營收
                dataGridView1.Rows[i].Cells[4].Value = myData[3].ToString();        // 月毛利
                                                                                    //dataGridView1.Rows[i].Cells[5].Value = (i + 1).ToString();          // 名次


                _c1 += double.Parse(myData[1].ToString());
                _c2 += double.Parse(dataGridView1.Rows[i].Cells[2].Value.ToString());
                _c3 += double.Parse(myData[2].ToString());
                _c4 += double.Parse(myData[3].ToString());
                i++;
            }
            dataGridView1.Rows.Add();
            dataGridView1.Rows[i].Cells[0].Value = "合計";
            dataGridView1.Rows[i].Cells[1].Value = _c1;
            dataGridView1.Rows[i].Cells[2].Value = _c2;
            dataGridView1.Rows[i].Cells[3].Value = _c3;
            dataGridView1.Rows[i].Cells[4].Value = _c4;
            dataGridView1.CurrentCell = dataGridView1.Rows[i].Cells[4];
            myData.Close();
            cmd.Dispose();

            conn.Close();
            conn.Dispose();


            toolStripStatusLabel1.Text = "";

            button2.Focus();
        }

        // 當日毛利總計
        private string get_day_profit_sum()
        {
            string ret_value = "";

            string yy = maskedTextBox1.Text.Substring(0, 3);
            string mm = maskedTextBox1.Text.Substring(4, 2);
            string dd = maskedTextBox1.Text.Substring(7, 2);

            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();

            string SQLcmd = "select sum(rpt_profit) ";
            SQLcmd += "from TraditionalChineseMedicine where rpt_date like '" + yy + mm + dd + "%'";

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

        // 目標總計
        private string get_total_goal()
        {
            string ret_value = "";

            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();

            string SQLcmd = "select sum(tg_num) from target";
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

        private void maskedTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Query();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

        private void Pres_Ranking_Load(object sender, EventArgs e)
        {
            string yy = (System.DateTime.Now.Year - 1911).ToString();
            string mm = System.DateTime.Now.Month.ToString();
            if (System.DateTime.Now.Month < 10)
                mm = "0" + mm;
            string dd = System.DateTime.Now.Day.ToString();
            if (System.DateTime.Now.Day < 10)
                dd = "0" + dd;

            maskedTextBox1.Text = yy + mm + dd;

            dataGridView1.Columns[0].HeaderText = maskedTextBox1.Text.Substring(4, 5);

            toolStripStatusLabel1.Text = "";
        }
    }
}
