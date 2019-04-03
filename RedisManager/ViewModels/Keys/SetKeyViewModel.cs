using RedisManager.Util;
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
    public class SetKeyViewModel : KeyViewModel
    {
        private ObservableCollection<string> _values;

        public SetKeyViewModel(string key, DbNodeViewModel parent)
            : base(key, parent)
        {

        }

        public override RedisType KeyType
        {
            get
            {
                return RedisType.Set;
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
                var values = this.Database.SetMembers(this.KeyName);
                this._values = new ObservableCollection<string>(values.Select(x=>x.ToString()));
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
                   
                    this.Database.SetRemove(this.KeyName, this.SelectedKeyValueItem);
                    this.Database.SetAdd(this.KeyName, this.EditingKeyValueItem);
                    this.KeyValue.Clear();
                    this.Database.SetMembers(this.KeyName).ToList().ForEach(x=>this.KeyValue.Add(x));
                    this.SelectedKeyValueItem = this.EditingKeyValueItem;
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
                    this.Database.SetAdd(this.KeyName, vm.Value);
                    this.KeyValue.Add(vm.Value);
                }));
            }
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
                    this.Database.SetRemove(this.KeyName,this.SelectedKeyValueItem);
                    this.KeyValue.Remove(this.SelectedKeyValueItem);
                    this.SelectedKeyValueItem = null;
                }, () => this.SelectedKeyValueItem != null));
            }
        }
    }
}
