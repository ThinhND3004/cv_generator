using CV_Generator.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV_Generator.DAL.Repositories
{
    public class AccountRepository
    {
        private CvgeneratorDbContext _context;

        public List<Account> GetAll()
        {
            _context = new CvgeneratorDbContext();
            return _context.Accounts.ToList();
        }

        public Account? GetAccountByEmail(string email)
        {
            _context = new CvgeneratorDbContext();
            Account account = _context.Accounts.FirstOrDefault(x => x.Email.ToLower() == email.ToLower());
            return account;
        }
    }
}
