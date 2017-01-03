using System;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.IO;
using System.Security.Cryptography;
using System.Net.Http;


namespace CBEP_slack_integration
{
    public partial class cbep_slack : ServiceBase
    {

        //Set your vars here.
        string hookurl = "";

        public string server = "";
        public string apitoken = "";


        //Vars below do not need to be changed.

        public string malicious = "0";
        public string apprequests = "0";
        public string suspicious = "0";
        public cbep_slack()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {


            Thread trd1 = new Thread(getmalicious);

            trd1.IsBackground = true;
            trd1.Start();

            Thread trd2 = new Thread(getsuspicious);

            trd2.IsBackground = true;
            trd2.Start();

            Thread trd3 = new Thread(getappreqs);

            trd3.IsBackground = true;
            trd3.Start();
        }

        protected override void OnStop()
        {
        }


        public async void getmalicious()
        {
            System.Threading.Thread.Sleep(backoff());
            string oldmalicious = "";
            do
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(server + "/api/bit9platform/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("x-auth-token", apitoken);

                HttpResponseMessage response;
                try
                {
                    oldmalicious = malicious;
                    response = await client.GetAsync("v1/fileCatalog?q=effectiveState:Unapproved&q=threat:100&limit=-1");
                    //MessageBox.Show(response.Content.ReadAsStringAsync().Result);
                    malicious = (response.Content.ReadAsStringAsync().Result).Replace(@"{""count"":", "").Replace("}", "");


                }
                catch
                {
                    sendmessage("Error getting malicious files value from Bit9.  Is the certificate on the Bit9 server trusted?");
                }

                if (int.Parse(oldmalicious) < int.Parse(malicious))
                {
                    sendmessage("RED ALERT!  Malicious file detected.  <https://protect.gldd.com/alerts.php|Bit9 Alerts>");
                }
                System.Threading.Thread.Sleep(backoff());
            } while (1 == 1);
        }

        public async void getappreqs()
        {
            System.Threading.Thread.Sleep(backoff());
            string oldappreqs = "";
            do
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(server + "/api/bit9platform/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("x-auth-token", apitoken);

                HttpResponseMessage response;
                try
                {
                    oldappreqs = apprequests;
                    response = await client.GetAsync("v1/approvalRequest?q=status:1&limit=-1");
                    //MessageBox.Show(response.Content.ReadAsStringAsync().Result);
                    apprequests = (response.Content.ReadAsStringAsync().Result).Replace(@"{""count"":", "").Replace("}", "");


                }
                catch
                {
                    sendmessage("Error getting approval requests value from Bit9.  Is the certificate on the Bit9 server trusted?");
                }

                if (int.Parse(oldappreqs) < int.Parse(apprequests))
                {
                    sendmessage("New Approval Request!  <https://protect.gldd.com/approval-requests.php|Approval Requests>");
                }
                System.Threading.Thread.Sleep(backoff());
            } while (1 == 1);
        }

        public async void getsuspicious()
        {
            System.Threading.Thread.Sleep(backoff());
            string oldsuspicious = "";
            do
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(server + "/api/bit9platform/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("x-auth-token", apitoken);

                HttpResponseMessage response;
                try
                {
                    oldsuspicious = suspicious;
                    response = await client.GetAsync("v1/fileCatalog?q=effectiveState:Unapproved&q=threat:50&limit=-1");
                    //MessageBox.Show(response.Content.ReadAsStringAsync().Result);
                    suspicious = (response.Content.ReadAsStringAsync().Result).Replace(@"{""count"":", "").Replace("}", "");


                }
                catch
                {
                    sendmessage("Error getting suspicious files value from Bit9.  Is the certificate on the Bit9 server trusted?");
                }

                if (int.Parse(oldsuspicious) < int.Parse(suspicious))
                {
                    sendmessage("Warning!  New suspicious file!  <https://protect.gldd.com/alerts.php|Bit9 Alerts>");
                }
                System.Threading.Thread.Sleep(backoff());
            } while (1 == 1);
        }


        private int backoff()
        {
            Random r = new Random();
            return r.Next(1000, 10000);
        }

