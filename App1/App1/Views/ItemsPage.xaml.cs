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
using App1.Services;

namespace App1.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemsPage : ContentPage
    {
        private ObservableCollection<Pessoa> _pessoas;
        private Subscription<Pessoa> listPessoaSubscription;

        ItemsViewModel viewModel;

        public ItemsPage()
        {
            InitializeComponent();
            BindingContext = viewModel = new ItemsViewModel();

            listPessoaSubscription = new Subscription<Pessoa>( async () => {
                Console.WriteLine( "WOLOLOOOOOOOOOO" );
                ItemsListView.ItemsSource = await SQLiteRepository.Query<Pessoa>("SELECT * FROM " + typeof(Pessoa).Name);
            });
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
            Pessoa pessoa = new Pessoa();
            pessoa.Id = null;
            pessoa.Nome = "cabulosa null" + _pessoas.Count;
            pessoa.Created = DateTime.Now;
            RestService.SendEntity<Pessoa>(pessoa);

            Pessoa pessoa2 = new Pessoa();
            pessoa2.Id = 66666;
            pessoa2.Nome = "cabulosa updated" + _pessoas.Count;
            pessoa2.Created = DateTime.Now;
            RestService.SendEntity<Pessoa>(pessoa2);
        }

        protected override async void OnAppearing()
        { 
            if (viewModel.Items.Count == 0)
                viewModel.LoadItemsCommand.Execute(null);

            ItemsListView.ItemsSource = await SQLiteRepository.Query<Pessoa>( "SELECT * FROM " + typeof( Pessoa ).Name );
            base.OnAppearing();
        }

        /// <summary>
        /// Click event of the Add Button. It sends a POST request adding a Post object in the server and in the collection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnAdd(object sender, EventArgs e)
        {
            //Pessoa pessoa = new Pessoa { Id = 66666 , Nome = "cabulosa" }; //Creating a new instane of Post with a Title Property and its value in a Timestamp format
            //RestService.send<Pessoa>(pessoa);
        }

        /// <summary>
        /// Click event of the Update Button. It sends a PUT request updating the first Post object in the server and in the collection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnUpdate(object sender, EventArgs e)
        {
            
        }

        /// <summary>
        /// Click event of the Delete Button. It sends a DELETE request removing the first Post object in the server and in the 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnDelete(object sender, EventArgs e)
        {
           
        }
    }
}