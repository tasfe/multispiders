using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaobaoCrawler.Modules
{
    using TaobaoCrawler.Modules.TaoBao;

    /// <summary>
    /// 各模块快捷访问入口帮助类
    /// </summary>
    public static partial class Modules
    {
        

        /// <summary>
        /// 快捷访问入口
        /// </summary>
        public static readonly TaoBaoModule TAOBAOModule = new TaoBaoModule();
    }
}

namespace TaobaoCrawler.Modules.TaoBao
{
    /// <summary>
    /// 淘宝采集模块
    /// </summary>
    public class TaoBaoModule:BaseModule
    {
        public override string ModuleName
        {
            get { return "TaoBaoModule"; }
        }

        protected override void RegisterMySubModules()
        {

        }

        protected override void RegisterMyEvents()
        {

        }

        protected override void RegisterMyCommands()
        {
            RegisterCommand(new UserInformationCommand());
        }
    }
}
