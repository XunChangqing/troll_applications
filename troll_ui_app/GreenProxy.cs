using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using TrotiNet;
using HtmlAgilityPack;
using log4net;
using System.Web;
using System.Drawing;

namespace troll_ui_app
{
    public class GreenProxy : ProxyLogic
    {
        static readonly ILog log = Log.Get();
        /// <summary>
        /// A simple regular expression for charset detection
        /// </summary>
        static Regex charset_regex = new Regex("charset=([\\w-]*)", RegexOptions.Compiled);
        static Object SyncObject = new Object();
        //domain_name => {set of porn_pic uris}
        static List<Tuple<String, HashSet<String>>> ImageSrcSetList = new List<Tuple<String, HashSet<String>>>();

        private String FullRequestUri;
        private String DomainName;
        private PornDatabase.DomainType DomainType;
        private PornDatabase.ForbiddenItemStatus _autoDomainStatus;
        private PornDatabase PornDB = new PornDatabase();

        private HashSet<String> GetPornSet(String domainName)
        {
            foreach(Tuple<String, HashSet<String>> tset in ImageSrcSetList)
            {
                if (tset.Item1 == domainName)
                    return tset.Item2;
            }
            Tuple<String, HashSet<String>> nset = new Tuple<string, HashSet<string>>(domainName, new HashSet<string>());
            //添加在结尾处，所有后面必须从头部删除，否则会发生bug
            ImageSrcSetList.Add(nset);
            if (ImageSrcSetList.Count > Properties.Settings.Default.maxNumDomainList)
            {
                log.Info("Remove the first domain in domainList: " + ImageSrcSetList[0].Item1);
                ImageSrcSetList.RemoveAt(0);
            }
                //ImageSrcSetList.RemoveAt(ImageSrcSetList.Count-1);
            return nset.Item2;
        }

        public GreenProxy(HttpSocket clientSocket)
            : base(clientSocket) {
        }

        static new public GreenProxy CreateProxy(HttpSocket clientSocket)
        {
            return new GreenProxy(clientSocket);
        }

        private string ToAbsoluteUrl(string relativeUrl)
        {
            if(relativeUrl.StartsWith("/")||
                relativeUrl.StartsWith("./"))
            {
                Uri baseuri = new Uri(FullRequestUri);
                Uri absolute = new Uri(baseuri, relativeUrl);
                return absolute.ToString();
            }
            return relativeUrl;
        }
        /// <summary>
        /// Guess the file encoding, judging from the response content type
        /// </summary>
        /// <param name="content">The input content</param>
        /// <returns>The input encoding, or null if it could not be
        /// determined</returns>
        Encoding GetFileEncoding()
        {
            string charset = null;

            // Check if the charset is specified in the response headers
            if (ResponseHeaders.Headers.ContainsKey("content-type"))
            {
                string contentType = ResponseHeaders.Headers["content-type"];
                Match m = charset_regex.Match(contentType);
                if (m.Success)
                    charset = m.Groups[1].Value;
            }

            if (charset == null)
            {
                // If the charset is not specified in the response headers,
                // ideally we should look for the charset in the set of
                // META tags of the page. We'll just bail out for now.
                return null;
            }

            try
            {
                return Encoding.GetEncoding(charset);
            }
            catch { }

            return null;
        }

        private void AddPorn()
        {
            log.Info("Try to find the page including: " + FullRequestUri);
            lock (SyncObject)
            {
                foreach (Tuple<String, HashSet<String>> tset in ImageSrcSetList)
                {
                    if (tset.Item2.Contains(FullRequestUri.ToLower()))
                    {
                        string domainName = GetDomain.GetDomainFromUrl(tset.Item1);
                        PornDatabase.DomainType dt = PornDB.GetDomainType(domainName);
                        //hit
                        log.Info("Hit: " + FullRequestUri + " => " + tset.Item1);
                        //add to database
                        PornDB.InsertPornPage(domainName, tset.Item1,
                            FullRequestUri, dt==PornDatabase.DomainType.Black);
                    }
                }
            }
        }

