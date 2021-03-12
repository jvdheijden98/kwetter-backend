using kwetter_backend.TempLocalPersistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kwetter_backend.Logic
{
    public class AccountLogic
    {
        private readonly AccountStorage _accountStorage;

        public AccountLogic(AccountStorage accountStorage)
        {
            _accountStorage = accountStorage;
        }

        public List<Account> GetAccounts()
        {
            return _accountStorage.GetAccounts();
        }

        public Account GetAccount(int id)
        {
            Account account = _accountStorage.GetAccount(id);
            return account;
        }

        public Task PutAccount(Account account)
        {
            throw new NotImplementedException();
        }

        public void PostAccount(Account account)
        {
            _accountStorage.AddAccount(account);
        }

        public void DeleteAccount(int id)
        {
            _accountStorage.DeleteAccount(id);
        }

        public bool AccountExists(int id)
        {
            return _accountStorage.AccountExists(id);
        }
    }
}
