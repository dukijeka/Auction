using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using ImageUtilities;

namespace Auction
{
    /// <summary>
    /// Summary description for ShowImage
    /// </summary>
    public class ShowImage : IHttpHandler
    {
        public static int WIDTH = 300;
        public static int HEIGHT = 300;
        public void ProcessRequest(HttpContext context)
        {
            String auctionNo;
            if (context.Request.QueryString["id"] != null)
                auctionNo = context.Request.QueryString["id"];
            else
                throw new ArgumentException("No parameter specified");

            context.Response.ContentType = "image/jpeg";
            Stream strm = ShowAuctionImage(auctionNo);
            byte[] buffer = new byte[4096];
            if (strm == null)
            {
                return;
            }
            int byteSeq = strm.Read(buffer, 0, 4096);

            while (byteSeq > 0)
            {
                context.Response.OutputStream.Write(buffer, 0, byteSeq);
                byteSeq = strm.Read(buffer, 0, 4096);
            }
            //context.Response.BinaryWrite(buffer);
        }

        public Stream ShowAuctionImage(string auctionNo)
        {
            string conn = ConfigurationManager.ConnectionStrings[1].ConnectionString;
            SqlConnection connection = new SqlConnection(conn);
            string sql = "SELECT Image FROM dbo.Auction WHERE ID = @ID";
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@ID", auctionNo);
            connection.Open();
            object img = cmd.ExecuteScalar();
            try
            {
                return new MemoryStream(((byte[])img).CreateThumbnail(WIDTH, HEIGHT));
            }
            catch(Exception e)
            {
                return null;
            }
            finally
            {
                connection.Close();
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}