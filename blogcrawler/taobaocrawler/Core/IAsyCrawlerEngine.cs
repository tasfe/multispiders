using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaobaoCrawler
{
    /// <summary>
    /// 抓取引擎异步框架
    /// </summary>
    /// <typeparam name="TCommandContext"></typeparam>
    /// <typeparam name="TUserRequestInfo"></typeparam>
    public interface IAsyCrawlerEngine<TCommandContext, TUserRequestInfo,TToken>
        where TCommandContext : ICommandContext<TToken>
        where TUserRequestInfo : IUserRequestInfo
        where TToken:IRequestToken
    {
        TToken BeginOnWork(TCommandContext context,TUserRequestInfo request,Action<TCommandContext> callback);

        void EndOnWork(BaseAsyEngineArg arg);
    }
}
