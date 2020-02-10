using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL;
using Dapper;

namespace Database
{
    /*
     * These are all the methods for accessing data from the database.
     * Methods Implemented: Create, Read (List and Individual), Update, Delete.
     */
    public class DB
    {
        //Get Methods - List
        
        public static List<Note> GetNotes() {
            using (IDbConnection cnn = new SQLiteConnection(Connection.LoadConnectionString())) {
                var output = cnn.Query< Note >("select * from Note",new DynamicParameters());
                return output.ToList();
            }
        }
        
        public static List<Map> GetMaps()
        {
            using (IDbConnection cnn = new SQLiteConnection(Connection.LoadConnectionString()))
            {
                var output = cnn.Query<Map>("select * from Map", new DynamicParameters());
                return output.ToList();
            }
        }

        public static List<NoteCategory> GetNoteCategories()
        {
            using (IDbConnection cnn = new SQLiteConnection(Connection.LoadConnectionString()))
            {
                var output = cnn.Query<NoteCategory>("select * from [Note Category]", new DynamicParameters());
                return output.ToList();
            }
        }

        public static List<Pin> getPins()
        {
            using (IDbConnection cnn = new SQLiteConnection(Connection.LoadConnectionString()))
            {
                var output = cnn.Query<Pin>("select * from Pin", new DynamicParameters());
                return output.ToList();
            }
        }

        public static List<RandomGenerator> getRandomGenerators()
        {
            using (IDbConnection cnn = new SQLiteConnection(Connection.LoadConnectionString()))
            {
                var output = cnn.Query<RandomGenerator>("select * from [Random Generator]", new DynamicParameters());
                return output.ToList();
            }
        }

        //Get Methods - Individual
        //There is no GetPinNote as it was deemed unnecessary.

        public static Note GetNote(int id) {
            using (IDbConnection cnn = new SQLiteConnection(Connection.LoadConnectionString()))
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("id",id,DbType.Int32,ParameterDirection.Input);
                var output = cnn.Query<Note>("select * from Note where note_id = :id", parameters);
                if (output.Count() > 0)
                    return output.First();
                else
                    return null;
            }
        }

