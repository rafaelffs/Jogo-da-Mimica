using Assets.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Connection
{
    public class Connection
    {
        private static readonly HttpClient client = new HttpClient();
        static string user = "rafaelffs";
        static string repository = "githubbuckets";
        static string bucketPath = "/bucket";
        static string branch = "master";
        static string category_url = $"https://api.github.com/repos/{user}/{repository}/contents{bucketPath}?ref={branch}";

        //public static async Task<List<Item>> ReturnRandomItemByCategories(List<Category> categories)
        //{
        //    client.DefaultRequestHeaders.Accept.Clear();
        //    client.DefaultRequestHeaders.Accept.Add(
        //        new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
        //    client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");


        //    foreach (Category category in categories)
        //    {
        //        string categoriesRawString = await client.GetStringAsync(category.Download_Url);
        //        string content = await client.GetStringAsync(category.Download_Url);
        //        string[] items = content.Split('\n');
        //        foreach (string item in items)
        //        {
        //            Console.WriteLine(item);
        //        }
        //    }
        //    return null;
        //}

        //public static async Task<List<Item>> ReturnItemsByCategories(List<Category> categories)
        //{
        //    List<Item> listItems = new List<Item>();
        //    client.DefaultRequestHeaders.Accept.Clear();
        //    client.DefaultRequestHeaders.Accept.Add(
        //        new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
        //    client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

        //    foreach (Category category in categories)
        //    {
        //        string content = await client.GetStringAsync(category.Download_Url);
        //        string[] items = content.Split('\n');
        //        foreach (string item in items)
        //        {
        //            if (item != "")
        //                listItems.Add(new Item() { Category = category, Name = item });
        //        }
        //    }

        //    return listItems;
        //}

        public static async Task<List<Category>> DownloadCategories()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

            string categoriesRawString = await client.GetStringAsync(category_url);
            List<Category> listCategories = JsonConvert.DeserializeObject<List<Category>>(categoriesRawString);
            return listCategories;
        }

        public static async Task<List<Item>> DownloadItemsByCategory(Category category)
        {
            List<Item> listItems = new List<Item>();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

            string content = await client.GetStringAsync(category.Download_Url);
            string[] items = content.Split('\n');
            foreach (string item in items)
            {
                if (item != "")
                    listItems.Add(new Item() { Category = category, Name = item });
            }

            return listItems;
        }
    }
}

