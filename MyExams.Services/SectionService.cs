﻿using MyExams.Database.Contracts;
using MyExams.Models;
using MyExams.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Services
{
    public class SectionService:ISectionService
    {
        private readonly ISectionRepository _sectionRepository;
        public SectionService(ISectionRepository sectionRepository)
        {
            _sectionRepository = sectionRepository;
        }
        public IEnumerable<Section> GetAll()
        {
            return _sectionRepository.GetAll();
        }
        public IEnumerable<Section> GetAllSectionsByTestId(int testId)
        {
<<<<<<< HEAD
<<<<<<< HEAD
            return _sectionRepository.Include(x => x.Test).Where(x => x.Test.Id == testId && x.Active).ToList();
=======
            return _sectionRepository.GetAll().Where(x => x.Test.Id == testId);
>>>>>>> parent of 37a36a8... GTest preview question order fixed, picture upload to section added
=======
            return _sectionRepository.GetAll().Where(x => x.Test.Id == testId);
>>>>>>> parent of 37a36a8... GTest preview question order fixed, picture upload to section added
        }
        public void AddSection(Section section)
        {
            _sectionRepository.Add(section);
            _sectionRepository.SaveChanges();
        }
        public void RemoveSection (Section section)
        {
            var sections = _sectionRepository.GetAll().Where(x => x.Test.Id == section.Test.Id);
            foreach (var item in sections)
            {
                if (item.OrderNo > section.OrderNo)
                {
                    item.OrderNo--;
                }
            }
            _sectionRepository.Remove(section);
            _sectionRepository.SaveChanges();
        }
        public void Update()
        {
            _sectionRepository.SaveChanges();
        }
    }
}
