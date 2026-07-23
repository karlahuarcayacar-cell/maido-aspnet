using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Maido.Infrastructure.DL.DALC.Persistence
{
    public class BDConexion : IBDConexion
    {
        private readonly IConfiguration iConfig;

        public BDConexion(IConfiguration _iConfig)
        {
            iConfig = _iConfig;
        }

        public IDbConnection CrearConexion()
        {
            SqlConnection con = new SqlConnection(iConfig.GetConnectionString("maido_db"));
            con.Open();
            return con;
        }
    }
}
