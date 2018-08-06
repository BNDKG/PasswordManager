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
        //是否使用纯密码模式(版本更新时普通模式可能会失效)
        bool PureFlag = false;

        string OriPath;
        //默认密钥12345678876543211234567887654abc
        string AES_KEY = "12345678876543211234567887654abc";
        string AES_KEY_2 = "000#CN;2#;@x~HcucaZDTcwJwyqNDH5&";

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
            PureFlag = checkBox5.Checked;

            OpenFileDialog file = new OpenFileDialog();

            if (PureFlag)
            {
                file.InitialDirectory = OriPath + "\\bak";
            }
            else
            {
                file.InitialDirectory = OriPath + "\\library";
            }

            

            if (file.ShowDialog() == DialogResult.OK)
            {

                string read = file.InitialDirectory + "\\" + file.SafeFileName;

                PassWordStruct aaa = new PassWordStruct();

                try
                {
                    if (PureFlag)
                    {
                        aaa = (PassWordStruct)aes_logic.LoadPasswordPure(read);
                    }
                    else
                    {
                        aaa = (PassWordStruct)aes_logic.LoadPassword(read);
                    }

                    textBox1.Text = aes_logic.AesDecrypt(aaa.password, AES_KEY);

                    textBox3.Text = aaa.name;
                    Clipboard.SetText(textBox1.Text);

                }
                catch (Exception)
                {
                    MessageBox.Show("密钥错误！！");
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string SavePath = OriPath + "\\library\\" + textBox3.Text + ".pwd";
            string SavePath2 = OriPath + "\\bak\\" + textBox3.Text + ".bak";

            if (System.IO.File.Exists(SavePath))
            {
                //这里可以改成跳出是否覆盖
                DialogResult dr = MessageBox.Show("该名称密码已存在，是否覆盖?", "警告", MessageBoxButtons.YesNo);

                if (dr == DialogResult.Yes)
                {
                    //MessageBox.Show("覆盖后原密码不可找回，是否确认覆盖?", "警告");

                }                
                else if (dr == DialogResult.No)
                {
                    return;
                }
                else
                {
                    return;
                }

            }

            string aespassword = aes_logic.AesEncrypt(textBox1.Text, AES_KEY);
            PassWordStruct aaa = new PassWordStruct();
            aaa.NO = 10;
            aaa.name = textBox3.Text;
            aaa.password = aespassword;

            aes_logic.SavePassword(aaa, SavePath);
            aes_logic.SavePasswordPure(aaa, SavePath2);

        }

        private void button5_Click(object sender, EventArgs e)
        {

            OnLoad(OriPath + "\\library");

            //AES_KEY = textBox4.Text;
            //AES_KEY_2 = textBox5.Text;

            foreach (string item in picPathList)
            {

                PassWordStruct aaa = new PassWordStruct();
                string buffcode = "";
                try
                {
                    /*
                    aaa.NO = 10;
                    aaa.name = System.IO.Path.GetFileName(item); ;
                    aaa.password = "f7GCOYZ0k7w0DDLsWX4LQzut3lsmlcr32S1MrLmPybU";
                    aes_logic.SavePassword(aaa, item);
                    */
                    
                    //反序列化读结构体
                    aaa = (PassWordStruct)aes_logic.LoadPassword(item);
                    //使用第一个密钥解密
                    buffcode = aes_logic.AesDecrypt(aaa.password, AES_KEY);
                    //换第二个密钥加密
                    string aespassword = aes_logic.AesEncrypt(buffcode, AES_KEY_2);
                    //序列化存入结构体
                    aaa.password = aespassword;
                    //存入对应位置
                   
                    


                }
                catch (Exception)
                {
                    MessageBox.Show("密钥错误！！！");
                }




            }

            int dfs = 9;

        }

        static List<string> picPathList = new List<string>();

        public static void OnLoad(string filepath)

        {

            //获取指定文件夹的所有文件

            string[] paths = Directory.GetFiles(filepath);

            foreach (var item in paths)

            {

                //获取文件后缀名

                string extension = Path.GetExtension(item).ToLower();

                if (extension == ".pwd")

                {

                    picPathList.Add(item);//添加到图片list中

                }

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
            NO = type;
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

        public static void SavePasswordPure(object password, string filename) //序列化保存
        {
            PassWordStruct aaa= (PassWordStruct)password;
            _SavePasswordPure(aaa.password, filename);

        }
        public static void _SavePasswordPure(string password, string filename) //序列化保存
        {
            try
            {
                FileStream fs = new FileStream(filename, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(password);
                sw.Close();
                fs.Close();
                MessageBox.Show("备份保存成功！");
            }
            catch (Exception)
            {
                MessageBox.Show("备份保存失败！！！");
            }
        }

        public static object LoadPasswordPure(string filename)            //序列化读取文件
        {
            PassWordStruct aaa = new PassWordStruct(9);
            aaa.password = _LoadPasswordPure(filename);

            return aaa;
        }
        public static string _LoadPasswordPure(string filename)            //序列化读取文件
        {
            string password = "";
            try
            {
                FileStream fs = new FileStream(filename, FileMode.Open);
                StreamReader sr = new StreamReader(fs);
                password = sr.ReadLine();

                sr.Close();
                fs.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("备份读取失败！！！");
            }

            return password;
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

