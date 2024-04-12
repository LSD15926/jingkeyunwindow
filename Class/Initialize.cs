using jingkeyun.Data;
using Pdd_Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jingkeyun.Class
{
    public static class InitUser
    {

        public static LoginUser User { get; set; } = new LoginUser();

        /// <summary>
        /// 已授权店铺
        /// </summary>
        public static List<Mallinfo> All_mall { get; set; } = new List<Mallinfo>();

        public static List<Mallinfo> Choose_mall { get; set; } = new List<Mallinfo>();
    }
}
