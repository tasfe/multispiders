using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaobaoCrawler
{
    /// <summary>
    /// 抓取引擎同步框架
    /// </summary>
    /// <typeparam name="TCommandContext"></typeparam>
    /// <typeparam name="TUserRequestInfo"></typeparam>
    public interface ICrawlerEngine<TCommandContext,TUserRequestInfo,TToken>
        where TCommandContext:ICommandContext<TToken>
        where TUserRequestInfo:IUserRequestInfo
        where TToken : IRequestToken
    {
        /// <summary>
        /// 客户向引擎发送服务请求,该请求异步另起一根线程完成
        /// </summary>
        /// <param name="context"></param>
        /// <param name="request"></param>
        TToken OnWork(TCommandContext context,TUserRequestInfo request);

        /// <summary>
        /// 客户需要停止服务请求
        /// </summary>
        /// <param name="context"></param>
        void OnStop(TToken token);

        /// <summary>
        /// 引擎启动时加载配置和启动全局设置
        /// </summary>
        /// <param name="arg"></param>
        void OnSetup(SetupConfigArg arg);

        /// <summary>
        /// 关闭引擎
        /// </summary>
        void OnClose();

        /// <summary>
        /// 处理用户请求,分送线程处理
        /// </summary>
        void OnMainLoop();

        #region 属性
        /// <summary>
        /// 是否忙
        /// </summary>
        bool IsBusy { get; }

        /// <summary>
        /// 正在工作线程数
        /// </summary>
        int WorkingTheads { get; }
        #endregion
    }
}
