﻿using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace App1.Models
{
    public class Pessoa : AbstractEntity
    {
        private string _nome;

        [JsonProperty("nome")] //This maps the element title of your web service to your model
        [MaxLength(144), Column("nome")]
        public string Nome
        {
            get => _nome;
            set
            {
                _nome = value;
                OnPropertyChanged(); //This notifies the View or ViewModel that the value that a property in the Model has changed and the View needs to be updated.
            }
        }

        
    }
}