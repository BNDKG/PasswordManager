using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;
//aes加密
using System.Security.Cryptography;
//序列化
using System.Runtime.Serialization.Formatters.Binary;

namespace PassWordManager
{
    public partial class Form1 : Form
    {
        string OriPath;

        string AES_KEY = "12345678876543211234567887654abc";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            OriPath = System.IO.Directory.GetCurrentDirectory();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int inputfigures = Convert.ToInt32(textBox2.Text);

            bool[] range = { true, true, true, true };

            range[0] = checkBox1.Checked;
            range[1] = checkBox2.Checked;
            range[2] = checkBox3.Checked;
            range[3] = checkBox4.Checked;

            //复制到显示
            textBox1.Text = aes_logic.PasswordCreate(inputfigures, range);
            //

            //复制到剪贴板
            Clipboard.SetText(textBox1.Text);


        }

        private void button2_Click(object sender, EventArgs e)
        {

            string path = OriPath;

            System.Diagnostics.Process.Start("explorer.exe", path);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();

            file.InitialDirectory = OriPath + "\\library";
            file.ShowDialog();

            string read = file.InitialDirectory + "\\" + file.SafeFileName;

            PassWordStruct aaa = new PassWordStruct();

            try
            {
                aaa = (PassWordStruct)aes_logic.LoadPassword(read);

                textBox1.Text = aes_logic.AesDecrypt(aaa.password, AES_KEY);

                textBox3.Text = aaa.name;
                Clipboard.SetText(textBox1.Text);

            }
            catch (Exception)
            {
                MessageBox.Show("错误路径！！！");
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            string SavePath = OriPath + "\\library\\" + textBox3.Text + ".pwd";

            if (System.IO.File.Exists(SavePath))
            {
                //这里可以改成跳出是否覆盖
                MessageBox.Show("该名称密码已存在");
            }
            else
            {
                string aespassword = aes_logic.AesEncrypt(textBox1.Text, AES_KEY);
                PassWordStruct aaa = new PassWordStruct();
                aaa.NO = 10;
                aaa.name = textBox3.Text;
                aaa.password = aespassword;

                aes_logic.SavePassword(aaa, SavePath);
            }

        }

    }

    [Serializable]
    public struct PassWordStruct  //自定义的数据类型。用来描述员工的信息。 
    {
        public int NO;
        public string key;
        public string name;
        public string password;
        public string info;

        public PassWordStruct(int type)
        {
            NO = 0;
            key = "";
            name = "";
            password = "";
            info = "";
        }

    }
    public class aes_logic
    {



        /// <summary>
        ///  AES 加密
        /// </summary>
        /// <param name="str">明文（待加密）</param>
        /// <param name="key">密文</param>
        /// <returns></returns>
        public static string AesEncrypt(string str, string key)
        {
            if (string.IsNullOrEmpty(str)) return null;
            Byte[] toEncryptArray = Encoding.UTF8.GetBytes(str);

            RijndaelManaged rm = new RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform cTransform = rm.CreateEncryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        /// <summary>
        ///  AES 解密
        /// </summary>
        /// <param name="str">明文（待解密）</param>
        /// <param name="key">密文</param>
        /// <returns></returns>
        public static string AesDecrypt(string str, string key)
        {
            if (string.IsNullOrEmpty(str)) return null;
            Byte[] toEncryptArray = Convert.FromBase64String(str);

            RijndaelManaged rm = new RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform cTransform = rm.CreateDecryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Encoding.UTF8.GetString(resultArray);
        }

        public static void SavePassword(object password, string filename) //序列化保存
        {
            try
            {
                FileStream fs = new FileStream(filename, FileMode.Create);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, password);
                fs.Close();
                MessageBox.Show("保存成功！");
            }
            catch (Exception)
            {
                MessageBox.Show("保存失败！！！");
            }
        }
        public static object LoadPassword(string filename)            //序列化读取文件
        {
            object password = new object();
            try
            {
                FileStream fs = new FileStream(filename, FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();
                password = bf.Deserialize(fs);
                fs.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("读取失败！！！");
            }

            return password;
        }

        public static string PasswordCreate(int figures, bool[] PasswordRange)
        {
            Random rnd = new Random(Guid.NewGuid().GetHashCode());

            Random rnd2 = new Random(Guid.NewGuid().GetHashCode());

            string[] arrays = new string[4];
            arrays[0] = "0123456789";
            arrays[1] = "abcdefghijklmnopqrstuvwxyz";
            arrays[2] = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            arrays[3] = "~!@#$%&*();/";

            string array = "";
            for (int i = 0; i < 4; i++)
            {
                if (PasswordRange[i])
                {
                    array += arrays[i];
                }

            }


            int[] counter = new int[100];

            int PasswordLength = figures;
            string output = "";

            for (int i = 0; i < PasswordLength; i++)
            {

                int nowindex;
                //随机生成生成下一个随机数的次数
                int creattimes = rnd.Next(0, 10);

                while (creattimes > 0)
                {
                    nowindex = rnd.Next(0, array.Length);
                    creattimes--;
                }

                nowindex = rnd.Next(0, array.Length);
                counter[nowindex]++;
                output += array[nowindex];


            }

            return output;
        }

    }


}
