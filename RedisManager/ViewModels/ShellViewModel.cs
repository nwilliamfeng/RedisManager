using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using System.Windows.Input;
using System.IO;
using Newtonsoft.Json;
using RedisManager.Util;
 

namespace RedisManager.ViewModels
{
    [Export(typeof(ShellViewModel))]
    public class ShellViewModel : Screen
    {
        private IEventAggregator _eventAggregator;
        private IWindowManager _windowManager;
        private const string CONFIG_FILE = "connecion.json";

        [ImportingConstructor]
        public ShellViewModel(PageModuleViewModel pageModule, IEventAggregator eventAggregator, IWindowManager windowManager)
        {
            this.DisplayName =  "Redis Manager" ;
            this.RedisConnections = new ObservableCollection<RedisConnectionViewModel>();
            this.LoadFromFile();
            this.PageModule = pageModule;
            this._eventAggregator = eventAggregator;
            this._windowManager = windowManager;         
        }

        private void LoadFromFile()
        {
            try
            {
                var cis = JsonConvert.DeserializeObject<RedisConnectionViewModel.RedisClientConfigInfo[]>(File.ReadAllText(CONFIG_FILE))
                     .Select(x => new RedisConnectionViewModel(x, this._eventAggregator));
                if (cis.Count() > 0)
                    this.RedisConnections = new ObservableCollection<RedisConnectionViewModel>(cis);
            }
            catch
            {

            }
        }

        public PageModuleViewModel PageModule { get; private set; }

        public ObservableCollection<RedisConnectionViewModel> RedisConnections { get; private set; }

        private ICommand _connectCommand;

        public ICommand ConnectCommand
        {
            get
            {
                return this._connectCommand ?? (this._connectCommand = new RelayCommand(() =>
                {
                    var dvm = new ConnectServerDialogViewModel();
                    var dr = this._windowManager.ShowDialog(dvm);
                    if (dr == false)
                        return;
                    var cnnStr = $"{dvm.Address}:{dvm.Port},password={dvm.Password}" ;                 
                    this.RedisConnections.Add(new RedisConnectionViewModel(dvm.ConnectionName, cnnStr, this._eventAggregator));
                    var cnns = this.RedisConnections.Select(x => x.Config).ToArray();
                    
                    File.WriteAllText(CONFIG_FILE, JsonConvert.SerializeObject(cnns));
                }));
            }
        }



    }
}
 
 