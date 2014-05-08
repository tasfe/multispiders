using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace taobaocrawler
{
    /// <summary>
    /// 抓取接口
    /// 注意:每一个抓取模块都需实现该接口
    /// </summary>
    interface ICrawlable
    {
        /// <summary>
        /// 工作接口,此方法要被分派到不同的线程去执行
        /// <param name="request_id">请求编号</param>
        /// </summary>
        void Work(long request_id);
    }
}
