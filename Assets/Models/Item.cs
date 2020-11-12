using Assets.Connection;
using Assets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public class Item
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Category Category { get; set; }

    public static Item ReturnOneRandomItemsByCategory(string category)
    {
        List<Item> items = new List<Item>();
        //Category categoryMovie = new Category()
        //{
        //    Id = 1,
        //    Name = "Movies"
        //};

        //Category categoryObject = new Category()
        //{
        //    Id = 2,
        //    Name = "Object"
        //};

        //Category categoryFood = new Category()
        //{
        //    Id = 3,
        //    Name = "Food"
        //};
        //Item item = new Item()
        //{
        //    Id = 1,
        //    Name = "Mortal Kombat",
        //    Category = categoryMovie

        //};

        //Item item2 = new Item()
        //{
        //    Id = 2,
        //    Name = "Titanic",
        //    Category = categoryMovie
        //};

        //Item item3 = new Item()
        //{
        //    Id = 3,
        //    Name = "Banana",
        //    Category = categoryFood

        //};

        //Item item4 = new Item()
        //{
        //    Id = 4,
        //    Name = "Keyboard",
        //    Category = categoryObject
        //};

        //items.Add(item);
        //items.Add(item2);
        //items.Add(item3);
        //items.Add(item4);
        //items = items.Where(x => x.Category.Name.ToLower() == category.ToLower()).ToList();
        Random rnd = new Random();
        int r = rnd.Next(items.Count);
        return items[r];
    }


}
