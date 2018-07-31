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
        private static readonly TimeSpan refreshTimeSpan = new TimeSpan( 0, 1, 0 );

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
            Task.Run( async () => {
                string content = await RestService.getAsync( RestHolder<T>.instance.SyncUri + lastRequestDateTime );
                List<T> list = JsonConvert.DeserializeObject<List<T>>( content );
                SQLiteRepository.sync<T>( list );
                lastRequestDateTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                RestHolder<T>.instance.Locked = false;
            });

            Device.StartTimer( refreshTimeSpan, () => {
                if ( !RestHolder<T>.instance.Locked )
                {
                    RestHolder<T>.instance.Locked = true;
                    Task.Run( async () => {
                        string content = await RestService.getAsync( RestHolder<T>.instance.SyncUri + lastRequestDateTime );
                        List<T> list = JsonConvert.DeserializeObject<List<T>>( content );
                        SQLiteRepository.sync<T>( list );
                        lastRequestDateTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                        RestHolder<T>.instance.Locked = false;
                    });
                }

                return true; //restart timer
            });
        }
    }
}
