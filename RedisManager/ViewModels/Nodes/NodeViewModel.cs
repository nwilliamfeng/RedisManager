using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisManager.ViewModels
{
    public abstract class NodeViewModel : PropertyChangedBase,INode
    {
        private bool _isExpanded;

        public bool IsExpanded
        {
            get { return this._isExpanded; }
            set
            {
                this._isExpanded = value;
                this.NotifyOfPropertyChange(() => this.IsExpanded);
            }
        }

     
    }

    public interface INode
    {
        bool IsExpanded { get; set; }
    }
}
