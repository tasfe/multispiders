using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Crawler.Engine.Utils;
using System.Threading;

namespace Crawler.Test
{
    class Program
    {
        private static Random rnd = new Random();

        static void Main(string[] args)
        {
            List<BackgroundWorker>  works = 
                new List<BackgroundWorker>();

            for (int j = 0; j < 30;j++ )
            {
                long time = rnd.Next(30,60);
                long times = rnd.Next(10,30);
                BackgroundWorker work = new BackgroundWorker();
                work.DoWork += work_DoWork1;
                work.RunWorkerCompleted += work_RunWorkerCompleted1;
                ConsoleExt.WriteLine("线程{0}启动...",j+1);
                work.RunWorkerAsync(new Argument1() { time=time,times=times,id=j+1});
                works.Add(work);
            }
            

            

            while(true)
            {
                Thread.Sleep(5*1000);
                bool    isBusy = false;
                foreach(BackgroundWorker w in works)
                {
                    isBusy = isBusy || w.IsBusy;
                }
                if(!isBusy)
                {
                    ConsoleExt.WriteLine("线程都结束了...");
                    break;
                }
            }
        }

        static void work_DoWork1(object sender, DoWorkEventArgs e)
        {
            long i = (e.Argument as Argument1).times;
            do
            {
                Thread.Sleep((int)(e.Argument as Argument1).time * 1000);
                ConsoleExt.WriteLine("线程{1}正在工作,进度{0}", i, (e.Argument as Argument1).id);
            }while(i-- != 0);
            e.Result = (e.Argument as Argument1).id;
        }

        static void work_RunWorkerCompleted1(object sender,RunWorkerCompletedEventArgs e)
        {
            ConsoleExt.WriteLine("线程{0}工作完成",Convert.ToInt32(e.Result));
        }
    }

    class Argument1
    {
        public long times { get; set; }

        public long time{get;set;}

        public int id { get; set; }
    }
}
