using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class WriteOffEntity
    {
        /// <summary>
        /// 核销码
        /// </summary>
        public CheckOffCodeEn CheckOffCode { get; set; }
        /// <summary>
        /// 核销信息
        /// </summary>
        public string CheckOffInformation { get; set; }

    }
}
