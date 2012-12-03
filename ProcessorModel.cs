using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ProcessorModel
{
    class Program
    {
        public static String[] Memory = new String[256];
        const int WireCount = 23;
        static int Main(string[] args)
        {
            StreamReader fin = new StreamReader("input.txt");
            int i=0;
            while(!fin.EndOfStream)
            {
                Memory[i] = fin.ReadLine();
                i++;
                if (i == 255) break;
            }
            fin.Close();
            StreamReader config = new StreamReader("config.txt");
            for(i=0; i<WireCount;i++)
            {
                wires_mask[i]=(config.Read()-'0');
            }
            config.Close();
            for (i = 0; i < WireCount; i++)
            {
                wires[i] = 0;
            }
          
            while (true)
            {
                MemoryPointer();
                CommandRegister();
                Plus();
                MemoryPointer();
                CommandDecoder();
                ALU();
                IndexRegister();
                GeneralPurposeRegister();
                IORegister();
                CommandPointer();
                MemWrite();
                if (wires[1] * wires_mask[1] == 0)
                    return 0;
            }
        }

        private static void MemWrite()
        {
            if (wires[8] * wires_mask[8]==1) 
            {
                string digits = "0123456789ABCDEF";
                var x = new StringBuilder(4);
                string com = "";
                int a = wires[16]*wires_mask[16];
                for (int i = 3; i > -1; i--)
                    {
                        x[i] = digits[a % 16];
                        a >>= 4;
                    }
                com = x.ToString();
                    Memory[(wires[14] * wires_mask[14])] = com;
            }
        }
        public static int [] wires = new int[WireCount];
        public static int [] wires_mask = new int[WireCount];
        public static int CommandPointer() 
        {
            wires[10] = Multyplexor(wires[9] * wires_mask[9], wires[10] * wires_mask[10] + 1, wires[14] * wires_mask[14], 0);
            return 0;
        }
        public static int CommandRegister() 
        {
            int com = wires[11] * wires_mask[11];
            wires[13] =  com & 0xFF;
            wires[12] = com >> 8;
            return 0;
        }

        public static int CommandDecoder()
        {
            int cop = wires[12] * wires_mask[12];
            int p = 0;
            int i = 0;
            switch (cop)
            {
                case 0: p = 0; break;
                case 0x11: i = 0; p = 1; break;
                case 0x15: i = 1; p = 1; break;
                case 0x02: p = 2; break;
                case 0x21: i = 0; p = 1; break;
                case 0x25: i = 1; p = 1; break;
                case 0x31: i = 0; p = 1; break;
                default: p = 4; break;                    
            }
            //int p = cop & 0x7;
            //int i = ( cop >> 2 ) & 0x7 ;
            wires[1] = Convert.ToInt16((wires[12]*wires_mask[12])  != 0xFF);
            wires[2] = Convert.ToInt16(p == 3);
            wires[3] = Convert.ToInt16(p == 1);
            wires[4] = Convert.ToInt16(p != 3);
            wires[5] = Convert.ToInt16(!(p == 2 || p == 3));
            wires[6] = cop >> 4;
            wires[7] = i;
            wires[8] = Convert.ToInt16(p == 0);
            if ((cop >> 4) == 0xF)
            {
                if (
                    (cop == 0xF0 && ((wires[21] * wires[21]) == 0))
                    || (cop == 0xF1 && (wires[21] * wires_mask[21] > 0))
                    || (cop == 0xF4 && (wires[19] * wires_mask[19] == 0))
                    || (cop == 0xF5 && (wires[19] * wires_mask[19] == 1))
                    )
                    wires[9] = 1;
                else wires[9] = 0;
            }
            return 0;
        }
        public static int Multyplexor(int f,int w1, int w2, int w3)
        {
            switch (f)
            {
                case 0: return w1;
                case 1: return w2;
                case 2: return w3;
                default: return 0;
            }
        }

        public static int Plus() 
        { 
            wires[14] = wires[22] * wires_mask[22] + wires[13] * wires_mask[13]; 
            wires[14] &= 0xFF;  
            return 0; 
        }
        public static int ALU() 
        {
            switch (wires[6] * wires_mask[6]) 
            {
                case 0: 
                    { 
                        wires[16]=wires[20]*wires_mask[20];
                        wires[17]=Convert.ToInt32(wires[16]>0);
                        return 0;
                    }
                case 1:
                    {
                        wires[16]=Multyplexor(wires[7]*wires_mask[7],wires[15]*wires_mask[15],wires[14]*wires_mask[14],wires[20]*wires_mask[20]);
                        wires[17]=Convert.ToInt32(wires[16]>0);
                        return 0;
                    }
                case 2:
                    {
                        wires[16]=0xFFFF&(wires[20]*wires_mask[20]+Multyplexor(wires[7]*wires_mask[7],wires[15]*wires_mask[15],wires[14]*wires_mask[14],wires[20]*wires_mask[20]));
                        wires[17]=Convert.ToInt32(wires[16]>0);
                        return 0;
                    }
                case 3:
                    {
                        wires[16] = 0xFFFF & (wires[20] * wires_mask[20] - Multyplexor(wires[7] * wires_mask[7], wires[15] * wires_mask[15], wires[14] * wires_mask[14], wires[20] * wires_mask[20]));
                        wires[17]=Convert.ToInt32(wires[16]>0);
                        return 0;
                    }
                case 0xF: return 0; // непонятно что выставлять
                default: return 0;;
            }
            //return 0;
        }
        public static int IndexRegister() 
        {
            if (wires[4] * wires_mask[4] != 0) 
            {
                wires[22] = Multyplexor(wires[5] * wires_mask[5], wires[16] * wires_mask[16], 0, 0);
            }
            return 0;
        }
        public static int GeneralPurposeRegister() 
        {
            if (wires[3] * wires_mask[3] != 0) 
            {
                wires[20] = wires[16] * wires_mask[16];
                wires[21] = wires[17] * wires_mask[17];
            }
            return 0;
        }
        public static int IORegister()
        {
            wires[19] = 1;
            return 0;
        }
        public static int MemoryPointer()
        {
            String command = Memory[wires[10]*wires_mask[10]];
            string digits = "0123456789ABCDEF";
            wires[11] = 0;
            for (int i = 0; i < 4; i++)
            {
                wires[11] *= 16;
                wires[11] += digits.LastIndexOf(command[i]);
            }
            command = Memory[wires[14] * wires_mask[14]];
            digits = "0123456789ABCDEF";
            wires[15] = 0;
            for (int i = 0; i < 4; i++)
            {
                wires[15] *= 16;
                wires[15] += digits.LastIndexOf(command[i]);
            }
            return 0;
        }
    }
}
