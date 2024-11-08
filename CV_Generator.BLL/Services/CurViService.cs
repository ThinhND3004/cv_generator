using CV_Generator.DAL.Entities;
using CV_Generator.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV_Generator.BLL.Services
{
    public class CurViService
    {
        private CurViRepository _repo = new();

        public void CreateCV(CurriculumVitae curriculumVitae)
        {
            _repo.Create(curriculumVitae);
        }

        public List<CurriculumVitae> GetCVsByLoginUserId(int id) 
        {
            return _repo.GetByUserId(id);
        }
    }
}
