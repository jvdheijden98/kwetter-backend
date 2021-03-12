using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kwetter_backend.TempLocalPersistence
{
    public class AccountStorage
    {
        private static List<Account> accounts = new List<Account>();

        public List<Account> GetAccounts()
        {
            return accounts;
        }

        public Account GetAccount(int id)
        {
            return accounts.Where(account => account.Id == id).FirstOrDefault();
        }

        public void AddAccount(int id, string username, string password)  // PostAccount
        {
            accounts.Add(new Account(id, username, password));
        }

        public void AddAccount(Account account) // PostAccount
        {
            accounts.Add(account);
        }

        public void DeleteAccount(int id)
        {
            Account accountToRemove = accounts.Where(account => account.Id == id).First();
            accounts.Remove(accountToRemove);
        }

        public bool AccountExists(int id)
        {
            bool accountExists = accounts.Where(account => account.Id == id).Any();
            if (accountExists)
            {
                return true;
            }
            return false;
        }
    }
}
