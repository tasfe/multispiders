using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using TaobaoCrawler.Modules.TaoBao;
using Crawler.Engine.Utils;

namespace TaobaoCrawler
{
    class Program
    {
        //TODO 暂时先用Loop来接受用户数据
        static void Main(string[] args)
        {
            ConsoleExt.WriteLine("引擎启动...");

            CrawlerEngineMulti.Hinstance.OnSetup(new SetupConfigArg());//引擎启动

            //根据配置读取要采集的请求
            List<string> urls = new List<string>();
            urls.Add("http://shop70731012.taobao.com");
            //urls.Add("http://shop104138582.taobao.com");
            //urls.Add("http://xwygg.taobao.com");

            //开始采集
            foreach(string url in urls)
            {
                ConsoleExt.WriteLine("采集{0}开始...",url);
            }

            foreach(string url in urls)
            {
                CrawlerEngineMulti.Hinstance.OnWork(
                    new CommandContext(0x10010001.ToString()), new UserInformationCmdArg() { ShopUrl=url});
            }

            ConsoleExt.WriteLine("采集中,请等待采集结束...");

            //主线程等待
            while (CrawlerEngineMulti.Hinstance.IsBusy)
            {
                Thread.Sleep(10*1000);                
            }
        }
    }
}
