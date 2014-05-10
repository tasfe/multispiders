using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaobaoCrawler
{
    /// <summary>
    /// 命令上下文
    /// </summary>
    public class BaseCommandContext : ICommandContext<IRequestToken>
    {
        public BaseCommandContext(string requestId)
        {
            RequestId = requestId;
        }

        /// <summary>
        /// 采集命令标识
        /// </summary>
        public IRequestToken Token { get; set; }

        /// <summary>
        /// 请求标识
        /// </summary>
        public string RequestId { get; private set; }
    }
}
