using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaobaoCrawler.Modules;
using System.ComponentModel;
using System.Threading;
using Crawler.Engine.Utils;

namespace TaobaoCrawler
{
    /// <summary>
    /// 多线程采集引擎
    /// </summary>
    class CrawlerEngineMulti : ICrawlerEngine<BaseCommandContext<IRequestToken>,IUserRequestInfo,IRequestToken>,
        IAsyCrawlerEngine<BaseCommandContext<IRequestToken>, IUserRequestInfo, IRequestToken>
    {
        private static object _synLock = new object();

        private bool _isStart = false;

        private bool _isBusy = false;

        private int _loopTimes = 5;

        /// <summary>
        /// MainLoop主线程
        /// </summary>
        private BackgroundWorker _mainWork =
            new BackgroundWorker();

        /// <summary>
        /// 后台线程集合
        /// </summary>
        private Dictionary<IRequestToken, BackgroundWorker> _works
            = new Dictionary<IRequestToken, BackgroundWorker>();

        /// <summary>
        /// 请求队列
        /// </summary>
        private Queue<Tuple<BaseCommandContext<IRequestToken>, IUserRequestInfo>> _requestQueue =
            new Queue<Tuple<BaseCommandContext<IRequestToken>, IUserRequestInfo>>();

        private object _synWorkLock = new object();

        private CrawlerEngineMulti() { }

        private static CrawlerEngineMulti _engine = null;
  
        public  static CrawlerEngineMulti  Hinstance
        {
            get
            {
                if(_engine == null)
                {
                    lock(_synLock)
                    {
                        if(_engine == null)
                        {
                            _engine = new CrawlerEngineMulti(); 
                        }
                    }
                }

                return _engine;
            }
        }

        /// <summary>
        /// 客户向引擎发送服务请求,该请求异步另起一根线程完成
        /// </summary>
        /// <param name="context"></param>
        /// <param name="request"></param>
        public virtual IRequestToken OnWork(BaseCommandContext<IRequestToken> context, IUserRequestInfo request)
        {
            long id = IdManager.Hinstance.New(IdType.USER_REQUEST_ID);
            context.Token = new RequestToken(id);
            //开始排队等候
            _requestQueue.Enqueue(new Tuple<BaseCommandContext<IRequestToken>, IUserRequestInfo>(context, request));
            return context.Token;
        }

        /// <summary>
        /// 客户需要停止服务请求
        /// </summary>
        /// <param name="context"></param>
        public virtual void OnStop(IRequestToken context)
        {

        }

        /// <summary>
        /// 引擎启动时加载配置和启动全局设置
        /// </summary>
        /// <param name="arg"></param>
        public virtual void OnSetup(SetupConfigArg arg)
        {
            if(!_isStart)
            {
                //启动模块组
                Modules.Modules.CRAWLERModule.Init();

                //启动用户监听用户请求线程
                _mainWork.DoWork    +=MainWork_DoWork;
                _mainWork.RunWorkerCompleted +=MainWork_RunWorkerCompleted;
                _mainWork.WorkerSupportsCancellation = true;
                _mainWork.RunWorkerAsync();
                _isStart = true; _isBusy = true;
            }
        }

        /// <summary>
        /// 关闭引擎
        /// </summary>
        public virtual void OnClose()
        {
            //TODO 这种强制取消讲影响现有子线程
            _mainWork.CancelAsync();
        }

        #region 主后台线程处理事件
        private void MainWork_DoWork(object sender, DoWorkEventArgs e)
        {
            OnMainLoop();
        }

        private void MainWork_RunWorkerCompleted(object sender,RunWorkerCompletedEventArgs e)
        {
            _isBusy = false;
        }
        #endregion

        #region 用户请求线程处理事件
        private void UserRequestWork_DoWork(object sender, DoWorkEventArgs e)
        {
            Tuple<ICommand<BaseCommandContext<IRequestToken>, IUserRequestInfo>,
                            BaseCommandContext<IRequestToken>, IUserRequestInfo> pair = e.Argument as
                Tuple<ICommand<BaseCommandContext<IRequestToken>, IUserRequestInfo>,
                            BaseCommandContext<IRequestToken>, IUserRequestInfo>;

            if(pair != null)
            {
                ICommand<BaseCommandContext<IRequestToken>, IUserRequestInfo> command = pair.Item1;
                BaseCommandContext<IRequestToken> commandContext = pair.Item2;
                IUserRequestInfo userRequest = pair.Item3;
                if(command != null)
                {
                    if(commandContext != null &&userRequest != null)
                    {
                        command.ExecuteEntityCommand(commandContext, userRequest);
                        e.Result = commandContext.Token;
                    }
                }
            }
        }

        private void UserRequestWork_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            RequestToken token = e.Result as RequestToken;
            if(token != null)
            {
                lock(_synWorkLock)
                {
                    _works.Remove(token);
                }
            }
        }
        #endregion

