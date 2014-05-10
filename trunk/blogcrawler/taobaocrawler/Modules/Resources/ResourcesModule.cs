using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaobaoCrawler.Modules
{
    using TaobaoCrawler.Modules.Resources;

    /// <summary>
    /// 各模块快捷访问入口帮助类
    /// </summary>
    public static partial class Modules
    {
        /// <summary>
        /// 快捷访问入口
        /// </summary>
        public static readonly ResourcesModule RESOURCESModule = new ResourcesModule();
    }
}

namespace TaobaoCrawler.Modules.Resources
{
    public class ResourcesModule:BaseModule
    {
        public override string ModuleName
        {
            get { return "ResourcesModule"; }
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
