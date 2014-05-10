using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Crawler.Engine.PowerCollections;

namespace TaobaoCrawler
{
    /// <summary>
    /// 编号管理器
    /// 描述:依赖于引擎的事件记录
    /// </summary>
    public class IdManager
    {
        private static object _synLock = new object();

        private static IdManager _manager = null;

        private Dictionary<IdType, long> _marks =
            new Dictionary<IdType, long>();
        public long New(IdType type) 
        {
            long mark = 0;
            if(_marks.ContainsKey(type))
            {
                mark = ++_marks[type];
            }else
            {
                _marks.Add(type,mark);
            }
            return mark;
        }

        private IdManager() { }

        public  static  IdManager   Hinstance
        {
            get
            {
                if(_manager == null)
                {
                    lock(_synLock)
                    {
                        if(_manager == null)
                        {
                            _manager = new IdManager(); 
                        }
                    }
                }

                return _manager;
            }
        }
    }

    public enum IdType
    {
        USER_REQUEST_ID
    }
}