        protected void ProcessImage()
        { 
            log.Info("Process Image: " + RequestLine.URI);
            byte[] response = GetContent();

            // From now on, the default State.NextStep ( == SendResponse()
            // at this point) must not be called, since we already read
            // the response.
            State.NextStep = null;

            // Decompress the message stream, if necessary
            Stream stream = GetResponseMessageStream(response);
            Bitmap bm = new Bitmap(stream);

            PornClassifier.ImageType imgType = PornClassifier.Instance.Classify(bm);
            IProgress<PornDatabase.PornItemType> ip = MainForm.Instance.TargetProcessedProgress as IProgress<PornDatabase.PornItemType>;
            bool isImageBad = false;
            if (imgType == PornClassifier.ImageType.Disguise)
            {
                isImageBad = true;
            }
            else if (imgType == PornClassifier.ImageType.Porn)
            {
                isImageBad = true;
                //添加到不良网页中，以进行不良网站检查，只针对色情图片进行处理
                //只在启用了色情网站侦测时才处理，但实际上由于未启用色情网站侦测时
                //html不处理，这里其实即使处理也无法hit
                if(Properties.Settings.Default.IsPornWebsiteProtectionTurnOn)
                    AddPorn();
            }
            else
                ip.Report(PornDatabase.PornItemType.Undefined);

            //if (p != null)
            //只在图片过滤功能开启时才将色情图片加入数据库，并替换图片
            if (isImageBad && Properties.Settings.Default.IsNetworkImageTurnOn)
            {
                PornDB.InsertPornPic(FullRequestUri, PornClassifier.ImageType.Disguise);
                bm.Save(Program.AppLocalDir + Properties.Settings.Default.imagesDir + "\\" + HttpUtility.UrlEncode(FullRequestUri));
                ip.Report(PornDatabase.PornItemType.NetworkImage);

                Graphics g = Graphics.FromImage(bm);
                SolidBrush solidBrush = new SolidBrush(Color.Red);
                g.FillRectangle(solidBrush, 0, 0, bm.Width, bm.Height);
                SolidBrush stringBrush = new SolidBrush(Color.Yellow);
                g.DrawString("山妖卫士", new Font("微软雅黑", bm.Width / 10, GraphicsUnit.Pixel), stringBrush, new Point(0, 0));
                g.Flush();
            }

            // Even if the response was originally transferred
            // by chunks, we are going to send it unchunked.
            // (We could send it chunked, though, by calling
            // TunnelChunkedDataTo, instead of TunnelDataTo.)
            ResponseHeaders.TransferEncoding = null;

            // Encode the modified content, and recompress it, as necessary.
            //String text = htmlDoc.Save()
            //htmlDoc.Save(HttpUtility.UrlEncode(RequestLine.URI));
            MemoryStream mstr = new MemoryStream();
            bm.Save(mstr, bm.RawFormat);
            byte[] output = CompressResponse(mstr.GetBuffer());
            ResponseHeaders.ContentLength = (uint)output.Length;

            // Finally, send the result.
            SendResponseStatusAndHeaders();
            SocketBP.TunnelDataTo(TunnelBP, output);

            // We are done with the request.
            // Note that State.NextStep has been set to null earlier.
        }

        protected void ProcessHtml()
        {
            log.Info("Process HTML: " + RequestLine.URI);
            // Let's assume we need to retrieve the entire file before
            // we can do the rewriting. This is usually the case if the
            // content has been compressed by the remote server, or if we
            // want to build a DOM tree.
            byte[] response = GetContent();

            // From now on, the default State.NextStep ( == SendResponse()
            // at this point) must not be called, since we already read
            // the response.
            State.NextStep = null;

            // Decompress the message stream, if necessary
            //byte[] content = ReadEverything(stream);
            Encoding fileEncoding = GetFileEncoding();

            bool bNoFileEncoding = false;
            if (fileEncoding == null)
            {
                bNoFileEncoding = true;
                fileEncoding = Encoding.UTF8;
                // We could not guess the file encoding, so it's better not
                // to modify anything.
                //ResponseHeaders.TransferEncoding = null;
                //ResponseHeaders.ContentLength = (uint)response.Length;
                //SendResponseStatusAndHeaders();
                //SocketBP.TunnelDataTo(TunnelBP, response);
                //return;
            }

            //load html document
            Stream stream = GetResponseMessageStream(response);
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.Load(stream, fileEncoding);
            // We are now in a position to get image src
            // if the white list does not contain the domain name
            if (DomainType != PornDatabase.DomainType.White)
            {
                HtmlNodeCollection nc = htmlDoc.DocumentNode.SelectNodes("//img[@src]");
                if (nc != null)
                {
                    log.Info("Images of page url: " + FullRequestUri);
                    lock (SyncObject)
                    {
                        HashSet<String> currentSet = GetPornSet(FullRequestUri);
                        foreach (HtmlNode imgsrc in nc)
                        {
                            HtmlAttribute src = imgsrc.Attributes["src"];
                            String asrc = ToAbsoluteUrl(src.Value);
                            currentSet.Add(asrc.ToLower());
                            log.Info("items: " + asrc.ToLower());
                        }
                    }
                }
            }
            //add p to body
            if (!bNoFileEncoding)
            {
                //var head = htmlDoc.DocumentNode.SelectSingleNode("//head");
                //var body = htmlDoc.DocumentNode.SelectSingleNode("//body");
                //if (head != null && body != null)
                //{
                //    HtmlNode pcss = htmlDoc.CreateElement("style");
                //    pcss.SetAttributeValue("type", "text/css");
                //    pcss.InnerHtml = HtmlDocument.HtmlEncode("#masatek-tag{color:green; background-color: gray; padding:5px; text-align:center; font-size: 150%;}");
                //    head.PrependChild(pcss);

                //    HtmlNode pMasaTag = htmlDoc.CreateElement("p");
                //    pMasaTag.SetAttributeValue("id", "masatek-tag");
                //    pMasaTag.InnerHtml = HtmlDocument.HtmlEncode("Powered by Masatek");
                //    body.PrependChild(pMasaTag);
                //}
            }


            // Even if the response was originally transferred
            // by chunks, we are going to send it unchunked.
            // (We could send it chunked, though, by calling
            // TunnelChunkedDataTo, instead of TunnelDataTo.)
            ResponseHeaders.TransferEncoding = null;

            // Encode the modified content, and recompress it, as necessary.
            //String text = htmlDoc.Save()
            //htmlDoc.Save(HttpUtility.UrlEncode(RequestLine.URI));
            if (!bNoFileEncoding)
            {
                //MemoryStream mstr = new MemoryStream();
                //htmlDoc.Save(mstr, fileEncoding);
                //response = CompressResponse(mstr.GetBuffer());
            }

            ResponseHeaders.ContentLength = (uint)response.Length;
            // Finally, send the result.
            SendResponseStatusAndHeaders();
            SocketBP.TunnelDataTo(TunnelBP, response);

            // We are done with the request.
            // Note that State.NextStep has been set to null earlier.
        }

