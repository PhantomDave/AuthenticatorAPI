using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PokerLibrary.Models;

namespace GameAPI.Helpers
{
    public class HttpHelper
    {
        private string Url { get; set; }
        private HttpClient HttpClient { get; set; }

        public HttpHelper(string url)
        {
            Url = url;
            HttpClient = new HttpClient();
        }

        public async Task<string> ValidateUserEmailAsync(string email)
        {
            try
            {
                HttpResponseMessage response = await HttpClient.GetAsync(Url + "/login/" + email);
                Console.WriteLine(Url + "/login/" + email);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception was registred! " + ex.StackTrace);
                return "";
            }
        }

        public async Task<string> LoginUserAsync(string email, string password)
        {
            dynamic user = new { Email = email, Password = password };

            string json = JsonConvert.SerializeObject(user);

            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await HttpClient.PostAsync(Url + "/login/", content);
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                return responseContent;
            }
            return "";
        }

        public async Task<bool> IsUserAuthenticatedAsync(string token)
        {
            HttpResponseMessage response = await HttpClient.GetAsync(
                Url + "/checkauth/?token=" + token
            );
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }
    }
}
