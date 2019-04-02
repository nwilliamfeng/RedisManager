﻿using RedisManager.Util;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RedisManager.ViewModels
{
    public class ListKeyViewModel : KeyViewModel
    {
        private ObservableCollection<string> _values;

        public ListKeyViewModel(string key, DbNodeViewModel parent)
            : base(key, parent)
        {

        }

        public override RedisType KeyType
        {
            get
            {
                return RedisType.List;
            }
        }

        private string _selectedKeyValueItem;

        public string SelectedKeyValueItem
        {
            get { return this._selectedKeyValueItem; }
            set
            {
                this._selectedKeyValueItem = value;
                this.NotifyOfPropertyChange(() => this.SelectedKeyValueItem);
                if (value == null)
                    this.EditingKeyValueItem = null;
                else
                    this.EditingKeyValueItem = value;
            }
        }

        private string _selectedEditingKeyValueItem;

        public string EditingKeyValueItem
        {
            get { return this._selectedEditingKeyValueItem; }
            set
            {
                this._selectedEditingKeyValueItem = value;
                this.NotifyOfPropertyChange(() => this.EditingKeyValueItem);
            }
        }

        public new ObservableCollection<string> KeyValue
        {
            get { return base.KeyValue as ObservableCollection<string>; }
        }

        protected override object GetKeyValue()
        {
            if (_values == null)
            {
               // var values = this.RedisClient.GetDataBase(this.DBIndex).LRang(this.KeyName);
              //  this._values = new ObservableCollection<string>(values);
            }
            return this._values;
        }



        private ICommand _updateCommand;

        public ICommand UpdateCommand
        {
            get
            {
                return this._updateCommand ?? (this._updateCommand = new RelayCommand(() =>
                {
                  //  var db = this.RedisClient.GetDataBase(this.DBIndex);
                 //   var idx = this.KeyValue.IndexOf(this.SelectedKeyValueItem);

                    //    db.LRemove(this.KeyName, idx,idx  );
                //    db.LSet(this.KeyName, idx, this.EditingKeyValueItem);
                    this.SelectedKeyValueItem = this.EditingKeyValueItem;
                    this.Reload();
                }, () => this.EditingKeyValueItem != null));
            }
        }

        private ICommand _insertRowCommand;

        public ICommand InsertRowCommand
        {
            get
            {
                return this._insertRowCommand ?? (this._insertRowCommand = new RelayCommand(() =>
                {
                    var vm = new KeyValueDialogViewModel { IsKeyTypeVisible = false, IsKeyVisible = false };
                    var dr = this.WindowManager.ShowDialog(vm);
                    if (dr == false)
                        return;

                   // var db = this.RedisClient.GetDataBase(this.DBIndex);
                  //  db.LPush(this.KeyName, vm.Value);
                    this.Reload();

                }));
            }
        }

        private void Reload()
        {
            this.KeyValue.Clear();
          //  this.RedisClient.GetDataBase(this.DBIndex).LRang(this.KeyName).ForEach(x => this.KeyValue.Add(x));
        }

        private ICommand _deleteRowCommand;

        public ICommand DeleteRowCommand
        {
            get
            {
                return this._deleteRowCommand ?? (this._deleteRowCommand = new RelayCommand(() =>
                {
                    var dr = MessageBox.Show(Application.Current.MainWindow, "确定要删除该行吗？", "删除", MessageBoxButton.OKCancel);
                    if (dr == MessageBoxResult.Cancel)
                        return;
                   // var db = this.RedisClient.GetDataBase(this.DBIndex);

                 //   db.LRemove(this.KeyName, 1, SelectedKeyValueItem);
                    this.KeyValue.Remove(this.SelectedKeyValueItem);
                    this.SelectedKeyValueItem = null;
                }, () => this.SelectedKeyValueItem != null));
            }
        }
    }
}
