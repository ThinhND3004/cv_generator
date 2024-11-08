using CV_Generator.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV_Generator.DAL.Repositories
{
    public class CurViRepository
    {
        private CvgeneratorDbContext _db;

        public void Create(CurriculumVitae cv)
        {
            _db = new CvgeneratorDbContext();
            _db.Add(cv);
            _db.SaveChanges();
        }

        public List<CurriculumVitae> GetAll()
        {
            _db = new();
            return _db.CurriculumVitaes.ToList();
        }

        public List<CurriculumVitae> GetByUserId(int userId)
        {
            _db = new();
            return _db.CurriculumVitaes.Where(x => x.CreateBy == userId).ToList();
        }
    }
}
