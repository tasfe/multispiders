using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaobaoCrawler;

namespace TaobaoCrawler.Modules.TaoBao
{
    public class UserInformationCmdArg:IUserRequestInfo
    {
        public string ShopUrl
        {
            get;
            set;
        }
    }
}
