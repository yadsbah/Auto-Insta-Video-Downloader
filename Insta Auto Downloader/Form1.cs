using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Net;
using System.Diagnostics;

namespace Insta_Auto_Downloader
{
    public partial class Form1 : Form
    {
        string olddd, folderpath="", redo = "";
        WebClient yad;
        int ch = 1,stat=0,browser=1;
        Stack<string> links = new Stack<string>();

        public Form1()
        {
            InitializeComponent();
        }
        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            HtmlElementCollection aa;
            if (webBrowser1.Url.ToString() == "http://insta-downloader.net/instagram-video")
            {
                 aa = webBrowser1.Document.GetElementsByTagName("input");
                foreach (HtmlElement xx in aa)
                {
                    if (xx.GetAttribute("name").Equals("video-url"))
                    {
                        xx.InnerText = textBox1.Text;
                    }
                }
                foreach (HtmlElement xx in aa)
                {
                    if (xx.GetAttribute("type").Equals("submit"))
                    {
                        xx.InvokeMember("Click");
                    }
                }
            }
            if(webBrowser1.Url.ToString() == "http://insta-downloader.net/download-video")
            {
                aa = webBrowser1.Document.GetElementsByTagName("a");
                foreach (HtmlElement xx in aa)
                {
                        if (xx.InnerText == "Download Video")
                    {
                        
                        string urlvid= xx.OuterHtml;
                        urlvid = urlvid.Replace("<a class=\"expanded button\" style=\"max-width: 220px;\" href=\"", "");
                        urlvid = urlvid.Replace("\" download=\"\">Download Video</a>", "");
                        if(urlvid!=olddd)
                           links.Push(urlvid);
                        olddd = urlvid;
                        break;
                    }
                
                }
                
            }
            browser = 1;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            yad = new WebClient();
            yad.DownloadProgressChanged += Yad_DownloadProgressChanged;
            yad.DownloadFileCompleted += Yad_DownloadFileCompleted;
        }
        private void Yad_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            ch = 1;
        }
        private void Yad_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Invoke(new MethodInvoker(delegate ()
            {
                progressBar.Minimum = 0;
                double receive = double.Parse(e.BytesReceived.ToString());
                double total = double.Parse(e.TotalBytesToReceive.ToString());
                double percentage = receive / total * 100;
                progressBar.Value = int.Parse(Math.Truncate(percentage).ToString());
            }));
        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://www.facebook.com/kakayado");
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (button2.Text == "Show Browser")
            {
                this.Size = new System.Drawing.Size(432, 430);
                button2.Text = "Hide Browser";
            }
            else {
                this.Size = new System.Drawing.Size(432, 174);
                button2.Text = "Show Browser";
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            string link="";
            textBox1.Text = Clipboard.GetText();
            link = Clipboard.GetText();
            if (link.Contains("instagram.com/p/") && link != redo && browser == 1)
            {
                browser = 0;
                webBrowser1.Navigate("http://insta-downloader.net/instagram-video");
            }
            redo = link;
            label2.Text = links.Count.ToString();
            if (ch==1 && links.Count != 0)
            {
                backgroundWorker1.RunWorkerAsync();
            }
        }
        private void panel3_Click(object sender, EventArgs e)
        {
            if (folderpath == "")
            {
                folderBrowserDialog1.ShowDialog();
                folderpath = folderBrowserDialog1.SelectedPath;
            }

            if (stat==0 && folderpath!="")
            {
                stat = 1;
                panel3.BackgroundImage = Properties.Resources.switch__1_;
                timer1.Start();
            }
            else if (stat == 1)
            {
                stat = 0;
                panel3.BackgroundImage = Properties.Resources.switch__2_;
                timer1.Stop();
            }
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
                        ch = 0;
                        string url = links.Pop();
                        Uri uri = new Uri(url);
                        string fileName = System.IO.Path.GetFileName(uri.AbsolutePath);
                        yad.DownloadFileAsync(uri, folderpath + "/" + fileName);
        }
    }
}
