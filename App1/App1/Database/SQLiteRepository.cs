using App1.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace App1.Database
{
    class SQLiteRepository
    {
        private static readonly string dbPath = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.Personal ), "appdatabase.db" );
        private static readonly SQLiteConnection db = new SQLiteConnection( dbPath );

        public static void init()
        {
            Console.WriteLine("Creating database, if it doesn't already exist");
            db.CreateTable<Pessoa>();
        }
        
        //Método para dar merge na list de entidades
        public static void sync<T>( List<T> entityList ) where T : new()
        {
            foreach ( T entity in entityList )
            {
                var rowsAffected = db.Update(entity);
                if (rowsAffected == 0) rowsAffected = db.Insert(entity);
            }
        }

        //Apaga do cache. Remove do cache local as entidades que foram deletadas na nuvem
        public static void syncDeletedEntities<T>( List<T> entityList ) where T : new()
        {
            foreach ( T entity in entityList )
            {
                db.Delete<T>( entity );
            }
        }

        //Retorna uma lista do que foi descrito na query
        public static async Task<ObservableCollection<T>> query<T>( string query ) where T : new()
        {
            return await Task.FromResult( new ObservableCollection<T>( db.Query<T>( query ) ) );      
        }

        //Retorna a entidade com o Id especificado
        public static async Task<T> findById<T>( long id ) where T : new()
        {
            return await Task.FromResult( db.Get<T>( id ) );
        }
    }
}
