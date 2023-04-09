using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public interface IStudentskaSluzba
    {
        void DodajStudenta(Student student, string token);
        void IzmeniStudenta(Student student, string token);
        bool PronadjiStudenta(string index, out Student student, string token);
        bool IzbrisiStudenta(string index, string token);
    }
}
