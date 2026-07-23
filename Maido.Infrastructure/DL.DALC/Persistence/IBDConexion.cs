using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Maido.Infrastructure.DL.DALC.Persistence
{
    public interface IBDConexion
    {
        IDbConnection CrearConexion();
    }
}
