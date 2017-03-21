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

        public static void SaveIds(string file)
        {
            using (FileStream fs = new FileStream(file, FileMode.Create))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                string json = JsonConvert.SerializeObject(DBUuids);
                sw.Write(json);
            }
        }

        public static void Add(string id, string description, UuidObject.UuidType type)
        {
            if (DBUuids.Where(u => { if (u.Id == id || u.Description == description) { return true; } else { return false; } }).Count() == 0)
            DBUuids.Add(new UuidObject() { Description = description, Id = id, Type = type });
        }

        public static UuidObject LookForId(string description)
        {
            var x = DBUuids.Where(u => { if (u.Description == description) { return true; } else { return false; } }).ToList();
            if (x.Count != 0)
            { return x.First(); }
            else
            { return null; }
        }

        public static UuidObject LookForDescription(string id)
        {
            var x = DBUuids.Where(u => { if (u.Id == id) { return true; } else { return false; } }).ToList();
            if (x.Count != 0)
            { return x.First(); }
            else
            { return null; }
        }

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
