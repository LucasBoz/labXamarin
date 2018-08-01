using App1.Models;
using App1.Services.Rest;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace App1.Services
{
    class RestService
    {
        private static readonly string Url = "http://192.168.20.16:8080/";
        private static readonly HttpClient _client = new HttpClient();

        public static void init()
        {
            //Obrigatório; Cria a configuração de REST para a entidade T especificada em <T>
            var pessoaHolder = new RestHolder<Pessoa> {
                SyncUri = "pessoa/merge?date=",
                InsertUri = "pessoa/insert",
                UpdateUri = "pessoa/update",
                DeleteUri = "pessoa/delete/" //concat ID
            };

            //...

            //Cria um timer que executa a function sempre a cada X periodos
            RestHolder<Pessoa>.StartTimer<Pessoa>();
            
            //...
        }

        //generic method to insert/update with rest HTTP PUT/POST
        public static async void merge<T>( AbstractEntity entity ) where T : new()
        {
            var holder = RestHolder<T>.instance;
            string content = JsonConvert.SerializeObject( entity );
            StringContent restContent = new StringContent( content, Encoding.UTF8, "application/json" );
            if( entity.Id == null ) await _client.PostAsync( Url + holder.InsertUri, restContent );
            else await _client.PutAsync( Url + holder.UpdateUri, restContent );
        }

        //generic method to delete with rest HTTP DELETE
        public static async void delete<T>( long id ) where T : new()
        {
            var holder = RestHolder<T>.instance;
            await _client.DeleteAsync( Url + holder.DeleteUri + id );
        }

        //generic method to rest HTTP GET
        public static async Task<string> getAsync( string uri )
        {
            string content = await _client.GetStringAsync(Url + uri);
            return await Task.FromResult( content );
        }
    }
}
