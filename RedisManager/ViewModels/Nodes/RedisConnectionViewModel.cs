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
            
            this.Items = new ObservableCollection<DbNodeViewModel>();
            this._eventAggregator = eventAggregator;
            this.Config = new RedisClientConfigInfo { ConnectionString = connectionString, ConnectionName = connectionName };
            this.Name = Config.ConnectionName;
        }

        public RedisConnectionViewModel(RedisClientConfigInfo config, IEventAggregator eventAggregator) :
            this(config.ConnectionName, config.ConnectionString, eventAggregator)
        {
        }

       
        public bool IsConnected
        {
            get { return this.Connection==null?false:this.Connection.IsConnected; }      
        }

        public RedisClientConfigInfo Config { get; private set; }


        public string Name { get; private set; }


        public ObservableCollection<DbNodeViewModel> Items { get; private set; }

      

        private ICommand _openCommand;

        public ICommand OpenCommand
        {
            get
            {
                return this._openCommand ?? (this._openCommand = new RelayCommand(() =>
                {
                     this._eventAggregator.PublishOnUIThread(new RedisConnectionEventArgs(this.Connection));
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
                        this.Connection = await ConnectionMultiplexer.ConnectAsync(this.Config.ConnectionString);
                        this.IsExpanded = true;

                        this.AddDbs();
                        this.NotifyOfPropertyChange(() => this.IsConnected);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(Application.Current.MainWindow, ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                   

                }));
            }
        }

        private ICommand _refreshCommand;

        public ICommand RefreshCommand
        {
            get
            {
                return this._refreshCommand ?? (this._refreshCommand = new RelayCommand( () =>
                {
                    this.Connection.Close();
                   
                    this.NotifyOfPropertyChange(() => this.IsConnected);
                    this.IsExpanded = false;
                    this.Connection = ConnectionMultiplexer.Connect(this.Config.ConnectionString);
                    this.AddDbs();
                    this.NotifyOfPropertyChange(() => this.IsConnected);
                    this.IsExpanded = true;

                }, () => this.IsConnected));
            }
        }

        private void AddDbs()
        {
            this.Items.Clear();
            for (var i = 0; i < MaxDBConnectCount; i++)
            {
                this.Items.Add(new DbNodeViewModel(i, this.Connection));
            }
        }



       
    }
}
