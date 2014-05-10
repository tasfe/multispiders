using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaobaoCrawler
{
    public interface ICommandContext<TToken>
        where TToken:IRequestToken
    {
        TToken Token { set; get; }//目前请求标识和命令标识是一样的

        /// <summary>
        /// 请求标识
        /// </summary>
        string RequestId { get;}
    }
}
