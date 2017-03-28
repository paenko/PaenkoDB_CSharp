using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaenkoDB
{
    static public class UuidManager
    {
        static List<UuidObject> DBUuids = new List<UuidObject>();

        /// <summary>
        /// Load the content for the UuidManager
        /// </summary>
        /// <param name="file">The filename of the configuration</param>
        public static void Load(string file)
        {
            if (File.Exists(file))
            {
                List<UuidObject> Uuids;
                using (FileStream fs = new FileStream(file, FileMode.Open))
                using (StreamReader sr = new StreamReader(fs))
                {
                    string json = sr.ReadToEnd();
                    Uuids = JsonConvert.DeserializeObject<List<UuidObject>>(json);
                }
                DBUuids = Uuids;
            }
            else
            {
                throw new FileNotFoundException() { };
            }
        }

        /// <summary>
        /// Save the content of the UuidManager
        /// </summary>
        /// <param name="file">The filename of the configuration</param>
        public static void SaveIds(string file)
        {
            using (FileStream fs = new FileStream(file, FileMode.Create))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                string json = JsonConvert.SerializeObject(DBUuids);
                sw.Write(json);
            }
        }

        /// <summary>
        /// Add a Uuid with a description to use later
        /// </summary>
        /// <param name="id">The Uuid that will be accessable through the description</param>
        /// <param name="description">The description that will can be accessable through the Uuid</param>
        /// <param name="type">Optional type for either a log or a file</param>
        public static void Add(string id, string description, UuidObject.UuidType type)
        {
            if (DBUuids.Where(u => { if (u.Id == id || u.Description == description) { return true; } else { return false; } }).Count() == 0)
            DBUuids.Add(new UuidObject() { Description = description, Id = id, Type = type });
        }

        /// <summary>
        /// Return a Uuid that has the specified description
        /// </summary>
        /// <param name="description">The description that is connected with the Uuid</param>
        /// <returns>The Uuid wich is searched after</returns>
        public static UuidObject LookForId(string description)
        {
            var x = DBUuids.Where(u => { if (u.Description == description) { return true; } else { return false; } }).ToList();
            if (x.Count != 0)
            { return x.First(); }
            else
            { return null; }
        }

        /// <summary>
        /// Return a description that is connected to the specified Uuid
        /// </summary>
        /// <param name="id">The Uuid that is connected to the description</param>
        /// <returns>The description which is searched after</returns>
        public static UuidObject LookForDescription(string id)
        {
            var x = DBUuids.Where(u => { if (u.Id == id) { return true; } else { return false; } }).ToList();
            if (x.Count != 0)
            { return x.First(); }
            else
            { return null; }
        }

        /// <summary>
        /// Return All UuidObjects in the UuidObject list. A type can be specified to narrow down the output.
        /// </summary>
        /// <param name="type">The type of UuidObject you want to return</param>
        /// <returns>All UuidObjects in the list with a specified type</returns>
        public static List<UuidObject> LookAll(UuidObject.UuidType type = UuidObject.UuidType.All)
        {
            var x = DBUuids.Where(u => { if (u.Type == type | type == UuidObject.UuidType.All) { return true; } else { return false; } }).ToList();
            if (x.Count != 0)
            { return x; }
            else
            { return null; }
        }
    }
}
