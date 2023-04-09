using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public interface IReplikator
    {
        void Posalji(List<Student> studenti);
        List<Student> Preuzmi(DateTime vremeReplikacije);
    }

}
