using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SBD_PROJEKT2
{
    internal class Primary
    {
        public Index index;
        public Overflow overflow;
        public string filename = "primary.dat";
        public int read_counter { get; set; }
        public int write_counter { get; set; }
        public int page_size { get; set; }
        public int number_of_pages { get; set; }
        public List<Record> Page { get; set; }
        public int offset { get; set; }
        public int record_size { get; set; }
        public Primary(int numer_of_pages, int page_size,Index index,Overflow overflow, int record_size)
        {
            this.page_size = page_size;
            this.number_of_pages = numer_of_pages;
            this.index = index;
            this.overflow = overflow;
            this.Page = new List<Record>();
            this.read_counter = 0;
            this.write_counter = 0;
            this.record_size = record_size;
        }

        public void Insert(Record record)
        {
            Page = Read(record.key);
            bool inserted = false;

            for (int i = 0; i < page_size - 1; i++)
            {
                if (record.key >= Page[i].key && record.key < Page[i + 1].key)
                {
                    record = overflow.add(record, Page[i]);
                    if (record != null)
                        Write(record.key);
                    inserted = true;
                    break;
                }
            }
            if (!inserted)
            {
                for (int i = 0; i < page_size; i++)
                {
                    if (Page[i].key == 0)
                    {
                        Page[i] = record;
                        Write(record.key);
                        inserted = true;
                        break;
                    }
                }
            }
            if(!inserted)
            {
                record =  overflow.add(record, Page[Page.Count-1]);
                if(record != null)
                    Write(record.key);
                inserted = true;
            }
        }
        public void Remove(int key)
        {
            Page = Read(key);
            for (int i = 0; i < page_size; i++)
            {
                if (Page[i].key == key)
                {
                    Page[i].deleted = 1;
                }
                else
                {
                    overflow.remove(Page[i],key);
                }
            }
        }
        public void Update(int oldkey, int newkey, uint value1, uint value2)
        {
            if (oldkey == newkey)
            {
                Page = Read(newkey);
                for (int i = 0; i < page_size; i++)
                {
                    if (Page[i].key == newkey)
                    {
                        Page[i].radius = value1;
                        Page[i].angle = value2;
                    }
                }
            }
            else
            {
                Remove(oldkey);
                Insert(new Record(newkey,value1,value2,0,0));
            }
        }
        public bool PageFull()
        {
            for (int i = 0; i < page_size; i++)
            {
                if (Page[i].key == 0)
                    return false;
            }
            return true;
        }

        public List<Record> Read(int key)
        {
            Page.Clear();
            for (int i = 0; i < page_size; i++)
            {
                Page.Add(new Record(0, 0, 0, 0, 0));
            }
            int page_number = index.BruteSearch(key);
            using (BinaryReader reader = new BinaryReader(File.Open(filename, FileMode.OpenOrCreate)))
            {
                reader.BaseStream.Position = page_size*(page_number-1)*record_size;
                for (int i = 0; i < page_size; i++)
                {
                    int tmp_key = reader.ReadInt32();
                    uint value1 = reader.ReadUInt32();
                    uint value2 = reader.ReadUInt32();
                    int pointer = reader.ReadInt32();
                    byte deleted = reader.ReadByte();
                    Page[i] = new Record(tmp_key, value1, value2, pointer, deleted);
                }
            }
            read_counter++;
            return Page;
        }
        public void Write(int key)
        {
            int page_number = index.BruteSearch(key);
            using (BinaryWriter writer = new BinaryWriter(File.Open(filename, FileMode.OpenOrCreate)))
            {
                writer.BaseStream.Position = page_size*(page_number-1) * record_size;
                for (int i = 0; i < page_size; i++)
                {
                    writer.Write(Page[i].key);
                    writer.Write(Page[i].radius);
                    writer.Write(Page[i].angle);
                    writer.Write(Page[i].pointer);
                    writer.Write(Page[i].deleted);
                }
            }
            write_counter++;
        }
    }
}
