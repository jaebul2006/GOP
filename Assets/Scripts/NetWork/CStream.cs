using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

 
    class CStream
    {
        private BUF_TYPE    m_bufType;
        private byte[]      m_Buffer;
        private int m_Seek;
        public int GetSeek() { return m_Seek; } 
        public enum BUF_TYPE { BUF_READ, BUF_WRITE }
        


        public CStream()
        {
            m_bufType = BUF_TYPE.BUF_READ;
            m_Buffer = null;
            m_Seek = 0;
        }
        public void SetReadBuffer( byte [] a_buf)
        {
            m_Buffer = a_buf;
            m_bufType = BUF_TYPE.BUF_READ;            
            m_Seek = 0;
        }

        public void SetWriteBuffer( byte[] a_buf)
        {
            m_Buffer = a_buf;
            m_bufType = BUF_TYPE.BUF_WRITE;
            m_Seek = 0;
        }

        //┌───────────────────────────────────────────────────┐
        //│ 이 름 : Write
        //│ 설 명 : 
        //└───────────────────────────────────────────────────┘
        public void Write(byte a_Value)
        {
            if (m_bufType != BUF_TYPE.BUF_WRITE)
            { throw new Exception("CStream::Write m_bufType:" + m_bufType); }
            if (m_Seek >= m_Buffer.Length)
            { throw new Exception("CStream::Write m_Seek:" + m_Seek + ", m_Buffer.Length:" + m_Buffer.Length); }

            Buffer.BlockCopy(BitConverter.GetBytes(a_Value), 0, m_Buffer, m_Seek, sizeof(byte));
            m_Seek += sizeof(byte);
        }
        public void Write(bool a_Value)
        {
            if (m_bufType != BUF_TYPE.BUF_WRITE)
            { throw new Exception("CStream::Write m_bufType:" + m_bufType); }
            if (m_Seek >= m_Buffer.Length)
            { throw new Exception("CStream::Write m_Seek:" + m_Seek + ", m_Buffer.Length:" + m_Buffer.Length); }

            Buffer.BlockCopy(BitConverter.GetBytes(a_Value), 0, m_Buffer, m_Seek, sizeof(bool));
            m_Seek += sizeof(bool);
        }
        public void Write(char a_Value)
        {
            if (m_bufType != BUF_TYPE.BUF_WRITE)
            { throw new Exception("CStream::Write m_bufType:" + m_bufType); }
            if (m_Seek >= m_Buffer.Length)
            { throw new Exception("CStream::Write m_Seek:" + m_Seek + ", m_Buffer.Length:" + m_Buffer.Length); }

            Buffer.BlockCopy(BitConverter.GetBytes(a_Value), 0, m_Buffer, m_Seek, sizeof(char));
            m_Seek += sizeof(char);
        }
        public void Write(double a_Value)
        {
            if (m_bufType != BUF_TYPE.BUF_WRITE)
            { throw new Exception("CStream::Write m_bufType:" + m_bufType); }
            if (m_Seek >= m_Buffer.Length)
            { throw new Exception("CStream::Write m_Seek:" + m_Seek + ", m_Buffer.Length:" + m_Buffer.Length); }

            Buffer.BlockCopy(BitConverter.GetBytes(a_Value), 0, m_Buffer, m_Seek, sizeof(double));
            m_Seek += sizeof(double);
        }
        public void Write(float a_Value)
        {
            if (m_bufType != BUF_TYPE.BUF_WRITE)
            { throw new Exception("CStream::Write m_bufType:" + m_bufType); }
            if (m_Seek >= m_Buffer.Length)
            { throw new Exception("CStream::Write m_Seek:" + m_Seek + ", m_Buffer.Length:" + m_Buffer.Length); }
            Buffer.BlockCopy(BitConverter.GetBytes(a_Value), 0, m_Buffer, m_Seek, sizeof(float));
            m_Seek += sizeof(float);
        }
        public void Write(int a_Value)
        {
            if (m_bufType != BUF_TYPE.BUF_WRITE)
            { throw new Exception("CStream::Write m_bufType:" + m_bufType); }
            if (m_Seek >= m_Buffer.Length)
            { throw new Exception("CStream::Write m_Seek:" + m_Seek + ", m_Buffer.Length:" + m_Buffer.Length); }

            Buffer.BlockCopy(BitConverter.GetBytes(a_Value), 0, m_Buffer, m_Seek, sizeof(int));
            m_Seek += sizeof(int);
        }
        public void Write(long a_Value)
        {
            if (m_bufType != BUF_TYPE.BUF_WRITE)
            { throw new Exception("CStream::Write m_bufType:" + m_bufType); }
            if (m_Seek >= m_Buffer.Length)
            { throw new Exception("CStream::Write m_Seek:" + m_Seek + ", m_Buffer.Length:" + m_Buffer.Length); }

            Buffer.BlockCopy(BitConverter.GetBytes(a_Value), 0, m_Buffer, m_Seek, sizeof(long));
            m_Seek += sizeof(long);
        }
        public void Write(short a_Value)
        {
            if (m_bufType != BUF_TYPE.BUF_WRITE)
            { throw new Exception("CStream::Write m_bufType:" + m_bufType); }
            if (m_Seek >= m_Buffer.Length)
            { throw new Exception("CStream::Write m_Seek:" + m_Seek + ", m_Buffer.Length:" + m_Buffer.Length); }

            Buffer.BlockCopy(BitConverter.GetBytes(a_Value), 0, m_Buffer, m_Seek, sizeof(short));
            m_Seek += sizeof(short);
        }
        public void Write(uint a_Value)
        {
            if (m_bufType != BUF_TYPE.BUF_WRITE)
            { throw new Exception("CStream::Write m_bufType:" + m_bufType); }
            if (m_Seek >= m_Buffer.Length)
            { throw new Exception("CStream::Write m_Seek:" + m_Seek + ", m_Buffer.Length:" + m_Buffer.Length); }

            Buffer.BlockCopy(BitConverter.GetBytes(a_Value), 0, m_Buffer, m_Seek, sizeof(uint));
            m_Seek += sizeof(uint);
        }
        public void Write(ulong a_Value)
        {
            if (m_bufType != BUF_TYPE.BUF_WRITE)
            { throw new Exception("CStream::Write m_bufType:" + m_bufType); }
            if (m_Seek >= m_Buffer.Length)
            { throw new Exception("CStream::Write m_Seek:" + m_Seek + ", m_Buffer.Length:" + m_Buffer.Length); }

            Buffer.BlockCopy(BitConverter.GetBytes(a_Value), 0, m_Buffer, m_Seek, sizeof(ulong));
            m_Seek += sizeof(ulong);
        }
        public void Write(ushort a_Value)
        {
            if (m_bufType != BUF_TYPE.BUF_WRITE)
            { throw new Exception("CStream::Write m_bufType:" + m_bufType); }
            if (m_Seek >= m_Buffer.Length)
            { throw new Exception("CStream::Write m_Seek:" + m_Seek + ", m_Buffer.Length:" + m_Buffer.Length); }

            Buffer.BlockCopy(BitConverter.GetBytes(a_Value), 0, m_Buffer, m_Seek, sizeof(ushort));
            m_Seek += sizeof(ushort);
        }
        // 스트링을 넣고 스트링 사이즈만큼 seek을 이동함 
        public void Write_Uni(string a_Str)
        {
            if (m_bufType != BUF_TYPE.BUF_WRITE)
            { throw new Exception("CStream::Write m_bufType:" + m_bufType); }
            if (m_Seek >= m_Buffer.Length)
            { throw new Exception("CStream::Write m_Seek:" + m_Seek + ", m_Buffer.Length:" + m_Buffer.Length); }

            byte[] Tempbuf = Encoding.Unicode.GetBytes(a_Str);
            Buffer.BlockCopy(Tempbuf, 0,
                m_Buffer, m_Seek,
                Tempbuf.Length);
            m_Seek += Tempbuf.Length;
        }
        public void Write_Ansi(string a_Str)
        {
            if (m_bufType != BUF_TYPE.BUF_WRITE)
            { throw new Exception("CStream::Write m_bufType:" + m_bufType); }
            if (m_Seek >= m_Buffer.Length)
            { throw new Exception("CStream::Write m_Seek:" + m_Seek + ", m_Buffer.Length:" + m_Buffer.Length); }

            byte[] Tempbuf = Encoding.ASCII.GetBytes(a_Str);
            Buffer.BlockCopy(Tempbuf, 0,
                m_Buffer, m_Seek,
                Tempbuf.Length);
            m_Seek += Tempbuf.Length;
        }

        // 길이와 배열을 지정하는 함수 
        public void Write(byte[] a_Value, int a_nLength)
        {
            if (m_bufType != BUF_TYPE.BUF_WRITE)
            { throw new Exception("CStream::Write m_bufType:" + m_bufType); }
            if (m_Seek >= m_Buffer.Length)
            { throw new Exception("CStream::Write m_Seek:" + m_Seek + ", m_Buffer.Length:" + m_Buffer.Length); }

            for (int i = 0; i < a_nLength; i++)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(a_Value[i]), 0, m_Buffer, m_Seek, sizeof(byte));
                m_Seek += sizeof(byte);
            }
        }        
        public void Write(bool[] a_Value, int a_nLength)
        {
            if (m_bufType != BUF_TYPE.BUF_WRITE)
            { throw new Exception("CStream::Write m_bufType:" + m_bufType); }
            if (m_Seek >= m_Buffer.Length)
            { throw new Exception("CStream::Write m_Seek:" + m_Seek + ", m_Buffer.Length:" + m_Buffer.Length); }

            for (int i = 0; i < a_nLength; i++)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(a_Value[i]), 0, m_Buffer, m_Seek, sizeof(bool));
                m_Seek += sizeof(bool);
            }
        }
        public void Write(char[] a_Value, int a_nLength)
        {
            if (m_bufType != BUF_TYPE.BUF_WRITE)
            { throw new Exception("CStream::Write m_bufType:" + m_bufType); }
            if (m_Seek >= m_Buffer.Length)
            { throw new Exception("CStream::Write m_Seek:" + m_Seek + ", m_Buffer.Length:" + m_Buffer.Length); }

            for (int i = 0; i < a_nLength; i++)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(a_Value[i]), 0, m_Buffer, m_Seek, sizeof(char));
                m_Seek += sizeof(char);
            }
        }
        public void Write(double[] a_Value, int a_nLength)
        {
            if (m_bufType != BUF_TYPE.BUF_WRITE)
            { throw new Exception("CStream::Write m_bufType:" + m_bufType); }
            if (m_Seek >= m_Buffer.Length)
            { throw new Exception("CStream::Write m_Seek:" + m_Seek + ", m_Buffer.Length:" + m_Buffer.Length); }

            for (int i = 0; i < a_nLength; i++)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(a_Value[i]), 0, m_Buffer, m_Seek, sizeof(double));
                m_Seek += sizeof(double);
            }
        }
        public void Write(float[] a_Value, int a_nLength)
        {
            if (m_bufType != BUF_TYPE.BUF_WRITE)
            { throw new Exception("CStream::Write m_bufType:" + m_bufType); }
            if (m_Seek >= m_Buffer.Length)
            { throw new Exception("CStream::Write m_Seek:" + m_Seek + ", m_Buffer.Length:" + m_Buffer.Length); }

            for (int i = 0; i < a_nLength; i++)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(a_Value[i]), 0, m_Buffer, m_Seek, sizeof(float));
                m_Seek += sizeof(float);
            }
        }
        public void Write(int[] a_Value, int a_nLength)
        {
            if (m_bufType != BUF_TYPE.BUF_WRITE)
            { throw new Exception("CStream::Write m_bufType:" + m_bufType); }
            if (m_Seek >= m_Buffer.Length)
            { throw new Exception("CStream::Write m_Seek:" + m_Seek + ", m_Buffer.Length:" + m_Buffer.Length); }

            for (int i = 0; i < a_nLength; i++)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(a_Value[i]), 0, m_Buffer, m_Seek, sizeof(int));
                m_Seek += sizeof(int);
            }
        }
        public void Write(long[] a_Value, int a_nLength)
        {
            if (m_bufType != BUF_TYPE.BUF_WRITE)
            { throw new Exception("CStream::Write m_bufType:" + m_bufType); }
            if (m_Seek >= m_Buffer.Length)
            { throw new Exception("CStream::Write m_Seek:" + m_Seek + ", m_Buffer.Length:" + m_Buffer.Length); }

            for (int i = 0; i < a_nLength; i++)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(a_Value[i]), 0, m_Buffer, m_Seek, sizeof(long));
                m_Seek += sizeof(long);
            }
        }
        public void Write(short[] a_Value, int a_nLength)
        {
            if (m_bufType != BUF_TYPE.BUF_WRITE)
            { throw new Exception("CStream::Write m_bufType:" + m_bufType); }
            if (m_Seek >= m_Buffer.Length)
            { throw new Exception("CStream::Write m_Seek:" + m_Seek + ", m_Buffer.Length:" + m_Buffer.Length); }

            for (int i = 0; i < a_nLength; i++)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(a_Value[i]), 0, m_Buffer, m_Seek, sizeof(short));
                m_Seek += sizeof(short);
            }
        }
        public void Write(uint[] a_Value, int a_nLength)
        {
            if (m_bufType != BUF_TYPE.BUF_WRITE)
            { throw new Exception("CStream::Write m_bufType:" + m_bufType); }
            if (m_Seek >= m_Buffer.Length)
            { throw new Exception("CStream::Write m_Seek:" + m_Seek + ", m_Buffer.Length:" + m_Buffer.Length); }

            for (int i = 0; i < a_nLength; i++)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(a_Value[i]), 0, m_Buffer, m_Seek, sizeof(uint));
                m_Seek += sizeof(uint);
            }
        }
        public void Write(ulong[] a_Value, int a_nLength)
        {
            if (m_bufType != BUF_TYPE.BUF_WRITE)
            { throw new Exception("CStream::Write m_bufType:" + m_bufType); }
            if (m_Seek >= m_Buffer.Length)
            { throw new Exception("CStream::Write m_Seek:" + m_Seek + ", m_Buffer.Length:" + m_Buffer.Length); }

            for (int i = 0; i < a_nLength; i++)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(a_Value[i]), 0, m_Buffer, m_Seek, sizeof(ulong));
                m_Seek += sizeof(ulong);
            }
        }
        public void Write(ushort[] a_Value, int a_nLength)
        {
            if (m_bufType != BUF_TYPE.BUF_WRITE)
            { throw new Exception("CStream::Write m_bufType:" + m_bufType); }
            if (m_Seek >= m_Buffer.Length)
            { throw new Exception("CStream::Write m_Seek:" + m_Seek + ", m_Buffer.Length:" + m_Buffer.Length); }

            for (int i = 0; i < a_nLength; i++)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(a_Value[i]), 0, m_Buffer, m_Seek, sizeof(ushort));
                m_Seek += sizeof(ushort);
            }
        }
        // 스트링을 넣고 a_nLength만큼 seek을 이동함 (a_nLength에 꼭 스트링사이즈는 넣지 않아도됨 다만 스트링사이즈보다 크게)
        public void Write(String a_Str, int a_nLength)
        {
            if (m_bufType != BUF_TYPE.BUF_WRITE)
            { throw new Exception("CStream::Write m_bufType:" + m_bufType); }
            if (m_Seek >= m_Buffer.Length)
            { throw new Exception("CStream::Write m_Seek:" + m_Seek + ", m_Buffer.Length:" + m_Buffer.Length); }

            byte[] Tempbuf = Encoding.Unicode.GetBytes(a_Str);
            Buffer.BlockCopy(Tempbuf, 0,
                m_Buffer, m_Seek,
                Tempbuf.Length);
            m_Seek += a_nLength;
        }

        public void Write_Uni(String a_Str, int a_nLength)
        {
            if (m_bufType != BUF_TYPE.BUF_WRITE)
            { throw new Exception("CStream::Write m_bufType:" + m_bufType); }
            if (m_Seek >= m_Buffer.Length)
            { throw new Exception("CStream::Write m_Seek:" + m_Seek + ", m_Buffer.Length:" + m_Buffer.Length); }

            byte[] Tempbuf = Encoding.Unicode.GetBytes(a_Str);
            Buffer.BlockCopy(Tempbuf, 0,
                m_Buffer, m_Seek,
                Tempbuf.Length);
            m_Seek += a_nLength;
        }
        public void Write_Ansi(String a_Str, int a_nLength)
        {
            if (m_bufType != BUF_TYPE.BUF_WRITE)
            { throw new Exception("CStream::Write m_bufType:" + m_bufType); }
            if (m_Seek >= m_Buffer.Length)
            { throw new Exception("CStream::Write m_Seek:" + m_Seek + ", m_Buffer.Length:" + m_Buffer.Length); }

            byte[] Tempbuf = Encoding.ASCII.GetBytes(a_Str);
            Buffer.BlockCopy(Tempbuf, 0,
                m_Buffer, m_Seek,
                Tempbuf.Length);
            m_Seek += a_nLength;
        }

        //┌───────────────────────────────────────────────────┐        
        //│ 이 름 : Read
        //│ 설 명 : 
        //└───────────────────────────────────────────────────┘    

        public byte Read_byte()
        {
            if (m_bufType != BUF_TYPE.BUF_READ)
            { throw new Exception("CStream::Read_byte m_bufType:" + m_bufType); }
            if (m_Seek >= m_Buffer.Length)
            { throw new Exception("CStream::Read_byte m_Seek:" + m_Seek + ", m_Buffer.Length:" + m_Buffer.Length); }

            byte Rtn = m_Buffer[m_Seek];
            m_Seek += sizeof(byte);
            return Rtn;
        }
        public bool Read_bool()
        {
            if (m_bufType != BUF_TYPE.BUF_READ)
            { throw new Exception("CStream::Read_bool m_bufType:" + m_bufType); }
            if (m_Seek >= m_Buffer.Length)
            { throw new Exception("CStream::Read_bool m_Seek:" + m_Seek + ", m_Buffer.Length:" + m_Buffer.Length); }
            
            bool Rtn = BitConverter.ToBoolean(m_Buffer, m_Seek);            
            m_Seek += sizeof(bool);
            return Rtn;
        }
        public char Read_char()
        {
            if (m_bufType != BUF_TYPE.BUF_READ)
            { throw new Exception("CStream::Read_char m_bufType:" + m_bufType); }
            if (m_Seek >= m_Buffer.Length)
            { throw new Exception("CStream::Read_char m_Seek:" + m_Seek + ", m_Buffer.Length:" + m_Buffer.Length); }

            char Rtn = BitConverter.ToChar(m_Buffer, m_Seek);
            m_Seek += sizeof(char);
            return Rtn;
        }
        public double Read_double()
        {
            if (m_bufType != BUF_TYPE.BUF_READ)
            { throw new Exception("CStream::Read_double m_bufType:" + m_bufType); }
            if (m_Seek >= m_Buffer.Length)
            { throw new Exception("CStream::Read_double m_Seek:" + m_Seek + ", m_Buffer.Length:" + m_Buffer.Length); }

            double Rtn = BitConverter.ToDouble(m_Buffer, m_Seek);
            m_Seek += sizeof(double);
            return Rtn;
        }
        public int Read_int()
        {
            if (m_bufType != BUF_TYPE.BUF_READ)
            { throw new Exception("CStream::Read_ushort m_bufType:" + m_bufType); }
            if (m_Seek >= m_Buffer.Length)
            { throw new Exception("CStream::Read_ushort m_Seek:" + m_Seek + ", m_Buffer.Length:" + m_Buffer.Length); }

            int Rtn = BitConverter.ToInt32(m_Buffer, m_Seek);
            m_Seek += sizeof(int);
            return Rtn;
        }
        public long Read_long()
        {
            if (m_bufType != BUF_TYPE.BUF_READ)
            { throw new Exception("CStream::Read_long m_bufType:" + m_bufType); }
            if (m_Seek >= m_Buffer.Length)
            { throw new Exception("CStream::Read_long m_Seek:" + m_Seek + ", m_Buffer.Length:" + m_Buffer.Length); }

            long Rtn = BitConverter.ToInt64(m_Buffer, m_Seek);
            m_Seek += sizeof(long);
            return Rtn;
        }
        public float Read_float()
        {
            if (m_bufType != BUF_TYPE.BUF_READ)
            { throw new Exception("CStream::Read_float m_bufType:" + m_bufType); }
            if (m_Seek >= m_Buffer.Length)
            { throw new Exception("CStream::Read_float m_Seek:" + m_Seek + ", m_Buffer.Length:" + m_Buffer.Length); }

            float Rtn = BitConverter.ToSingle(m_Buffer, m_Seek);
            m_Seek += sizeof(float);
            return Rtn;
        }
        public short Read_short()
        {
            if (m_bufType != BUF_TYPE.BUF_READ)
            { throw new Exception("CStream::Read_short m_bufType:" + m_bufType); }
            if (m_Seek >= m_Buffer.Length)
            { throw new Exception("CStream::Read_short m_Seek:" + m_Seek + ", m_Buffer.Length:" + m_Buffer.Length); }

            short Rtn = BitConverter.ToInt16(m_Buffer, m_Seek);
            m_Seek += sizeof(short);
            return Rtn;
        }
        public uint Read_uint()
        {
            if (m_bufType != BUF_TYPE.BUF_READ)
            { throw new Exception("CStream::Read_uint m_bufType:" + m_bufType); }
            if (m_Seek >= m_Buffer.Length)
            { throw new Exception("CStream::Read_uint m_Seek:" + m_Seek + ", m_Buffer.Length:" + m_Buffer.Length); }

            uint Rtn = BitConverter.ToUInt32(m_Buffer, m_Seek);
            m_Seek += sizeof(uint);
            return Rtn;
        }
        public ulong Read_ulong()
        {
            if (m_bufType != BUF_TYPE.BUF_READ)
            { throw new Exception("CStream::Read_ulong m_bufType:" + m_bufType); }
            if (m_Seek >= m_Buffer.Length)
            { throw new Exception("CStream::Read_ulong m_Seek:" + m_Seek + ", m_Buffer.Length:" + m_Buffer.Length); }

            ulong Rtn = BitConverter.ToUInt64(m_Buffer, m_Seek);
            m_Seek += sizeof(ulong);
            return Rtn;
        }
        public ushort Read_ushort()
        {
            if (m_bufType != BUF_TYPE.BUF_READ) 
            { throw new Exception("CStream::Read_ushort m_bufType:" + m_bufType); }
            if (m_Seek >= m_Buffer.Length)
            { throw new Exception("CStream::Read_ushort m_Seek:" + m_Seek + ", m_Buffer.Length:" + m_Buffer.Length); }

            ushort Rtn = BitConverter.ToUInt16(m_Buffer, m_Seek);
            m_Seek += sizeof(ushort);
            return Rtn;
        }
        
        public string Read_string_Uni( int a_nLength )
        {
            if (m_bufType != BUF_TYPE.BUF_READ)
            { throw new Exception("CStream::Read_ushort m_bufType:" + m_bufType); }
            if (m_Seek >= m_Buffer.Length)
            { throw new Exception("CStream::Read_ushort m_Seek:" + m_Seek + ", m_Buffer.Length:" + m_Buffer.Length); }
            
            string Rtn = Encoding.Unicode.GetString(m_Buffer, m_Seek, a_nLength);
            Rtn = Rtn.Replace("\0", string.Empty);
            m_Seek += a_nLength;
            return Rtn;
        }
        public string Read_string_Ansi(int a_nLength)
        {
            if (m_bufType != BUF_TYPE.BUF_READ)
            { throw new Exception("CStream::Read_ushort m_bufType:" + m_bufType); }
            if (m_Seek >= m_Buffer.Length)
            { throw new Exception("CStream::Read_ushort m_Seek:" + m_Seek + ", m_Buffer.Length:" + m_Buffer.Length); }

            string Rtn = Encoding.ASCII.GetString(m_Buffer, m_Seek, a_nLength);
            Rtn = Rtn.Replace("\0", string.Empty);
            m_Seek += a_nLength;
            return Rtn;
        }
    }

 