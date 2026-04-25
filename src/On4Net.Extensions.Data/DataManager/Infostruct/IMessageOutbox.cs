using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace On4Net.Extensions.Data.DataManager.Infostruct;

public interface IMessageOutbox
{
    bool HasMessage { get; }

    IMessageOutbox On(IDbConnection connection);

    Task SendAsync(string queueUrl, string groupId, IEnumerable<string> messages, CancellationToken cancellationToken);

    Task PublishMessagesAsync(IDbConnection connection, CancellationToken cancellationToken);
}
