using System;
using Xamarin.Forms;
using App1.Views;
using Xamarin.Forms.Xaml;
using App1.Database;
using App1.Services;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace App1
{
	public partial class App : Application
	{
		
		public App ()
		{
			InitializeComponent();
			MainPage = new MainPage();
		}

		protected override void OnStart ()
		{
            // Handle when your app starts
            SQLiteRepository.Init();
            RestService.Init();
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
