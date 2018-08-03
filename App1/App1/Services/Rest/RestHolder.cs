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
        private static readonly TimeSpan refreshTimeSpan = new TimeSpan( 0, 0, 5 );

        //self instance singleton para RestEntityHolder, que auxilia a mexer com as Uri's REST
        public static RestHolder<T> instance = null;

        private string _syncUri;
        private string _insertUri;
        private string _updateUri;
        private string _deleteUri;

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

        public static void StartAutoSync<T>() where T : new()
        {
            RunTask<T>();

            Device.StartTimer( refreshTimeSpan, () => {
                RunTask<T>();
                return true; //restart timer
            });
        }

        private static async void RunTask<T>() where T : new()
        {
            try
            {
                //Pega o DateTime da ultima requisição desta Uri
                DateTime lastRequest = Prefs.getDateTime(RestHolder<T>.instance.SyncUri);

                //Request e Sync
                long unixTimestamp = lastRequest.Ticks - new DateTime(1970, 1, 1).Ticks;
                string content2 = await RestService.GetAsync(RestHolder<T>.instance.SyncUri + ( unixTimestamp / TimeSpan.TicksPerMillisecond ) );
                List<T> list2 = JsonConvert.DeserializeObject<List<T>>(content2);
                SQLiteRepository.Sync<T>(list2);

                //Seta o DateTime da ultima requisição para AGORA
                Prefs.setDateTime(RestHolder<T>.instance.SyncUri, DateTime.Now);
            }
            catch( Exception e )
            {
                Console.WriteLine( e.Message );
            }
        }
    }
}
