using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Windows.Forms;

namespace MatrixController
{
    public partial class Form1 : Form
    {
        private SerialPort port;
        bool debug = false;
        bool hoverEnable = false;
        bool invertEnable = false;

        List<CheckBox> Matrix_1 = new List<CheckBox>();
        List<CheckBox> Matrix_2 = new List<CheckBox>();
        List<CheckBox> Matrix_3 = new List<CheckBox>();
        List<CheckBox> Matrix_4 = new List<CheckBox>();
        List<CheckBox> boxes = new List<CheckBox>();

        public Form1()
        {
            InitializeComponent();
            string portName = "COM8";
            port = new SerialPort(portName,
          9600, Parity.None, 8, StopBits.One);
            try
            {
                port.Open();
            }
            catch(Exception e)
            {
                // Console.WriteLine(e.StackTrace);
                Console.WriteLine("Cannot Open Serial Port ("+portName+")");
            }
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Console.WriteLine("Started");
            foreach (Control c in this.groupBox1.Controls)
            {
                if (c is CheckBox)
                {
                    if(c.Name.Length == 9)
                    {
                        c.Name = c.Name.Insert(8, "00");
                    }
                    c.Name = c.Name.Remove(0, 8);
                    Matrix_1.Add(c as CheckBox);
                }
            }
            foreach (Control c in this.groupBox2.Controls)
            {
                if (c is CheckBox)
                {
                    if (c.Name.Length == 10)
                    {
                        c.Name = c.Name.Insert(8, "0");
                    }
                    c.Name = c.Name.Remove(0, 8);
                    Matrix_2.Add(c as CheckBox);
                }
            }
            foreach (Control c in this.groupBox3.Controls)
            {
                if (c is CheckBox)
                {
                    if (c.Name.Length == 10)
                    {
                        c.Name = c.Name.Insert(8, "0");
                    }
                    c.Name = c.Name.Remove(0, 8);
                    Matrix_3.Add(c as CheckBox);
                }
            }
            foreach (Control c in this.groupBox4.Controls)
            {
                if (c is CheckBox)
                {
                    c.Name = c.Name.Remove(0, 8);
                    Matrix_4.Add(c as CheckBox);
                }
            }
            Matrix_1 = Sort(Matrix_1);
            Matrix_2 = Sort(Matrix_2);
            Matrix_3 = Sort(Matrix_3);
            Matrix_4 = Sort(Matrix_4);

            for(int i=1;i<=8;i++)
            {
                for(int j= (i - 1) * 8; j < 8*i; j++)
                {
                    boxes.Add(Matrix_1[j]);
                }
                for (int j = (i - 1) * 8; j < 8 * i; j++)
                {
                    boxes.Add(Matrix_2[j]);
                }
                for (int j = (i - 1) * 8; j < 8 * i; j++)
                {
                    boxes.Add(Matrix_3[j]);
                }
                for (int j = (i - 1) * 8; j < 8 * i; j++)
                {
                    boxes.Add(Matrix_4[j]);
                }

            }
            for(int i=0;i<256;i++)
            {
                boxes[i].MouseHover += new EventHandler(mouseHover);
            }
            if(debug)
            {
                Timer tt = new Timer();
                tt.Interval = 25;
                tt.Tick += new EventHandler(ticked);
                tt.Start();
            }
            Console.WriteLine("Ready.");
        }
        void mouseHover(object sender, EventArgs e)
        {
            if(hoverEnable)
            {
                CheckBox checkbox = sender as CheckBox;
                checkbox.Checked = !checkbox.Checked;
            }
        }


        int count = 0;
        void ticked(object sender, EventArgs e)
        {
            if(count > 255)
            {
                return;
            }
            boxes[count].Checked = true;
            Console.WriteLine(boxes[count].Name);
            count++;
        }

        List<CheckBox> Sort(List<CheckBox> data)
        {
            for(int i=0;i<data.Count;i++)
            {
                for(int j= i+1;j<data.Count;j++)
                {
                    if(Convert.ToInt32(data[i].Name) > Convert.ToInt32(data[j].Name))
                    {
                        CheckBox temp = data[i];
                        data[i] = data[j];
                        data[j] = temp;
                    }
                }
            }
            return data;
        }

        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        
        string binForBool(bool state,bool inverted)
        {
            if(inverted)
            {
                return state == true ? "1" : "0";
            }
            return state == true ? "1" : "0";
        }

        string hexForBin(String bin)
        {
            return Convert.ToInt32(bin,2).ToString("X2");
        }

        string binForHex(String Hex)
        {
            return Convert.ToString(Convert.ToInt32(Hex, 16),2).PadLeft(8,'0');
        }

        bool boolForBin(String bin,bool inverted)
        { 
            if(inverted)
            {
                return bin == "1" ? false : true;
            }
            return bin == "1" ? true : false;
        }

        public  string RotateLeft(string data, int count)
        {
            uint value = Convert.ToUInt32(data);
            return Convert.ToString((value << count) | (value >> (32 - count)),2).PadLeft(8,'0');
        }

        public static string RotateRight(string data, int count)
        {
            uint value = Convert.ToUInt32(data);
            return Convert.ToString((value >> count) | (value << (32 - count)),2).PadLeft(8, '0');
        }


        void play(string str)
        {
            if (str.Length < 4)
            {
                int v = (int)Math.Ceiling(4.0 / str.Length);
                for(int i=0;i<v;i++)
                {
                    str += " ";
                }
            }
            int chA = str[0];
            int chB = str[1];
            int chC = str[2];
            int chD = str[3];


            int count = 0;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Matrix_1[count++].Checked = boolForBin(binForHex(Fonts.chars[chA, j]).Substring(i, 1),invertEnable);
                }
            }
            count = 0;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Matrix_2[count++].Checked = boolForBin(binForHex(Fonts.chars[chB, j]).Substring(i, 1), invertEnable);
                }
            }
            count = 0;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Matrix_3[count++].Checked = boolForBin(binForHex(Fonts.chars[chC, j]).Substring(i, 1), invertEnable);
                }
            }
            count = 0;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Matrix_4[count++].Checked = boolForBin(binForHex(Fonts.chars[chD, j]).Substring(i, 1), invertEnable);
                }
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            play(textBox1.Text);
            int counter = 0;
            List<string> HEX = new List<string>();
            for (int i=0;i<8;i++)
            {
                string hexVal = "";
                for(int j=0;j<4;j++)
                {
                    string bin = "";
                    for(int k=0;k<8;k++)
                    {
                        bin += binForBool(boxes[counter++].Checked,invertEnable);
                    }
                    hexVal = hexForBin(bin);
                    HEX.Add(hexVal);
                }
            }
            Console.WriteLine("Writing Data:");
            string data = String.Join("", HEX);
            Console.WriteLine(data);
            if(port.IsOpen)
            {
                port.WriteLine(data);
            } 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for(int i=0;i<256;i++)
            {
                boxes[i].Checked = false;
            }
        }


        private void checkBox257_CheckedChanged(object sender, EventArgs e)
        {
            hoverEnable = checkBox257.Checked;
        }

        private void checkBox258_CheckedChanged(object sender, EventArgs e)
        {
            invertEnable = checkBox258.Checked;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = 1; i <= 8; i++)
            {
                bool old = boxes[(i-1)*8].Checked;
                for (int j = (i - 1) * 8; j < 8 * i; j++)
                {
                    boxes[j].Checked = boxes[j + 1].Checked;
                }
                boxes[(i*8) - 1].Checked = old;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }
    }
}
