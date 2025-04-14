using Microsoft.AspNetCore.Mvc;

namespace ToDoList.Helpers
{
    public static class Helper
    {
        public static async Task<bool> CheckTokenExpired(HttpResponseMessage resposne, Controller controller)
        {
            if(resposne.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                controller.TempData["Error"] = "Session expired. Please log in.";
                return true;
            }
            return false;
        }
    }
}
