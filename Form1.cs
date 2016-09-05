using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace WindowsFormsApplication32
{
    public partial class Form1 : Form
    {
        SqlConnection conn; //连接对象
        DataSet ds;         //结果存储容器
        SqlDataAdapter sda; //执行命令对象
        SqlCommand cmd;     //命令
        #region 略略略略略
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.Hide();
            button2.Hide();
            label1.Text = "输入要查询的编码（股票码、公司码、天相码或券码）";
            string str = "server=txic_kernel;database=txstock;uid=tx_reader;pwd=123456";
            try
            {
                conn = new SqlConnection(str);
                richTextBox1.Text = "数据库已连接成功！\n";
            }
            catch (Exception ex)
            {
                richTextBox1.Text = ex.Message;
            }
            textBox1.Text = "输入要查询的编码，请输入纯数字码";
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)//检索按钮
        {
            //button2_Click(sender, e);
            dataGridView1.DataSource = null;
            string Code = textBox1.Text.Trim();
            Boolean wow = check(Code);//不包含两端的空格
            if (wow)
            {
                richTextBox1.Text += "\n以下是包含该编码的查询结果";
                TrySearch(Code);
            }
            else
                richTextBox1.Text += "\n输入有误";
            button2.Show();
        }

        private void button2_Click(object sender, EventArgs e)//清空结果
        {
            richTextBox1.Text = "";
            textBox1.Text = "输入要查询的编码，请输入纯数字码";
            button2.Hide();
            dataGridView1.Hide();
        }
        private Boolean check(string InputStr)
        {
            int length = InputStr.Length;
            for (int i = 0; i < length; i++)
            {
                if (57 < InputStr[i] || InputStr[i] < 48)
                    return false;
            }
            return true;
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                button1_Click(sender, e);
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                button2_Click(sender, e);
        }
        #endregion
        private void search(string Code)
        {
            String Wtf=
                "declare @Code int set @Code="+Code.ToString()+ " select d.f_institution_id "
                +" as '公司编码',f.f_chn_short_name as '公司名称',e.f_market_code as '股票编码',e.f_chn_short_name as '股票名称',b.f_security_id as '股票券码'"
                + ",'交易状态'=(case b.f_status when 0 then '正常' when 1 then '发行未上市' when 2 then '暂停上市' when 3 then '摘牌' when 4 then '发行失败' else 'null' end)"
                + ",a.f_trans_obj_id as '天相交易实体码',a.f_close as '收盘价',a.f_trade_date as '日期'"
                + " from txstock.dbo.t_stock_price as a inner join txstock.dbo.t_stock_trans_obj as b on a.f_data_state>0 and b.f_data_state>0 and a.f_trans_obj_id=b.f_trans_obj_id"
                + " and a.f_trade_date=(select MAX(t.f_trade_date) from txstock.dbo.t_stock_price as t where t.f_trans_obj_id=a.f_trans_obj_id)"
                + " and a.f_trade_date between ISNULL(b.f_start_date,19491001) and ISNULL(b.f_end_date,29991231) left join txstock.dbo.t_common_stock as c"
                + " on c.f_data_state>0 and c.f_security_id=b.f_security_id left join txstock.dbo.t_corp_basic as d on d.f_data_state>0 and d.f_institution_id=c.f_institution_id"
                + " inner join txpublic.dbo.t_trans_obj as e on e.f_data_state>0 and e.f_trans_obj_id= a.f_trans_obj_id and a.f_trade_date between ISNULL(b.f_start_date,19491001) and ISNULL(b.f_end_date,29991231)"
                + " left join txstock.dbo.t_inst_rename  as f on f.f_data_state>0and f.f_institution_id=d.f_institution_id"
                + " and a.f_trade_date between ISNULL(f.f_start_date,19491001) and ISNULL(f.f_end_date,29991231)"
                + " where d.f_institution_id=@Code or e.f_market_code=@Code or b.f_security_id=@Code or a.f_trans_obj_id=@Code order by e.f_market_code"
            ;
            cmd = new SqlCommand(Wtf,conn);
            sda = new SqlDataAdapter();
            sda.SelectCommand = cmd;
            ds = new DataSet();
            sda.Fill(ds,"cs");
            dataGridView1.DataSource = ds.Tables[0];
        }

        private void TrySearch(string Code)
        {
            try
            {
                search(Code);
                dataGridView1.Show();
            }
            catch(Exception ex)
            {
                richTextBox1.Text += ex.Message;
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
