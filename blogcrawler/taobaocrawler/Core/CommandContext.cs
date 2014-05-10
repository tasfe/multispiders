using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaobaoCrawler
{
    public class CommandContext : BaseCommandContext<IRequestToken>
    {
        public CommandContext(string requestId)
            : base(requestId)
        {

        }
    }
}
