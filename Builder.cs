using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SBD_PROJEKT2
{
    internal class Builder
    {
        public Primary primary;
        public Index index;
        public Overflow overflow;

        public string filename = "organizer.dat";
        public int primary_page_size { get; set; }
        public int index_page_size { get; set; }
        public int number_of_pages { get; set; }
        public int record_size { get; set; }
        public double alpha { get; set; }
        public int page_fill { get; set; }
        public int records_on_page { get; set; }
        public long organizer_position { get; set; }
        public int new_number_of_pages { get; set; }
        public Builder(int primary_page_size, int index_page_size, int number_of_pages, int record_size)
        {
            this.primary_page_size = primary_page_size;
            this.index_page_size = index_page_size;
            this.number_of_pages = number_of_pages;
            this.record_size = record_size;
            this.overflow = new Overflow(record_size,primary_page_size);
            this.index = new Index(number_of_pages, primary_page_size, index_page_size, record_size);
            this.primary = new Primary(number_of_pages, primary_page_size, index, overflow,record_size);
            this.alpha = 0.5;
            this.page_fill = (int)Math.Ceiling(primary_page_size * alpha);
            this.records_on_page = 0;
            this.new_number_of_pages = 1;
            this.organizer_position = 0;
            File.Delete(primary.filename);
            File.Delete(overflow.filename);
            File.Delete(index.filename);
            File.Delete(filename);
        }

        public void Build()
        {
            using (BinaryWriter writer1 = new BinaryWriter(File.Open(primary.filename, FileMode.OpenOrCreate)))
            {
                using (BinaryWriter writer2 = new BinaryWriter(File.Open(index.filename, FileMode.OpenOrCreate)))
                {
                    for (int i = 0; i < number_of_pages; i++)
                    {
                        for (int j = 0; j < primary_page_size; j++)
                        {
                            writer1.Write(0); //KEY
                            writer1.Write(0); //RADIUS
                            writer1.Write(0); //ANGLE
                            writer1.Write(0); //POINTER
                            writer1.Write((byte)0); //DELETED
                        }
                        writer2.Write((i) * 10);
                        writer2.Write(i+1);
                    }
                }
            }
            AddSpecialRecord();
        }
        public void Reorganization()
        {
            List<Record> Records = new List<Record>();
            using (BinaryReader reader = new BinaryReader(File.Open(primary.filename, FileMode.OpenOrCreate)))
            {
                int page_counter = 0;
                while (true)
                {
                    if(page_counter == number_of_pages)
                        break;
                    for (int i = 0; i < primary_page_size; i++) //Read page from primary
                    {
                        int tmp_key = reader.ReadInt32();
                        uint value1 = reader.ReadUInt32();
                        uint value2 = reader.ReadUInt32();
                        int pointer = reader.ReadInt32();
                        byte deleted = reader.ReadByte();
                        if(tmp_key != 0 && deleted == 0)
                        {
                            Record record = new Record(tmp_key, value1, value2, pointer, deleted);
                            Records.Add(record);
                            if (record.pointer != 0)
                            {
                                Records.AddRange(overflow.GetChain(record));
                                record.pointer = 0;
                            }
                        }
                    }
                    AddToNewPage(Records);
                    Records.Clear();
                    page_counter++;
                }
                FixLastPage();
            }
            File.Delete(primary.filename);
            File.Delete(index.filename);
            File.Move(filename, primary.filename);
            File.Move(index.new_filename, index.filename);
            File.Delete(overflow.filename);
            overflow.stack_size = 0;
            number_of_pages = new_number_of_pages;
        }
        public void FixLastPage()
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(filename, FileMode.OpenOrCreate)))
            {
                writer.BaseStream.Position = organizer_position;
                if (records_on_page > 0)
                {
                    for (int i = 0; i < primary_page_size - records_on_page; i++)
                    {
                        writer.Write(0);
                        writer.Write(0);
                        writer.Write(0);
                        writer.Write(0);
                        writer.Write((byte)0);
                    }
                }
                records_on_page = 0;
            }

        }
        public void AddToNewPage(List<Record> page)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(filename, FileMode.OpenOrCreate)))
            {
                writer.BaseStream.Position = organizer_position;
                for (int i = 0; i < page.Count; i++)
                {
                    if (records_on_page == 0)
                    {
                        if(page[i].key == -1)
                            index.AddNewRecord(new_number_of_pages, 0);
                        else
                            index.AddNewRecord(new_number_of_pages, page[i].key);
                    }
                        
                    if (records_on_page < page_fill)
                    {
                        writer.Write(page[i].key);
                        writer.Write(page[i].radius);
                        writer.Write(page[i].angle);
                        writer.Write(page[i].pointer);
                        writer.Write(page[i].deleted);
                        records_on_page++;
                    }
                    else
                    {
                        i--;
                        for (int j = 0; j < primary_page_size-page_fill; j++)
                        {
                            writer.Write(0);
                            writer.Write(0);
                            writer.Write(0);
                            writer.Write(0);
                            writer.Write((byte)0);
                        }
                        records_on_page = 0;
                        new_number_of_pages++;
                    }
                }
                organizer_position = writer.BaseStream.Position;
            }
        }
        public void AddSpecialRecord()
        {
            int page_number = 1;
            using (BinaryWriter writer = new BinaryWriter(File.Open(primary.filename, FileMode.OpenOrCreate)))
            {
                writer.BaseStream.Position = (page_number-1) * record_size;
                writer.Write(-1);
                writer.Write(0);
                writer.Write(0);
                writer.Write(0);
                writer.Write((byte)0);
            }
            primary.write_counter++;
        }
        public void Display()
        {
            Console.Clear();
            using (BinaryReader reader1 = new BinaryReader(File.Open(primary.filename, FileMode.OpenOrCreate)))
            {
                Console.WriteLine("Primary table:");
                Console.WriteLine("key|radius|angle|pointer|deleted");
                for (int i = 1; i <= number_of_pages; i++)
                {
                    Console.WriteLine("----------------------");
                    Console.WriteLine("Page number " + i);
                    for (int j = 0; j < primary_page_size; j++)
                    {
                        int key = reader1.ReadInt32();
                        uint value1 = reader1.ReadUInt32();
                        uint value2 = reader1.ReadUInt32();
                        int pointer = reader1.ReadInt32();
                        byte deleted = reader1.ReadByte();
                        //MOGE ZROBIC NEW RECORD I METODĘ TOSTRING
                        if(key == 0)
                        {
                            Console.Write("X" + " ");
                            Console.Write("X" + " ");
                            Console.Write("X" + " ");
                            Console.Write("V" + " ");
                            Console.Write("NOT" + " ");
                        }
                        else
                        {
                            Console.Write(key+ " ");
                            Console.Write(value1 + " ");
                            Console.Write(value2 + " ");
                            if (pointer == 0)
                                Console.Write("V" + " ");
                            else
                                Console.Write(pointer + " ");
                            if(deleted == 1)
                                Console.Write("YES ");
                            else
                                Console.Write("NOT ");
                        }
                        Console.WriteLine();
                    }
                }
            }
            using (BinaryReader reader2 = new BinaryReader(File.Open(overflow.filename, FileMode.OpenOrCreate)))
            {
                Console.WriteLine();
                Console.WriteLine("Overflow table:");
                Console.WriteLine("key|radius|angle|pointer|deleted");
                for (int i = 0; i < overflow.stack_size; i++)
                {
                    int key = reader2.ReadInt32();
                    uint value1 = reader2.ReadUInt32();
                    uint value2 = reader2.ReadUInt32();
                    int pointer = reader2.ReadInt32();
                    byte deleted = reader2.ReadByte();
                    //MOGE ZROBIC NEW RECORD I METODĘ TOSTRING
                    if (key == 0)
                    {
                        Console.Write("X" + " ");
                        Console.Write("X" + " ");
                        Console.Write("X" + " ");
                        Console.Write("V" + " ");
                        Console.Write("NOT" + " ");
                    }
                    else
                    {
                        Console.Write(key + " ");
                        Console.Write(value1 + " ");
                        Console.Write(value2 + " ");
                        if (pointer == 0)
                            Console.Write("V" + " ");
                        else
                            Console.Write(pointer + " ");
                        if (deleted == 1)
                            Console.Write("YES ");
                        else
                            Console.Write("NOT ");
                    }
                    Console.WriteLine();
                }

            }
            using (BinaryReader reader3 = new BinaryReader(File.Open(index.filename, FileMode.OpenOrCreate)))
            {
                Console.WriteLine();
                Console.WriteLine("Index table:");
                Console.WriteLine("key|pageNo");
                for (int i = 0; i < number_of_pages; i++)
                {
                    int key = reader3.ReadInt32();
                    Console.Write(key + " ");
                    int number = reader3.ReadInt32();
                    Console.WriteLine(number + " ");
                }
                
            }
        }
    }
}
