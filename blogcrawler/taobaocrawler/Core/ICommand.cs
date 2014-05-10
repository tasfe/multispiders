using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaobaoCrawler
{
    public interface ICommand<TCommandContext,TUserRequestInfo> :ICommand
        where TCommandContext:ICommandContext<IRequestToken>
        where TUserRequestInfo:IUserRequestInfo
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context">命令的上下文信息</param>
        /// <param name="requestInfo">用户的请求数据</param>
        void ExecuteEntityCommand(TCommandContext context,TUserRequestInfo requestInfo);
    }

    public interface ICommand
    {
        string Name { get; }
    }
}
