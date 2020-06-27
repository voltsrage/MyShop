using MyShop.Core.Contracts;
using MyShop.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.WebUI.Tests.Mocks
{
    class MockContent<T> : IRepository<T> where T : BaseEntity
    {
        List<T> items;
        string classname;

        public MockContent()
        {
            items = new List<T>();
        }

        public void Commit()
        {
            return;
        }

        public void Insert(T t)
        {
            items.Add(t);
        }

        public void Update(T t)
        {
            T tToUpdate = items.Find(p => p.Id == t.Id);

            if (tToUpdate != null)
            {
                tToUpdate = t;
            }
            else
            {
                throw new Exception(classname + " not found!");
            }
        }

        public T Find(string Id)
        {
            T t = items.Find(p => p.Id == Id);

            if (t != null)
            {
                return t;
            }
            else
            {
                throw new Exception(classname + " not found!");
            }
        }

        public IQueryable<T> Collection()
        {
            return items.AsQueryable();
        }

        public void Delete(string Id)
        {
            T tToDelete = items.Find(p => p.Id == Id);

            if (tToDelete != null)
            {
                items.Remove(tToDelete);
            }
            else
            {
                throw new Exception(classname + " not found!");
            }
        }
    }
}
