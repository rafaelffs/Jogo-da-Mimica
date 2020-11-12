using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Models
{
    [System.Serializable]
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Download_Url { get; set; }
        public string FormattedName { get => Name.Replace(".txt", ""); }

        //public static List<Category> GetCategories()
        //{
        //    List<Category> categories = new List<Category>()
        //    {
        //        new Category(){ Id = 1, Name= "Movies"},
        //        new Category(){ Id = 2, Name= "Food"},
        //        new Category(){ Id = 3, Name= "Objects"}
        //    };
        //    return categories;
        //}
    }
}
