﻿using RedisManager.Util;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RedisManager.ViewModels
{
    public class StringKeyViewModel : KeyViewModel
    {
        private string _value;
        public StringKeyViewModel(string key, DbNodeViewModel parent)
            : base(key, parent)
        {
           this._value = this.Database.StringGet(this.KeyName);
        }

        public override RedisType KeyType
        {
            get
            {
                return RedisType.String;
            }
        }

        public new string KeyValue
        {
            get { return base.KeyValue as string; }
            set
            {
                this._value = value;
                this.NotifyOfPropertyChange(() => this.KeyValue);
            }
        }

        protected override object GetKeyValue()
        {
            return this._value;
        }

        private ICommand _updateCommand;

        public ICommand UpdateCommand
        {
            get
            {
                return this._updateCommand ?? (this._updateCommand = new RelayCommand(() =>
                {
                    this.Database.StringSet(this.KeyName,this.KeyValue);
                 
                }));
            }
        }

    
    }
}
