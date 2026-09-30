//using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpongeBobTycoon.Model.FieldTypes;

namespace SpongeBobTycoon.Model
{
    public class FieldChangedEventArgs : EventArgs
    {
        public int x;
        public int y;
        public FieldType fieldType;

        public FieldChangedEventArgs(int x, int y, FieldType ft)
        {
            this.x = x; this.y = y; this.fieldType = ft;
        }
    }
}
