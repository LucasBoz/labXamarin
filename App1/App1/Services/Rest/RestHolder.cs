using App1.Configuration;
using App1.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace App1.Services.Rest
{
    class RestHolder<T>
    {
        private static readonly TimeSpan refreshTimeSpan = new TimeSpan(0, 0, 3);

        //self instance singleton para RestEntityHolder, que auxilia a mexer com as Uri's REST
        public static RestHolder<T> instance = null;
        private static long? lastRequestDateTime = null;

        private string _syncUri;
        private string _insertUri;
        private string _updateUri;
        private string _deleteUri;

        private bool threadLock = false;

        public RestHolder()
        {
            instance = instance ?? this;
        }

        public string InsertUri
        {
            get => _insertUri ?? typeof(T).Name.ToLower() + "/" + "insert";
            set => _insertUri = value;
        }

        public string UpdateUri
        {
            get => _updateUri ?? typeof(T).Name.ToLower() + "/" + "update";
            set => _updateUri = value;
        }

        public string DeleteUri
        {
            get => _deleteUri ?? typeof(T).Name.ToLower() + "/" + "delete";
            set => _deleteUri = value;
        }

        public string SyncUri
        {
            get => _syncUri ?? typeof(T).Name.ToLower() + "/" + "list";
            set => _syncUri = value;
        }

        public bool Locked
        {
            get => threadLock;
            set => threadLock = value;
        }

        public static void StartTimer<T>() where T : new()
        {
            runTask<T>();

            Device.StartTimer(refreshTimeSpan, () => {
                if (!RestHolder<T>.instance.Locked)
                {
                    //lock thread
                    RestHolder<T>.instance.Locked = true;
                    //run thread
                    runTask<T>();
                }

                return true; //restart timer
            });
        }

        private static async void runTask<T>() where T : new()
        {
            //Pega o DateTime da ultima requisição desta Uri
            DateTime lastRequest = Prefs.getDateTime(RestHolder<T>.instance.SyncUri);

            //Request e Sync
            long unixTimestamp = lastRequest.Ticks - new DateTime(1970, 1, 1).Ticks;
            string content2 = await RestService.getAsync(RestHolder<T>.instance.SyncUri + ( unixTimestamp / TimeSpan.TicksPerMillisecond ) );
            List<T> list2 = JsonConvert.DeserializeObject<List<T>>(content2);
            SQLiteRepository.sync<T>(list2);

            //Seta o DateTime da ultima requisição para AGORA
            Prefs.setDateTime(RestHolder<T>.instance.SyncUri, DateTime.Now);

            //unlock thread
            RestHolder<T>.instance.Locked = false;
        }
    }
}
