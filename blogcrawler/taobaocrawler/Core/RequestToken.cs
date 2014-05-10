using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaobaoCrawler
{
    public class RequestToken:IRequestToken
    {
        public RequestToken(long cId)
        {
            Id = cId;
        }

        public long Id { private set; get; }
    }
}
