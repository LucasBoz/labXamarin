using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace App1.Models
{
    public class AbstractEntity : INotifyPropertyChanged
    {
        protected long? _id = null;

        [JsonProperty("id")] //This maps the element title of your web service to your model
        [PrimaryKey, Column("id")]
        public long? Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(); //This notifies the View or ViewModel that the value that a property in the Model has changed and the View needs to be updated.
            }
        }

        //This is how you create your OnPropertyChanged() method
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged( [CallerMemberName] string propertyName = null )
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
