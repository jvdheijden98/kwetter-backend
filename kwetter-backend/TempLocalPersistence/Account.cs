using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kwetter_backend.TempLocalPersistence
{
    public class Account
    {
        public int Id { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }

        public Account(int id, string username, string password)
        {
            Id = id;
            Username = username;
            Password = password;
        }
    }
}