        /// <summary>
        /// 处理用户请求,分送线程处理
        /// </summary>
        public virtual void OnMainLoop()
        {
            ConsoleExt.WriteLine("主线程启动...");

            IEnumerable<ICommand<BaseCommandContext<IRequestToken>, IUserRequestInfo>> commands = GetInitCommands();

            while(true)
            {
                Thread.Sleep(_loopTimes * 1000);
                if(_requestQueue.Count>0)
                {
                    Tuple<BaseCommandContext<IRequestToken>, IUserRequestInfo> pair = _requestQueue.Peek();

                    List<ICommand<BaseCommandContext<IRequestToken>, IUserRequestInfo>> lstCommands = 
                        commands.ToList<ICommand<BaseCommandContext<IRequestToken>, IUserRequestInfo>>();
                    foreach (ICommand<BaseCommandContext<IRequestToken>, IUserRequestInfo> command in commands)
                    {
                        if (command.Name == pair.Item1.RequestId)
                        {
                            _requestQueue.Dequeue();
                            ConsoleExt.WriteLine("启动抓取线程 Token: {0}...",command.Name);
                            //另外起一根线程去处理
                            BackgroundWorker worker = new BackgroundWorker();
                            worker.DoWork += UserRequestWork_DoWork;
                            worker.RunWorkerCompleted += UserRequestWork_RunWorkerCompleted;
                            worker.WorkerSupportsCancellation = true;
                            worker.RunWorkerAsync(new Tuple<ICommand<BaseCommandContext<IRequestToken>, IUserRequestInfo>,
                            BaseCommandContext<IRequestToken>, IUserRequestInfo>(command, pair.Item1, pair.Item2));

                            lock(_synWorkLock)
                            {
                                _works.Add(pair.Item1.Token,worker);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 异步请求服务
        /// </summary>
        /// <param name="context"></param>
        /// <param name="request"></param>
        /// <param name="callback"></param>
        public virtual IRequestToken BeginOnWork(BaseCommandContext<IRequestToken> context, IUserRequestInfo request, Action<BaseCommandContext<IRequestToken>> callback)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg"></param>
        public virtual void EndOnWork(BaseAsyEngineArg arg)
        {

        }

        protected IEnumerable<ICommand<BaseCommandContext<IRequestToken>, IUserRequestInfo>> GetInitCommands()
        {
            IEnumerable<ICommand> commands = Modules.Modules.CRAWLERModule.GetInitCommands();
            List<ICommand>  lstCmds = commands.ToList<ICommand>();
            ICommand<BaseCommandContext<IRequestToken>, IUserRequestInfo> cmd =
                lstCmds[0] as ICommand<BaseCommandContext<IRequestToken>, IUserRequestInfo>;
            IEnumerable<ICommand<BaseCommandContext<IRequestToken>, IUserRequestInfo>> cmds =
                commands.OfType<ICommand<BaseCommandContext<IRequestToken>, IUserRequestInfo>>();
            return cmds;
        }

        #region 事件
        public event EventHandler<OnSetupEventArgs> Setup;

        public  void    RaiseSetup(OnSetupEventArgs args)
        {
            if(Setup != null)
            {
                Setup.Invoke(this,args);
            }
        }

        public event EventHandler<OnWorkEventArgs> Work;

        public void RaiseWork(OnWorkEventArgs args)
        {
            if(Work != null)
            {
                Work.Invoke(this,args);
            }
        }

        public event EventHandler<OnStopEventArgs> Stop;

        public void RaiseStop(OnStopEventArgs args)
        {
            if(Stop != null)
            {
                Stop.Invoke(this,args);
            }
        }
        #endregion

        #region 属性
        /// <summary>
        /// 是否忙
        /// </summary>
        public bool IsBusy {
            get
            {
                return WorkingTheads > 0 || _isBusy;
            }
        }

        /// <summary>
        /// 正在工作线程数
        /// </summary>
        public int WorkingTheads 
        {
            get
            {
                int workCount = 0;
                lock(_synWorkLock)
                {
                    workCount = _works.Count;
                }
                return workCount;
            }
        }

        #endregion
    }

    public class OnSetupEventArgs : EventArgs
    {
        public OnSetupEventArgs(SetupConfigArg config)
        {
            Config = config;
        }

        public SetupConfigArg Config
        {
            get;
            private set;
        }
    }

    public class OnWorkEventArgs : EventArgs
    {
        public OnWorkEventArgs(BaseCommandContext<IRequestToken> context, IUserRequestInfo req)
        {
            Context = context;
            UserRequest = req;
        }

        public BaseCommandContext<IRequestToken> Context
        {
            private set;
            get;
        }

        public IUserRequestInfo UserRequest
        {
            private set;
            get;
        }
    }

    public class OnStopEventArgs : EventArgs
    {

    }
}
