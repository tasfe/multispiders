using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaobaoCrawler.Modules
{
    using TaobaoCrawler.Modules.Crawler;

    /// <summary>
    /// 各模块快捷访问入口帮助类
    /// </summary>
    public static partial class Modules
    {
        /// <summary>
        /// 快捷访问入口
        /// </summary>
        public static readonly CrawlerModule CRAWLERModule = new CrawlerModule();
    }
}

namespace TaobaoCrawler.Modules.Crawler
{
    /// <summary>
    /// 所有模块的顶级模块
    /// 描述:总模块的调度等
    /// </summary>
    public class CrawlerModule:BaseModule
    {
        public override string ModuleName
        {
            get{return "CrawlerModule";}
        }

        protected override void RegisterMySubModules()
        {
            RegisterSubModule(Modules.RESOURCESModule);
            RegisterSubModule(Modules.TAOBAOModule);
        }

        protected override void RegisterMyEvents()
        {

        }

        protected override void RegisterMyCommands()
        {

        }
    }
}