        protected override void OnReceiveResponse()
        {
            // Use the content-type field of the response headers to
            // Determine which HTTP content we want to modify.
            //bool bModifyContent = false;
            bool bProcessHtml = false;
            bool bProcessImage = false;
            //if (!(DomainType == PornDatabase.DomainType.White) && ResponseHeaders.Headers.ContainsKey("content-type"))
            if (ResponseHeaders.Headers.ContainsKey("content-type"))
            {
                if (DomainType != PornDatabase.DomainType.White && Properties.Settings.Default.IsPornWebsiteProtectionTurnOn)
                    bProcessHtml = ResponseHeaders.Headers["content-type"].Contains("text/html");
                //if (DomainType != PornDatabase.DomainType.White &&
                //    (Properties.Settings.Default.IsPornWebsiteProtectionTurnOn || Properties.Settings.Default.IsNetworkImageTurnOn))
                if (Properties.Settings.Default.IsPornWebsiteProtectionTurnOn || Properties.Settings.Default.IsNetworkImageTurnOn)
                    bProcessImage = ResponseHeaders.Headers["content-type"].Contains("image/jpeg") ||
                        ResponseHeaders.Headers["content-type"].Contains("image/png");
            }
            else
                log.Info("Ignore request: " + RequestLine.URI);

            // Rewriting may also depend on the user agent.
#if false
            if (RequestHeaders.Headers.ContainsKey("user-agent"))
                if (RequestHeaders.Headers["user-agent"].ToLower().Contains("msie"))
                {
                    // ...
                }
#endif

            // Do not rewrite anything unless we got a 200 status code.
            if (ResponseStatusLine.StatusCode != 200) 
            {
                bProcessHtml = false;
                bProcessImage = false;
            }

            if (bProcessHtml)
                ProcessHtml();
            else if (bProcessImage)
                ProcessImage();

            return;
            // Tell the browser not to cache our modified version.
            //ResponseHeaders.CacheControl = "no-cache, no-store, must-revalidate";
            //ResponseHeaders.Expires = "Fri, 01 Jan 1990 00:00:00 GMT";
            //ResponseHeaders.Pragma = "no-cache";
        }

        static byte[] ReadEverything(Stream input)
        {
            byte[] buffer = new byte[128 * 1024]; // ugly, but don't care
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                    ms.Write(buffer, 0, read);
                return ms.ToArray();
            }
        }

        protected override void OnReceiveRequest()
        {
            //store complete request uri first, it will parsed and the host will be removed later
            FullRequestUri = RequestLine.URI;
            DomainName = GetDomain.GetDomainFromUrl(RequestLine.URI);
            DomainType = PornDB.GetDomainType(DomainName);
            _autoDomainStatus = PornDB.GetAutoDomainStatus(DomainName);
            log.Debug(GetDomain.GetDomainFromUrl(RequestLine.URI));
                //DomainType == PornDatabase.DomainType.Black ||
            if (DomainType != PornDatabase.DomainType.White &&
                _autoDomainStatus == PornDatabase.ForbiddenItemStatus.Normal &&
                Properties.Settings.Default.IsPornWebsiteProtectionTurnOn)
            {
                PornDB.InsertBlockedPage(FullRequestUri);
                SocketBP.Send403();
                //SocketBP.SendHttpError();
                State.NextStep = AbortRequest;
                log.Info("Block request in tmp black list: " + RequestLine.URI);
            }
        }
    }
}
