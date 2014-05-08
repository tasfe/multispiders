using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace taobaocrawler.Modules
{
    /// <summary>
    /// 各模块快捷访问入口帮助类
    /// </summary>
    public static partial class Modules
    {
        /// <summary>
        /// 快捷访问入口
        /// </summary>
        public static readonly CrawlerModule Crawler = new CrawlerModule();
    }

    /// <summary>
    /// 所有模块的顶级模块
    /// 描述:线程的调度,总模块的启动等
    /// </summary>
    public class CrawlerModule:BaseModule
    {
        public override string ModuleName
        {
            get{return "CrawlerModule";}
        }

        protected override void RegisterMySubModules()
        {

        }

        protected override void RegisterMyEvents()
        {

        }

        protected override void RegisterMyCommands()
        {

        }
    }
}
