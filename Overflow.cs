using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SBD_PROJEKT2
{
    internal class Overflow
    {
        public string filename = "overflow.dat";
        public int stack_size = 0;
        public int read_counter { get; set; }
        public int write_counter { get; set; }
        public  int record_size { get; set; }
        public long position { get; set; }
        public int page_size { get; set; }
        public int stack_index { get; set; }
        public Overflow(int record_size, int page_size)
        {
            this.record_size = record_size;
            this.page_size = page_size;
        }

        public Record add(Record record,Record anchor)
        {
            if(anchor.pointer == 0)
            {
                Write(record,stack_size,true);
                anchor.pointer = stack_size;
                return anchor;
            }
            else
            {
                Record first = anchor;
                Record second = Read(anchor.pointer);
                int oldanchor = anchor.pointer - 1;
                if (record.key >= first.key && record.key < second.key)
                {
                    record.pointer = anchor.pointer;
                    Write(record, stack_size, true);
                    anchor.pointer = stack_size;
                    second.pointer = 0;
                    Write(second, oldanchor, false);
                    return anchor;
                }
                else
                {
                    while (true)
                    {
                        int old_index = anchor.pointer - 1;
                        Record current = Read(anchor.pointer);
                        if (current.pointer == 0)
                        {
                            Write(record, stack_size, true);
                            current.pointer = stack_size;
                            Write(current, old_index, false);
                            break;
                        }
                        Record next = Read(current.pointer);
                        if (record.key >= current.key && record.key < next.key)
                        {
                            int nextrecordidx = current.pointer - 1;
                            record.pointer = current.pointer;
                            Write(record, stack_size, true);
                            current.pointer = stack_size;
                            Write(current, old_index, false);
                            next.pointer = 0;
                            Write(next, nextrecordidx, false);
                            break;
                        }
                        anchor = current;
                    }
                    return null;
                }
            }
        }

        public void remove(Record anchor, int key)
        {
            //dodać czytanie stronami
            if (anchor.pointer != 0)
            {
                while (true)
                {
                    int record_index = anchor.pointer - 1;
                    Record record = Read(anchor.pointer);
                    if (record.key == key)
                    {
                        record.deleted = 1;
                        Write(record, record_index, false);
                        break;
                    }
                    else if (record.pointer == 0)
                        break;
                    anchor = record;
                }
            } 
        }
        public List<Record> GetChain(Record anchor)
        { 
            //dodać czytanie stronami
            List<Record> chain = new List<Record>();
            while (true)
            {
                if(anchor.pointer == 0)
                    break;
                Record record = Read(anchor.pointer);
                Record tmp = new Record(0, 0, 0, 0, 0);
                tmp = CopyRecord(record);
                tmp.pointer = 0;
                if(tmp.deleted == 0)
                    chain.Add(tmp);
                anchor = record;
            }
            return chain;
        }
        public Record Read(int index)
        {
            position = 0;
            stack_index = 0;
            while(true)
            {
                List<Record> Page = ReadPage();
                if (index > stack_index && index <= stack_index + 3)
                {
                    for (int i = 0; i < Page.Count; i++)
                    {
                        if(stack_index + i == index - 1)
                            return Page[i];
                    }
                }
                stack_index += 3;
            }
        }
        public List<Record> ReadPage()
        {
            List<Record> Page = new List<Record>();
            using (BinaryReader reader = new BinaryReader(File.Open(filename, FileMode.OpenOrCreate)))
            {
                reader.BaseStream.Position = position;
                for (int i = 0; i < page_size; i++)
                {
                    if(reader.BaseStream.Position + record_size <= reader.BaseStream.Length)
                    {
                        int key = reader.ReadInt32();
                        uint value1 = reader.ReadUInt32();
                        uint value2 = reader.ReadUInt32();
                        int pointer = reader.ReadInt32();
                        byte deleted = reader.ReadByte();
                        Page.Add(new Record(key, value1, value2, pointer, deleted));
                    }
                }
                position = reader.BaseStream.Position;
            }
            read_counter++;
            return Page;
        }
        public void Write(Record record, int index, bool newrecord)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(filename, FileMode.OpenOrCreate)))
            {
                writer.BaseStream.Position = (index) * record_size;
                writer.Write(record.key);
                writer.Write(record.radius);
                writer.Write(record.angle);
                writer.Write(record.pointer);
                writer.Write(record.deleted);
            }
            if (newrecord)
                stack_size++;

            write_counter++;
        }
        public Record CopyRecord(Record old)
        {
            Record newrecord = new Record(old.key,old.radius,old.angle,old.pointer,old.deleted);
            return newrecord;
        }
    }
}
