using System;

namespace SBD_PROJEKT2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int number_of_pages = 5;
            int record_size = 17;
            int primary_page_size = 3;
            int index_page_size = 2;
            Builder builder = new Builder(primary_page_size,index_page_size,number_of_pages,record_size);
            builder.Build();
            builder.Display();
            //builder.GetPrimary()
            //builder.primary.Insert(new Record(5, 9, 9, 0));
            builder.primary.Insert(new Record(10, 1, 1, 0, 1));
            builder.Display();
            builder.primary.Insert(new Record(20, 2, 2, 0, 1));
            builder.primary.Insert(new Record(30, 3, 3, 0, 1));
            builder.primary.Insert(new Record(34, 32, 3, 0, 0));
            builder.primary.Insert(new Record(40, 4, 4, 0, 1));
            //builder.primary.Insert(new Record(50, 5, 5, 0));
            builder.primary.Insert(new Record(6, 2, 3, 0, 0));
            builder.primary.Insert(new Record(3, 1, 4, 0, 0));
            builder.Display();
            builder.primary.Insert(new Record(60, 6, 6, 0, 0));
            builder.Display();
            builder.primary.Insert(new Record(70, 7, 7, 0, 0));
            builder.Display();
            builder.primary.Insert(new Record(65, 8, 8, 0, 0));
            builder.Display();
            builder.primary.Insert(new Record(75, 9, 9, 0, 0));
            builder.Display();
            builder.primary.Insert(new Record(25, 4, 4, 0, 0));
            builder.primary.Insert(new Record(24, 5, 4, 0, 0));
            builder.Display();
            builder.primary.Insert(new Record(72, 100, 100, 0, 0));
            builder.Display();

            builder.primary.Insert(new Record(22, 22, 22, 0, 0));
            builder.Display();
            builder.primary.Insert(new Record(26, 21, 37, 0, 0));
            builder.primary.Insert(new Record(27, 420, 420, 0, 0));
            builder.primary.Insert(new Record(29, 420, 420, 0, 0));
            builder.primary.Insert(new Record(28, 420, 420, 0, 0));
            builder.primary.Insert(new Record(11, 98, 67, 0, 0));
            builder.primary.Insert(new Record(15, 89, 76, 0, 0));
            builder.Display();
            builder.primary.Remove(65);
            builder.Display();
            builder.primary.Update(3,4,69,69);
            builder.Display();
            //builder.Reorganization();
            builder.Display();
           // builder.primary.Insert(new Record(40, 189, 2137, 0, 0));
            //builder.Display();
            /*
            
            */
        }
    }
}
