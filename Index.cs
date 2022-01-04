using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SBD_PROJEKT2
{
    internal class Index
    {
        public string filename = "index.dat";
        public string new_filename = "newindex.dat";
        public int number_of_pages { get; set; }
        public int page_size { get; set; }
        public int idx_page_size { get; set; }
        public int read_counter { get; set; }
        public int write_counter { get; set; }
        public int new_index_size { get; set; }
        public int record_size { get; set; }
        public List<Tuple<int,int>> page = new List<Tuple<int, int>>();

        public Index(int number_of_pages, int page_size, int idx_page_size, int record_size)
        {
            this.number_of_pages = number_of_pages;
            this.page_size = page_size;
            this.idx_page_size = idx_page_size;
            this.read_counter = 0;
            this.write_counter = 0;
            this.record_size = record_size;
            this.new_index_size = 0;
        }

        public void AddNewRecord(int new_page_number, int first_key_of_page)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(new_filename, FileMode.OpenOrCreate)))
            {
                writer.BaseStream.Position = new_index_size * 8;
                writer.Write(first_key_of_page);
                writer.Write(new_page_number);
                new_index_size++;
            }
        }


        public int FindPageNumber(int key)
        {
            return BruteSearch(key);
        }

        public void BisectForPage()
        {
            int middle = 0, start = 0;
            if (number_of_pages % 2 == 0)
            {
                middle = number_of_pages / 2;
                start = middle - (idx_page_size / 2 - 1);
            }
            else
            {
                middle = number_of_pages / 2 + 1;
                start = middle - idx_page_size / 2;
            }
        }

        public void BisectForRecord()
        {

        }

        public int Read(int start)
        {
            page.Clear();
            using (BinaryReader reader = new BinaryReader(File.Open(filename, FileMode.OpenOrCreate)))
            {
                reader.BaseStream.Position = start*8;
                if (reader.BaseStream.Position + 16 > reader.BaseStream.Length)
                    return 0;
                for (int i = 0; i < idx_page_size; i++)
                {
                    int key = reader.ReadInt32();
                    int pageno = reader.ReadInt32();
                    page.Add(new Tuple<int, int>(key, pageno));
                }
            }
            read_counter++;
            return 1;
        }

        public int BruteSearch(int key)
        {
            int current_number = 0, current_key, next_key, next_number;
            Read(0);
            if (key == -1)
                return 1;
            for (int i = 0; i < number_of_pages; i++)
            {
                current_key = page[0].Item1;
                current_number = page[0].Item2;
                page.RemoveAt(0);
                next_key = page[0].Item1;
                next_number = page[0].Item2;
                if (page.Count == 1)
                {
                    if (Read(i + 1) == 0)
                    {
                        if (current_key <= key && key < next_key)
                            return current_number;
                        else
                            return current_number + 1;
                    }
                        
                }
                    
                if (current_key <= key && key < next_key)
                {
                    return current_number;
                }
            }
            return current_number;
        }
        public int BruteSearchRecord(int key)
        {
            return 0;
        }
    }
}
