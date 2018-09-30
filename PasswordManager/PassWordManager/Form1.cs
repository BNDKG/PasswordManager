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

using System.Runtime.Serialization;
using System.Net.Sockets;

using System.Web.Script.Serialization;

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
            OriPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
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
            string aaab = aes_logic.AesEncrypt("111", "12345678876543211234567887654abc");


            string path = OriPath;

            System.Diagnostics.Process.Start("explorer.exe", path);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            PureFlag = checkBox5.Checked;

            OpenFileDialog file = new OpenFileDialog();

            if (PureFlag)
            {
                file.InitialDirectory = OriPath + "bak";
            }
            else
            {
                file.InitialDirectory = OriPath + "library";
            }
            //label5.Text = file.InitialDirectory;



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

                    textBox6.Text = aaa.name;
                    textBox3.Text = aaa.info;
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
            string SavePath = OriPath + "library\\" + textBox3.Text + ".pwd";
            string SavePath2 = OriPath + "bak\\" + textBox3.Text + ".bak";

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
            aaa.name = textBox6.Text;
            aaa.password = aespassword;
            aaa.info = textBox3.Text;

            aes_logic.SavePassword(aaa, SavePath);
            aes_logic.SavePasswordPure(aaa, SavePath2);

        }

        private void button5_Click(object sender, EventArgs e)
        {

            OnLoad(OriPath + "library");

            //AES_KEY = textBox4.Text;
            //AES_KEY_2 = textBox5.Text;

            foreach (string item in pswPathList)
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
                    /*
                    //使用第一个密钥解密
                    buffcode = aes_logic.AesDecrypt(aaa.password, AES_KEY);
                    //换第二个密钥加密
                    string aespassword = aes_logic.AesEncrypt(buffcode, AES_KEY_2);
                    //序列化存入结构体
                    aaa.password = aespassword;
                    */
                    aaa.info = System.IO.Path.GetFileNameWithoutExtension(item);
                    //存入对应位置
                    aes_logic.SavePassword(aaa, item);



                }
                catch (Exception)
                {
                    MessageBox.Show("密钥错误！！！");
                }


            }

            int dfs = 9;

        }

        static List<string> pswPathList = new List<string>();

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

                    pswPathList.Add(item);//添加到list中

                }

            }

        }

        private void button6_Click(object sender, EventArgs e)
        {
            //删除文件夹
            //Directory.Delete("dd", true);
            File.Delete("sfs");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            
        }


        static List<PassWordStruct> curpasswordList;

        private void button7_Click(object sender, EventArgs e)
        {
            /*
            try
            {

                Int32 port = Convert.ToInt32(textBox7.Text);
                string server = textBox8.Text;


                string[] messages1 = { "001", "0256" };
                string[] messages = { "1", "BNDKG", "kkk", "fdgr" };


                TcpClient client = new TcpClient(server, port);

                //设置接收超时
                client.ReceiveTimeout = 5000;

                NetworkStream stream = client.GetStream();

                Byte[] data1 = aes_logic.StringArrToBytes(messages1);

                stream.Write(data1, 0, data1.Length);

                Byte[] data = ObjectToBytes(messages);
                // Send the message to the connected TcpServer. 
                stream.Write(data, 0, data.Length);



                // Receive the TcpServer.response.

                // Buffer to store the response bytes.
                data = new Byte[256];


                // String to store the response ASCII representation.
                String responseData = String.Empty;
                String responseData2 = String.Empty;

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);

                string[] receive1 = (string[])BytesToObject(data);

                int blen = Convert.ToInt32(receive1[1]);
                int clen = Convert.ToInt32(receive1[2]);
                int dlen = Convert.ToInt32(receive1[3]);

                Byte[] data2 = new Byte[blen];
                Byte[] data3 = new Byte[clen];
                Byte[] data4 = new Byte[dlen];

                Int32 bytes2 = stream.Read(data2, 0, data2.Length);
                Int32 bytes3 = stream.Read(data3, 0, data3.Length);
                Int32 bytes4 = stream.Read(data4, 0, data4.Length);

                //responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

                string[] receive2 = (string[])BytesToObject(data2);
                string[] receive3 = (string[])BytesToObject(data3);
                string[] receive4 = (string[])BytesToObject(data4);

                int ii = 0;
                curpasswordList = new List<PassWordStruct>();


                foreach (var name in receive2)
                {
                    PassWordStruct buffer = new PassWordStruct(1);
                    buffer.name = name;
                    buffer.password = receive3[ii];
                    buffer.info= receive4[ii];
                    curpasswordList.Add(buffer);

                    ii++;
                }


                // Close everything.
                stream.Close();
                client.Close();
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine("ArgumentNullException: {0}", ex);
            }
            catch (SocketException ex)
            {
                Console.WriteLine("SocketException: {0}", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine("TimeoutException: {0}", ex);
            }
            */

            pswsync();



        }
        private void ConnectCheck()
        {
            try
            {

                Int32 port = Convert.ToInt32(textBox7.Text);
                string server = textBox8.Text;

                //表示c# 客户端 接下来数据长度是256byte
                string[] messages1 = { "001", "0256" };

                string Username = textBox10.Text;
                string key= textBox9.Text;

                //测试服务器是否在线
                string[] messages = { "0", "", "", "Superbndkg" };

                TcpClient client = new TcpClient(server, port);
                //设置接收超时
                client.ReceiveTimeout = 500;

                NetworkStream stream = client.GetStream();

                Byte[] data1 = aes_logic.StringArrToBytes(messages1);
                stream.Write(data1, 0, data1.Length);

                Byte[] data = ObjectToBytes(messages);
                stream.Write(data, 0, data.Length);

                //设置接收消息的长度
                data = new Byte[256];

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);

                string[] receive1 = (string[])BytesToObject(data);

                int error = Convert.ToInt32(receive1[0]);
                if (error == 1)
                {
                    MessageBox.Show("服务器正常运行");

                }
                else
                {
                    MessageBox.Show("服务器忙");
                }

                // Close everything.
                stream.Close();
                client.Close();
            }
            catch (ArgumentNullException ex)
            {
                MessageBox.Show("其他错误");
                Console.WriteLine("ArgumentNullException: {0}", ex);
            }
            catch (SocketException ex)
            {
                MessageBox.Show("服务器连接失败");
                Console.WriteLine("SocketException: {0}", ex);
            }
            catch (Exception ex)
            {
                MessageBox.Show("服务器忙");
                Console.WriteLine("TimeoutException: {0}", ex);
            }
        }

        private void pswsync()
        {
            try
            {

                Int32 port = Convert.ToInt32(textBox7.Text);
                string server = textBox8.Text;

                //表示c# 客户端 接下来数据长度是256byte
                string[] messages1 = { "001", "0256" };

                string Username = textBox10.Text;
                string key = textBox9.Text;
                string Superadmin = textBox11.Text;

                //代表新建用户 用户名 密钥 超级管理员
                string[] messages = { "1", Username, key, Superadmin };

                TcpClient client = new TcpClient(server, port);

                //设置接收超时
                client.ReceiveTimeout = 5000;

                NetworkStream stream = client.GetStream();

                Byte[] data1 = aes_logic.StringArrToBytes(messages1);
                stream.Write(data1, 0, data1.Length);

                Byte[] data = ObjectToBytes(messages);
                stream.Write(data, 0, data.Length);

                // Buffer to store the response bytes.
                data = new Byte[256];

                // String to store the response ASCII representation.
                String responseData = String.Empty;
                String responseData2 = String.Empty;

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);

                string[] receive1 = (string[])BytesToObject(data);

                int error = Convert.ToInt32(receive1[0]);
                if (error == 1)
                {
                    int blen = Convert.ToInt32(receive1[1]);
                    int clen = Convert.ToInt32(receive1[2]);
                    int dlen = Convert.ToInt32(receive1[3]);

                    Byte[] data2 = new Byte[blen];
                    Byte[] data3 = new Byte[clen];
                    Byte[] data4 = new Byte[dlen];

                    Int32 bytes2 = stream.Read(data2, 0, data2.Length);
                    Int32 bytes3 = stream.Read(data3, 0, data3.Length);
                    Int32 bytes4 = stream.Read(data4, 0, data4.Length);

                    //responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

                    string[] receive2 = (string[])BytesToObject(data2);
                    string[] receive3 = (string[])BytesToObject(data3);
                    string[] receive4 = (string[])BytesToObject(data4);

                    int ii = 0;
                    curpasswordList = new List<PassWordStruct>();


                    foreach (var name in receive2)
                    {
                        PassWordStruct buffer = new PassWordStruct(1);
                        buffer.name = name;
                        buffer.password = receive3[ii];
                        buffer.info = receive4[ii];
                        curpasswordList.Add(buffer);

                        ii++;
                    }
                }
                else
                {
                    MessageBox.Show("下载失败");
                }

                // Close everything.
                stream.Close();
                client.Close();
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine("ArgumentNullException: {0}", ex);
            }
            catch (SocketException ex)
            {
                Console.WriteLine("SocketException: {0}", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine("TimeoutException: {0}", ex);
            }
        }
        private void CreateUser()
        {
            try
            {

                Int32 port = Convert.ToInt32(textBox7.Text);
                string server = textBox8.Text;

                //表示c# 客户端 接下来数据长度是256byte
                string[] messages1 = { "001", "0256" };

                string Username = textBox10.Text;
                string key = textBox9.Text;
                string Superadmin= textBox11.Text;

                //代表新建用户 用户名 密钥 超级管理员
                string[] messages = { "3", Username, key, Superadmin };

                TcpClient client = new TcpClient(server, port);

                //设置接收超时
                client.ReceiveTimeout = 5000;

                NetworkStream stream = client.GetStream();

                Byte[] data1 = aes_logic.StringArrToBytes(messages1);
                stream.Write(data1, 0, data1.Length);

                Byte[] data = ObjectToBytes(messages);
                stream.Write(data, 0, data.Length);

                data = new Byte[256];
                Int32 bytes = stream.Read(data, 0, data.Length);

                string[] receive1 = (string[])BytesToObject(data);

                int error = Convert.ToInt32(receive1[0]);
                if (error == 1)
                {
                    MessageBox.Show("创建成功");
                }
                else if(error == 2)
                {
                    MessageBox.Show("超级管理员密码错误");
                }
                else if (error == 3)
                {
                    MessageBox.Show("用户名已存在");
                }

                // Close everything.
                stream.Close();
                client.Close();
            }
            catch (ArgumentNullException ex)
            {
                MessageBox.Show("其他错误");
                Console.WriteLine("ArgumentNullException: {0}", ex);
            }
            catch (SocketException ex)
            {
                MessageBox.Show("服务器连接失败");
                Console.WriteLine("SocketException: {0}", ex);
            }
            catch (Exception ex)
            {
                MessageBox.Show("服务器忙");
                Console.WriteLine("TimeoutException: {0}", ex);
            }
        }
        private void UploadPSW()
        {
            try
            {
                int port = Convert.ToInt32(textBox7.Text);
                string server = textBox8.Text;

                //表示c# 客户端 接下来数据长度是256byte
                string[] messages1 = { "001", "0256" };

                string Username = textBox10.Text;
                string key = textBox9.Text;

                //提交上传请求并同时提交用户名密码
                string[] messages = { "2", Username, key, "Superbndkg" };

                TcpClient client = new TcpClient(server, port);
                //设置接收超时
                client.ReceiveTimeout = 500;

                NetworkStream stream = client.GetStream();

                Byte[] data1 = aes_logic.StringArrToBytes(messages1);
                stream.Write(data1, 0, data1.Length);

                Byte[] data = ObjectToBytes(messages);
                stream.Write(data, 0, data.Length);


                //设置接收消息的长度
                data = new Byte[256];
                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);

                string[] receive1 = (string[])BytesToObject(data);

                int error = Convert.ToInt32(receive1[0]);
                if (error == 1)
                {
                    MessageBox.Show("服务器连接成功,正在上传");
                    

                    byte[][] backmsgs = UploadPSWbak();

                    foreach (var singlemsg in backmsgs)
                    {
                        int len = singlemsg.Length;

                        stream.Write(singlemsg, 0, len);

                        //这里考虑下要不要wait
                    }

                    //设置接收消息的长度
                    data = new Byte[256];
                    // Read the first batch of the TcpServer response bytes.
                    bytes = stream.Read(data, 0, data.Length);

                    receive1 = (string[])BytesToObject(data);

                    error = Convert.ToInt32(receive1[0]);
                    if (error==1)
                    {
                        //上传成功
                        MessageBox.Show("上传成功");
                    }
                    else if (error == 2)
                    {
                        MessageBox.Show("重复密码校验");
                    }
                    else
                    {
                        MessageBox.Show("未知错误");
                    }
                }
                else if (error == 2)
                {
                    MessageBox.Show("没有此账号");
                }
                else
                {
                    MessageBox.Show("账号密码错误");
                }

                // Close everything.
                stream.Close();
                client.Close();
            }
            catch (ArgumentNullException ex)
            {
                MessageBox.Show("其他错误");
                Console.WriteLine("ArgumentNullException: {0}", ex);
            }
            catch (SocketException ex)
            {
                MessageBox.Show("服务器连接失败");
                Console.WriteLine("SocketException: {0}", ex);
            }
            catch (Exception ex)
            {
                MessageBox.Show("服务器忙");
                Console.WriteLine("TimeoutException: {0}", ex);
            }
        }
        private byte[][] UploadPSWbak()
        {
            byte[][] backmsgs = new byte[4][];
            backmsgs[0] = new byte[] { 0x00 };


            OnLoad(OriPath + "library");

            //AES_KEY = textBox4.Text;
            //AES_KEY_2 = textBox5.Text;

            string[] buffname = new string[pswPathList.Count()];
            string[] buffpsw = new string[pswPathList.Count()];
            string[] buffinfo = new string[pswPathList.Count()];

            int i = 0;
            foreach (string item in pswPathList)
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
                    //这里测试一下buffname[i] = aaa.name;
                    buffname[i] = aaa.info;
                    buffpsw[i] = aaa.password;
                    buffinfo[i] = aaa.info;


                }
                catch (Exception)
                {
                    MessageBox.Show("密钥错误！！！");
                }
                i++;

            }
            byte[] msgb = ObjectToBytes(buffname);
            byte[] msgc = ObjectToBytes(buffpsw);
            byte[] msgd = ObjectToBytes(buffinfo);

            string blen = (msgb.Length).ToString();
            string clen = (msgc.Length).ToString();
            string dlen = (msgd.Length).ToString();

            //第一位功能待定(可以定为只新增或完全同步模式),后三位为数据长度
            string[] userinfo = { "0", blen, clen, dlen };
            byte[] msga = ObjectToBytes(userinfo);

            backmsgs[0] = msga;
            backmsgs[1] = msgb;
            backmsgs[2] = msgc;
            backmsgs[3] = msgd;


            return backmsgs;
        }


        /// <summary> 
        /// 将一个object对象序列化，返回一个byte[]         
        /// </summary> 
        /// <param name="obj">能序列化的对象</param>         
        /// <returns></returns> 
        public static byte[] ObjectToBytes(object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter(); formatter.Serialize(ms, obj); return ms.GetBuffer();
            }
        }

        /// <summary> 
        /// 将一个序列化后的byte[]数组还原         
        /// </summary>
        /// <param name="Bytes"></param>         
        /// <returns></returns> 
        public static object BytesToObject(byte[] Bytes)
        {
            using (MemoryStream ms = new MemoryStream(Bytes))
            {
                IFormatter formatter = new BinaryFormatter(); return formatter.Deserialize(ms);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            //先读取本地
            libOnLoad();
            
            foreach (var curpwdstruct in curpasswordList)
            {
                string searchinfo = curpwdstruct.info;
                if (bakPWDinfoList.Contains(searchinfo))
                {
                    //先不同步
                    //todo 判断是否相同 不相同则询问
                    //PassWordStruct aaa = (PassWordStruct)aes_logic.LoadPassword(read);
                    int index = bakPWDinfoList.FindIndex(zLamda => zLamda == searchinfo);

                }
                else
                {
                    savesingle(curpwdstruct.info, curpwdstruct.name, curpwdstruct.password);
                }
                
            }


            int dd = 3;
        }
        private void savesingle(string info,string name,string password)
        {
            string SavePath = OriPath + "library\\" + info + ".pwd";
            string SavePath2 = OriPath + "bak\\" + info + ".bak";

            //string aespassword = aes_logic.AesEncrypt(textBox1.Text, AES_KEY);
            PassWordStruct aaa = new PassWordStruct();
            aaa.NO = 10;
            aaa.name = name;
            aaa.password = password;
            aaa.info = info;

            aes_logic.SavePassword(aaa, SavePath);
            aes_logic.SavePasswordPure(aaa, SavePath2);
        }

        static List<string> bakPWDPathList = new List<string>();
        static List<string> bakPWDinfoList = new List<string>();

        private void libOnLoad()
        {
            string bakpath = OriPath + "library\\";
            //获取指定文件夹的所有文件
            string[] paths = Directory.GetFiles(bakpath);
            foreach (var item in paths)
            {
                //获取文件后缀名
                string extension = Path.GetExtension(item).ToLower();
                if (extension == ".pwd")
                {

                    string savename = Path.GetFileNameWithoutExtension(item).ToLower();
                    bakPWDPathList.Add(item);
                    bakPWDinfoList.Add(savename);
                }
            }

        }

        private void button9_Click(object sender, EventArgs e)
        {
            ConnectCheck();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            CreateUser();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            UploadPSW();
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

    [Serializable]
    public struct PassWordDic
    {
        //密码字典结构
        public string Name;
        public string password;
        public string otherinfo;
        public int numberofpassword;
        //public PassWordStruct[] MYpasswords;
        public List<PassWordStruct> MYpasswordList;


    }


    public class aes_logic
    {

        public static string EncryptWithMD5(string source)
        {
            byte[] sor = Encoding.UTF8.GetBytes(source);
            MD5 md5 = MD5.Create();
            byte[] result = md5.ComputeHash(sor);
            StringBuilder strbul = new StringBuilder(40);
            for (int i = 0; i < result.Length; i++)
            {
                strbul.Append(result[i].ToString("x2"));//加密结果"x2"结果为32位,"x3"结果为48位,"x4"结果为64位

            }
            return strbul.ToString();
        }

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

        public static string[] BytesToStringArr(byte[] Bytes)
        {
            string nfefe = Encoding.UTF8.GetString(Bytes);
            return FromJSON<string[]>(nfefe);
        }
        public static byte[] StringArrToBytes(string[] StringArr)
        {
            string get = ToJSON(StringArr);
            return Encoding.UTF8.GetBytes(get);
        }

        /// <summary>
        /// 内存对象转换为json字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJSON(object obj)
        {
            StringBuilder sb = new StringBuilder();
            JavaScriptSerializer json = new JavaScriptSerializer();
            json.Serialize(obj, sb);
            return sb.ToString();
        }

        /// <summary>
        /// Json字符串转内存对象
        /// </summary>
        /// <param name="jsonString"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T FromJSON<T>(string jsonString)
        {
            JavaScriptSerializer json = new JavaScriptSerializer();
            return json.Deserialize<T>(jsonString);
        }



    }


}

