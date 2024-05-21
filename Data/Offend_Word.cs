using Newtonsoft.Json;
using Pdd_Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jingkeyun;
using jingkeyun.Class;
using System.Data;
using System.Diagnostics;
using Sunny.UI;

namespace jingkeyun.Data
{
    public class Offend_Word
    {
        public static BackData get()
        {
            BackData backData = new BackData();
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>(); //参数列表
                object data = apiHelp.postApi(ConfigurationManager.AppSettings["OffendList"].ToString(), parameters);
                backData = JsonConvert.DeserializeObject<BackData>(data.ToString());
            }
            catch (Exception ex)
            {
                backData.Code = 2001;
                backData.Mess = ex.Message.ToString();
            }
            return backData;
        }

        public static List<OffenWord> GetAll( string sql="")
        {
    
            List<OffenWord> Offends=new List<OffenWord>();
            try
            {
                string strSql = "select * from Offend where 1=1 "+sql;
                DataSet ds = SQLiteHelper.ExecuteQuery(strSql);
                DataTable dt = ds.Tables[0];//数据列表
                foreach (DataRow dr in dt.Rows)
                {
                    OffenWord offenWord = new OffenWord();
                    offenWord.word=dr["word"].ToString();
                    offenWord.id = MyConvert.ToInt(dr["id"]);
                    Offends.Add(offenWord);
                }
            }
            catch (Exception ex)
            {

            }
            
            return Offends;
        }
    }
    public class OffenWord
    {
        public int id {  get; set; }
        public string word { get; set; }
    }
}
