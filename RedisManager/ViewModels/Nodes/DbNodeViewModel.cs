using Caliburn.Micro;
using RedisManager.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using StackExchange.Redis;

namespace RedisManager.ViewModels
{
    public class DbNodeViewModel : NodeViewModel
    {
        private ConnectionMultiplexer Connection { get; set; }
        private int _dbIdx;
      
        private int _offSet = 0;
        private const int PAGE_SIZE = 20;

        public DbNodeViewModel(int dbIdx, ConnectionMultiplexer connection)
        {
            this.Connection = connection;
            this._dbIdx = dbIdx;

            this.Keys = new ObservableCollection<KeyViewModel>();
            this.LoadKeysAsync(true);

        }

        private IWindowManager _windowManager;

        private IWindowManager WindowManager
        {
            get
            {
                if (this._windowManager == null)
                    this._windowManager = IoC.Get<IWindowManager>();
                return this._windowManager;
            }
        }


        private async void LoadKeysAsync(bool needClear)
        {
            if (needClear)
                this.Keys.Clear();
            var keys=this.Connection.SelfServer().Keys(this._dbIdx, pageSize: 50, pageOffset: this._offSet);
            this._offSet+=keys.Count();
           
            if (keys.Count()==0)
                return;
            var lst = await Task.Run(() =>
            {
                List<KeyViewModel> result = new List<KeyViewModel>();
                foreach (var key in keys)
                {
                    var keyType = this.Connection.GetDatabase(this._dbIdx).KeyType(key);
                   
                    var vm = KeyViewModel.Create(keyType, key, this);
                    if (vm != null)
                        result.Add(vm);
                }
                return result;
            });

            lst.ForEach(x => this.Keys.Add(x));
            this.NotifyOfPropertyChange(() => this.Name);
        }


        public ObservableCollection<KeyViewModel> Keys { get; private set; }


        public int Index
        {
            get { return this._dbIdx; }
        }

       

      

        public string Name
        {
            get
            {
                return string.Format("db{0}", Index);
            }
        }

        private ICommand _loadKeysCommand;

        public ICommand LoadKeysCommand
        {
            get
            {
                return this._loadKeysCommand ?? (this._loadKeysCommand = new RelayCommand(() =>
                {
                    this.DoRefresh();
                }));
            }
        }

        private void DoRefresh()
        {
            this.LoadKeysAsync(true);
            this.IsExpanded = true;
        }

        private ICommand _loadNextPageCommand;

        public ICommand LoadNextPageCommand
        {
            get
            {
                return this._loadNextPageCommand ?? (this._loadNextPageCommand = new RelayCommand(() =>
                {
                    this.LoadKeysAsync(false);
                    this.IsExpanded = true;
                }, () => this._offSet != 0));
            }
        }

        private ICommand _addKeyCommand;

        public ICommand AddKeyCommand
        {
            get
            {
                return this._addKeyCommand ?? (this._addKeyCommand = new RelayCommand(() =>
                {

                    var vm = new KeyValueDialogViewModel { IsKeyTypeVisible = true };
                    var dr = this.WindowManager.ShowDialog(vm);
                    if (dr == false)
                        return;
                    var db = this.Connection.GetDatabase(this._dbIdx);
                    switch (vm.KeyType)
                    {
                        case RedisType.String:
                            db.StringSet(vm.Key, vm.Value);
                            break;
                        case RedisType.Hash:
                            db.HashSet(vm.Key, vm.SubKey, vm.Value);
                            break;
                        case RedisType.Set:
                            db.SetAdd(vm.Key, vm.Value);
                            break;
                        case RedisType.SortedSet:
                            db.SortedSetAdd(vm.Key, vm.Value, double.Parse(vm.SubKey));
                            break;
                        case RedisType.List:
                            db.ListLeftPush(vm.Key, vm.Value);
                            break;
                    }
                    this.DoRefresh();


                }));
            }
        }

    }
}
