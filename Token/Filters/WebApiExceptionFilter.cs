using Microsoft.Owin.Logging;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Filters;
using Token.Methods;
using WebGrease;

namespace Token.Filters
{
    public class WebApiErroLogger : ExceptionLogger
    {
        public override async Task LogAsync(ExceptionLoggerContext context, CancellationToken cancellationToken)
        {
            try
            {
                new Thread(() => UtilityHelper.WriteLog(context.Exception, "ExceptionError_Logger", "ExceptionError_Logger")).Start();

            }
            catch (Exception ex)
            {
                throw ex;//  MessageBox.Show(ek.Message);
            }
        }
    }
}