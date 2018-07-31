using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using App1.Models;
using App1.Views;
using App1.ViewModels;
using System.Net.Http;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using App1.Database;

namespace App1.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ItemsPage : ContentPage
	{
        private const string Url = "http://192.168.20.16:8080/pessoa/listAll"; //This url is a free public api intended for demos
        private readonly HttpClient _client = new HttpClient(); //Creating a new instance of HttpClient. (Microsoft.Net.Http)
        private ObservableCollection<Pessoa> _pessoas; //Refreshing the state of the UI in realtime when updating the ListView's Collection

        ItemsViewModel viewModel;

        public ItemsPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new ItemsViewModel();
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var item = args.SelectedItem as Item;
            if (item == null)
                return;

            await Navigation.PushAsync(new ItemDetailPage(new ItemDetailViewModel(item)));

            // Manually deselect item.
            ItemsListView.SelectedItem = null;
        }

        async void AddItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new NewItemPage()));
        }

        protected override async void OnAppearing()
        { 
            if (viewModel.Items.Count == 0)
                viewModel.LoadItemsCommand.Execute(null);

            ItemsListView.ItemsSource = await SQLiteRepository.query<Pessoa>( "SELECT * FROM " + typeof( Pessoa ).Name );
            base.OnAppearing();
        }

        /// <summary>
        /// Click event of the Add Button. It sends a POST request adding a Post object in the server and in the collection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnAdd(object sender, EventArgs e)
        {
            Pessoa pessoa = new Pessoa { Id = 12211212 , Nome = "Teste" }; //Creating a new instane of Post with a Title Property and its value in a Timestamp format
            string content = JsonConvert.SerializeObject(pessoa); //Serializes or convert the created Post into a JSON String
            await _client.PostAsync(Url, new StringContent(content, Encoding.UTF8, "application/json")); //Send a POST request to the specified Uri as an asynchronous operation and with correct character encoding (utf9) and contenct type (application/json).
            _pessoas.Insert(0, pessoa); //Updating the UI by inserting an element into the first index of the collection 
        }

        /// <summary>
        /// Click event of the Update Button. It sends a PUT request updating the first Post object in the server and in the collection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnUpdate(object sender, EventArgs e)
        {
            Pessoa pessoa = _pessoas[0]; //Assigning the first Post object of the Post Collection to a new instance of Post
            pessoa.Nome += " [updated]"; //Appending an [updated] string to the current value of the Title property
            string content = JsonConvert.SerializeObject(pessoa); //Serializes or convert the created Post into a JSON String
            await _client.PutAsync(Url + "/" + pessoa.Id, new StringContent(content, Encoding.UTF8, "application/json")); //Send a PUT request to the specified Uri as an asynchronous operation.
        }

        /// <summary>
        /// Click event of the Delete Button. It sends a DELETE request removing the first Post object in the server and in the 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnDelete(object sender, EventArgs e)
        {
            Pessoa pessoa = _pessoas[0]; //Assigning the first Post object of the Post Collection to a new instance of Post
            await _client.DeleteAsync(Url + "/" + pessoa.Id); //Send a DELETE request to the specified Uri as an asynchronous 
            _pessoas.Remove(pessoa); //Removes the first occurrence of a specific object from the Post collection. This will be visible on the UI instantly
        }
    }
}