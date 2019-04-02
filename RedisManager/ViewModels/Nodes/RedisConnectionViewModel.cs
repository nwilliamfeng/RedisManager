using Caliburn.Micro;
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
    public class RedisConnectionViewModel : NodeViewModel
    {
        public class RedisClientConfigInfo
        {
            public string ConnectionName { get; set; }
            public string ConnectionString { get; set; }
        }
        private const int MaxDBConnectCount = 16;

        
        private ConnectionMultiplexer Connection { get; set; }
        private IEventAggregator _eventAggregator;
        public RedisConnectionViewModel(string connectionName, string connectionString, IEventAggregator eventAggregator)
        {     
            this.Connection = ConnectionMultiplexer.Connect(connectionString);          
            this.Items = new ObservableCollection<DbNodeViewModel>();
            this._eventAggregator = eventAggregator;
            this.Config = new RedisClientConfigInfo { ConnectionString = connectionString, ConnectionName = connectionName };
            this.Name = Config.ConnectionName;
        }

        public RedisConnectionViewModel(RedisClientConfigInfo config, IEventAggregator eventAggregator) :
            this(config.ConnectionName, config.ConnectionString, eventAggregator)
        {
        }

        private bool _isConnected;
        public bool IsConnected
        {
            get { return this._isConnected; }
            private set
            {
                this._isConnected = value;
                this.NotifyOfPropertyChange(() => this.IsConnected);
            }
        }

        public RedisClientConfigInfo Config { get; private set; }


        private async Task<List<Tuple<int, int>>> GetDbs()
        {
            return await Task.Run(() =>
            {
                int idx = 0;
                List<Tuple<int, int>> lst = new List<Tuple<int, int>>();
                while (idx < MaxDBConnectCount)
                {
                    this.Connection.GetDatabase(idx) ;
                       lst.Add(new Tuple<int, int>(idx, this._client.DBSize()));
                 
                        idx++;
                }

                return lst;
            });

        }


        public string Name { get; private set; }


        public ObservableCollection<DbNodeViewModel> Items { get; private set; }

      

        private ICommand _openCommand;

        public ICommand OpenCommand
        {
            get
            {
                return this._openCommand ?? (this._openCommand = new RelayCommand(() =>
                {
                  //  this._eventAggregator.PublishOnUIThread(new RedisClientDetailEventArgs(this.Raw));
                }));
            }
        }

        private ICommand _connectCommand;

        public ICommand ConnectCommand
        {
            get
            {
                return this._connectCommand ?? (this._connectCommand = new RelayCommand(async () =>
                {
                    if (this.IsConnected)
                    {
                        this.IsExpanded = !this.IsExpanded;
                        return;
                    }
                    try
                    {
                        await ExecuteConnect();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(Application.Current.MainWindow, ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    this.IsExpanded = true;
                    this.IsConnected = true;
                    this.Items.Clear();
                    var lst = await this.GetDbs();
                    lst.ForEach(x =>
                    {
                        var dbNode = new DbNodeViewModel(x.Item1, x.Item2, this.is);
                        this.Items.Add(dbNode);
                    });

                }));
            }
        }

        private ICommand _refreshCommand;

        public ICommand RefreshCommand
        {
            get
            {
                return this._refreshCommand ?? (this._refreshCommand = new RelayCommand(async () =>
                {
                    this.IsConnected = false;
                    this.IsExpanded = false;
                    this.IsConnected = true;
                    this.Items.Clear();
                    var lst = await this.GetDbs();
                    lst.ForEach(x =>
                    {
                    //    var dbNode = new DbNodeViewModel(x.Item1, x.Item2, this._client);
                     //   this.Items.Add(dbNode);
                    });
                    this.IsExpanded = true;

                }, () => this.IsConnected));
            }
        }



        private async Task ExecuteConnect()
        {
            await Task.Run(() =>
            {
              //  this._client.Connect();
            });
        }
    }
}
