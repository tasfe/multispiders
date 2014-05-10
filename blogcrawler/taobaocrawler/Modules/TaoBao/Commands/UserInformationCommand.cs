using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaobaoCrawler;
using Crawler.Engine.Utils;
using System.Threading;

namespace TaobaoCrawler.Modules.TaoBao
{
    public class UserInformationCommand : ICommand<BaseCommandContext<IRequestToken>, UserInformationCmdArg>
    {
        public void ExecuteEntityCommand(BaseCommandContext<IRequestToken> context, UserInformationCmdArg requestInfo)
        {
            ConsoleExt.WriteLine("抓取淘宝数据开始...");

            int i = 30;
            //Test
            while(i-- != 0)
            {
                Thread.Sleep(5 * 1000); ConsoleExt.WriteLine("进度{0}",i);
            }
        }

        public string Name { 
            get {
                //编号前四位为模块编号,后四位为模块内命令编号
                return 0x10010001.ToString();
            } 
        }
    }
}
