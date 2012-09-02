using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroLog.Layouts
{
    public class FileSnapshotLayout : Layout
    {
        protected internal override string GetFormattedString(LogEventInfo info)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Seq: ");
            builder.Append(info.SequenceID);
            builder.Append(" Time: ");
            builder.Append(info.TimeStamp.ToString(LogManager.DateTimeFormat));
            builder.Append(" Thread: ");
            builder.Append(Environment.CurrentManagedThreadId);
            builder.Append(" Logger: ");
            builder.Append(info.Logger);
            builder.Append(" - ");
            builder.Append(info.Message);

            if(info.Exception != null)
            {
                builder.Append("\r\n------------------------\r\n");
                builder.Append(info.Exception);
            }

            return builder.ToString();
        }
    }
}
