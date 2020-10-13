using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TestStore_server.Models;

namespace TestStore_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        List<User> usersList = new List<User> { new User { email= "sa@gmail.com", firstName= "s", lastName= "a", password= "alexey", role= "admin" },
        new User{email="alexey.yurchenko.itp@gmail.com", firstName= "Alexey", lastName= "Yurchenko", password= "alexey", role= "user" } };

        [HttpGet]
        public List<User> GetUsers()
        {
            return usersList;
        }


    }   
}
