using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisManager.ViewModels
{
    public class HashKeyItemViewModel : PropertyChangedBase
    {
        private string _key;

        public HashKeyItemViewModel(string key, string value)
        {
            this._key = key;
            this._value = value;
        }

        public HashKeyItemViewModel()
        {

        }


        public string Key
        {
            get { return this._key; }
            set
            {
                this._key = value;
                this.NotifyOfPropertyChange(() => this.Key);
            }
        }

        private string _value;

        public string Value
        {
            get { return this._value; }
            set
            {
                this._value = value;
                this.NotifyOfPropertyChange(() => this.Value);
            }
        }

    }
}
