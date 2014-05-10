using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaobaoCrawler
{
    public interface IRequestToken
    {
        /// <summary>
        /// 请求标识id
        /// </summary>
        long Id { get; }
    }
}
