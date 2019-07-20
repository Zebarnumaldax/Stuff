using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public class Test
    {
        //Площадь круги
        static public double Sq (double r)
        {
            return 3.14 * r * r;
        }

        //Площадь треугольника с проверкой на "прямоугольность"
        static public double Sq (double a, double b, double c)
        {
            if (a > b && a > c)
            {
                if (a*a==(b*b+c*c))
                {
                    return 0.5 * a * b;
                }
            }
            else if (b > a && b > c)
            {
                if (b * b == (a * a + c * c))
                {
                    return 0.5 * a * c;
                }
            }
            else if(c>a&&c>b)
            {
                if (c * c == (a * a + b * b))
                {
                    return 0.5 * a * c;
                }

            }
                double p = (a + b + c) / 2;
                return Math.Sqrt(p * (p - a) * (p - b) * (p - c));
        }
    }
}