        public static Map GetMap(int id)
        {
            using (IDbConnection cnn = new SQLiteConnection(Connection.LoadConnectionString()))
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("id", id, DbType.Int32, ParameterDirection.Input);
                var output = cnn.Query<Map>("select * from Map where map_id = :id", parameters);
                if (output.Count() > 0)
                    return output.First();
                else
                    return null;
            }
        }

        public static NoteCategory GetNoteCategory(int id)
        {
            using (IDbConnection cnn = new SQLiteConnection(Connection.LoadConnectionString()))
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("id", id, DbType.Int32, ParameterDirection.Input);
                var output = cnn.Query<NoteCategory>("select * from [Note Category] where category_id = :id", parameters);
                return output.First();
            }
        }

        public static Pin GetPin(int id)
        {
            using (IDbConnection cnn = new SQLiteConnection(Connection.LoadConnectionString()))
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("id", id, DbType.Int32, ParameterDirection.Input);
                var output = cnn.Query<Pin>("select * from Pin where pin_id = :id", parameters);
                if (output.Count() > 0)
                    return output.First();
                else
                    return new Pin();
            }
        }

        public static RandomGenerator GetRandomGenerator(int id)
        {
            using (IDbConnection cnn = new SQLiteConnection(Connection.LoadConnectionString()))
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("id", id, DbType.Int32, ParameterDirection.Input);
                var output = cnn.Query<RandomGenerator>("select * from [Random Generator] where rng_id = :id", parameters);
                return output.First();
            }
        }

        //Create Methods

        public static void Add(Note note) {
            using (IDbConnection cnn = new SQLiteConnection(Connection.LoadConnectionString()))
            {
                cnn.Execute("insert into Note(note_title,note_content,category_id) values (@note_title,@note_content,@category_id)",note);
            }
        }

        public static void Add(Map map)
        {
            using (IDbConnection cnn = new SQLiteConnection(Connection.LoadConnectionString()))
            {
                cnn.Execute("insert into Map(map_name,map_file,map_x,map_y,parent_map_id) values (@map_name,@map_file,@map_x,@map_y,@parent_map_id)", map);
            }
        }

        public static void Add(NoteCategory noteCategory)
        {
            using (IDbConnection cnn = new SQLiteConnection(Connection.LoadConnectionString()))
            {
                cnn.Execute("insert into [Note Category](category_title,category_parent) values (@category_title,@category_parent)", noteCategory);
            }
        }

        public static void Add(Pin pin)
        {
            using (IDbConnection cnn = new SQLiteConnection(Connection.LoadConnectionString()))
            {
                cnn.Execute("insert into Pin(pin_title,pin_content,pin_x,pin_y,parent_map_id,attached_note_id) values (@pin_title,@pin_content,@pin_x,@pin_y,@parent_map_id,@attached_note_id)", pin);
            }
        }


        public static void Add(RandomGenerator rng)
        {
            using (IDbConnection cnn = new SQLiteConnection(Connection.LoadConnectionString()))
            {
                cnn.Execute("insert into [Random Generator](rng_title,rng_content) values (@rng_title,@rng_content)", rng);
            }
        }

        //Update Methods

        public static void Update(Note note) {
            using (IDbConnection cnn = new SQLiteConnection(Connection.LoadConnectionString()))
            {
                cnn.Execute("update Note set note_title = @note_title,note_content = @note_content,category_id = @category_id where note_id = @note_id", note);
            }
        }

        public static void Update(Map map)
        {
            using (IDbConnection cnn = new SQLiteConnection(Connection.LoadConnectionString()))
            {
                cnn.Execute("update Map set map_name = @map_name,map_file = @map_file,map_x = @map_x, map_y = @map_y, parent_map_id = @parent_map_id where map_id = @map_id", map);
            }
        }

        public static void Update(NoteCategory noteCategory)
        {
            using (IDbConnection cnn = new SQLiteConnection(Connection.LoadConnectionString()))
            {
                cnn.Execute("update [Note Category] set category_title = @category_title,category_parent = @category_parent where category_id = @category_id", noteCategory);
            }
        }

        public static void Update(Pin pin)
        {
            using (IDbConnection cnn = new SQLiteConnection(Connection.LoadConnectionString()))
            {
                cnn.Execute("update Pin set pin_title = @pin_title,pin_content = @pin_content,pin_x = @pin_x,pin_y = @pin_y,parent_map_id = @parent_map_id where pin_id = @pin_id", pin);
            }
        }

        public static void Update(RandomGenerator rng)
        {
            using (IDbConnection cnn = new SQLiteConnection(Connection.LoadConnectionString()))
            {
                cnn.Execute("update [Random Generator] set rng_title = @rng_title,rng_content = @rng_content where rng_id = @rng_id", rng);
            }
        }

        //Delete Methods

        public static void DeleteNote(int id) {
            using (IDbConnection cnn = new SQLiteConnection(Connection.LoadConnectionString()))
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("id", id, DbType.Int32, ParameterDirection.Input);
                cnn.Execute("delete from note where note_id = :id", parameters);
            }
        }

        public static void DeleteMap(int id)
        {
            using (IDbConnection cnn = new SQLiteConnection(Connection.LoadConnectionString()))
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("id", id, DbType.Int32, ParameterDirection.Input);
                cnn.Execute("delete from Map where map_id = :id", parameters);
            }
        }

        public static void DeleteNoteCategory(int id)
        {
            using (IDbConnection cnn = new SQLiteConnection(Connection.LoadConnectionString()))
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("id", id, DbType.Int32, ParameterDirection.Input);
                cnn.Execute("delete from [Note Category] where category_id = :id", parameters);
            }
        }

        public static void DeletePin(int id)
        {
            using (IDbConnection cnn = new SQLiteConnection(Connection.LoadConnectionString()))
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("id", id, DbType.Int32, ParameterDirection.Input);
                cnn.Execute("delete from Pin where pin_id = :id", parameters);
            }
        }

        public static void DeleteNote(int noteId,int pinId)
        {
            using (IDbConnection cnn = new SQLiteConnection(Connection.LoadConnectionString()))
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("id1", noteId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("id2", pinId, DbType.Int32, ParameterDirection.Input);
                cnn.Execute("delete from pin_note where note_id = :id1 and pin_id = :id2", parameters);
            }
        }

        public static void DeleteRandomGenerator(int id)
        {
            using (IDbConnection cnn = new SQLiteConnection(Connection.LoadConnectionString()))
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("id", id, DbType.Int32, ParameterDirection.Input);
                cnn.Execute("delete from [Random Generator] where rng_id = :id", parameters);
            }
        }

        
    }
}