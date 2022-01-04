using System;
using System.Collections.Generic;
using System.Text;

namespace SBD_PROJEKT2
{
    internal class Record
    {
        public UInt32 radius { get; set; }
        public UInt32 angle { get; set; }
        public int key { get; set; }
        public int pointer { get; set; }
        public byte deleted { get; set; }
        public Record(int key, uint radius, uint angle, int pointer, byte deleted)
        {
            this.radius = radius;
            this.angle = angle;
            this.key = key;
            this.pointer = pointer;
            this.deleted = deleted;
        }
    }
}
