using APIOffice.pddApi;
using jingkeyun.Class;
using jingkeyun.Data;
using jingkeyun.Windows;
using Newtonsoft.Json;
using Pdd_Models;
using Pdd_Models.Models;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace jingkeyun.Pinduoduo
{
    public partial class OffendList : UIPage
    {
        
        public event UpdateLogDelegate UpdLog;
        Thread List = null;
        public OffendList()
        {
            InitializeComponent();
            this.列表.RowTemplate.DefaultCellStyle.Font = new Font(FontHelper.pfc.Families[0], 12);
            this.列表.ColumnHeadersDefaultCellStyle.Font = new Font(FontHelper.pfc.Families[0], 13);
            StyleHelper.SetSymbolButtonColor(uiSymbolButton1,StyleHelper.OkButton);
            StyleHelper.SetSymbolButtonColor(uiSymbolButton8, StyleHelper.OkButton);
            myPagination1.Style=UIStyle.Purple;
            
        }
        private void mailList_Load(object sender, EventArgs e)
        {
            ShowData();
        }


        private void ShowData()
        {
            this.列表.Rows.Clear();
            string sql = "";
            if (textBoxX1.Text != "")
            {
                sql = " and word like '%" + textBoxX1.Text + "%'";
            }
            List<OffenWord> Words = Offend_Word.GetAll(sql);
            myPagination1.TotalCount = Words.Count;
            for (int i = 0; i < Words.Count; i++)
            {
                列表.Rows.Add();
                列表.Rows[i].Cells["Offend"].Value = Words[i].word;
                列表.Rows[i].Cells["id"].Value = Words[i].id;
            }
            MyMessageBox.IsShowLoading=false;
            列表.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dataGridView_RowPostPaint);
        }

        private void dataGridView_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            try
            {
                DataGridView dgv = sender as DataGridView;
                Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
                                                    e.RowBounds.Location.Y,
                                                    dgv.RowHeadersWidth - 4,
                                                    e.RowBounds.Height);


                TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
                                        dgv.RowHeadersDefaultCellStyle.Font,
                                        rectangle,
                                        dgv.RowHeadersDefaultCellStyle.ForeColor,
                                        TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
            }
            catch (Exception ex)
            {

            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }
        List<string> Ids = new List<string>();
        /// <summary>
        /// 获取选中
        /// </summary>
        private int getCheck()
        {
            Ids.Clear();
            for (int i = 0; i < 列表.Rows.Count; i++)
            {
                if (Convert.ToBoolean(列表.Rows[i].Cells["check"].Value))
                {
                    Ids.Add(列表.Rows[i].Cells["id"].Value.ToString());
                }
            }
            return Ids.Count;
        }


        private void uiButton3_Click(object sender, EventArgs e)
        {
            if (getCheck() == 0)
            {
                UIMessageTip.Show("请选择要删除的违规词!");
                return;
            }
            //删除店铺
            if (MyMessageBox.ShowAsk("是否确认删除选中的违规词？"))
            {
                string strSql = $"delete from Offend where id in ({string.Join(",",Ids)})";
                if (SQLiteHelper.ExecSQL(strSql, null))
                {
                    UIMessageTip.ShowOk("删除成功！");
                }
                else
                {
                    MyMessageBox.ShowError("删除失败！");
                }
                ShowData();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < 列表.Rows.Count; i++)
            {
                列表.Rows[i].Cells["check"].Value = checkBox1.Checked;

            }
        }


        private void uiSymbolButton8_Click(object sender, EventArgs e)
        {
            ShowData();
        }

        private void uiSymbolButton4_Click(object sender, EventArgs e)
        {
            OffenAdd f= new OffenAdd();
            if (f.ShowDialog() == DialogResult.OK)
            {
                ShowData();
            }
        }

        private void uiSymbolButton5_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialog1.Filter = "工作表|*.xlsx";
                openFileDialog1.Multiselect = false;
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    DataTable dt = MyConvert.ExcelToDataTable(openFileDialog1.FileName);
                    int cnt = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        if (string.IsNullOrEmpty(row[0].ToString()))
                            continue;
                        string sql = "insert into Offend(word) values(@word)";
                        SQLiteParameter[] paras = new SQLiteParameter[]
                        {
                         new SQLiteParameter("@word",row[0])
                        };
                        int i = SQLiteHelper.ExecuteNonQuery(sql, paras);
                        if (i != -99)
                            cnt += i;
                        
                    }
                   
                    MessageBox.Show($"成功添加{cnt}条数据");
                    ShowData();
                }
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowError("导入出错！");
            }
            
        }
    }
}
