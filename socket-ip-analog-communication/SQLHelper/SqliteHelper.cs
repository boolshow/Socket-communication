using Microsoft.Data.Sqlite;
using System.Data;

namespace SQLHelper
{
    public class SqliteHelper : IDisposable
    {
        public static string DbFile
        {
            get { return Environment.CurrentDirectory + "\\SimpleDb.sqlite"; }
        }
        private SqliteConnection conn;
        public SqliteHelper()
        {
            conn = openDataConnection();
        }
        public static SqliteConnection SimpleDbConnection()
        {
            var connStr = @"Data Source=" + DbFile;//连接字符串
            var conn = new SqliteConnectionStringBuilder(connStr)
            {
                Mode = SqliteOpenMode.ReadWriteCreate,
                Password = "adfersawwsd"
            }.ToString();//使用这个方式设置密码，避免sql注入
            return new SqliteConnection(conn);//创建SQLite连接
        }

        /// <summary>
        /// 打开数据库链接；
        /// </summary>
        /// <returns></returns>
        private SqliteConnection openDataConnection()
        {
            var conn = SimpleDbConnection();
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            return conn;
        }

        /// <summary>
        /// 释放数据连接；
        /// </summary>
        public void Dispose()
        {
            conn.Close();
            conn.Dispose();
        }
    }

}