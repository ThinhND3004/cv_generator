using CV_Generator.DAL.Entities;
using CV_Generator.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV_Generator.BLL.Services
{
    public class AccountService
    {
        private AccountRepository _repo = new();

        public Account? GetAccountByEmail(string email)
        {
            return _repo.GetAccountByEmail(email);
        }

        public bool CheckLogin(Account account, string password)
        {
            bool result = false;
            if (account.Password == password)
            {
                result = true;
            }
            return result;
        }
    }
}