        private void sendmessage(string message)
        {
            string endresult = (@"{ ""text"": """ + message + @""" }");

            string result = String.Empty;

            try
            {

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(hookurl);
                request.Method = "POST";
                request.ContentType = "application/json";
                string postData = endresult;
                byte[] bytes = Encoding.UTF8.GetBytes(postData);
                request.ContentLength = bytes.Length;
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);

                WebResponse response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);

                result = reader.ReadToEnd();
                stream.Dispose();
                reader.Dispose();


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }

        public static string Encrypt(string PlainText, string Password,
                string Salt = "Kosher", string HashAlgorithm = "SHA1",
               int PasswordIterations = 2, string InitialVector = "OFRna73m*aze01xY",
               int KeySize = 256)
        {
            if (string.IsNullOrEmpty(PlainText))
                return "";
            byte[] InitialVectorBytes = Encoding.ASCII.GetBytes(InitialVector);
            byte[] SaltValueBytes = Encoding.ASCII.GetBytes(Salt);
            byte[] PlainTextBytes = Encoding.UTF8.GetBytes(PlainText);
            PasswordDeriveBytes DerivedPassword = new PasswordDeriveBytes(Password, SaltValueBytes, HashAlgorithm, PasswordIterations);
            byte[] KeyBytes = DerivedPassword.GetBytes(KeySize / 8);
            RijndaelManaged SymmetricKey = new RijndaelManaged();
            SymmetricKey.Mode = CipherMode.CBC;
            byte[] CipherTextBytes = null;
            using (ICryptoTransform Encryptor = SymmetricKey.CreateEncryptor(KeyBytes, InitialVectorBytes))
            {
                using (MemoryStream MemStream = new MemoryStream())
                {
                    using (CryptoStream CryptoStream = new CryptoStream(MemStream, Encryptor, CryptoStreamMode.Write))
                    {
                        CryptoStream.Write(PlainTextBytes, 0, PlainTextBytes.Length);
                        CryptoStream.FlushFinalBlock();
                        CipherTextBytes = MemStream.ToArray();
                        MemStream.Close();
                        CryptoStream.Close();
                    }
                }
            }
            SymmetricKey.Clear();
            return Convert.ToBase64String(CipherTextBytes);
        }

        public static string Decrypt(string CipherText, string Password,
              string Salt = "Kosher", string HashAlgorithm = "SHA1",
              int PasswordIterations = 2, string InitialVector = "OFRna73m*aze01xY",
              int KeySize = 256)
        {
            if (string.IsNullOrEmpty(CipherText))
                return "";
            byte[] InitialVectorBytes = Encoding.ASCII.GetBytes(InitialVector);
            byte[] SaltValueBytes = Encoding.ASCII.GetBytes(Salt);
            byte[] CipherTextBytes = Convert.FromBase64String(CipherText);
            PasswordDeriveBytes DerivedPassword = new PasswordDeriveBytes(Password, SaltValueBytes, HashAlgorithm, PasswordIterations);
            byte[] KeyBytes = DerivedPassword.GetBytes(KeySize / 8);
            RijndaelManaged SymmetricKey = new RijndaelManaged();
            SymmetricKey.Mode = CipherMode.CBC;
            byte[] PlainTextBytes = new byte[CipherTextBytes.Length];
            int ByteCount = 0;
            using (ICryptoTransform Decryptor = SymmetricKey.CreateDecryptor(KeyBytes, InitialVectorBytes))
            {
                using (MemoryStream MemStream = new MemoryStream(CipherTextBytes))
                {
                    using (CryptoStream CryptoStream = new CryptoStream(MemStream, Decryptor, CryptoStreamMode.Read))
                    {

                        ByteCount = CryptoStream.Read(PlainTextBytes, 0, PlainTextBytes.Length);
                        MemStream.Close();
                        CryptoStream.Close();
                    }
                }
            }
            SymmetricKey.Clear();
            return Encoding.UTF8.GetString(PlainTextBytes, 0, ByteCount);
        }


    }

    static class SubstringExtensions
    {
        /// <summary>
        /// Get string value between [first] a and [last] b.
        /// </summary>
        public static string Between(this string value, string a, string b)
        {
            int posA = value.IndexOf(a);
            int posB = value.LastIndexOf(b);
            if (posA == -1)
            {
                return "";
            }
            if (posB == -1)
            {
                return "";
            }
            int adjustedPosA = posA + a.Length;
            if (adjustedPosA >= posB)
            {
                return "";
            }
            return value.Substring(adjustedPosA, posB - adjustedPosA);
        }

        /// <summary>
        /// Get string value after [first] a.
        /// </summary>
        public static string Before(this string value, string a)
        {
            int posA = value.IndexOf(a);
            if (posA == -1)
            {
                return "";
            }
            return value.Substring(0, posA);
        }

        /// <summary>
        /// Get string value after [last] a.
        /// </summary>
        public static string After(this string value, string a)
        {
            int posA = value.LastIndexOf(a);
            if (posA == -1)
            {
                return "";
            }
            int adjustedPosA = posA + a.Length;
            if (adjustedPosA >= value.Length)
            {
                return "";
            }
            return value.Substring(adjustedPosA);
        }
    }
}
