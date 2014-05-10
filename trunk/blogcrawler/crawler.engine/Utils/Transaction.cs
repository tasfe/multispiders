using System;
namespace Crawler.Engine.Utils
{
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// 事务步骤
    /// </summary>
    public class Step
    {
        #region Private
        internal Step(string name)
        {
            Successful = true;
            Completed = false;
            _Name = string.IsNullOrWhiteSpace(name) ? Guid.NewGuid().ToString().Substring(0, 5) : name;
        }

        internal bool Successful;

        internal bool Completed;

        internal Transaction _Transaction = null;

        protected string _Name;
        public string Name
        {
            get { return _Name; }
        }
        #endregion

        #region Public
        /// <summary>
        /// 成功完成事务后调用
        /// </summary>
        public void Success()
        {
            Debug.Assert(!Completed, "当前事务已完成");
            Debug.Assert(_Transaction.IsEnd, "当前事务未完成定义");
            Completed = true;
            Successful = true;
            _Transaction.Update();
        }

        /// <summary>
        /// 事务完成失败调用
        /// </summary>
        public void Fail()
        {
			Debug.Assert(!Completed, "当前事务已完成");
            Debug.Assert(!_Transaction.IsEnd, "当前事务未完成定义");
            Completed = true;
            Successful = false;
            _Transaction.Update();
        }
        #endregion
    }

    /// <summary>
    /// 事务主体，可包含子步骤/子事务
    /// </summary>
    public class Transaction : Step
    {
        #region Private
        private List<Step> _StepList = new List<Step>();

        private Action<bool> _Callback = null;

		//private int _InProcess;

        private bool _IsEnd;
        public bool IsEnd
        {
            get { return _IsEnd; }
        }
        /// <summary>
        /// 更新事务状态
        /// </summary>
        /// <remarks>此方法用于事务所有子步骤/子事务定义完成后。由于事务创建后，状态根据其子步骤/子事务进行更新。当子步骤/子事务为空时，事务本身将永远处于未完成状态。调用此方法可强制进行事务状态检查。</remarks>
        internal void Update()
        {
			if (_IsEnd && !Completed)
            {
                //对未完成的事务进行更新
                //对已完成的事务，此步重复操作忽略无效。
                bool comp = true;
                bool succ = true;
                foreach (Step step in _StepList)
                {
                    comp = comp && step.Completed;
                    succ = succ && step.Successful;
                }

                //更新当前状态：一旦完成、永远完成；一旦失败、永远失败
                Completed = Completed || comp;
                Successful = Successful && succ;

                if (comp)
                {
                    //完成状态发生改变，执行回调
                    if (_Callback != null)
                    {
						_Callback(succ);
                    }
                    //清空步骤列表，以防重复执行回调
                    _StepList.Clear();

                    //如果是子事务，更新父事务状态
                    if (_Transaction != null)
                    {
                        if (succ)
                        {
                            _Transaction.Update();
                        }
                        else
                        {
                            _Transaction.Update();
                        }
                    }
                }
            }
        }
        #endregion

        #region Public
        /// <summary>
        /// 事务主体构造函数
        /// </summary>
        /// <param name="mcallback">自定义回调函数</param>
        public Transaction(Action<bool> mcallback, string name = "")
            : base(name)
        {
            _Callback = mcallback;
            _IsEnd = false;
		}

        /// <summary>
        /// 创建子事务
        /// </summary>
        /// <param name="mcallback">自定义回调函数，默认为null。</param>
        /// <returns></returns>
        public Transaction CreateTransaction(Action<bool> mcallback = null, string name = "")
        {
			Debug.Assert(!Completed, "当前事务已完毕，创建失败");

            string tname = string.IsNullOrWhiteSpace(name) ? _Name + "_" + _StepList.Count : name;
            Transaction child = new Transaction(mcallback, _Name + "_" + _StepList.Count);
            _StepList.Add(child);
            child._Transaction = this;
			return child;
        }

        /// <summary>
        /// 创建子步骤
        /// </summary>
        /// <returns></returns>
        public Step CreateStep(string name = "")
        {
            Debug.Assert(!Completed,"当前事务已完毕，创建失败");

            string stepname = string.IsNullOrWhiteSpace(name)?_Name + "_" + _StepList.Count:name;
            Step s = new Step(stepname);
            s._Transaction = this;
            _StepList.Add(s);
			return s;
        }
        /// <summary>
        /// 完成子事务/子步骤创建
        /// </summary>
        public void EndCreate()
        {
			_IsEnd = true;
            Update();
        }
        #endregion
    }
}
